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
    public class ImageController : BaseController
    {
        readonly IImageService image;
        public ImageController(IImageService iims)
        {
            image = iims;
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Form(int? id)
        {
            int imageId = id.HasValue ? id.Value : 0;
            if (imageId > 0)
            {
               
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var result = image.GetList(p);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = image.Delete(ids);
            return Json(result);
        }
    }
}