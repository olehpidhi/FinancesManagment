using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinancesManagment.Controllers;
using FinancesManagment.Models;
using System.Web.Mvc;

namespace FinancesManagmentUnitTest
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var controller = new HomeController();
            var result = controller.Index() as ActionResult;
        }
    }
}
