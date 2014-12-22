using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using learnyesensmarter.Models;

namespace learnyesensmarter.Controllers
{
    public class AuthorController : Controller
    {
        //
        // GET: /Author/

        public ActionResult Index()
        {
            return View(new TaskModels());
        }

        public ActionResult NewTask(string question_view_name)
        {
            if (String.IsNullOrEmpty(question_view_name)) throw new Exception("Unlogged exception in AuthorController - NewTask() : Empty String");

            try
            {
                return View(question_view_name);
            }
            catch (Exception e)
            {
                throw new Exception("Unlogged exception in AuthorController - NewTask() : " + e.Message);
                //return Error View
            }
        }
    }
}
