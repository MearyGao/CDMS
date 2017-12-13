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
    public class MenuController : BaseController
    {
        readonly IMenuService menuService;

        public MenuController(IMenuService ims)
        {
            menuService = ims;
        }

        // GET: Sys/Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(int? id)
        {
            int menuId = id.HasValue ? id.Value : 0;
            if (menuId > 0)
            {
                ViewBag.Json = JsonHelper.ToJson(menuService.Get(menuId));
            }
            return View();
        }

        public ActionResult Icon()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetList()
        {
            var list = menuService.GetTreeList();
            LayuiPaginationOut result = new LayuiPaginationOut(list);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetAuthMenuList()
        {
            var list = menuService.GetAuthMenuList();
            return Json(list);
        }

        public ActionResult Button()
        {
            string url = HttpContext.Request.Path.ToLower();
            var model = menuService.GetAuthMenuList(url, MenuType.Button);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult GetTreeSelectList()
        {
            var list = menuService.GetTreeSelectList();
            return Json(list);
        }

        [HttpPost]
        public ActionResult Save(Menu model)
        {
            if (!model.IMG.IsEmpty())
                model.IMG = Uri.UnescapeDataString(model.IMG);
            var result = menuService.Save(model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = menuService.Delete(ids);
            return Json(result);
        }
    }
}