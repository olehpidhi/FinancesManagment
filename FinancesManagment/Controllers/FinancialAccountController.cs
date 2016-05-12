using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinancesManagment.Models;
using FinancesManagment.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FinancesManagment.Controllers
{
    [Authorize]
    public class FinancialAccountController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: FinancialAccount
        public ActionResult Index()
        {
            return View(unitOfWork.FinancialAccountsRepository.Get());
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
            unitOfWork.FinancialAccountsRepository.Insert(newAccount);
            int objectsAdded = unitOfWork.Save();
            FinancialAccountRole ownerRole = unitOfWork.FinancialAccountRolesRepository.Get(r => r.Title == "Owner").FirstOrDefault();
            FinancialAccountMember newMember = new FinancialAccountMember();
            var userId = User.Identity.GetUserId();

            ApplicationUser user = unitOfWork.UserRepository.GetByID(userId);

            newMember.FinancialAccountId = newAccount.Id;
            newMember.FinancialAccountRoleId = ownerRole.Id;
            newMember.ApplicationUserId = userId;
            newMember.ApplicationUser = user;
            
            unitOfWork.FinancialAccountMembersRepository.Insert(newMember);
            objectsAdded += unitOfWork.Save();
            if (objectsAdded == 2)
            {
                return RedirectToAction("Edit",new { Id = newAccount.Id });
            }
            ViewBag.Message = "Failed to creare account";
            return View();
        }

        public ActionResult Edit(int id)
        {
            FinancialAccount account = unitOfWork.FinancialAccountsRepository.GetByID(id);
            var members = Queries.GetAccountMembers(account).ToList();
            return View(account);
        } 
    }
}