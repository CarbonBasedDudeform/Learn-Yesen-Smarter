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

        /// <summary>
        /// Dummy Question Retrieval Class for testing the Retrieve Method
        /// </summary>
        public class DummyRetriever : IQuestionRetrieval
        {
            public DummyRetriever()
            {
                Source = "Dummy Source";
                Query = "Dummy Query";
            }

            public string Source { get; set; }
            public string Query { get; set; }
        }

        [TestMethod]
        public void Retrieve_zero_index()
        {
            //Arrange
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(dummy);

            //Act
            ViewResult result = controller.Retrieve(0) as ViewResult;

            Assert.AreEqual(dummy.Source, result.ViewBag.Source);
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
    }
}
