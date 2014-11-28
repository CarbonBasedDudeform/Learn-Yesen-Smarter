using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace learnyesensmarter.Controllers
{
    public class QuestionsController : Controller
    {
        /// <summary>
        /// Question controller constructor for dependency injection.
        /// This allows for testing the question controller class without actually talking to a database.
        /// It also allows, if needs be in the future, to talk to different databases other than the default one
        /// by creating various implementations of IQuestionRetrieval, IQuestionInserter
        /// </summary>
        /// <param name="questionRetriever">Class responsible for retrieving a question and how its retrieved.</param>
        /// <param name="questionInserter">Class responsible for inserting a question, where it gets inserted and how. </param>
        /// <remarks>
        /// The parameters default to null because C# requires compile-time constants.
        /// To use the default database, call QuestionsController() without paramters.
        /// </remarks>
        public QuestionsController(IQuestionRetrieval questionRetriever = null, IQuestionInserter questionInserter = null)
        {
            _questionRetriever = questionRetriever;
            _questionInserter = questionInserter;
        }

        /// <summary>
        /// Empty Constructor uses the default values
        /// </summary>
        public QuestionsController()
        {
            var dbproxy = new DatabaseProxy();
            _questionRetriever = dbproxy;
            _questionInserter = dbproxy;
        }

        //
        // GET: /Questions/

        public ActionResult Index()
        {
            return View();
        }

        #region Question Retrieval

        private IQuestionRetrieval _questionRetriever;

        public ActionResult Retrieve(int id)
        {
            if (id < 0) return View("Error");

            ViewBag.Result = _questionRetriever.Retrieve();
            return View();
        }

        #endregion

        #region Question Insertion

        private IQuestionInserter _questionInserter;

        public ActionResult Insert(string question)
        {
            ViewBag.Result = _questionInserter.Insert(question);
            return View();
        }

        #endregion
    }
}
