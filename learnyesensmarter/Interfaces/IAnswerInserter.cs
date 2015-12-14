using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using learnyesensmarter.Models;

namespace learnyesensmarter.Interfaces
{
    public interface IAnswerInserter
    {
        int InsertAnswer(AnswerModel answers);
    }
}
