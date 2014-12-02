using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace learnyesensmarter.Interfaces
{
    public interface IQuestionRetriever
    {
        string RetrieveQuestion(int ID);
    }
}