using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace learnyesensmarter.Interfaces
{
    public interface IPriorityUpdater
    {
        float UpdatePriority(int questionID, float priority);
    }
}
