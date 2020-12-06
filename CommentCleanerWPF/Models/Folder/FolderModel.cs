using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace CommentCleanerWPF.Models.Folder
{
    public class FolderModel
    {
        #region - Fields & Properties
        public string FullPath { get; set; }

        public FolderModel Parent { get; set; }
        public ObservableCollection<FolderModel> Children { get; set; }
        #endregion

        #region - Constructors
        public FolderModel( ) { }
        #endregion

        #region - Methods

        #endregion

        #region - Full Properties
        public string Name
        {
            get
            {
                return Path.GetDirectoryName(FullPath);
            }
        }
        #endregion
    }
}
