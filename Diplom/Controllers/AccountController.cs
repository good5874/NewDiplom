using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplom.Models;
using Diplom.Data;
using Microsoft.AspNetCore.Identity;
using Diplom.ViewModels;

namespace Diplom.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            List<AccountViewModel> viewModels = new List<AccountViewModel>();

            var users = _userManager.Users;

            foreach (IdentityUser identityUser in users)
            {
                viewModels.Add(
                    new AccountViewModel()
                    {
                        Id = identityUser.Id,
                        UserName = identityUser.UserName,
                        Email = identityUser.Email,
                        Phone = identityUser.PhoneNumber,
                    });
            }

            return View(viewModels);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _userManager.FindByIdAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            AccountViewModel viewModel = new AccountViewModel()
            {
                Id = account.Id,
                Email = account.Email,
                UserName = account.UserName,
                Phone = account.PhoneNumber
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,Phone,Password,ConfirmPassword")] AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser(model.UserName);
                await _userManager.SetEmailAsync(user, model.Email);
                await _userManager.SetPhoneNumberAsync(user, model.Phone);
                await _userManager.CreateAsync(user, model.Password);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _userManager.FindByIdAsync(id);

            if (account == null)
            {
                return NotFound();
            }
            AccountViewModel viewModel = new AccountViewModel()
            {
                Id = account.Id,
                Email = account.Email,
                UserName = account.UserName,
                Phone = account.PhoneNumber
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,Phone,Password,ConfirmPassword")] AccountViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _userManager.FindByIdAsync(id);
                    await _userManager.SetEmailAsync(account, model.Email);
                    await _userManager.SetUserNameAsync(account, model.UserName);
                    await _userManager.SetPhoneNumberAsync(account, model.Phone);
                    await _userManager.AddPasswordAsync(account, model.Password);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _userManager.FindByIdAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            AccountViewModel viewModel = new AccountViewModel()
            {
                Id = account.Id,
                Email = account.Email,
                UserName = account.UserName,
                Phone = account.PhoneNumber
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var account = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(account);
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
