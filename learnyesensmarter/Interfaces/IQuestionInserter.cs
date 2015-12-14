using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using learnyesensmarter.Models;

namespace learnyesensmarter.Interfaces
{
    public interface IQuestionInserter
    {
        int Insert(QuestionModel question);
    }
}