using CommentCleanerWPF.Models.Folder;

using System;
using System.Collections.Generic;
using System.Text;

namespace CommentCleanerWPF.ViewModels
{
    public class FolderDialogViewModel : ViewModelBase
    {
        #region - Fields & Properties
        private FolderModel _baseFolder;
        #endregion

        #region - Constructors
        public FolderDialogViewModel( ) { }
        #endregion

        #region - Methods

        #endregion

        #region - Full Properties
        public FolderModel BaseFolder
        {
            get { return _baseFolder; }
            set
            {
                _baseFolder = value;
                NotifyOfPropertyChange(nameof(BaseFolder));
            }
        }
        #endregion
    }
}
