using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace learnyesensmarter.Controllers
{
    public class PerformController : Controller
    {
        //
        // GET: /Perform/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Selects five tasks for the user to perform.
        /// </summary>
        /// <returns>A View of five tasks for the user to perform</returns>
        public ActionResult PerformFive()
        {
            return View();
        }

        /// <summary>
        /// Allows the user to select tasks from categories or from a global list
        /// </summary>
        /// <returns>A view mixing partial views of a category view and a global list view</returns>
        public ActionResult ManualPerformance()
        {
            return View();
        }
    }
}
