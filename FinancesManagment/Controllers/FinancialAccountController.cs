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

        [HttpPost]
        public ActionResult DeleteAccount(int Id)
        {
            var userId = User.Identity.GetUserId();
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == Id && m.ApplicationUser.Id == userId).FirstOrDefault();
            if (member == null)
            {
                return Json(new { status = "You aren't member of the account", success = false });
            }
            if (member.FinancialAccountRole.Title != "Owner")
            {
                return Json(new { status = "You don't have permission to delete account", success = false });
            }
            var acc = unitOfWork.FinancialAccountsRepository.GetByID(member.FinancialAccount.Id);
            if (acc.Summary != 0)
            {
                return Json(new { status = "The account is not empty", success = false });
            }
            foreach (var m in acc.FinancialAccountMembers)
            {
                unitOfWork.MemberPermissionsRepository.DeleteRange(m.MemberPermissions);
                unitOfWork.TransactionsRepository.DeleteRange(m.Transactions);
            }
            unitOfWork.FinancialAccountMembersRepository.DeleteRange(acc.FinancialAccountMembers);
            var name = acc.Name;
            unitOfWork.FinancialAccountsRepository.Delete(acc);
            unitOfWork.Save();
            return Json(new { status = string.Format("{0} was successfully deleted", name), success = true });
        }

        public ActionResult Edit(int Id)
        {
            var userId = User.Identity.GetUserId();
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == Id && m.ApplicationUser.Id == userId).FirstOrDefault();
            if (member == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (member.FinancialAccountRole.Title == "Owner")
            {
                return View(member);
            }
            return View("AccountView", member);
        }

        [HttpPost]
        public ActionResult Edit(int Id, string Name)
        {
            var userId = User.Identity.GetUserId();
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == Id && m.ApplicationUser.Id == userId).FirstOrDefault();
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
            unitOfWork.MemberPermissionsRepository.DeleteRange(member.MemberPermissions);
            unitOfWork.TransactionsRepository.DeleteRange(member.Transactions);
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

        public ActionResult EditPermissions(int Id)
        {
            var member = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            var userId = User.Identity.GetUserId();
            var user = unitOfWork.FinancialAccountMembersRepository.Get(m => m.ApplicationUser.Id == userId && m.FinancialAccount.Id == member.FinancialAccount.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (user.FinancialAccountRole.Title != "Owner")
            {
                return RedirectToAction("Edit", new { Id = user.FinancialAccount.Id });
            }
            return View(member);
        }

        [HttpPost]
        public ActionResult AddPermission(int Id, int Permission)
        {
            var memberPermission = unitOfWork.MemberPermissionsRepository.Get(p => p.Permission.Id == Permission && p.FinancialAccountMember.Id == Id).FirstOrDefault();
            string result = "Permission already set";
            if (memberPermission == null)
            {
                var member = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
                var permission = unitOfWork.PermissionsRepository.GetByID(Permission);
                MemberPermission newMemberPermission = new MemberPermission { Permission = permission, FinancialAccountMember = member };
                unitOfWork.MemberPermissionsRepository.Insert(newMemberPermission);
                int objectsSaved = unitOfWork.Save();
                if (objectsSaved > 0)
                {
                    result = "Permission added";
                }
                else
                {
                    result = "Failed to add permission";
                }
            }
            return Json(new { status = result });
        }

        [HttpPost]
        public ActionResult RemovePermission(int Id)
        {
            var memberPermission = unitOfWork.MemberPermissionsRepository.GetByID(Id);
            unitOfWork.MemberPermissionsRepository.Delete(memberPermission);
            int objectDeleted = unitOfWork.Save();
            if (objectDeleted > 0)
            {
                return Json(new { status = "Permission removed", success = true });
            }
            return Json(new { status = "Failed to remove permission", success = false });
        }

        public ActionResult SetQuote(int Id)
        {
            FinancialAccountMember member = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            var userId = User.Identity.GetUserId();
            FinancialAccountMember user = unitOfWork.FinancialAccountMembersRepository.Get(m => m.FinancialAccount.Id == member.FinancialAccount.Id && m.ApplicationUser.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var permission = user.MemberPermissions.Find(p => p.Permission.Title == "Set quote");
            if (permission == null)
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

        [HttpPost]
        public ActionResult MakeTransaction(int Id, decimal Amount, string Category)
        {
            var accountMember = unitOfWork.FinancialAccountMembersRepository.GetByID(Id);
            if (accountMember.MemberPermissions.Find(p => p.Permission.Title == "Make transaction") != null)
            {
                var financialAccount = accountMember.FinancialAccount;
                financialAccount.Summary += Amount;
                var transaction = new Transaction { Amount = Amount, Category = Category, FinancialAccountMember = accountMember };
                unitOfWork.TransactionsRepository.Insert(transaction);
                unitOfWork.FinancialAccountsRepository.Update(financialAccount);
                int result = unitOfWork.Save();

                if (result > 0)
                {
                    return Json(new { status = "Transaction successfull.", summary = financialAccount.Summary });
                }
            }
            return Json(new { status = "Transaction failed."});
        }
    }
}