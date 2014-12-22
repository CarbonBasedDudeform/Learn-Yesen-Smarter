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
    public class PerformControllerTest
    {
        [TestMethod]
        public void Perform_Index_Returns_View()
        {
            // Arrange
            PerformController controller = new PerformController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Perform_PerformFive_Returns_View()
        {
            var controller = new PerformController();
            var result = controller.PerformFive() as ViewResult;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Perform_ManualPerformance_Returns_View()
        {
            var controller = new PerformController();
            var result = controller.ManualPerformance() as ViewResult;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
