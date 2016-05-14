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
            IEnumerable<Permission> ownerPermissions = unitOfWork.PermissionsRepository.Get();
            foreach (var permission in ownerPermissions)
            {
                unitOfWork.MemberPermissionsRepository.Insert(new MemberPermission
                {
                    FinancialAccountMember = newMember,
                    Permission = permission
                });
            }
            objectsAdded += unitOfWork.Save();
            if (objectsAdded > 0)
            {
                return RedirectToAction("Edit",new { Id = newAccount.Id });
            }
            ViewBag.Message = "Failed to create account";
            return View();
        }

        public ActionResult Edit(int Id)
        {
            var userId = User.Identity.GetUserId();
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == Id && m.ApplicationUser.Id == userId).FirstOrDefault();
            if (member == null)
            {
                return View("Error");
            }
            if (member.FinancialAccountRole.Title == "Owner")
            {
                return View(member.FinancialAccount);
            }
            return View("AccountView", member);
        }

        [HttpPost]
        public ActionResult Edit(int Id, string Name)
        {
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == Id && m.ApplicationUser.Id == User.Identity.GetUserId()).FirstOrDefault();
            if (member == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (member.FinancialAccountRole.Title == "Owner")
            {
                member.FinancialAccount.Name = Name;
                unitOfWork.FinancialAccountsRepository.Update(member.FinancialAccount);
                unitOfWork.Save();
                ViewBag.Message = "Account was saved";
                return View(member.FinancialAccount);
            }
            return RedirectToAction("Edit", new { Id = member.FinancialAccount.Id });
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
            var userId = User.Identity.GetUserId();
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.Get(m => m.ApplicationUser.Id == userId && m.FinancialAccount.Id == Id).FirstOrDefault();
            if (member == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (member.MemberPermissions.Find(p => p.Permission.Title == "Add user") != null)
            {
                FinancialAccountMember newMember = new FinancialAccountMember();
                newMember.FinancialAccount = member.FinancialAccount;
                return View(newMember);
            }
            return RedirectToAction("Edit", new { Id = member.FinancialAccount.Id });
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

        public string EditPermissions(int Id)
        {
            return Id.ToString();
        }

        public ActionResult SetQuote(int Id)
        {
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            FinancialAccountMember user = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == member.FinancialAccount.Id && m.ApplicationUser.Id == User.Identity.GetUserId()).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (user.MemberPermissions.Find(p => p.Permission.Title == "Set quote") == null)
            {
                return RedirectToAction("Edit", new { Id = user.FinancialAccount.Id });
            }
            return View(member);
        }

        [HttpPost]
        public ActionResult SetQuote(int Id, decimal Quote)
        {
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            member.Quote = Quote;
            unitOfWork.FinancialAccountMembersRepository.Update(member);
            int updatedObjects = unitOfWork.Save();
            if (updatedObjects > 0)
            {
                return RedirectToAction("Edit", new { Id = member.FinancialAccount.Id });
            }
            else
            {
                ViewBag.Message = "Failed to change the quote";
                return View(member);
            }
        }
        public ActionResult MakeTransaction(int Id, decimal Amount, string Category)
        {
            var accountMember = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            var financialAccount = accountMember.FinancialAccount;
            financialAccount.Summary += Amount;
            var transaction = new Transaction(Amount, Category, accountMember);
            unitOfWork.TransactionsRepository.Insert(transaction);
            unitOfWork.FinancialAccountsRepository.Update(financialAccount);
            int result = unitOfWork.Save();

            if (result > 0)
            {
                return Json(new { status = "Transaction successfull."});
            }
            else
            {
                return Json(new { status = "Transaction failed." });
            }

        }
    }
}