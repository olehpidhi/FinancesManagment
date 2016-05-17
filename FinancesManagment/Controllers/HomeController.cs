using FinancesManagment.DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinancesManagment.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userAccounts = unitOfWork.FinancialAccountMembersRepository.Get(m => m.ApplicationUser.Id == userId);
            return View(userAccounts);
        }
    }
}