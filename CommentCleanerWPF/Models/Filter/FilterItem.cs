using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommentCleanerWPF.Models
{
    public class FilterItem
    {
        #region - Fields & Properties
        public string Name { get; set; }
        #endregion

        #region - Constructors
        public FilterItem( string name )
        {
            Name = name;
        }
        #endregion

        #region - Methods
        public static string[] ToStringArray( IEnumerable<FilterItem> data )
        {
            return data.Select(item => item.Name).ToArray();
        }
        #endregion

        #region - Full Properties

        #endregion
    }
}
