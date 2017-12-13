using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Data;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Service
{
    public interface IMenuColumnService : IDependency
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得列对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MenuColumn Get(int id);

        /// <summary>
        /// 获得列列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        AjaxResult GetColumnList(int menuId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        LayuiPaginationOut GetColumnList(int menuId, string key);

        /// <summary>
        /// 保存列信息
        /// </summary>
        /// <param name="old"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        AjaxResult Save(MenuColumn old, MenuColumn model);

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="uid"></param>
        /// <param name="cids"></param>
        /// <returns></returns>
        AjaxResult AddColumns(int pid, int[] cids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        AjaxResult Delete(int[] ids);
    }

    internal class MenuColumnService : IMenuColumnService
    {
        readonly IMenuColumnRepository columnRep;
        readonly ILogService log;
        public MenuColumnService(IMenuColumnRepository imcr, ILogService ils)
        {
            columnRep = imcr;
            log = ils;
            log.Type = TableType.SYS_MENUCOLUMN;
            log.Title = "菜单列";
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            return columnRep.GetList(p);
        }

        public LayuiPaginationOut GetColumnList(int menuId, string key)
        {
            var list = columnRep.GetColumnList(menuId, key);

            return new LayuiPaginationOut(list);
        }

        public MenuColumn Get(int id)
        {
            return columnRep.GetEntity(m => m.ID == id && m.ENABLED == true);
        }

        public AjaxResult GetColumnList(int menuId)
        {
            var list = columnRep.GetColumnList(menuId);
            if (list != null)
            {
                var tempList = from item in list
                               select new
                               {
                                   text = item.COLUMNNAME,
                                   value = string.Format("{0}|{1}|{2}", item.TABLEID, item.COLUMNTYPE, item.COLUMNTEXT)
                               };
                return new AjaxResult(true, data: tempList);
            }
            return new AjaxResult(false, "");
        }

        public AjaxResult Save(MenuColumn old, MenuColumn model)
        {
            bool addFlag = model.ID < 1;
            var user = log.User;
            model.CREATEBY = user.ACCOUNT;
            model.CREATEDATE = DateTime.Now;
            model.UPDATEBY = model.CREATEBY;
            model.UPDATEDATE = model.CREATEDATE;
            model.ENABLED = true;
            if (addFlag)
            {
                bool existFlag = columnRep.Exist(m => m.TYPE == model.TYPE && m.MENUID == model.MENUID && m.NAME == model.NAME && m.ENABLED == true);
                if (existFlag) return new AjaxResult(false, "已经存在该菜单列");
                int columnId = columnRep.Add<int>(model);
                bool flag = columnId > 0;
                if (flag) RemoveCache();
                ActionType type = ActionType.SYS_ADD;
                string msg = WebConst.GetActionMsg(type, flag);
                return new AjaxResult(flag, msg);
            }
            else
            {
                bool existFlag = columnRep.Exist(m => m.TYPE == model.TYPE && m.MENUID == model.MENUID && m.NAME == model.NAME && m.ENABLED == true && m.ID != model.ID);
                if (existFlag) return new AjaxResult(false, "已经存在该菜单列");

                bool flag = columnRep.Update(model, m => new
                {
                    m.MENUID,
                    m.NAME,
                    m.CONDITIONTYPE,
                    m.INPUTTYPE,
                    m.TABLEID,
                    m.UPDATEBY,
                    m.TYPE,
                    m.FIELDTEXT,
                    m.FIELDTYPE,
                    m.UPDATEDATE,
                    m.SORTID,
                    m.FIELDVALUE
                }, m => m.ID == model.ID);
                if (flag) RemoveCache();
                ActionType type = ActionType.SYS_UPDATE;
                string msg = WebConst.GetActionMsg(type, flag);
                return new AjaxResult(flag, msg);
            }
        }

        public AjaxResult AddColumns(int pid, int[] cids)
        {
            var user = log.User;
            bool flag = columnRep.AddColumns(pid, user.ACCOUNT, cids);
            string msg = flag ? "分配成功" : "分配失败";
            return new AjaxResult(flag, msg);
        }

        public AjaxResult Delete(int[] ids)
        {
            ActionType type = ActionType.SYS_DELETE;
            bool flag = columnRep.Delete(ids);
            if (flag) RemoveCache();
            string msg = WebConst.GetActionMsg(type, flag);
            return new AjaxResult(flag, msg);
        }

        private void RemoveCache()
        {
            string key = ServiceConst.MenuColumnListCache;

            CacheHelper.Remove(key);
        }
    }
}
