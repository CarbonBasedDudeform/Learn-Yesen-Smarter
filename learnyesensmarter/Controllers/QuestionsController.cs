using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace learnyesensmarter.Controllers
{
    public class QuestionsController : Controller, IQuestionRetrieval
    {
        //
        // GET: /Questions/

        public ActionResult Index()
        {
            return View();
        }

    }
}
