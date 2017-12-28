using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;
using CDMS.Data;

namespace CDMS.Service
{
    public interface IMenuDataSourceService : IDependency
    {
        /// <summary>
        /// 查询数据源实体
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
        MenuDataSource Get(int columnId);

        /// <summary>
        /// 保存数据源
        /// </summary>
        /// <param name="old"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        AjaxResult Save(MenuDataSource old, MenuDataSource model);
    }

    internal class MenuDataSourceService : IMenuDataSourceService
    {
        readonly IMenuDataSourceRepository dsRep;
        readonly ILogService logRep;
        public MenuDataSourceService(IMenuDataSourceRepository imdsr, ILogService ils)
        {
            dsRep = imdsr;
            logRep = ils;
            logRep.Title = "数据源";
            logRep.Type = TableType.SYS_MENUDATASOURCE;
        }

        public MenuDataSource Get(int columnId)
        {
            return dsRep.GetEntity(m => m.COLUMNID == columnId && m.ENABLED == true);
        }

        public AjaxResult Save(MenuDataSource old, MenuDataSource model)
        {
            bool addFlag = model.ID < 1;
            var user = logRep.User;
            model.ENABLED = true;
            model.CREATEBY = user.ACCOUNT;
            model.CREATEDATE = DateTime.Now;
            model.UPDATEBY = model.CREATEBY;
            model.UPDATEDATE = model.CREATEDATE;

            if (addFlag)
            {
                int dsId = dsRep.Add<int>(model);
                bool flag = dsId > 0;
                ActionType type = ActionType.SYS_ADD;
                string msg = WebConst.GetActionMsg(type, flag);
                return new AjaxResult(flag, msg);
            }
            else
            {
                bool flag = dsRep.Update(model, m => new
                {
                    m.DATA,
                    m.DEFAULTTEXT,
                    m.DEFAULTVALUE,
                    m.OPTOINDATA,
                    m.PARAMETERDATA,
                    m.TYPE,
                    m.UPDATEBY,
                    m.UPDATEDATE
                }, m => m.ID == model.ID);
                ActionType type = ActionType.SYS_UPDATE;
                string msg = WebConst.GetActionMsg(type, flag);
                return new AjaxResult(flag, msg);
            }
        }
    }
}
