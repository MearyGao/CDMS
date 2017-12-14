using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Data;
using CDMS.Utility;

namespace CDMS.Service
{
    public interface IDictionaryService : IDependency
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<Dictionary> GetDictionaryList(string type);

        /// <summary>
        /// 获得类型列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetRootDicList();

        /// <summary>
        /// 删除字典信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        AjaxResult Delete(string[] ids);
        /// <summary>
        /// 添加字典信息
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        AjaxResult Save(Dictionary old, Dictionary dic);
        /// <summary>
        /// 获得字典信息(修改返填单条数据)
        /// </summary>
        /// <param name="dicid"></param>
        /// <returns></returns>
        Dictionary Get(int dicid);
    }
    public class DictionaryService : IDictionaryService
    {
        readonly IDictionaryRepository dicRep;
        readonly ILogService log;

        public DictionaryService(IDictionaryRepository ibs, ILogService ils)//依赖注入
        {
            dicRep = ibs;
            log = ils;
            log.Title = "字典表";
            log.Type = TableType.SYS_DICTIONARY;
        }
        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public AjaxResult Delete(string[] ids)
        {
            bool flag = dicRep.Delete(ids);
            string msg = WebConst.GetActionMsg(ActionType.SYS_DELETE, flag);//操作日志
            log.AppendDelete(msg, "字典编码", ids).AddSystem(ActionType.SYS_DELETE, ids);
            if (flag) RemoveCache();
            return new AjaxResult(flag, msg);
        }

        public Dictionary Get(int dicid)
        {
            return dicRep.GetEntity(m => m.ID == dicid && m.ENABLED == true);
        }

        public IEnumerable<Dictionary> GetDictionaryList(string type)
        {
            string key = ServiceConst.DictionaryListCache;
            var list = CacheHelper.Get<IEnumerable<Dictionary>>(key);
            if (list != null && list.Count() > 0) return list;
            list = dicRep.GetList(m => m.TYPE == type && m.ENABLED == true);
            CacheHelper.Add(key, list);
            return list;
        }

        public IEnumerable<KeyValue> GetRootDicList()
        {
            var list = this.GetDictionaryList("0");
            if (list != null && list.Count() > 0)
            {
                var tempList = from item in list
                               select new KeyValue(item.TEXT.ToString(), item.VALUE);
                return tempList;
            }
            return null;
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            return dicRep.GetList(p);
        }

        public AjaxResult Save(Dictionary old, Dictionary dic)
        {
            bool addFlag = dic.ID < 1;
            var user = log.User;

            dic.CREATEBY = user.ACCOUNT;
            dic.CREATEDATE = DateTime.Now;
            dic.UPDATEBY = user.ACCOUNT;
            dic.UPDATEDATE = DateTime.Now;
            dic.ENABLED = true;

            if (addFlag)
            {
                int dicId = dicRep.Add<int>(dic);
                bool flag = dicId > 0;
                ActionType type = ActionType.SYS_ADD;
                string msg = WebConst.GetActionMsg(type, flag);
                log.Append(msg);
                log.AppendLine();
                log.AppendAdd("字典类型", dic.TYPE.ToString()).AppendAdd("字典编码", dic.CODE).AppendAdd("文本", dic.TEXT).AppendAdd("值", dic.VALUE);
                log.AddSystem(type, dicId);
                if (flag) RemoveCache();
                return new AjaxResult(flag, msg);
            }
            else
            {
                bool flag = dicRep.Update(dic, m => new
                {
                    m.TYPE,
                    m.TEXT,
                    m.CODE,
                    m.VALUE,
                    m.REMARK,
                    m.ENABLED,
                    m.SORTID,
                    m.UPDATEBY,
                    m.UPDATEDATE
                }, m => m.ID == dic.ID);
                ActionType type = ActionType.SYS_UPDATE;
                string msg = WebConst.GetActionMsg(type, flag);
                log.Append(msg);
                log.AppendLine();
                log.AppendUpdate("字典类型", old.TYPE.ToString(), dic.TYPE.ToString()).AppendUpdate("字典编码", old.CODE, dic.CODE).AppendUpdate("文本", old.TEXT, dic.TEXT).AppendUpdate("值", old.VALUE, dic.VALUE);
                log.AddSystem(type, dic.ID);
                if (flag) RemoveCache();
                return new AjaxResult(flag, msg);
            }
        }

        private void RemoveCache()
        {
            string key = ServiceConst.DictionaryListCache;
            CacheHelper.Remove(key);
        }
    }
}
