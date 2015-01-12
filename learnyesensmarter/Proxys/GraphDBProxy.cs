using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net.Http;
using System.Net.Http.Headers;

using Neo4jClient;
using Newtonsoft.Json;

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
            _client.Connect();
            //execute the answer models query
            ((IRawGraphClient)_client).ExecuteCypher(answer.CypherQuery);
            //after completion return the id of the question/answer
            return answer.QuestionID;
        }

        /// <summary>
        /// Retrieves the answers from the Graph Database
        /// </summary>
        /// <typeparam name="T">The Answer Model the graph nodes should bind to.</typeparam>
        /// <param name="question_id">The ID of the Question Associated with the Answer</param>
        /// <returns>JSON of the IEnumerable<Node<t> type containing the answers</returns>
        public string RetrieveAnswer<T>(int question_id)
        {
            _client.Connect();

            var answers = _client.Cypher.Start(new { n = Neo4jClient.Cypher.All.Nodes })
                          .Where("n.questionID = " + question_id)
                          .Return<Node<T>>("n");

            string JSONResults = JsonConvert.SerializeObject(answers.Results);
            return JSONResults;
        }
    }
}