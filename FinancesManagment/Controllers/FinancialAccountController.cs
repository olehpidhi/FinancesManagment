using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinancesManagment.Models;
using FinancesManagment.DAL;

namespace FinancesManagment.Controllers
{
    public class FinancialAccountController : Controller
    {

        private IFinancialAccountRepository financialAccountsRepository;

        public FinancialAccountController()
        {
            this.financialAccountsRepository = new FinancialAccountsRepository(ApplicationDbContext.Create());
        }
        // GET: FinancialAccount
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string Name)
        {
            FinancialAccount newAccount = new FinancialAccount();
            newAccount.Name = Name;
            newAccount.Summary = 0;
            financialAccountsRepository.AddFinancialAccount(newAccount);
            int objectsAdded = financialAccountsRepository.Save();
            if (objectsAdded > 0)
            {
                return Json(new { status = "Finacial account added successfully." });
            }
            return Json(new { status = "Failed to add financial account" });
        }
    }
}