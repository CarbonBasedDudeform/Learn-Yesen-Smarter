using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.IO;

using Newtonsoft.Json;
using learnyesensmarter.Models;

namespace learnyesensmarter.Controllers
{
    public class AuthorController : Controller
    {
        //
        // GET: /Author/

        public ActionResult Index()
        {
            return View(new AuthorTaskModel());
        }

        /// <summary>
        /// Takes a string with information about the new task and redirects to the correct view page for it.
        /// </summary>
        /// <param name="question">string containing a serialized QuestionType object</param>
        /// <returns></returns>
        public ActionResult NewTask(string question)
        {
            if (String.IsNullOrEmpty(question)) throw new Exception("Unlogged exception in AuthorController - NewTask() : Empty String");

            try
            {
                var questionType = JsonConvert.DeserializeObject<QuestionType>(question);
                return View(questionType.ViewName);
            }
            catch (Exception e)
            {
                throw new Exception("Unlogged exception in AuthorController - NewTask() : " + e.Message);
                //return Error View
            }
        }

        [HttpPost]
        public ActionResult AuthorNewCommand()
        {
            return View();
        }

    }
}
