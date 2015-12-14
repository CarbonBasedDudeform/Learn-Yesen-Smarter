using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using learnyesensmarter.Models;

namespace learnyesensmarter.Interfaces
{
    public interface IQuestionRetriever
    {
        string RetrieveQuestion(int ID);
        QuestionPerformModel[] RetrieveQuestions(int startID, int quantity);
    }
}