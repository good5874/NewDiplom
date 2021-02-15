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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            List<AccountViewModel> viewModels = new List<AccountViewModel>();

            var users = _userManager.Users
                .Include(e => e.Plurality)
                .Include(e => e.Plurality.Position)
                .Include(e => e.Plurality.Employee)
                .Where(e=>e.Plurality != null);

            foreach (User user in users)
            {
                viewModels.Add(
                    new AccountViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        PluralityView = user.Plurality.View
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

            var user = _userManager.Users
                 .Include(e => e.Plurality)
                 .Include(e => e.Plurality.Position)
                 .Include(e => e.Plurality.Employee)
                 .FirstOrDefault(e => e.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            AccountViewModel viewModel = new AccountViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                PluralityView = user.Plurality.View
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            _context.Employees.Load();
            _context.Positions.Load();
            ViewData["PluralityId"] = new SelectList(_context.Plurality, "Id", "View");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,Phone,Password,ConfirmPassword,PluralityId")] AccountViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new User(model.UserName);
                await _userManager.SetEmailAsync(user, model.Email);
                await _userManager.SetPhoneNumberAsync(user, model.Phone);
                await _userManager.AddPasswordAsync(user, model.Password);
                user.PluralityId = model.PluralityId;
                _context.Users.Add(user);
                _context.SaveChanges();


                return RedirectToAction(nameof(Index));
            }
            _context.Employees.Load();
            _context.Positions.Load();
            ViewData["PluralityId"] = new SelectList(_context.Plurality, "Id", "View");
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.Users
                .Include(e => e.Plurality)
                .Include(e => e.Plurality.Position)
                .Include(e => e.Plurality.Employee)
                .FirstOrDefault(e => e.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            AccountViewModel viewModel = new AccountViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                PluralityId = user.PluralityId
            };

            _context.Employees.Load();
            _context.Positions.Load();
            ViewData["PluralityId"] = new SelectList(_context.Plurality, "Id", "View");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,Phone,Password,ConfirmPassword,PluralityId,Plurality")] AccountViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    await _userManager.SetEmailAsync(user, model.Email);
                    await _userManager.SetUserNameAsync(user, model.UserName);
                    await _userManager.SetPhoneNumberAsync(user, model.Phone);
                    await _userManager.AddPasswordAsync(user, model.Password);
                    user.PluralityId = model.PluralityId;

                    await _userManager.UpdateAsync(user);


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

            _context.Employees.Load();
            _context.Positions.Load();
            ViewData["PluralityId"] = new SelectList(_context.Plurality, "Id", "View");
            return View(model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.Users
                .Include(e => e.Plurality)
                .Include(e => e.Plurality.Position)
                .Include(e => e.Plurality.Employee)
                .FirstOrDefault(e => e.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            AccountViewModel viewModel = new AccountViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                PluralityView = user.Plurality.View
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
