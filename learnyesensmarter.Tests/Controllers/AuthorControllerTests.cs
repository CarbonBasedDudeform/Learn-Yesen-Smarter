using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using learnyesensmarter;
using learnyesensmarter.Controllers;
using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;

namespace learnyesensmarter.Tests.Controllers
{
    [TestClass]
    public class AuthorControllerTest
    {
        //the point of these tests is to just ensure they are returning a view as expected.
        [TestMethod]
        public void Author_Index_Returns_View()
        {
            // Arrange
            var controller = new AuthorController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Author_NewTask_Returns_View()
        {
            var controller = new AuthorController();
            ViewResult result = controller.NewTask(new CommandQuestion().ViewName) as ViewResult;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "An empty string was inappropriately allowed.")]
        public void Author_NewTask_Empty_string_throws_exception()
        {
            var controller = new AuthorController();
            ViewResult result = controller.NewTask(String.Empty) as ViewResult;
        }
    }
}
