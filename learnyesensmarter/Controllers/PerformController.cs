using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using learnyesensmarter.Models;

namespace learnyesensmarter.Controllers
{
    public class PerformController : Controller
    {
        QuestionsController _questionsController;
        AnswersController _answersController;

        public PerformController()
        {
            _questionsController = new QuestionsController();
            _answersController = new AnswersController();
        }

        public PerformController(QuestionsController altQuestionController = null, AnswersController altAnswerController = null)
        {
            _questionsController = altQuestionController;
            _answersController = altAnswerController;
        }

        //
        // GET: /Perform/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Selects five tasks for the user to perform.
        /// </summary>
        /// <returns>A View of five tasks for the user to perform</returns>
        public ActionResult PerformFive()
        {
            //Grab top Five Priority Tasks from DB
            //populate a model with 5 questions and their IDs and PartialView Names based on Type
            
            return View();
        }

        /// <summary>
        /// Allows the user to select tasks from categories or from a global list
        /// </summary>
        /// <returns>A view mixing partial views of a category view and a global list view</returns>
        public ActionResult ManualPerformance()
        {
            //Ennumerate questions, ordered by priority
            //Take(50) or so
            var questions = _questionsController.RetrieveQuestions(80, 120);
            return View(questions);
        }
        
        /// <summary>
        /// Performa a Single Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Task(string questionID, string question, string questionType)
        {
            var tasks = new PerformTaskTypes();
            int qType = Int32.Parse(questionType);

            string ViewName = tasks.Tasks[qType].ViewName;
            var model = new PerformTask();
            model.questionID = Int32.Parse(questionID);
            model.Prompt = question;
            model.numberOfAnswers = _answersController.RetrieveNumberOfAnswers(model.questionID); //this will need to call the graphdb to find the number of totalSubIDs, if none then 1, so default to 1.
            return View("Task/" + ViewName, model);
        }

    }
}
