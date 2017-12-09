using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Entity;
using CDMS.Service;
using CDMS.Utility;

namespace CDMS.Web.Areas.Sys.Controllers
{
    public class UserController : BaseController
    {
        readonly IUserService userService;

        public UserController(IUserService ius)
        {
            userService = ius;
        }

        // GET: Sys/Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form(int? id)
        {
            int userId = id.HasValue ? id.Value : 0;
            if (userId > 0)
            {
                ViewBag.UserJson = JsonHelper.ToJson(userService.Get(userId));
            }
            return View();
        }

        [HttpPost]
        public ActionResult Save(User model)
        {
            if (!string.IsNullOrEmpty(model.IMG))
                model.IMG = Uri.UnescapeDataString(model.IMG);
            if (string.IsNullOrEmpty(model.PWD))
                model.PWD = EncryptHelper.Encrypt("111111");
            var result = userService.Save(model);
            return Json(result);
        }
        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var list = userService.GetList(p);
            return Json(list);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = userService.Delete(ids);
            return Json(result);
        }

        public ActionResult UploadUserFace()
        {
            var result = userService.UploadUserFace();
            return Json(result);
        }
    }
}