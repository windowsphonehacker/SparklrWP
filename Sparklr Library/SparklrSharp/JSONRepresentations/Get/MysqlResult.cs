using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// Represents the result of an MySQL query
    /// </summary>
    public class MysqlResult
    {
        //{"affectedRows":1,"insertId":0}
        /// <summary>
        /// The number of affected rows
        /// </summary>
        public int affectedRows { get; set; }
        /// <summary>
        /// The last insert id
        /// </summary>
        public int insertId { get; set; }
    }
}
