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
    public class RoleController : BaseController
    {
        readonly IRoleService roleService;
        public RoleController(IRoleService irs)
        {
            roleService = irs;
        }
        // GET: Sys/Role
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(int? id)
        {
            int key = id.HasValue ? id.Value : 0;
            if (key > 0)
            {
                ViewBag.RoleJson = JsonHelper.ToJson(roleService.Get(key));
            }
            return View();
        }

        public ActionResult Auth(int roleId)
        {
            ViewBag.RoleId = roleId;
            var menus = roleService.GetRoleMenus(roleId);
            ViewBag.MenuJson = JsonHelper.ToJson(menus);
            return View();
        }

        public ActionResult AuthUser(int roleId)
        {
            ViewBag.RoleId = roleId;
            var menus = roleService.GetRoleUsers(roleId);
            ViewBag.UserJson = JsonHelper.ToJson(menus);
            return View();
        }

        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var result = roleService.GetList(p);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Save(Role old, Role model)
        {
            var result = roleService.Save(old, model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = roleService.Delete(ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult AddRoleMenus(int roleId, int[] ids)
        {
            var result = roleService.AddRoleMenus(roleId, ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult AddRoleUsers(int roleId, int[] ids, int[] deleteIds)
        {
            var result = roleService.AddRoleUsers(roleId, ids, deleteIds);
            return Json(result);
        }
    }
}