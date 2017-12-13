using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Utility;
using CDMS.Entity;

namespace CDMS.Web.Controllers
{
    /// <summary>
    /// 主要放 所有用户都可以访问的页面或操作 包括不登录
    /// </summary>
    public class CommonController : Controller
    {
        // GET: Common
        public ActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage()
        {
            var files = HttpContext.Request.Files;
            if (files == null || files.Count < 1) return Json(new LayuiUploadImageOut(1, "没有找到上传图片"));

            UploadParameter p = new UploadParameter(files[0]);
            string path = "/UploadFiles/ArticleImages/{0}/{1}";
            path = string.Format(path, DateTime.Now.ToDateString(), p.GetDefaultFileName());
            p.SavePath = HttpContext.Server.MapPath(path);
            var result = UploadHelper.UploadFile(UploadType.IMAGE, p);
            if (result.Status == UploadStatus.Success)
            {
                var image = new LayuiUploadImageOut(0, result.Msg, path);
                return Json(image);
            }
            else
            {
                var image = new LayuiUploadImageOut(1, result.Msg);
                return Json(image);
            }
        }
    }
}