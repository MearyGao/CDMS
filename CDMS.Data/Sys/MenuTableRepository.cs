using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IMenuTableRepository : IRepository<MenuTable>
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得表列表
        /// </summary>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetTableList(string dbKey);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(int[] ids);
    }

    public class MenuTableRepository : RepositoryBase<MenuTable>, IMenuTableRepository
    {
        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            sql.SelectAll();
            sql.Where(m => m.ENABLED == true);

            //var menuSql = sql.Join<Menu>((t, s) => t.MENUID == s.ID, aliasName: "b");
            //menuSql.Select(m => m.NAME);

            MenuTable table = p.json.ToObject<MenuTable>();
            if (table != null)
            {
                //if (table.MENUID > 0) sql.And(m => m.MENUID == table.MENUID);
                if (!table.TABLENAME.IsEmpty())
                {
                    sql.And(m => m.TABLENAME.Contains(table.TABLENAME));
                }
            }

            sql.OrderBy(m => m.SORTID, m => m.ID);

            var list = base.GetPageList(p);

            return new LayuiPaginationOut(p, list);
        }

        public IEnumerable<dynamic> GetTableList(string dbKey)
        {
            base.ChangeDb(dbKey);

            string sqlText = @"SELECT a.name AS TABLENAME,b.name AS SCHEMANAME FROM(
SELECT a.name,a.object_id,a.schema_id,'表' AS TYPE FROM sys.tables AS a 
UNION ALL
SELECT b.name,b.object_id,b.schema_id,'视图' AS TYPE  FROM sys.views AS b
) AS a LEFT JOIN sys.schemas AS b ON a.schema_id=b.schema_id
ORDER BY a.name";

            return base.GetDynamicList(sqlText, null);
        }

        public bool Delete(int[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new { ENABLED = false }, m => m.ENABLED);

            int count = Execute();

            return count > 0;
        }
    }
}
