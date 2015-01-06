using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using learnyesensmarter;
using learnyesensmarter.Controllers;
using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;

namespace learnyesensmarter.Tests.Controllers
{
    [TestClass]
    public class AuthorControllerTest
    {
        //the point of these tests is to just ensure they are returning a view as expected.
        [TestMethod]
        public void Author_Index_Returns_View()
        {
            // Arrange
            var controller = new AuthorController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Author_NewTask_Returns_View()
        {
            var controller = new AuthorController();
            ViewResult result = controller.NewTask(JsonConvert.SerializeObject(new CommandQuestion())) as ViewResult;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "An empty string was inappropriately allowed.")]
        public void Author_NewTask_Empty_string_throws_exception()
        {
            var controller = new AuthorController();
            ViewResult result = controller.NewTask(String.Empty) as ViewResult;
        }

        class MockInserter : IQuestionInserter
        {
            public string Result = "FAIL";
            public string Insert(QuestionModel question)
            {
                Result = question.Question;
                return Result;
            }
        }

        [TestMethod]
        public void Author_AuthorNewCommand_enters_question_into_question_db()
        {
            //create mock inserter
            MockInserter mock = new MockInserter();
            var controller = new AuthorController();
            string expected_question = "this is a test?";
            ViewResult response = controller.AuthorNewCommand() as ViewResult;
            //test to see that the controller is calling the insert method.
            //the result field of the mock object should be the same as the given question.
            Assert.AreEqual(mock.Result, expected_question);
        }
    }
}
