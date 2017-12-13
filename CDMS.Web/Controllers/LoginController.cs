using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Entity;
using CDMS.Service;
using CDMS.Utility;

namespace CDMS.Web.Controllers
{
    public class LoginController : Controller
    {
        readonly IUserService uService;
        public LoginController(IUserService us)
        {
            uService = us;
        }
        // GET: Login
        public ActionResult Index(string redirectUrl = "")
        {
            bool isLogin = uService.IsLogin();
            if (isLogin)
            {
                return RedirectToAction("index", "main");
            }
            ViewBag.ActionURL = redirectUrl;
            return View();
        }

        public ActionResult Pwd(string pwd)
        {
            if (string.IsNullOrEmpty(pwd)) pwd = "111111";
            var obj = new { pwd = pwd, pwdstring = EncryptHelper.Encrypt(pwd) };
            return Json(obj);
        }

        [HttpPost]
        public ActionResult CheckLogin(string eid, string pwd)
        {
            var result = uService.Login(eid, pwd);
            return Json(result);
        }
    }
}