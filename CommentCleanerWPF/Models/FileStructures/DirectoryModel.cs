using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommentCleanerWPF.Models.FileStructures
{
    public class DirectoryModel
    {
        #region - Fields & Properties
        public string FullPath { get; set; }
        public DirectoryModel[] Children { get; set; }
        #endregion

        #region - Constructors
        public DirectoryModel( ) { }
        #endregion

        #region - Methods
        public void GetChildren( )
        {
        }
        #endregion

        #region - Full Properties
        #endregion
    }
}
