using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IButtonRepository : IRepository<Button>
    {
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 添加菜单按钮
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="ids">按钮ID数组</param>
        /// <param name="createBy">创建人</param>
        /// <returns></returns>
        bool AddButtons(int pid, int[] ids, string createBy);

        bool Delete(int[] ids);
    }

    public class ButtonRepository : RepositoryBase<Button>, IButtonRepository
    {
        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            sql.SelectAll();

            sql.And(m => m.ENABLED == true);
            //string key = p.json;
            //if (!key.IsEmpty())
            //{
            //    sql.And().Begin();
            //    sql.Or(m => m.NAME.Contains(key));
            //    sql.Or(m => m.REMARK.Contains(key));
            //    sql.End();
            //}
            Dictionary<string, object> dic;
            string sqlText = ToSql(p.json, out dic);
            sql.And(sqlText).AddParameters(dic);
            sql.OrderBy(m => m.SORTID, m => m.ID);
            var list = base.GetPageList(p);
            return new LayuiPaginationOut(p, list);
        }

        public bool Delete(int[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new Button() { ENABLED = false }, m => m.ENABLED);

            int count = base.Execute(sql.GetSql(), sql.GetParameters());
            return count > 0;
        }

        public bool AddButtons(int pid, int[] ids, string createBy)
        {
            var sqlButton = base.GetSqlLam<Button>("b");
            sqlButton.Select(m => new
            {
                m.CLASSNAME,
                m.IMG,
                m.NAME,
                m.SORTID,
                m.CODE
            }).Select(string.Format("1 AS ENABLED,3 AS TYPE, {0} AS PARENTID, '{1}' AS CREATEBY, GETDATE() AS CREATEDATE ", pid, createBy));
            sqlButton.Where(m => m.ENABLED == true).In(m => m.ID, ids);

            var sqlMenu = base.GetSqlLam<Menu>();
            sqlMenu.InsertWithQuery(m => new
            {
                m.CLASSNAME,
                m.IMG,
                m.NAME,
                m.SORTID,
                m.CODE,
                m.ENABLED,
                m.TYPE,
                m.PARENTID,
                m.CREATEBY,
                m.CREATEDATE
            }, sqlButton);

            int count = base.Execute(sqlMenu.GetSql(), sqlMenu.GetParameters());
            return count > 0 ? true : false;
        }
    }
}
