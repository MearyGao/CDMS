using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Entity;
using CDMS.Utility;
using CDMS.Service;

namespace CDMS.Web.Controllers
{
    public class MainController : BaseController
    {
        readonly IMenuService menuService;
        public MainController(IMenuService ims)
        {
            menuService = ims;
        }

        [IgnoreAuth]
        public ActionResult Index()
        {
            ViewBag.User = base.User;
            return View();
        }

        [IgnoreAuth]
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Button()
        {
            string url = HttpContext.Request.Path.ToLower();
            var buttons = menuService.GetAuthMenuList(url, MenuType.Button);
            return PartialView(buttons);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var result = base.UserService.Logout();
            
            return Json(result);
        }
    }
}