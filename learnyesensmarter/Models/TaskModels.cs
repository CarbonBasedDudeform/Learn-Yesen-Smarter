using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
}
