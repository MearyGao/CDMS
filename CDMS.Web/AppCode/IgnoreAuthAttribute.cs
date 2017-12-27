using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDMS.Web
{
    /// <summary>
    /// 忽略授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IgnoreAuthAttribute : Attribute
    {

    }
}