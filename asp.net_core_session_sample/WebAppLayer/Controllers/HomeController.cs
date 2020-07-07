using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataContextLayer.DataContext;
using DataContextLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppLayer.Models;

namespace WebAppLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISession session;

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            this.session = httpContextAccessor.HttpContext.Session;
        }
        private EFDataContext db = new EFDataContext();

        [BindProperty]
        public AdminAccount admin { get; set; }

        [ValidateAntiForgeryToken]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("userName") == null)
            {
                return RedirectToAction("Signin", "Home");
            }
            else
            {
                var user_check = db.adminAccount.ToList();
                return View(user_check);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(AdminAccount model, bool? value)
        {
            if (model != null)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(model.Password + model.EmailId);
                Byte[] encodedBytes = md5.ComputeHash(originalBytes);

                string hashedPassword = BitConverter.ToString(encodedBytes).Replace("-", "");
                var nouser = db.adminAccount.Where(u => u.EmailId == model.EmailId && u.Password != hashedPassword).Any();
                var newudb = db.adminAccount.Where(u => u.EmailId == model.EmailId && u.Password == hashedPassword).FirstOrDefault();
                if (newudb != null)
                {

                    HttpContext.Session.SetString("userName", newudb.UserName);

                    //Session["ID"] = newudb.ClientID.ToString();
                    //Session["UserName"] = newudb.UserName.ToString();

                    return RedirectToAction("Index", "Home");
                }
                else if (nouser == true)
                {
                    ModelState.AddModelError("", "Password doesnot match with email-id");
                }
                else
                {
                    ModelState.AddModelError("", "credentials mis-match");
                }
            }
            return View();
        }

        public IActionResult Signout()
        {
            if (ModelState.IsValid == true)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }
    }
}

