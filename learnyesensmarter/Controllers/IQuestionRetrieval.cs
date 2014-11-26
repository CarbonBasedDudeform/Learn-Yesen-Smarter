using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace learnyesensmarter.Controllers
{
    public interface IQuestionRetrieval
    {
        string Source { get; set; }
        string Query { get; set; }
    }
}