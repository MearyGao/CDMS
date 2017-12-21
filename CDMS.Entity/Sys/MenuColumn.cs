using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    [Table("SYS_MENUCOLUMN")]
    public class MenuColumn
    {
        /// <summary>
        ///ID
        /// </summary>
        [Column(true)]
        public int ID { get; set; }
        /// <summary>
        ///MENUID
        /// </summary>
        public string MENUID { get; set; }

        /// <summary>
        /// 表ID
        /// </summary>
        public int TABLEID { get; set; }
        /// <summary>
        ///NAME
        /// </summary>
        public string NAME { get; set; }
        /// <summary>
        ///TYPE
        /// </summary>
        public int TYPE { get; set; }
        /// <summary>
        ///CONDITIONTYPE
        /// </summary>
        public int CONDITIONTYPE { get; set; }
        /// <summary>
        ///INPUTTYPE
        /// </summary>
        public int INPUTTYPE { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public int FIELDTYPE { get; set; }
        /// <summary>
        /// 字段描述
        /// </summary>
        public string FIELDTEXT { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string FIELDVALUE { get; set; }
        /// <summary>
        ///SORTID
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
