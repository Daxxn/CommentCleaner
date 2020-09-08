using CommentCleanerWPF.Models.FileStructures;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommentCleanerWPF.Models.Filter
{
    public class Filter
    {
        #region - Fields & Properties
        public Regex Regex { get; set; }
        public FileType Type { get; set; }
        #endregion

        #region - Constructors
        public Filter( ) { }
        #endregion

        #region - Methods

        #endregion

        #region - Full Properties

        #endregion
    }
}
