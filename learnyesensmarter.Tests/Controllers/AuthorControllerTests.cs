using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Neo4jClient;

using learnyesensmarter;
using learnyesensmarter.Controllers;
using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;

namespace learnyesensmarter.Tests.Controllers
{
    [TestClass]
    public class AuthorControllerTest
    {
        class MockInserter : IQuestionInserter
        {
            public string Result = "FAIL";
            public int Insert(QuestionModel question)
            {
                Result = question.Question;
                return -1;
            }
        }


        //the point of these tests is to just ensure they are returning a view as expected.
        [TestMethod]
        public void Author_Index_Returns_View()
        {
            // Arrange
            //create mock inserter
            MockInserter mock = new MockInserter(); //used throughout to force the authorcontroller to use the testing version of questioncontroller, otherwise exceptions are thrown
            QuestionsController qc = new QuestionsController(questionInserter: mock);
            var controller = new AuthorController(qc);

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Author_NewTask_Returns_View()
        {
            //create mock inserter
            MockInserter mock = new MockInserter();
            QuestionsController qc = new QuestionsController(questionInserter: mock);
            var controller = new AuthorController(qc);
            ViewResult result = controller.NewTask(JsonConvert.SerializeObject(new CommandQuestion())) as ViewResult;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "An empty string was inappropriately allowed.")]
        public void Author_NewTask_Empty_string_throws_exception()
        {
            //create mock inserter
            MockInserter mock = new MockInserter();
            QuestionsController qc = new QuestionsController(questionInserter: mock);
            var controller = new AuthorController(qc);
            ViewResult result = controller.NewTask(String.Empty) as ViewResult;
        }

        [TestMethod]
        public void Author_AuthorNewCommand_enters_question_into_question_db()
        {
            //create mock inserter
            MockInserter mock = new MockInserter();
            QuestionsController qc = new QuestionsController(questionInserter: mock);
            var controller = new AuthorController(qc);
            string expected_question = "this is a test?";
            ViewResult response = controller.AuthorNewCommand(expected_question, "could be anything, null, test, ear elephant") as ViewResult;
            //test to see that the controller is calling the insert method.
            //the result field of the mock object should be the same as the given question.
            Assert.AreEqual(mock.Result, expected_question);
        }

        class Mockdb : IAnswerInserter
        {
            public int InsertAnswer(AnswerModel answer)
            {
                Result += answer.CypherQuery.QueryText;
                return -1;
            }

            public string Result = "";
        }


        [TestMethod]
        public void Author_AuthorNewCommand_enters_answer_into_answer_db()
        {
            var mock = new Mockdb();
            var answerController = new AnswersController(answerInserter: mock);
            var controller = new AuthorController(AltAnswersController: answerController);

            controller.AuthorNewCommand("test prompt", "test answer");

            Assert.AreEqual("pass", mock.Result);
        }

        //should output the correct cypher query for the pros+cons in that format
        [TestMethod]
        public void Author_AuthorNewProsAndCons_ouputs_correct_cypher()
        {
            var mock = new Mockdb();
            var inserter_mock = new MockInserter();
            var answerController = new AnswersController(answerInserter: mock);
            var questionsController = new QuestionsController(questionInserter: inserter_mock);
            var controller = new AuthorController(AltAnswersController: answerController, AltQuestionsController: questionsController);

            string pros = "cat, sat, mat -SEPARATOR- dog, slept, floor";
            string cons = "mouse, run up, tree -SEPARATOR- kangaroo, jumps";
            controller.AuthorNewProsAndCons("test prompt", pros, cons);

            string expected_cypher_query = "create (:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans0} })-[:PRO]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans1} })-[:PRO]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans2} })"
                                         + "create (:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans0} })-[:PRO]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans1} })-[:PRO]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans2} })"
                                         + "create (:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans0} })-[:CON]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans1} })-[:CON]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans2} })"
                                         + "create (:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans0} })-[:CON]->(:Answer { questionID: {qID}, subID: {subID}, totalSubs: {totalSub}, Answer: {ans1} })";

            Assert.AreEqual(expected_cypher_query, mock.Result);
        }

        [TestMethod]
        public void Author_AuthorNewExplanation_outputs_correct_cypher()
        {
        }

        [TestMethod]
        public void Author_AuthorNewExplanation_inserts_correct_prompt()
        {
        }

        const string FailureViewName = "Task/Result/Failure";
        [TestMethod]
        public void Author_AuthorNewReview_insert_doesnt_allow_empty()
        {
            //create mock inserter
            MockInserter mock = new MockInserter();
            QuestionsController qc = new QuestionsController(questionInserter: mock);
            var controller = new AuthorController(qc);
            var result = controller.AuthorNewReview("");

            Assert.AreEqual(result.ViewName, FailureViewName);
        }
    }
}
