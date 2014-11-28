using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace learnyesensmarter.Controllers
{
    public class DatabaseProxy : IQuestionInserter, IQuestionRetriever
    {
        public string Retrieve()
        {
            return "Default";
        }

        public string Insert(string question)
        {
            return question;
        }
    }
}
