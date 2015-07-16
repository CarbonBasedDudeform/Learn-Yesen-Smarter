using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using learnyesensmarter;
using learnyesensmarter.Controllers;
using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;

namespace learnyesensmarter.Tests.Controllers
{
    [TestClass]
    public class QuestionsControllerTests
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            QuestionsController controller = new QuestionsController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        #region Question Retrieval Tests

        /// <summary>
        /// Dummy Question Retrieval Class for testing the Retrieve Method
        /// </summary>
        public class DummyRetriever : IQuestionRetriever, ICategoryRetriever
        {
            public string RetrieveQuestion(int id)
            {
                return "dummy";
            }

            public QuestionPerformModel[] RetrieveQuestions(int id, int quantity)
            {
                var temp = new QuestionPerformModel[1];
                var tempQPM = new QuestionPerformModel();
                tempQPM.question = "dummy";
                tempQPM.questionID = 1;
                tempQPM.questionType = 1;

                temp[0] = tempQPM;
                return temp;
            }

            public string RetrieveCategory(int id)
            {
                if (id == IDENTITY_ID)
                    return "identity";
                else
                    return "Something else";
            }

            public int RetrieveCategoryID(string cat)
            {
                if (cat.Equals("identity"))
                    return IDENTITY_ID;
                else
                    return 9000;
            }
        }

        [TestMethod]
        public void Retrieve_zero_index()
        {
            //Arrange
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(questionRetriever: dummy);

            //Act
            ViewResult result = controller.Retrieve(0) as ViewResult;

            Assert.AreEqual(dummy.RetrieveQuestion(0), result.ViewBag.Result);
        }

        [TestMethod]
        public void Retrieve_negative_index()
        {
            //Arrange
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(questionRetriever: dummy);

            //Act
            ViewResult result = controller.Retrieve(-1) as ViewResult;
            ViewResult unexpected = controller.Retrieve(0) as ViewResult;

            Assert.AreNotEqual(unexpected.ViewName, result.ViewName);
        }

        const int IDENTITY_ID = 1; 
        [TestMethod]
        public void Retrieve_category_id()
        {
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(categoryRetriever: dummy);

            int result = controller.RetrieveCategoryID("identity");

            Assert.AreEqual(IDENTITY_ID, result);
        }

        [TestMethod]
        public void Retrieve_category_identity_id()
        {
            DummyRetriever dummy = new DummyRetriever();
            QuestionsController controller = new QuestionsController(categoryRetriever: dummy);

            string result = controller.RetrieveCategory(IDENTITY_ID);

            Assert.AreEqual(result, "identity");
        }
        #endregion

        #region Question Insertion Tests

        [TestMethod]
        public void Insert_category_identity_name()
        {
            DummyInserter dummy = new DummyInserter();
            QuestionsController controller = new QuestionsController(categoryInserter: dummy);
            int result = controller.InsertCategory("identity");

            Assert.AreEqual(result, IDENTITY_ID);
        }

        public class DummyInserter : IQuestionInserter, ICategoryInserter
        {
            public int Insert(QuestionModel question)
            {
                return question.Question.Length;
            }

            public int InsertCategory(string name)
            {
                if (name == "identity") return IDENTITY_ID;
                else return 9000;
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
            QuestionsController controller = new QuestionsController(questionInserter: dummy);
            QuestionModel question = new QuestionModel();
            question.Question = "this is a question?";

            //Act
            int result = controller.Insert(question); 
            
            Assert.AreEqual(question.Question.Length, result);
        }

        #endregion
    }
}
