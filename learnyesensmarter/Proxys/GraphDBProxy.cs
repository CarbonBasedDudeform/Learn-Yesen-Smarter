using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

using Neo4jClient;
//using Newtonsoft.Json;

using learnyesensmarter.Interfaces;
using learnyesensmarter.Models;

namespace learnyesensmarter.Proxys
{
    public class GraphDBProxy : IAnswerInserter, IAnswerRetriever
    {
        //server info split between string and uri class to aid readability
        private const string SERVER_ADDRESS = "http://localhost:7474/db/data";
        private static Uri SERVER_URI = new Uri(SERVER_ADDRESS);
        private GraphClient _client = new GraphClient(SERVER_URI);

        /// <summary>
        /// Inserts an Answer into the Neo4j Graph Database
        /// </summary>
        /// <param name="answer">Answer Model containing information on the answer</param>
        /// <returns>The Question ID associated with the answer.</returns>
        /// <remarks>Question ID is used instead of an independent Answer ID because anything that could be achieved
        /// with the answer ID should be achievable with the Question ID. Both are seperate and only used for inter communications.
        /// So they can be used for different purposes internally. Having an Answer ID would only increase the sql database size and work.</remarks>
        public int InsertAnswer(AnswerModel answer)
        {
            try
            {
                _client.Connect();
                //execute the answer models query
                ((IRawGraphClient)_client).ExecuteCypher(answer.CypherQuery);
                //after completion return the id of the question/answer
                return answer.QuestionID;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Retrieves the answers from the Graph Database
        /// </summary>
        /// <typeparam name="T">The Answer Model the graph nodes should bind to.</typeparam>
        /// <param name="question_id">The ID of the Question Associated with the Answer</param>
        /// <returns>JSON of the IEnumerable<Node<t> type containing the answers</returns>
        public string RetrieveAnswer<T>(int question_id)
        {
            try
            {
                _client.Connect();

                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                              .Where("n.questionID = " + question_id)
                              .Return<Node<T>>("n");

                DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(T));
                MemoryStream stream = new MemoryStream();
                foreach (var result in answers.Results)
                {
                    serialiser.WriteObject(stream, result.Data);
                }

                //move back to the start of the stream before reading
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                string JSONResults = reader.ReadToEnd();
                return JSONResults;
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }

        public string RetrieveMultipleAnswer<T>(int question_id)
        {
            _client.Connect();

            var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                          .Where("n.questionID = " + question_id)
                          .Return<Node<T>>("n");

            DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(List<T>));
            MemoryStream stream = new MemoryStream();
            var myList = new List<T>();
            foreach (var result in answers.Results)
            {
                myList.Add(result.Data);
            }
            serialiser.WriteObject(stream, myList);
            //move back to the start of the stream before reading
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string JSONResults = reader.ReadToEnd();
            return JSONResults;
        }

        const int DEFAULT_NUMBER_OF_ANSWERS = 1;
        public int RetrieveNumberOfAnswers(int question_id)
        {
            int result = DEFAULT_NUMBER_OF_ANSWERS;
            try
            {
                _client.Connect();
                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                                            .Where("n.questionID = " + question_id)
                                            .Return<int?>("n.totalSubs");

                //using this result way of doing it rather than returning here because otherwise an exception gets thrown
                //trying to catch the exception here doesn't work for some reason
                if (answers.Results.Count() > 0)
                {
                    //casting here rather than in cypher query because an exception gets thrown that we're not allowed to handle if null is returned
                    //and attempted to be cast to int (non-nullable type)
                    var what = answers.Results.ElementAt(0);
                    if (what != null) result = (int)what;
                }
            }
            catch (AggregateException e)
            {
                //this should be better exception handling than just a vague catch all
                //log error here
                Console.WriteLine(e.Message);
                throw e;
            }

            return result;
        }

        public int RetrieveNumberOfAnswers(int question_id, string relationship)
        {
            int result = DEFAULT_NUMBER_OF_ANSWERS;
            try
            {
                _client.Connect();
                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                                            .Match("(_)-["+relationship+"]-(n)")
                                            .Where("n.questionID = " + question_id)
                                            .Return<int?>("n.totalSubs");

                //using this result way of doing it rather than returning here because otherwise an exception gets thrown
                //trying to catch the exception here doesn't work for some reason
                if (answers.Results.Count() > 0)
                {
                    //casting here rather than in cypher query because an exception gets thrown that we're not allowed to handle if null is returned
                    //and attempted to be cast to int (non-nullable type)
                    var what = answers.Results.ElementAt(0);
                    if (what != null) result = (int)what;
                }
            }
            catch (Exception e)
            {
                //this should be better exception handling than just a vague catch all
                //log error here
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public int RetrieveNumberOfCons(int question_id)
        {
            int result = DEFAULT_NUMBER_OF_ANSWERS;
            try
            {
                _client.Connect();
                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                                            .Where("n.questionID = " + question_id + " AND n.IsPro = false")
                                            .Return<int?>("n.totalSubs");

                //using this result way of doing it rather than returning here because otherwise an exception gets thrown
                //trying to catch the exception here doesn't work for some reason
                if (answers.Results.Count() > 0)
                {
                    //casting here rather than in cypher query because an exception gets thrown that we're not allowed to handle if null is returned
                    //and attempted to be cast to int (non-nullable type)
                    var what = answers.Results.ElementAt(0);
                    if (what != null) result = (int)what;
                }
            }
            catch (Exception e)
            {
                //this should be better exception handling than just a vague catch all
                //log error here
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public int RetrieveNumberOfPros(int question_id)
        {
            int result = DEFAULT_NUMBER_OF_ANSWERS;
            try
            {
                _client.Connect();
                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                                            .Where("n.questionID = " + question_id + " AND n.IsPro = true")
                                            .Return<int?>("n.totalSubs");

                //using this result way of doing it rather than returning here because otherwise an exception gets thrown
                //trying to catch the exception here doesn't work for some reason
                if (answers.Results.Count() > 0)
                {
                    //casting here rather than in cypher query because an exception gets thrown that we're not allowed to handle if null is returned
                    //and attempted to be cast to int (non-nullable type)
                    var what = answers.Results.ElementAt(0);
                    if (what != null) result = (int)what;
                }
            }
            catch (Exception e)
            {
                //this should be better exception handling than just a vague catch all
                //log error here
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public int RetrieveNumberOfRows(int question_id)
        {
            int result = DEFAULT_NUMBER_OF_ANSWERS;
            try
            {
                _client.Connect();
                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                                            .Where("n.questionID = " + question_id)
                                            .Return<int?>("n.totalRows");

                //using this result way of doing it rather than returning here because otherwise an exception gets thrown
                //trying to catch the exception here doesn't work for some reason
                if (answers.Results.Count() > 0)
                {
                    //casting here rather than in cypher query because an exception gets thrown that we're not allowed to handle if null is returned
                    //and attempted to be cast to int (non-nullable type)
                    var what = answers.Results.ElementAt(0);
                    if (what != null) result = (int)what;
                }
            }
            catch (Exception e)
            {
                //this should be better exception handling than just a vague catch all
                //log error here
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public int RetrieveNumberOfCols(int question_id)
        {
            int result = DEFAULT_NUMBER_OF_ANSWERS;
            try
            {
                _client.Connect();
                var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                                            .Where("n.questionID = " + question_id)
                                            .Return<int?>("n.totalCols");

                //using this result way of doing it rather than returning here because otherwise an exception gets thrown
                //trying to catch the exception here doesn't work for some reason
                if (answers.Results.Count() > 0)
                {
                    //casting here rather than in cypher query because an exception gets thrown that we're not allowed to handle if null is returned
                    //and attempted to be cast to int (non-nullable type)
                    var what = answers.Results.ElementAt(0);
                    if (what != null) result = (int)what;
                }
            }
            catch (Exception e)
            {
                //this should be better exception handling than just a vague catch all
                //log error here
                Console.WriteLine(e.Message);
            }

            return result;
        }
    }
}