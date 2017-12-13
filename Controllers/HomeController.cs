using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using beltexam.Models;
using Microsoft.EntityFrameworkCore;

namespace beltexam.Controllers
{
    public class HomeController : Controller
    {
        private beltexamContext _context;

        public HomeController(beltexamContext context)
        {
            _context = context;
        }

// ***************************************** GETS ***************************************** //

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index");
        }

// ***************************************** POSTS ***************************************** //

        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserViewModel model)
        {
            User CheckUser = _context.Users.SingleOrDefault(user => user.Email == model.Email);
            if (CheckUser != null) {
                ViewBag.EmailExistReg = "This email already exists. Please login.";
            } else {
                if (ModelState.IsValid) {
                    User NewUser = new User();
                    NewUser.FirstName = model.FirstName;
                    NewUser.LastName = model.LastName;
                    NewUser.Email = model.Email;
                    NewUser.Password = model.Password;
                    NewUser.CreatedAt = DateTime.Now;
                    NewUser.UpdatedAt = DateTime.Now;
                    _context.Users.Add(NewUser);
                    _context.SaveChanges();
                    User current = _context.Users.SingleOrDefault(user => user.Email == model.Email);
                    HttpContext.Session.SetInt32("current", current.userid);
                    return RedirectToAction("Dashboard", "Store", new { num = current.userid });
                }
                
            }
            return View("Index");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string loginEmail, string loginPassword)
        {
            User CheckUser = _context.Users.SingleOrDefault(user => user.Email == loginEmail);
            if (CheckUser == null) {
                ViewBag.EmailExist = "This email does not exist. Please register.";
            } else if (CheckUser.Password != loginPassword) {
                ViewBag.PassMatch = "Your password is incorrect.";
            } else {
                User current = _context.Users.SingleOrDefault(user => user.Email == loginEmail);
                HttpContext.Session.SetInt32("current", current.userid);
                System.Console.WriteLine(current.userid);
                return RedirectToAction("Dashboard", "Store", new { num = current.userid });
            }
            return View("Index");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }



        public IActionResult Error()
        {
            return View("Error");
        }



    }
}
