using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    [Table("SYS_MENUTABLE")]
    public class MenuTable
    {
        /// <summary>
        ///ID
        /// </summary>
        [Column(true)]
        public int ID { get; set; }
        /// <summary>
        ///数据库名称
        /// </summary>
        public string DBNAME { get; set; }
        /// <summary>
        ///表名
        /// </summary>
        public string TABLENAME { get; set; }
        /// <summary>
        ///架构名
        /// </summary>
        public string SCHEMANAME { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string ALIASNAME { get; set; }
        /// <summary>
        ///排序ID
        /// </summary>
        public int SORTID { get; set; }
        /// <summary>
        ///CREATEBY
        /// </summary>
        public string CREATEBY { get; set; }
        /// <summary>
        ///CREATEDATE
        /// </summary>
        public DateTime CREATEDATE { get; set; }
        /// <summary>
        ///UPDATEBY
        /// </summary>
        public string UPDATEBY { get; set; }
        /// <summary>
        ///UPDATEDATE
        /// </summary>
        public DateTime UPDATEDATE { get; set; }
        /// <summary>
        ///ENABLED
        /// </summary>
        public bool ENABLED { get; set; }
    }
}
