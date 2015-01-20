using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

using Neo4jClient;

using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;
using learnyesensmarter.Proxys;

namespace learnyesensmarter.Controllers
{
    public class AnswersController : Controller
    {
        IAnswerInserter _answerInserter;
        IAnswerRetriever _answerRetriever;
        IPriorityUpdater _priorityUpdater;

        public AnswersController()
        {
            var gdbproxy = new GraphDBProxy();
            _answerInserter = gdbproxy;
            _answerRetriever = gdbproxy;

            _priorityUpdater = new DatabaseProxy();
        }

        public AnswersController(IAnswerInserter answerInserter = null, IAnswerRetriever answerRetriever = null, IPriorityUpdater priorityUpdater = null)
        {
            _answerInserter = answerInserter;
            _answerRetriever = answerRetriever;
            _priorityUpdater = priorityUpdater;
        }

        public int Insert(AnswerModel model)
        {
            return _answerInserter.InsertAnswer(model);
        }

        public string Retrieve<T>(int questionID)
        {
            return _answerRetriever.RetrieveAnswer<T>(questionID);
        }

        const float FLOATING_ERROR_ACCOMODATION = 0.0001f;

        [HttpPost]
        public ActionResult VerifyCommand(string qid, string answer)
        {
            int questionID = Int32.Parse(qid);
            //grab answers from graphdb by questionID
            string storedAnswer = _answerRetriever.RetrieveAnswer<CommandAnswer>(questionID);
            //deserialize into a CommandAnswer object
            DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(CommandAnswer));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(storedAnswer));
            CommandAnswer ans = (CommandAnswer)serialiser.ReadObject(stream);

            //match up these answers with the users given answer
             //break up the string into an array
            string[] usersAnswerSegments = answer.Split(' ');
            string[] storedAnswerSegments = ans.Answer.Split(' ');

            //number of matching answer segments found
            int matches = 0;
            foreach (string current in storedAnswerSegments)
            {
                //loop through stored answers and try to find a matching answer given in users
                //currently, this has an appalling performance and won't scale for large answers
                foreach (string userCurrent in usersAnswerSegments)
                {
                    if (userCurrent == current) matches++;
                }
            }

                          //cast everything to float to ensure float division
            float score = (float)matches / (float)storedAnswerSegments.Length;

            //update the priority in the sql db
            float priority = 1 - score; //means worse you do -> higher priority, better you do -> low priority
            _priorityUpdater.UpdatePriority(questionID, priority);

            //pass the correct answers % into the view via model
            if ((1 - score) < FLOATING_ERROR_ACCOMODATION) //can't do if score == 1, because score may actually be 0.99999999, or 1.00000001
            {
                return View("../Perform/Task/Result/Success");
            }
            else
            {
                return View("../Perform/Task/Result/Failure");
            }
        }

        [HttpPost]
        public ActionResult VerifyReview(string questionID)
        {
            //update the priority in the sql database
            int qid = Int32.Parse(questionID);
            _priorityUpdater.UpdatePriority(qid, 0);

            return View("../Perform/Task/Result/Success");
        }

        [HttpPost]
        public ActionResult VerifyExplanation()
        {
            //grab answers from graphdb by questonID
            //match up these answers with the users given answers
            //update the priorit in the sql db
            //pass the correct % answers into the view via model
            return View("../Perform/Task/Result/Failure");
        }

        [HttpPost]
        public ActionResult VerifyLabeltheDiagram()
        {
            //grab answers from graphdb by questionID
            //match up these answers with the users given answers
            //update the priority in the sql db
            //pass the correct % answers into the view via model
            return View("../Perform/Task/Result/Failure");
        }

        [HttpPost]
        public ActionResult VerifyProsandCons()
        {
            //grab answers from graphdb by questionID
            //match up these answers with the users given answers
            //update the priority in the sql db
            //pass the correct % answers into the view via model
            return View("../Perform/Task/Result/Failure");
        }

        [HttpPost]
        public ActionResult VerifyTable()
        {
            //grab answers from graphdb by questionID
            //match up these answers with the users given answers
            //update the priority in the sql db
            //pass the correct % answers into the view via model
            return View("../Perform/Task/Result/Failure");
        }

    }
}
 