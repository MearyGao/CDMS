using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Service
{
    public interface ICaCheService : IDependency
    {
        /// <summary>
        /// 获得缓存key 列表
        /// </summary>
        /// <returns></returns>
        LayuiPaginationOut GetCacheKeys(string key);

        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        AjaxResult Remove(string key);

        /// <summary>
        /// 移除选择项缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        AjaxResult RemoveList(string[] keys);

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        /// <returns></returns>
        AjaxResult RemoveAll();
    }

    public class CaCheService : ICaCheService
    {
        public LayuiPaginationOut GetCacheKeys(string key)
        {
            var list = CacheHelper.GetAllKeys();
            if (list != null)
            {
                var tempList = from item in list
                               orderby item
                               select new { KEY = item };
                if (!key.IsEmpty())
                {
                    key = key.ToLower();
                    tempList = tempList.Where(m => m.KEY.ToLower().Contains(key));
                }
                return new LayuiPaginationOut(tempList);
            }
            return new LayuiPaginationOut();
        }

        public AjaxResult Remove(string key)
        {
            try
            {
                CacheHelper.Remove(key);
                return new AjaxResult(true, "移除缓存成功");
            }
            catch (Exception e)
            {
                return new AjaxResult(false, "移除缓存失败--" + e.Message);
            }
        }

        public AjaxResult RemoveList(string[] keys)
        {
            try
            {
                foreach (var key in keys)
                {
                    CacheHelper.Remove(key);
                }
                return new AjaxResult(true, "移除缓存成功");
            }
            catch (Exception e)
            {
                return new AjaxResult(false, "移除缓存失败--" + e.Message);
            }
        }

        public AjaxResult RemoveAll()
        {
            try
            {
                CacheHelper.Clear();
                return new AjaxResult(true, "移除缓存成功");
            }
            catch (Exception e)
            {
                return new AjaxResult(false, "移除缓存失败--" + e.Message);
            }
        }
    }
}
