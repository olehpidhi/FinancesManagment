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
            // var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            newAccount = unitOfWork.FinancialAccountsRepository.GetByID(newAccount.Id);
            newMember.FinancialAccountRole = ownerRole;
            newMember.FinancialAccount = newAccount;
            newMember.ApplicationUserId = userId;
            unitOfWork.FinancialAccountMembersRepository.Insert(newMember);
            objectsAdded += unitOfWork.Save();
            if (objectsAdded == 2)
            {
                return RedirectToAction("Edit",new { Id = newAccount.Id });
            }
            ViewBag.Message = "Failed to create account";
            return View();
        }

        public ActionResult Edit(int Id)
        {
            FinancialAccount account = unitOfWork.FinancialAccountsRepository.GetByID(Id);
            return View(account);
        }

        [HttpPost]
        public ActionResult Edit(int Id, string Name)
        {
            FinancialAccount account = unitOfWork.FinancialAccountsRepository.GetByID(Id);
            account.Name = Name;
            unitOfWork.FinancialAccountsRepository.Update(account);
            unitOfWork.Save();
            ViewBag.Message = "Account was saved";
            return View(account);
        }

        [HttpPost]
        public ActionResult RemoveMember(int Id)
        {
            var member = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            var Email = member.ApplicationUser.Email;
            unitOfWork.FinancialAccountMembersRepository.Delete(member);
            int membersDeleted = unitOfWork.Save();
            if (membersDeleted > 0)
            {
                return Json(new { email = Email, success = true });
            }
            else
            {
                return Json(new { email = Email, success = false });
            }
        }

        public string AddUser()
        {
            return "Add user page";
        }

        public string MakeTransaction()
        {
            return "Make transaction page";
        }
    }
}