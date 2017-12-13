using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Service;
using CDMS.Entity;

namespace CDMS.Web
{
    /// <summary>
    /// 授权
    /// </summary>
    public class AuthAttribute : ActionFilterAttribute
    {
        public IMenuService MenuService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string url = filterContext.HttpContext.Request.Path.ToLower();

            var model = MenuService.GetAuthMenuList(url);

            if (model == null || !model.HaveMenuRight())
            {
                bool isAjax = filterContext.HttpContext.Request.IsAjaxRequest();
                if (isAjax)
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new AjaxResult(false, "对不起,您的访问未经授权")
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/common/error");
                }
            }
        }
    }
}