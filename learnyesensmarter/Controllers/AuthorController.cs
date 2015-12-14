using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.IO;

using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                ViewBag.Separator = SEPARATOR;
                return View("Task/" + questionType.ViewName);
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
                //log error
                ViewBag.msg = "Something went wrong when trying to create the new command question :'(";
                return View("Task/Result/Failure");
            }

            return View("Task/Result/Success");
        }

        public class EmptyReviewException : Exception
        {
            public EmptyReviewException()
            {
            }
        }

        [HttpPost]
        public ViewResult AuthorNewReview(string review)
        {
            try
            {
                bool TheresNoContentToReview = review == String.Empty;
                if ( TheresNoContentToReview )
                {
                    throw new EmptyReviewException();
                }

                //insert the question
                var questionModel = new QuestionModel();
                questionModel.Question = review;
                questionModel.QuestionType = (int)QuestionTypeIDs.REVIEW;
                _questionsController.Insert(questionModel);
                //doesn't insert answer as a review task only reviews the question, there is no answer
            }
            catch (EmptyReviewException ere)
            {
                ViewBag.msg = "Whoops, looks like you forgot to add something to review";
                return View("Task/Result/Failure");
            }
            catch (Exception e)
            {
                ViewBag.msg = "Something went wrong when trying to create the new review question :'(";
                return View("Task/Result/Failure");
            }

            return View("Task/Result/Success");
        }

        private string[] FilterInput(string input)
        {
            return input.Split(' ');
        }
        //this is used throughout in the insert answer sections to separate answers that have been collated together into one string
        private const string SEPARATOR = " -SEPARATOR- ";

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
                string[] pros = Regex.Split(collatedPros, SEPARATOR); //uses regex to split the collated pros into seperate pros
                                                                            //!IMPORTANT!: Leaves an empty string in last place of array
                string[] cons = Regex.Split(collatedCons, SEPARATOR);

                var answerModel = new AnswerModel();
                answerModel.QuestionType = questionModel.QuestionType;
                answerModel.QuestionID = questionID;

                //setup Cypher Query
                var parameters = new Dictionary<string, object>();
                parameters["qID"] = questionID; //this is done here because it is the same for all nodes for this answer.
                parameters["totalSubs"] = pros.Length-1; //-1 to account for the empty string at the end which is not inserted into the db

                //Note: Cypher Query is creating a relationship in a direction but the match query for this type of question ignores this relationship
                      //It is only done because Neo4j requires it.

                //first construct the query for creating the pros.
                #region ProQuery
                //answers for this type of question contain the question id and then a sub id and the total number of sub ids, as well as the answer
                //the sub id indicates the number, for example, of pros. If the users supplies 5 pros then this constructs a relationship of key words for each 
                string prosQuery = "create ";
                for (int i = 0; i < (pros.Length-1); i++) //-1 to account for blank space at the end
                {
                    //seperate the answer based on spaces - orginally this was comma seperated but that caused https://github.com/CarbonBasedDudeform/Learn-Yesen-Smarter/issues/40
                    var current = FilterInput(pros[i]);
                    //these properties stay the same for all nodes in this sentence
                    parameters["subID"] = i;
                    
                    for (int j = 0; j < current.Length; j++)
                    {
                        //these properties change with each node
                        parameters["ans"+j] = current[j];

                        prosQuery += "(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSubs}, Answer: {ans"+j+"}, IsPro: True})";

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
                #endregion

                //repeat the same for the cons
                #region ConQuery
                parameters["totalSubs"] = cons.Length - 1; //-1 for blank space at end
                string consQuery = "create";
                for (int i = 0; i < (cons.Length-1); i++) //-1 to account for blank space at the end
                {
                    //seperate the answer based on spaces - orginally this was comma seperated but that caused https://github.com/CarbonBasedDudeform/Learn-Yesen-Smarter/issues/40
                    var current = FilterInput(cons[i]);
                    //these properties stay the same for all nodes in this sentence
                    parameters["subID"] = i;

                    for (int j = 0; j < current.Length; j++)
                    {
                        //these properties change with each node
                        parameters["ans"+j] = current[j];

                        consQuery += "(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSubs}, Answer: {ans" + j + "}, IsPro: False})";

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
                #endregion
            }
            catch (Exception e)
            {
                ViewBag.msg = "Something went wrong when trying to create the new pros and cons question :'(";
                return View("Task/Result/Failure");
            }

            return View("Task/Result/Success");
        }

        [HttpPost]
        public ViewResult AuthorNewExplanation(string Prompt, string collatedAnswers)
        {
            try
            {
                //insert the question
                var questionModel = new QuestionModel();
                questionModel.Question = Prompt;
                questionModel.QuestionType = (int)QuestionTypeIDs.EXPLANATION;
                int questionID = _questionsController.Insert(questionModel);

                //insert the answer
                string[] answers = Regex.Split(collatedAnswers, SEPARATOR);

                var answerModel = new AnswerModel();
                answerModel.QuestionType = questionModel.QuestionType;
                answerModel.QuestionID = questionID;


                var parameters = new Dictionary<string, object>();
                parameters["qID"] = questionID; //this is done here because it is the same for all nodes for this answer.
                parameters["totalSubs"] = answers.Length - 1; //-1 to account for the empty string at the end which is not inserted into the db

                //Note: Cypher Query is creating a relationship in a direction but the match query for this type of question ignores this relationship
                //It is only done because Neo4j requires it.

                //first construct the query for creating the pros.
                //answers for this type of question contain the question id and then a sub id and the total number of sub ids, as well as the answer
                //the sub id indicates the number, for example, of pros. If the users supplies 5 pros then this constructs a relationship of key words for each 
                string expanationQuery = "create ";
                for (int i = 0; i < (answers.Length - 1); i++) //-1 to account for blank space at the end
                {
                    //seperate the answer based on spaces - orginally this was comma seperated but that caused https://github.com/CarbonBasedDudeform/Learn-Yesen-Smarter/issues/40
                    var current = FilterInput(answers[i]);

                    //these properties stay the same for all nodes in this segment
                    parameters["subID"] = i;

                    for (int j = 0; j < current.Length; j++)
                    {
                        //these properties change with each node
                        parameters["ans" + j] = current[j];

                        expanationQuery += "(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSubs}, Answer: {ans" + j + "} })";

                        //if there are more to come, then add the PRO relation, otherwise ^^ is the terminating node.
                        if ((j + 1) < current.Length)
                        {
                            expanationQuery += "-[:EXPLANATION]->";
                        }
                    }

                    answerModel.CypherQuery = new Neo4jClient.Cypher.CypherQuery(expanationQuery, parameters, Neo4jClient.Cypher.CypherResultMode.Projection);
                    _answersController.Insert(answerModel);
                    //reset the query to it's original state
                    expanationQuery = "create ";
                } 
            }
            catch (Exception e)
            {
                ViewBag.msg = "Something went wrong when trying to create the new explanation question :'(";
                return View("Task/Result/Failure");
            }
            return View("Task/Result/Success");
        }

        [HttpPost]
        public ViewResult AuthorNewTable(string prompt, string randomised, string collatedTable, string numberOfRows, string numberOfCols)
        {
            try
            {
                //insert the question
                var questionModel = new QuestionModel();
                questionModel.Question = prompt;
                questionModel.QuestionType = (int)QuestionTypeIDs.TABLE;
                int questionID = _questionsController.Insert(questionModel);

                //insert the answer
                GridModel[] Answers = JsonConvert.DeserializeObject<GridModel[]>(collatedTable);
                var answerModel = new AnswerModel();
                answerModel.QuestionType = questionModel.QuestionType;
                answerModel.QuestionID = questionID;


                var parameters = new Dictionary<string, object>();
                parameters["qID"] = questionID; //this is done here because it is the same for all nodes for this answer.
                parameters["totalSubs"] = Answers.Length; //-1 to account for the empty string at the end which is not inserted into the db
                parameters["totalRows"] = Int32.Parse(numberOfRows);
                parameters["totalCols"] = Int32.Parse(numberOfCols);

                string tableQuery = "create ";
                for (int i = 0; i < Answers.Length; i++)
                {
                    //these properties stay the same for all nodes in this segment
                    /*
                     * necessary to tag each variable with i to differentiate between the nodes. Other versions of inserting the answer haven't needed this
                     * because they execute the query at the end of the for loop, this, however, executes the query after the for loop so if i isn't added
                     * then all the nodes end up with the same value (the last set value)
                     */
                    parameters["subID"+i] = i;
                    parameters["ans"+i] = Answers[i].Val; //
                    parameters["x"+i] = Answers[i].X;
                    parameters["y"+i] = Answers[i].Y;

                    tableQuery += "(:Answer { questionID: {qID}, subID: {subID" + i + "}, totalSubs: {totalSubs}, Answer: {ans" + i + "}, X: {x" + i + "}, Y: {y" + i + "}, totalRows: {totalRows}, totalCols: {totalCols} })";

                    if ((i + 1) < Answers.Length)
                    {
                        tableQuery += "-[:TABLE]->";
                    }
                }

                answerModel.CypherQuery = new Neo4jClient.Cypher.CypherQuery(tableQuery, parameters, Neo4jClient.Cypher.CypherResultMode.Projection);
                _answersController.Insert(answerModel);
            }
            catch (Exception e)
            {
                ViewBag.msg = "Something went wrong when trying to create the new table question :'(";
                return View("Task/Result/Failure");
            }

            return View("Task/Result/Success");
        }
    }
}
