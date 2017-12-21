using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IMenuColumnRepository : IRepository<MenuColumn>
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得列列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetColumnList(int menuId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetColumnList(int menuId, string key);

        /// <summary>
        /// 添加菜单列
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="cids"></param>
        /// <returns></returns>
        bool AddColumns(int pid, string uid, int[] cids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(int[] ids);

        /// <summary>
        /// 查询菜单列信息
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
        dynamic GetColumn(int columnId);

        /// <summary>
        /// 获得表列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<MenuTable> GetTableList();
    }

    public class MenuColumnRepository : RepositoryBase<MenuColumn>, IMenuColumnRepository
    {
        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            sql.SelectAll();

            var tableSql = sql.Join<MenuTable>((c, t) => c.TABLEID == t.ID, aliasName: "b");
            tableSql.Select(m => m.TABLENAME);

            sql.Where(m => m.ENABLED == true);

            var model = p.json.ToObject<MenuColumn>();
            if (model != null)
            {
                if (model.TABLEID > 0) sql.And(m => m.TABLEID == model.TABLEID);
                if (!model.NAME.IsEmpty())
                {
                    sql.And().Begin();
                    sql.Or(m => m.NAME.Contains(model.NAME));
                    tableSql.Or(m => m.TABLENAME.Contains(model.NAME));
                    sql.End();
                }
            }

            sql.OrderBy(m => m.TABLEID, m => m.TYPE, m => m.SORTID);

            var list = base.GetDynamicPageList(p, sql);

            return new LayuiPaginationOut(p, list);
        }

        public bool Delete(int[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new { ENABLED = false }, m => m.ENABLED);

            int count = Execute();

            return count > 0;
        }

        public IEnumerable<dynamic> GetColumnList(int tableId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@TABLEID", tableId);
            return base.GetDynamicList("[P_GETTABLECOLUMN]", dic, System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<dynamic> GetColumnList(int menuId, string key)
        {
            string sqlText = @";WITH 
TREE AS( 
SELECT ID,PARENTID,NAME AS MENUNAME FROM dbo.SYS_MENU 
WHERE ID =@mid
UNION ALL 
SELECT a.ID,a.PARENTID,a.NAME AS MENUNAME FROM dbo.SYS_MENU AS a ,TREE AS b
WHERE b.PARENTID=a.ID
) 
SELECT b.*,a.MENUNAME,c.DBNAME,c.SCHEMANAME,c.TABLENAME FROM TREE AS a RIGHT JOIN dbo.SYS_MENUCOLUMN AS b ON a.ID=b.MENUID
LEFT JOIN dbo.SYS_MENUTABLE AS c ON b.MENUID=c.MENUID AND b.TABLEID=c.ID
WHERE b.ENABLED=1 AND a.ID IS NOT NULL ";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@mid", menuId);
            if (!key.IsEmpty())
            {
                sqlText += " AND B.NAME LIKE @key ";
                dic.Add("@key", key);
            }
            return base.GetDynamicList(sqlText, dic);
        }

        public bool AddColumns(int pid, string uid, int[] cids)
        {
            return base.UseTran(() =>
            {
                //string sqlText = @"DELETE FROM dbo.SYS_MENU WHERE PARENTID=@pid";
                //Dictionary<string, object> dic = new Dictionary<string, object>();
                //dic.Add("pid", pid);
                //base.Execute(sqlText, dic);

                string sqlText = string.Format(@"INSERT INTO dbo.SYS_MENU
        ( NAME ,
          TYPE ,
          PARENTID ,
          DISPLAY ,
          OBJECTID ,
          SORTID ,
          CREATEBY ,
          CREATEDATE ,
          UPDATEBY ,
          UPDATEDATE ,
          ENABLED
        )
SELECT FIELDTEXT,4,{0},1,MENUID,SORTID,'{1}',GETDATE(),'{1}',GETDATE(),1 FROM dbo.SYS_MENUCOLUMN WHERE ID IN({2})", pid, uid, string.Join(",", cids));
                base.Execute(sqlText, null);
            });
        }

        public dynamic GetColumn(int columnId)
        {
            sql.SelectAll();
            sql.Where(m => m.ENABLED == true && m.ID == columnId);
            var tableSql = sql.Join<MenuTable>((c, t) => c.TABLEID == t.ID, aliasName: "b");
            tableSql.Select(m => m.TABLENAME);
            return GetDynamicList(sql.GetSql(), sql.GetParameters()).FirstOrDefault();
        }

        public IEnumerable<MenuTable> GetTableList()
        {
            var tableSql = base.GetSqlLam<MenuTable>();
            tableSql.SelectAll();
            tableSql.Where(m => m.ENABLED == true);
            tableSql.OrderBy(m => m.DBNAME, m => m.TABLENAME);
            return base.GetList<MenuTable>(tableSql.GetSql(), tableSql.GetParameters());
        }
    }
}
