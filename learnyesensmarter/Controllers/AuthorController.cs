using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.IO;

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
                var questionModel = new QuestionModel();
                questionModel.Question = Prompt;
                questionModel.QuestionType = (int)QuestionTypeIDs.COMMAND;
                int questionID = _questionsController.Insert(questionModel);
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
                throw new Exception("unlogged exception in AuthorNewCommand");
            }

            return View("Success");
        }

        [HttpPost]
        public ViewResult AuthorNewReview(string review)
        {
            try
            {
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
    }
}
