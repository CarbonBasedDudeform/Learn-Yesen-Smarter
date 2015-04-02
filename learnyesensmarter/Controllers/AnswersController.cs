using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Globalization;

using Neo4jClient;
using Newtonsoft.Json;

using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;
using learnyesensmarter.Proxys;

namespace learnyesensmarter.Controllers
{
    public class AnswersController : Controller
    {
        #region Constructors and Properties
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
        #endregion
        #region Mother of all RetrieveNumberOfX functions
        public int RetrieveNumberOfAnswers(int question_id)
        {
            return _answerRetriever.RetrieveNumberOfAnswers(question_id);
        }

        public int RetrieveNumberOfPros(int question_id)
        {
            return _answerRetriever.RetrieveNumberOfPros(question_id);
        }

        public int RetrieveNumberOfCons(int question_id)
        {
            return _answerRetriever.RetrieveNumberOfCons(question_id);
        }

        public int RetrieveNumberOfCols(int question_id)
        {
            return _answerRetriever.RetrieveNumberOfCols(question_id);
        }

        public int RetrieveNumberOfRows(int question_id)
        {
            return _answerRetriever.RetrieveNumberOfRows(question_id);
        }

#endregion

        #region General Insert and Retrieve Functions
        public int Insert(AnswerModel model)
        {
            return _answerInserter.InsertAnswer(model);
        }

        public string Retrieve<T>(int questionID)
        {
            return _answerRetriever.RetrieveAnswer<T>(questionID);
        }

        private void MassPriorityUpdate()
        {

        }

        #endregion

        #region Find Methods used for Finding Matches

        const int EQUAL = 0;
        public int FindNumberOfMatches(string[] subject, string[] target)
        {
            //make lower case & remove whitespace
            for (int i = 0; i < subject.Length; i++) { subject[i] = subject[i].ToLower(); subject[i] = subject[i].Trim(); }
            for (int i = 0; i < target.Length; i++)  { target[i] = target[i].ToLower(); target[i] = target[i].Trim(); }
            
            int result = 0;
            foreach (string currentSubject in subject)
            {
                foreach (string currentTarget in target)
                {
                    //compares strings ignoring casec
                    if (string.Compare(currentSubject, currentTarget, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase ) == EQUAL)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public int FindBestMatchMultiple(string[] subject, Dictionary<int, List<string>> target)
        {
            int totalMatches = 0;
            foreach (var cur_sub in subject)
            {
                int currentBestMatchIndex = -1;
                int currentBestMatch = 0;

                var sub_segments = cur_sub.Split(' ');

                for (int i = 0; i < target.Count; i++)
                {
                    int matches = FindNumberOfMatches(sub_segments, target.ElementAt(i).Value.ToArray());
                    if (matches > currentBestMatch)
                    {
                        currentBestMatch = matches;
                        currentBestMatchIndex = i;
                    }
                }

                //then remove the most similar stored answer from the array so that users
                //cannot simply enter the same answer 5 times, for example, and get a perfect score while neglecting the 4 other answers
                target.Remove(currentBestMatchIndex);
                totalMatches += currentBestMatch;
            }

            return totalMatches;
        }

        #endregion

        #region Verify Functions
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
            int matches = FindNumberOfMatches(usersAnswerSegments, storedAnswerSegments);

                          //cast everything to float to ensure float division
            float score = (float)matches / (float)storedAnswerSegments.Length;

            //update the priority in the sql db
            float priority = 1 - score; //means worse you do -> higher priority, better you do -> low priority
            _priorityUpdater.UpdatePriority(questionID, priority);

            //pass the correct answers % into the view via model
            var results = new ResultModel();
            results.score = score;
            //horrible hacky conversion
            var temp = new Dictionary<int, List<string>>();
            temp[0] = storedAnswerSegments.ToList();
            results.correctAnswers = temp;
            if ((1 - score) < FLOATING_ERROR_ACCOMODATION) //can't do if score == 1, because score may actually be 0.99999999, or 1.00000001
            {
                return View("../Perform/Task/Result/Success", results);
            }
            else
            {
                return View("../Perform/Task/Result/Failure", results);
            }
        }

        [HttpPost]
        public ActionResult VerifyReview(string questionID)
        {
            //A review task is just something the user is shown to 'review' by reading, therefore being shown it means they
            //score 100%, no need to verify anything just
            //update the priority in the sql database
            int qid = Int32.Parse(questionID);
            _priorityUpdater.UpdatePriority(qid, 0);

            return View("../Perform/Task/Result/Success");
        }

        [HttpPost]
        public ActionResult VerifyExplanation(string qid, string collatedAnswers)
        {
            int questionID = Int32.Parse(qid);
            //grab answers from graphdb by questionID
            string storedAnswer = _answerRetriever.RetrieveMultipleAnswer<ExplanationAnswer>(questionID);
            //deserialize into a CommandAnswer object
            DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(List<ExplanationAnswer>));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(storedAnswer));
            var stored_ans_uncollated = (List<ExplanationAnswer>)serialiser.ReadObject(stream);
            var stored_ans = new Dictionary<int, List<string>>(); //int = subID, string[] = answers;
            //Join answers with matching subIDs
                //create a list for each subID
            foreach (var cur in stored_ans_uncollated)
            {
                stored_ans[cur.subID] = new List<string>();
            }

            //add answers to the appropriate list -- so all answers with the same subId are on the same list
            foreach (var cur in stored_ans_uncollated)
            {
                stored_ans[cur.subID].Add(cur.Answer);
            }

            //reuse json serialiser for the users given answers
            serialiser = new DataContractJsonSerializer(typeof(string[]));
            stream = new MemoryStream(Encoding.UTF8.GetBytes(collatedAnswers));
            var users_ans = (string[])serialiser.ReadObject(stream);

            //match up these answers with the users given answers
            //this is computationally expensive and won't scale but it'll do for now, just to get it working

            //for each user given answer, try to find a stored answer which it matches up with
            int totalMatches = FindBestMatchMultiple(users_ans, stored_ans);         
            //calculate total number of stored answer segments
            
            float score = 0.0f;
            int totalAnswerSegments = stored_ans_uncollated.Count;
            if (totalAnswerSegments >= 0) score = (float)totalMatches / (float)totalAnswerSegments;
            
            float priority = 1 - score;
            //update the priorit in the sql db
            _priorityUpdater.UpdatePriority(questionID, priority);
            //pass the correct answers % into the view via model
            var results = new ResultModel();
            results.score = score;
            results.correctAnswers = stored_ans;
            if ((1 - score) < FLOATING_ERROR_ACCOMODATION) //can't do if score == 1, because score may actually be 0.99999999, or 1.00000001
            {
                return View("../Perform/Task/Result/Success", results);
            }
            else
            {
                return View("../Perform/Task/Result/Failure", results);
            }
        }

        [HttpPost]
        public ActionResult VerifyLabeltheDiagram()
        {
            throw new Exception("This task type is unsupported/unimplemented at the moment");
            //grab answers from graphdb by questionID
            //match up these answers with the users given answers
            //update the priority in the sql db
            //pass the correct % answers into the view via model
            return View("../Perform/Task/Result/Failure");
        }

        [HttpPost]
        public ActionResult VerifyProsandCons(string qid, string collatedPros, string collatedCons)
        {
            int questionID = Int32.Parse(qid);
            //grab answers from graphdb by questionID
            string storedAnswer = _answerRetriever.RetrieveMultipleAnswer<ProsandConsAnswer>(questionID);
            //deserialize into a CommandAnswer object
            DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(List<ProsandConsAnswer>));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(storedAnswer));
            var stored_ans_uncollated = (List<ProsandConsAnswer>)serialiser.ReadObject(stream);
            var stored_pros = new Dictionary<int, List<string>>(); //int = subID, List = answers;
            var stored_cons = new Dictionary<int, List<string>>(); //int = subID, List = answers;
            //Join answers with matching subIDs
            foreach (var cur in stored_ans_uncollated)
            {
                if (cur.IsPro) stored_pros[cur.subID] = new List<string>();
                else stored_cons[cur.subID] = new List<string>();
            }

            //add the answers to the lists

            foreach (var cur in stored_ans_uncollated)
            {
                if (cur.IsPro) stored_pros[cur.subID].Add(cur.Answer);
                else stored_cons[cur.subID].Add(cur.Answer);
            }

            //reuse json serialiser for the users given answers
            serialiser = new DataContractJsonSerializer(typeof(string[]));
            stream = new MemoryStream(Encoding.UTF8.GetBytes(collatedPros));
            var users_pros = (string[])serialiser.ReadObject(stream);
            stream = new MemoryStream(Encoding.UTF8.GetBytes(collatedCons));
            var users_cons = (string[])serialiser.ReadObject(stream);

            //match up these answers with the users given answers
                //similar to explanation answer verification but also needs to ensure that the answer's best match
                //has a pro/con status the same as the stored answer.
            int totalMatches = 0;
            #region Find Pros
            //for each user supplied pro, break it down into segments and then try to find the best match in the stored pros
            totalMatches += FindBestMatchMultiple(users_pros, stored_pros);
            #endregion
            #region Find Cons
            totalMatches += FindBestMatchMultiple(users_cons, stored_cons);
            #endregion

            //stored_ans_uncollated.count is the number of nodes the graphdb returned so it is also the total number of correct answer segments
            int totalAnswers = stored_ans_uncollated.Count;

            float score = 0.0f;
            if (totalAnswers > 0) score = (float)totalMatches / (float)totalAnswers;
            float priority = 1 - score;
            //update the priorit in the sql db
            _priorityUpdater.UpdatePriority(questionID, priority);

            var results = new ResultModel();
            results.score = score;
            results.pros = stored_pros;
            results.cons = stored_cons;

            //pass the correct answers % into the view via model
            if ((1 - score) < FLOATING_ERROR_ACCOMODATION) //can't do if score == 1, because score may actually be 0.99999999, or 1.00000001
            {
                return View("../Perform/Task/Result/Success", results);
            }
            else
            {
                return View("../Perform/Task/Result/Failure", results);
            }
        }

        [HttpPost]
        public ActionResult VerifyTable(string qid, string collatedTable)
        {
            int questionID = Int32.Parse(qid);
            //grab answers from graphdb by questionID
            string storedAnswer = _answerRetriever.RetrieveMultipleAnswer<RetrieveTableAnswerModel>(questionID);
            //deserialize into a CommandAnswer object
            DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(List<RetrieveTableAnswerModel>));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(storedAnswer));
            var stored_ans_uncollated = (List<RetrieveTableAnswerModel>)serialiser.ReadObject(stream);
            var stored_ans = new Dictionary<int, List<string>>(); //int = subID, List = answers;
            //Join answers with matching subIDs
            //create a list for each subID
            foreach (var cur in stored_ans_uncollated)
            {
                stored_ans[cur.subID] = new List<string>();
            }

            //add answers to the appropriate list -- so all answers with the same subId are on the same list
            foreach (var cur in stored_ans_uncollated)
            {
                stored_ans[cur.subID].Add(cur.Answer);
            }
            //reuse json serialiser for the users given answers
            serialiser = new DataContractJsonSerializer(typeof(TableElement[]));
            stream = new MemoryStream(Encoding.UTF8.GetBytes(collatedTable));
            var users_ans = (TableElement[])serialiser.ReadObject(stream);

            //create a string list, so we can easily convert to an array to send to the find method
            //this method assumes we are ignoring the X,Y values - which atm i think will only be used for randomising which values are shown earlier so shouldn't matter
            var users_ans_list = new List<string>();
            foreach (var cur in users_ans)
            {
                users_ans_list.Add(cur.Val);
            }
            //match up these answers with the users given answers
            int matches = FindBestMatchMultiple(users_ans_list.ToArray(), stored_ans);
            int totalAnswers = 0;
            foreach (var cur in stored_ans_uncollated)
            {
                totalAnswers += cur.Answer.Split(',').Length; //holy moly, getting the current answer, splitting it into it's keyword components and then getting the number of components
            }
            float score = 0.0f;
            if (totalAnswers > 0) score = (float)matches / (float)totalAnswers;
            float priority = 1 - score;
            //update the priority in the sql db
            _priorityUpdater.UpdatePriority(questionID, priority);

            //populate model
            var results = new ResultModel();
            results.score = score;
            results.correctAnswers = stored_ans;
            //pass the correct % answers into the view via model
            if ((1 - score) < FLOATING_ERROR_ACCOMODATION) //can't do if score == 1, because score may actually be 0.99999999, or 1.00000001
            {
                return View("../Perform/Task/Result/Success", results);
            }
            else
            {
                return View("../Perform/Task/Result/Failure", results);
            }
        }

        #endregion
    }
}
 