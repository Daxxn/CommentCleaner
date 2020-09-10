using System;
using System.Collections.Generic;
using System.Text;

namespace CommentCleanerWPF.Models
{
    public class CodeFile
    {
        #region - Fields & Properties
        public Guid Id { get; private set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string UnchangedCode { get; set; }
        public string CleanedCode { get; set; }
        public Exception Error { get; set; }
        #endregion

        #region - Constructors
        public CodeFile( )
        {
            Id = Guid.NewGuid();
        }
        #endregion

        #region - Methods

        #endregion

        #region - Full Properties
        public bool ErrorOut
        {
            get
            {
                return Error == null ? false : true;
            }
        }
        #endregion
    }
}
