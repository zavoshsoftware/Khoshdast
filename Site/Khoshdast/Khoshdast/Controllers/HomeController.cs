using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace Khoshdast.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            HomeViewModel home=new HomeViewModel();
            return View(home);
        }
        [Route("payment")]
        public ActionResult payment()
        {
            return View();
        }
        
        [Route("About")]
        public ActionResult About()
        {
            AboutViewModel about=new AboutViewModel();
            return View(about);
        }
        [Route("Contact")]
        public ActionResult Contact()
        {
            ContactViewModel contact=new ContactViewModel();
            return View(contact);
        }
    }
}