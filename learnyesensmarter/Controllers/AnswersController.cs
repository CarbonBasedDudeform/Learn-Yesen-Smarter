﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace learnyesensmarter.Controllers
{
    public class AnswersController : Controller, IAnswerRetrieval
    {
        //
        // GET: /Answers/

        public ActionResult Index()
        {
            return View();
        }

    }
}
 