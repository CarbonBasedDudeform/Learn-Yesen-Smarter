using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.IO;

using System.Text.RegularExpressions;

using Newtonsoft.Json;
using learnyesensmarter.Models;

namespace learnyesensmarter.Controllers
{
    public class AuthorController : Controller
    {
        QuestionsController _questionsController;
        AnswersController _answersController;
        public AuthorController()
        {
            _questionsController = new QuestionsController();
            _answersController = new AnswersController();
        }

        /// <summary>
        /// Constructor for testing purposes or using a different QuestionsController than the default one.
        /// </summary>
        /// <param name="AltQuestionsController"></param>
        public AuthorController(QuestionsController AltQuestionsController = null, AnswersController AltAnswersController = null)
        {
            _questionsController = AltQuestionsController;
            _answersController = AltAnswersController;
        }
        //
        // GET: /Author/

        public ActionResult Index()
        {
            return View(new AuthorTaskModel());
        }

        /// <summary>
        /// Takes a string with information about the new task and redirects to the correct view page for it.
        /// </summary>
        /// <param name="question">string containing a serialized QuestionType object</param>
        /// <returns></returns>
        public ActionResult NewTask(string question)
        {
            if (String.IsNullOrEmpty(question)) throw new Exception("Unlogged exception in AuthorController - NewTask() : Empty String");

            try
            {
                var questionType = JsonConvert.DeserializeObject<QuestionType>(question);
                return View(questionType.ViewName);
            }
            catch (Exception e)
            {
                throw new Exception("Unlogged exception in AuthorController - NewTask() : " + e.Message);
                //return Error View
            }
        }

        [HttpPost]
        public ActionResult AuthorNewCommand(string Prompt, string Answer)
        {
            try
            {
                //insert the question
                var questionModel = new QuestionModel();
                questionModel.Question = Prompt;
                questionModel.QuestionType = (int)QuestionTypeIDs.COMMAND;
                int questionID = _questionsController.Insert(questionModel);

                //insert the answer
                var answerModel = new AnswerModel();
                answerModel.QuestionType = questionModel.QuestionType;
                answerModel.QuestionID = questionID;

                //setup Cypher Query
                var parameters = new Dictionary<string, object>();
                parameters.Add("qID", questionID);
                parameters.Add("ans", Answer);

                answerModel.CypherQuery = new Neo4jClient.Cypher.CypherQuery("create (a:Answer{ questionID: {qID}, Answer: {ans}  })", parameters, Neo4jClient.Cypher.CypherResultMode.Projection);
                                                                 
                _answersController.Insert(answerModel);
            }
            catch (Exception e)
            {
                throw new Exception("unlogged exception in AuthorNewCommand: " + e.Message);
            }

            return View("Success");
        }

        [HttpPost]
        public ViewResult AuthorNewReview(string review)
        {
            try
            {
                //insert the question
                var questionModel = new QuestionModel();
                questionModel.Question = review;
                questionModel.QuestionType = (int)QuestionTypeIDs.REVIEW;
                _questionsController.Insert(questionModel);
                //doesn't insert answer as a review task only reviews the question, there is no answer
            }
            catch (Exception e)
            {
                throw new Exception("unlogged exception in AuthorNewCommand");
            }

            return View("Success");
        }

        [HttpPost]
        public ViewResult AuthorNewProsAndCons(string Prompt, string collatedPros, string collatedCons)
        {
            try
            {
                //insert the question
                var questionModel = new QuestionModel();
                questionModel.Question = Prompt;
                questionModel.QuestionType = (int)QuestionTypeIDs.PROSANDCONS;
                int questionID = _questionsController.Insert(questionModel);
                
                //insert the answer
                string[] pros = Regex.Split(collatedPros, " -SEPARATOR- "); //uses regex to split the collated pros into seperate pros
                                                                            //!IMPORTANT!: Leaves an empty string in last place of array
                string[] cons = Regex.Split(collatedCons, " -SEPARATOR- ");

                var answerModel = new AnswerModel();
                answerModel.QuestionType = questionModel.QuestionType;
                answerModel.QuestionID = questionID;

                //setup Cypher Query
                var parameters = new Dictionary<string, object>();
                parameters["qID"] = questionID; //this is done here because it is the same for all nodes for this answer.
                parameters["totalSub"] = pros.Length-1; //-1 to account for the empty string at the end which is not inserted into the db

                //Note: Cypher Query is creating a relationship in a direction but the match query for this type of question ignores this relationship
                      //It is only done because Neo4j requires it.

                //first construct the query for creating the pros.
                //answers for this type of question contain the question id and then a sub id and the total number of sub ids, as well as the answer
                //the sub id indicates the number, for example, of pros. If the users supplies 5 pros then this constructs a relationship of key words for each 
                string prosQuery = "create ";
                for (int i = 0; i < (pros.Length-1); i++) //-1 to account for blank space at the end
                {
                    //seperate the answer based on comma -- this assumes the user supplies a comma seperated list of keywords and allows for key phrases as well, eg. "fluffy cat, sat, soft mat" 
                    var current = pros[i].Split(','); 
                    //these properties stay the same for all nodes in this sentence
                    parameters["subID"] = i;
                    
                    for (int j = 0; j < current.Length; j++)
                    {
                        //these properties change with each node
                        parameters["ans"+j] = current[j];

                        prosQuery += "(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans"+j+"} })";

                        //if there are more to come, then add the PRO relation, otherwise ^^ is the terminating node.
                        if ((j + 1) < current.Length)
                        {
                            prosQuery += "-[:PRO]->";
                        }
                    }

                    answerModel.CypherQuery = new Neo4jClient.Cypher.CypherQuery(prosQuery, parameters, Neo4jClient.Cypher.CypherResultMode.Projection);
                    _answersController.Insert(answerModel);
                    //reset the query to it's original state
                    prosQuery = "create ";
                }                

                //repeat the same for the cons
                parameters["totalSub"] = cons.Length - 1; //-1 for blank space at end
                string consQuery = "create";
                for (int i = 0; i < (cons.Length-1); i++) //-1 to account for blank space at the end
                {
                    //seperate the answer based on comma -- this assumes the user supplies a comma seperated list of keywords and allows for key phrases as well, eg. "fluffy cat, sat, soft mat" 
                    var current = cons[i].Split(',');
                    //these properties stay the same for all nodes in this sentence
                    parameters["subID"] = i;

                    for (int j = 0; j < current.Length; j++)
                    {
                        //these properties change with each node
                        parameters["ans"+j] = current[j];

                        consQuery += "(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans" + j + "} })";

                        //if there are more to come, then add the PRO relation, otherwise ^^ is the terminating node.
                        if ((j + 1) < current.Length)
                        {
                            consQuery += "-[:CON]->";
                        }
                    }

                    answerModel.CypherQuery = new Neo4jClient.Cypher.CypherQuery(consQuery, parameters, Neo4jClient.Cypher.CypherResultMode.Projection);
                    _answersController.Insert(answerModel);

                    //reset the query to it's original state
                    consQuery = "create ";
                }        
            }
            catch (Exception e)
            {
                throw new Exception("unlogged exception in AuthorNewCommand");
            }

            return View("Success");
        }
    }
}
