using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;
using learnyesensmarter.Proxys;

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
        /// <param name="categoryRetriever">Class responsible for retrieving category information.</param>
        /// <remarks>
        /// The parameters default to null because C# requires compile-time constants.
        /// To use the default database, call QuestionsController() without paramters.
        /// Easiest to use by using named parameters eg. questionInserter: myQuestionInserter
        /// </remarks>
        public QuestionsController(IQuestionRetriever questionRetriever = null, IQuestionInserter questionInserter = null, ICategoryRetriever categoryRetriever = null, ICategoryInserter categoryInserter = null)
        {
            _questionRetriever = questionRetriever;
            _questionInserter = questionInserter;
            _categoryRetriever = categoryRetriever;
            _categoryInserter = categoryInserter;
        }

        /// <summary>
        /// Empty Constructor uses the default values
        /// </summary>
        public QuestionsController()
        {
            var dbproxy = new DatabaseProxy();
            //question interfaces
            _questionRetriever = dbproxy;
            _questionInserter = dbproxy;

            //category interfaces
            _categoryRetriever = dbproxy;
            _categoryInserter = dbproxy;
        }

        //
        // GET: /Questions/

        public ActionResult Index()
        {
            return View();
        }

        #region Question Retrieval

        private IQuestionRetriever _questionRetriever;

        /// <summary>
        /// Retrieves the text representation of the Question.
        /// </summary>
        /// <param name="id">The Question ID.</param>
        /// <returns>The view</returns>
        public ActionResult Retrieve(int id)
        {
            //check it's a valid ID
            if (id < 0) return View("Error");

            ViewBag.Result = _questionRetriever.RetrieveQuestion(id);
            return View();
        }

        //possibly refactor category related code into a category controller?
        private ICategoryRetriever _categoryRetriever;

        /// <summary>
        /// Obtains the Id of a category specified by Name
        /// </summary>
        /// <param name="category">The name of the category</param>
        /// <returns>An Integer representation of the Category</returns>
        public int RetrieveCategoryID(string category)
        {
            return _categoryRetriever.RetrieveCategoryID(category);
        }

        /// <summary>
        /// Obtains the name of the category from its ID
        /// </summary>
        /// <param name="id">The ID of the category</param>
        /// <returns>The Name of the Category</returns>
        public string RetrieveCategory(int id)
        {
            return _categoryRetriever.RetrieveCategory(id);
        }

        #endregion

        #region Question Insertion

        private ICategoryInserter _categoryInserter;

        public int InsertCategory(string name)
        {
            return _categoryInserter.InsertCategory(name);
        }

        private IQuestionInserter _questionInserter;

        /// <summary>
        /// Stores a question along with relevant information such as its category
        /// </summary>
        /// <param name="question">Model of a question including the string representation of the question, the category ID, and the category name </param>
        /// <returns>The View</returns>
        public int Insert(QuestionModel question)
        {
            return _questionInserter.Insert(question);
        }

        #endregion
    }
}
