using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinancesManagment.Controllers
{
    public class FinancialAccountController : Controller
    {
        // GET: FinancialAccount
        public ActionResult Index()
        {
            return View();
        }
    }
}