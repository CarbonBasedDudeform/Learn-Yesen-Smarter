using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using learnyesensmarter;
using learnyesensmarter.Controllers;

namespace learnyesensmarter.Tests.Controllers
{
    [TestClass]
    public class QuestionsControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            QuestionsController controller = new QuestionsController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNull(result.ViewBag.Message);
        }

        #region Question Retrieval Tests

        /// <summary>
        /// Dummy Question Retrieval Class for testing the Retrieve Method
        /// </summary>
        public class DummyRetriever : IQuestionRetrieval
        {
            public string Retrieve()
            {
                return "dummy";
            }
        }

        [TestMethod]
        public void Retrieve_zero_index()
        {
            //Arrange
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(dummy);

            //Act
            ViewResult result = controller.Retrieve(0) as ViewResult;

            Assert.AreEqual(dummy.Retrieve(), result.ViewBag.Result);
        }

        [TestMethod]
        public void Retrieve_negative_index()
        {
            //Arrange
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(dummy);

            //Act
            ViewResult result = controller.Retrieve(-1) as ViewResult;
            ViewResult unexpected = controller.Retrieve(0) as ViewResult;

            Assert.AreNotEqual(unexpected.ViewName, result.ViewName);
        }

        #endregion

        #region Question Insertion Tests

        public class DummyInserter : IQuestionInserter
        {
            public string Insert(string question)
            {
                return question;
            }
        }

        /// <summary>
        /// This test simply tests that the insert method follows the correct logic of executing a query.
        /// </summary>
        [TestMethod]
        public void Insert_Question()
        {
            //Arrange
            DummyInserter dummy = new DummyInserter();
            QuestionsController controller = new QuestionsController(null, dummy);
            string question = "this is a question?";

            //Act
            ViewResult result = controller.Insert(question) as ViewResult; 
            
            Assert.AreEqual(dummy.Insert(question), result.ViewBag.Result);
        }

        #endregion
    }
}
