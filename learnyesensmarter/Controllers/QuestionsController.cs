using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace learnyesensmarter.Controllers
{
    /// <summary>
    /// This is the default retriever used by the Question Controller for retrieving questions
    /// </summary>
    public class DefaultRetriever : IQuestionRetrieval
    {
        public DefaultRetriever()
        {
            //Set Default Values
            Source = "test";
            Query = "testQuery";
        }

        public string Source { get; set; }
        public string Query { get; set; }
    }

    public class QuestionsController : Controller
    {
        /// <summary>
        /// Question controller constructor for dependency injection.
        /// This allows for testing the question controller class without actually talking to a database.
        /// It also allows, if needs be in the future, to talk to different databases other than the default one
        /// by creating various implementations of IQuestionRetrieval
        /// </summary>
        /// <param name="questionRetriever">Class containing information on where to get the question from.</param>
        public QuestionsController(IQuestionRetrieval questionRetriever)
        {
            _questionRetriever = questionRetriever;
        }

        /// <summary>
        /// Empty Constructor uses the default values
        /// </summary>
        public QuestionsController()
        {
            _questionRetriever = new DefaultRetriever();
        }

        //
        // GET: /Questions/

        public ActionResult Index()
        {
            return View();
        }

        private IQuestionRetrieval _questionRetriever;

        public ActionResult Retrieve(int id)
        {
            if (id < 0) return View("Error");

            ViewBag.Source = _questionRetriever.Source;
            return View();
        }
    }
}
