using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;
using CDMS.Data;

namespace CDMS.Service
{
    public interface IMenuTableService : IDependency
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 保存菜单表信息
        /// </summary>
        /// <param name="old"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        AjaxResult Save(MenuTable old, MenuTable model);

        /// <summary>
        /// 获得菜单表信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MenuTable Get(int id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        AjaxResult Delete(int[] ids);

        /// <summary>
        /// 获得表列表
        /// </summary>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        AjaxResult GetTableList(string dbKey);
    }

    internal class MenuTableService : IMenuTableService
    {
        readonly IMenuTableRepository tableRep;
        readonly ILogService log;
        public MenuTableService(IMenuTableRepository imtr, ILogService ils)
        {
            tableRep = imtr;
            log = ils;
            log.Title = "菜单表";
            log.Type = TableType.SYS_MENUTABLE;
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            return tableRep.GetList(p);
        }

        public AjaxResult Save(MenuTable old, MenuTable model)
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
                int tableId = tableRep.Add<int>(model);
                bool flag = tableId > 0;
                ActionType type = ActionType.SYS_ADD;
                string msg = WebConst.GetActionMsg(type, flag);
                return new AjaxResult(flag, msg);
            }
            else
            {
                bool flag = tableRep.Update(model, m => new
                {
                    m.DBNAME,
                    m.TABLENAME,
                    m.SCHEMANAME,
                    m.SORTID
                }, m => m.ID == model.ID);

                ActionType type = ActionType.SYS_UPDATE;
                string msg = WebConst.GetActionMsg(type, flag);
                return new AjaxResult(flag, msg);
            }
        }

        public MenuTable Get(int id)
        {
            return tableRep.GetEntity(m => m.ID == id && m.ENABLED == true);
        }

        public AjaxResult Delete(int[] ids)
        {
            bool flag = tableRep.Delete(ids);
            ActionType type = ActionType.SYS_DELETE;
            string msg = WebConst.GetActionMsg(type, flag);
            return new AjaxResult(flag, msg);
        }

        public AjaxResult GetTableList(string dbKey)
        {
            var list = tableRep.GetTableList(dbKey);

            var tempList = from item in list
                           select new
                           {
                               value = item.SCHEMANAME,
                               text = item.TABLENAME
                           };

            return new AjaxResult(true, data: tempList);
        }
    }
}
