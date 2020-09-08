using CommentCleanerWPF.Models;
using CommentCleanerWPF.Models.FileStructures;
using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CommentCleanerWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region - Fields & Properties
        private FileModel _fileModel;

        private string _filePath = "";
        private string _dirPath;

        private ObservableCollection<FilterItem> _fileFilters = new ObservableCollection<FilterItem>();
        private string _newFileFilter;

        //private ObservableCollection<FileModel> _fileTypes;
        private ObservableCollection<FileType> _fileTypes;
        //private FileModel _selectedFIleType;
        private FileType _selectedFIleType;

        private CodeFile _selectedCodeFile;
        private ObservableCollection<CodeFile> _allCodeFiles;
        #endregion

        #region - Constructors
        public MainViewModel( )
        {
            FileModel = FileModel.Instance;
            DirPath = @"C:\Users\Cody\source\repos\CommentTestApp\CommentTestApp";
            //FileTypes = new ObservableCollection<FileModel>
            //{
            //    new FileModel
            //    {
            //        Name="CSharp",
            //        Extension=".cs",
            //    },
            //    new FileModel
            //    {
            //        Name="XAML",
            //        Extension=".xaml",
            //    },
            //    new FileModel
            //    {
            //        Name="All",
            //        Extension=null,
            //        IsAll = true,
            //    }
            //};
            FileTypes = new ObservableCollection<FileType>(FileType.Types);
            SelectedFileType = FileTypes[ 0 ];
        }
        #endregion

        #region - Methods
        public void OpenFileEvent( object sender, EventArgs e )
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Title = "Select File"
            };

            if ( dialog.ShowDialog() == true )
            {
                FilePath = dialog.FileName;
            }
        }

        public void OpenDirEvent( object sender, EventArgs e )
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                AddExtension = false,
            };
        }

        public void AddNewFileFilterEvent( object sender, EventArgs e )
        {
            FileFilters.Add(new FilterItem(NewFileFilter));
            NewFileFilter = null;
        }

        public void DeleteFileFilterEvent( object sender, SelectionChangedEventArgs e )
        {
            if ( e.AddedItems.Count > 0 )
            {
                FileFilters.Remove(( FilterItem )e.AddedItems[ 0 ]);
            }
        }

        public async void RunCleanerEventAsync( object sender, EventArgs e )
        {
            try
            {
                if ( FilePath == "" && DirPath == "")
                {
                    throw new Exception("No file selected.");
                }
                AllCodeFiles = new ObservableCollection<CodeFile>(FileModel.RunCleaner(DirPath, FilterItem.ToStringArray(FileFilters)));
                //AllCodeFiles = new ObservableCollection<CodeFile>( SelectedFileType.RunCleaner(DirPath, FilterItem.ToStringArray(FileFilters)));
                //using ( StreamReader reader = new StreamReader(FilePath) )
                //{
                //    //Code = await SelectedFileType.RunCleanerRegexAsync(reader);
                //}
            }
            catch ( Exception exe )
            {
                MessageBox.Show(exe.Message, "Error");
            }
        }

        public async void SaveFileEventAsync( object sender, EventArgs e )
        {
            try
            {
                using ( StreamWriter writer = new StreamWriter(FilePath) )
                {
                    await writer.WriteAsync(SelectedCodeFile.CleanedCode);
                    writer.Flush();
                }
            }
            catch ( Exception exe )
            {
                MessageBox.Show(exe.Message, "Error");
            }
        }

        public void LoadedEvent( object sender, EventArgs e )
        {
            FileFilters = new ObservableCollection<FilterItem>(GetFileFilters(Settings.Default.IgnoredSubFiles, Settings.Default.IgnoredSubDelimiter));
        }

        private FilterItem[] GetFileFilters( string settingsValue, string delimiter )
        {
            try
            {
                List<FilterItem> output = new List<FilterItem>();
                string[] names = settingsValue.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                foreach ( var name in names )
                {
                    if ( !String.IsNullOrWhiteSpace(name) )
                    {
                        output.Add(new FilterItem(name));
                    }
                }
                return output.ToArray();
            }
            catch ( Exception e )
            {
                MessageBox.Show($"Unable to build filters: {e.Message}", "Error");
                return new FilterItem[ 0 ];
            }
        }
        #endregion

        #region - Full Properties
        public FileModel FileModel
        {
            get { return _fileModel; }
            set
            {
                _fileModel = value;
            }
        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(nameof(FilePath));
                NotifyOfPropertyChange(nameof(FileName));
            }
        }

        public string DirPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                NotifyOfPropertyChange(nameof(DirPath));
                NotifyOfPropertyChange(nameof(GoodDir));
            }
        }

        public ObservableCollection<FilterItem> FileFilters
        {
            get { return _fileFilters; }
            set
            {
                _fileFilters = value;
                NotifyOfPropertyChange(nameof(FileFilters));
            }
        }

        public string NewFileFilter
        {
            get { return _newFileFilter; }
            set
            {
                _newFileFilter = value;
                NotifyOfPropertyChange(nameof(NewFileFilter));
            }
        }

        public bool GoodDir
        {
            get => Directory.Exists(DirPath);
        }

        public string FileName
        {
            get
            {
                try
                {
                    return Path.GetFileName(FilePath);
                }
                catch ( Exception )
                {
                    return "";
                }
            }
        }

        //public ObservableCollection<FileModel> FileTypes
        public ObservableCollection<FileType> FileTypes
        {
            get { return _fileTypes; }
            set
            {
                _fileTypes = value;
                NotifyOfPropertyChange(nameof(FileTypes));
            }
        }

        //public FileModel SelectedFileType
        public FileType SelectedFileType
        {
            get { return _selectedFIleType; }
            set
            {
                _selectedFIleType = value;
                NotifyOfPropertyChange(nameof(SelectedFileType));
            }
        }

        public CodeFile SelectedCodeFile
        {
            get { return _selectedCodeFile; }
            set
            {
                _selectedCodeFile = value;
                NotifyOfPropertyChange(nameof(SelectedCodeFile));
            }
        }

        public ObservableCollection<CodeFile> AllCodeFiles
        {
            get { return _allCodeFiles; }
            set
            {
                _allCodeFiles = value;
                NotifyOfPropertyChange(nameof(AllCodeFiles));
            }
        }
        #endregion
    }
}
