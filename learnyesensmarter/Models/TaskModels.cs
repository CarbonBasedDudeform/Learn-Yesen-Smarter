using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace learnyesensmarter.Models
{
    public class AuthorTaskModel
    {
        public List<QuestionType> TypesOfQuestions = new List<QuestionType>()
                                              {
                                                  new ReviewQuestion(),
                                                  new ProsAndConsQuestion(),
                                                  new LabelTheDiagramQuestion(),
                                                  new ExplanationQuestion(),
                                                  new CommandQuestion(),
                                                  new TableQuestion()
                                              };
        public QuestionType question;
    }

    #region Questions

    public class QuestionType
    {
        public virtual string DisplayName { get; set; }
        public virtual string ViewName { get; set; }
        public virtual int ID { get; set; }
    }

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //!!!!!!!!!!    EXTREMELY IMPORTANT THESE DON'T CHANGE AS THEY MATCH UP WITH INTS STORED IN THE DATABASE    !!!!!!!!!!!!!!!!
    public enum QuestionTypeIDs { REVIEW, PROSANDCONS, LABELTHEDIAGRAM, EXPLANATION, COMMAND, TABLE};
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class PerformTaskTypes
    {
        public Dictionary<int, QuestionType> Tasks = new Dictionary<int, QuestionType>()
        {
            {(int)QuestionTypeIDs.REVIEW, new ReviewQuestion()},
            {(int)QuestionTypeIDs.PROSANDCONS, new ProsAndConsQuestion()},
            {(int)QuestionTypeIDs.LABELTHEDIAGRAM, new LabelTheDiagramQuestion()},
            {(int)QuestionTypeIDs.EXPLANATION, new ExplanationQuestion()},
            {(int)QuestionTypeIDs.COMMAND, new CommandQuestion()},
            {(int)QuestionTypeIDs.TABLE, new TableQuestion()}
        };
    }

    public class PerformTask
    {
        public int questionID { get; set; }
        public string Prompt { get; set; }
        public int numberOfAnswers { get; set; }
    }

    public class ReviewQuestion : QuestionType
    {
        public override string DisplayName { get { return "Review"; } }
        public override string ViewName { get { return "Review"; } }
        public override int ID { get { return (int)QuestionTypeIDs.REVIEW; } }
    }

    public class ProsAndConsQuestion : QuestionType
    {
        public override string DisplayName { get { return "Pros and Cons"; } }
        public override string ViewName { get { return "ProsandCons"; } }
        public override int ID { get { return (int)QuestionTypeIDs.PROSANDCONS; } }
    }

    public class LabelTheDiagramQuestion : QuestionType
    {
        public override string DisplayName { get { return "Label The Diagram"; } }
        public override string ViewName { get { return "LabeltheDiagram"; } }
        public override int ID { get { return (int)QuestionTypeIDs.LABELTHEDIAGRAM; } }
    }

    public class ExplanationQuestion : QuestionType
    {
        public override string DisplayName { get { return "Explanation"; } }
        public override string ViewName { get { return "Explanation"; } }
        public override int ID { get { return (int)QuestionTypeIDs.EXPLANATION; } }
    }

    public class CommandQuestion : QuestionType
    {
        public override string DisplayName { get { return "Command"; } }
        public override string ViewName { get { return "Command"; } }
        public override int ID { get { return (int)QuestionTypeIDs.COMMAND; } }
    }

    public class TableQuestion : QuestionType
    {
        public override string DisplayName { get { return "Fill in the Table"; } }
        public override string ViewName { get { return "Table"; } }
        public override int ID { get { return (int)QuestionTypeIDs.TABLE; } }
    }

    #endregion

    public class QuestionModel
    {
        public string Question { get; set; }
        public int QuestionType { get; set; }
    }

    public class CategoryModel
    {
        public string Category { get; set; }
        public int CategoryID { get; set; }
        public int QuestionID { get; set; }
    }

    public class AnswerModel
    {
        public Neo4jClient.Cypher.CypherQuery CypherQuery { get; set; }
        public int QuestionType { get; set; }
        public int QuestionID { get; set; }
    }

    public class GraphDBCommandAnswerModel
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
    }

    public class GridModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Val { get; set; }
    }

    public class RetrieveTableAnswerModel
    {
        public string Answer {get;set;}
        public int questionID {get;set;}
        public int subID {get;set;}
        public int totalSubs {get;set;}
        public int Y {get; set;}
        public int X {get; set;}
    }

    [DataContract]
    public class CommandAnswer
    {
        [DataMember]
        public int questionID { get; set; }
        [DataMember]
        public string Answer { get; set; }
    }

    [DataContract]
    public class ExplanationAnswer
    {
        [DataMember]
        public int questionID { get; set; }
        [DataMember]
        public string Answer { get; set; }
        [DataMember]
        public int subID { get; set; }
        [DataMember]
        public int totalSubs { get; set; }
    }

    public class QuestionPerformModel
    {
        public int questionID { get; set; }
        public int questionType { get; set; }
        public string question { get; set; }
    }
}
