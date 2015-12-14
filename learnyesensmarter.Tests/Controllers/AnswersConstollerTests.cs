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
    public class AnswersControllerTest
    {
        const int FIXED_ANSWER = -3142;

        class DummyInserter : IAnswerInserter
        {
            public int InsertAnswer(AnswerModel model)
            {
                return FIXED_ANSWER;
            }
        }

        //ensures the insert method is calling the correct logic.
        [TestMethod]
        public void Answers_Inserts_answer()
        {
            //Arrange
            DummyInserter mock = new DummyInserter();
            var controller = new AnswersController(answerInserter: mock);

            //Act
            int result = controller.Insert(new AnswerModel() { QuestionID = 0, QuestionType = 0 });

            Assert.AreEqual(FIXED_ANSWER, result);
        }
    }
}
