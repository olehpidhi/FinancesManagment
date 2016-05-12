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
            newMember.FinancialAccountRole = ownerRole;
            newMember.FinancialAccount = newAccount;
            newMember.ApplicationUser = user;
            
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

        public ActionResult AddUser(int Id)
        {
            FinancialAccount account = unitOfWork.FinancialAccountsRepository.GetByID(Id);
            FinancialAccountMember member = new FinancialAccountMember();
            member.FinancialAccount = account;
            return View(member);
        }

        [HttpPost]
        public ActionResult AddUser(int Id, int Role, string Email)
        {
            FinancialAccount account = unitOfWork.FinancialAccountsRepository.GetByID(Id);
            FinancialAccountRole role = unitOfWork.FinancialAccountRolesRepository.GetByID(Role);
            ApplicationUser user = unitOfWork.UserRepository.Get(u => u.Email == Email).FirstOrDefault();
            FinancialAccountMember member = new FinancialAccountMember();
            member.FinancialAccount = account;
            if (user == null)
            {
                ViewBag.Message = "There is no such user";
                return View(member);
            }
            if (unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == account.Id && m.ApplicationUser.Id == user.Id).FirstOrDefault() != null)
            {
                ViewBag.Message = "User is a member of account already.";
                return View(member);
            }
            member.FinancialAccountRole = role;
            member.ApplicationUser = user;
            unitOfWork.FinancialAccountMembersRepository.Insert(member);
            int savedMembers = unitOfWork.Save();
            if (savedMembers > 0)
            {
                return RedirectToAction("Edit", new { Id = account.Id });
            }
            ViewBag.Message = string.Format("Failed to add {1}", user.Email);
            return View(member);
        }

        public string MakeTransaction(int Id)
        {
            return "Make transaction page";
        }
    }
}