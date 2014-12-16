using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace learnyesensmarter.Models
{
    public class TaskModels
    {
    }

    public enum QuestionType { REVIEW, PROS_AND_CONS, LABEL_THE_DIAGRAM, EXPLANATION, COMMAND };
    public class QuestionModel
    {
        public string Question { get; set; }
        public string Category { get; set; }
        public int CategoryID { get; set; }
        public QuestionType QuestionType { get; set; }
    }

    public class AnswerModel
    {
        public Neo4jClient.Cypher.CypherQuery CypherQuery { get; set; }
        public QuestionType QuestionType { get; set; }
        public int QuestionID { get; set; }
    }
}