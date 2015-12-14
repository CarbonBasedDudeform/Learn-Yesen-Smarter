using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using learnyesensmarter.Models;

namespace learnyesensmarter.Interfaces
{
    public interface IAnswerRetriever
    {
        string RetrieveAnswer<T>(int question_id);
        string RetrieveMultipleAnswer<T>(int question_id);
        int RetrieveNumberOfAnswers(int question_id);
        int RetrieveNumberOfCons(int question_id);
        int RetrieveNumberOfPros(int question_id);
        int RetrieveNumberOfCols(int question_id);
        int RetrieveNumberOfRows(int question_id);
    }
}