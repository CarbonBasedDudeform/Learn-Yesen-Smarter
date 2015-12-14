using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace learnyesensmarter.Interfaces
{
    public interface ICategoryRetriever
    {
        string RetrieveCategory(int id);

        int RetrieveCategoryID(string category);
    }
}
