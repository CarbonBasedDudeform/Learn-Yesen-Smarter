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
        public AuthorController()
        {
            _questionsController = new QuestionsController();
        }

        /// <summary>
        /// Constructor for testing purposes or using a different QuestionsController than the default one.
        /// </summary>
        /// <param name="AltQuestionsController"></param>
        public AuthorController(QuestionsController AltQuestionsController)
        {
            _questionsController = AltQuestionsController;
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
                QuestionModel model = new QuestionModel();
                model.Question = Prompt;
                model.QuestionType = (int)QuestionTypeIDs.COMMAND;
                _questionsController.Insert(model);
            }
            catch (Exception e)
            {
                throw new Exception("unlogged exception in AuthorNewCommand");
            }

            return View("Failure");
        }

    }
}
