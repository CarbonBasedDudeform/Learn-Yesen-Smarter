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

        public ActionResult NewTask(string question)
        {
            //if (String.IsNullOrEmpty(question_view_name)) throw new Exception("Unlogged exception in AuthorController - NewTask() : Empty String");

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
    }
}
