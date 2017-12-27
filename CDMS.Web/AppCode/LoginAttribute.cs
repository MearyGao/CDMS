using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using CDMS.Service;
using CDMS.Entity;

namespace CDMS.Web
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginAttribute : ActionFilterAttribute
    {
        public LoginAttribute()
        {

        }

        public IUserService UserService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            bool isLogin = UserService.IsLogin();

            if (!isLogin)
            {
                bool isAjax = filterContext.HttpContext.Request.IsAjaxRequest();
                if (isAjax)
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new AjaxResult(false, "已经退出登录,请重新登录")
                    };
                }
                else
                {
                    string action = filterContext.HttpContext.Request.Url.ToString();
                    action = filterContext.HttpContext.Server.UrlEncode(action);
                    UrlHelper URL = new UrlHelper(filterContext.RequestContext);
                    string url = URL.Content(string.Format("~/login?redirectUrl={0}", action));

                    filterContext.Result = new RedirectResult(url);
                }
            }
        }
    }
}