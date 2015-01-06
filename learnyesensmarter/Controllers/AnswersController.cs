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
    public class AnswersController : Controller
    {
        IAnswerInserter _answerInserter;
        IAnswerRetriever _answerRetriever;

        public AnswersController()
        {
            var gdbproxy = new GraphDBProxy();
            _answerInserter = gdbproxy;
            _answerRetriever = gdbproxy;
        }

        public AnswersController(IAnswerInserter answerInserter = null, IAnswerRetriever answerRetriever = null)
        {
            _answerInserter = answerInserter;
            _answerRetriever = answerRetriever;
        }

        public int Insert(AnswerModel model)
        {
            return _answerInserter.InsertAnswer(model);
        }

    }
}
 