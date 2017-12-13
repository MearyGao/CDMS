using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    public class ServiceConst
    {
        /// <summary>
        /// 用户授权列表 缓存 key {0} 为 用户ID
        /// </summary>
        public static readonly string UserAuthListCache = "UserAuthList_{0}";

        /// <summary>
        /// 菜单列表缓存
        /// </summary>
        public static readonly string MenuColumnListCache = "MenuColumnList";
    }
}
