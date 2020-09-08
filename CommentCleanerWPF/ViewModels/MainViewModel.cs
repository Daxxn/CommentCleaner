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
        private string _filePath = "";
        private string _dirPath;

        private ObservableCollection<FilterItem> _fileFilters;
        private string _newFileFilter;

        private DirectoryInfo _rootDir;

        private ObservableCollection<FileModel> _fileTypes;
        private FileModel _selectedFIleType;

        private string _selectedLineTerm;

        private string _unchangedCode;
        private string _cleanedCode;
        private CodeFile _selectedCodeFile;
        private ObservableCollection<CodeFile> _allCodeFiles;
        #endregion

        #region - Constructors
        public MainViewModel( )
        {
            DirPath = @"C:\Users\Cody\source\repos\CommentTestApp\CommentTestApp";
            RootDir = new DirectoryInfo(DirPath);
            FileTypes = new ObservableCollection<FileModel>
            {
                new FileModel
                {
                    Name="CSharp",
                    Extension=".cs",
                    BasicComment="//",
                    StartComment="/*",
                    EndComment="*/"
                },
                new FileModel
                {
                    Name="XAML",
                    Extension=".xaml",
                    StartComment="<!--",
                    EndComment="-->"
                }
            };
            SelectedFileType = FileTypes[ 0 ];

            FileFilters = new ObservableCollection<FilterItem>
            {
                new FilterItem(".g"),
                new FilterItem(".designer")
            };
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
                AllCodeFiles = new ObservableCollection<CodeFile>( SelectedFileType.RunCleaner(DirPath, FilterItem.ToStringArray(FileFilters)));
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
            
        }
        #endregion

        #region - Full Properties
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

        public DirectoryInfo RootDir
        {
            get { return _rootDir; }
            set
            {
                _rootDir = value;
                NotifyOfPropertyChange(nameof(RootDir));
            }
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

        public ObservableCollection<FileModel> FileTypes
        {
            get { return _fileTypes; }
            set
            {
                _fileTypes = value;
                NotifyOfPropertyChange(nameof(FileTypes));
            }
        }

        public FileModel SelectedFileType
        {
            get { return _selectedFIleType; }
            set
            {
                _selectedFIleType = value;
                NotifyOfPropertyChange(nameof(SelectedFileType));
            }
        }

        public string SelectedLineTerm
        {
            get { return _selectedLineTerm; }
            set
            {
                _selectedLineTerm = value;
                NotifyOfPropertyChange(nameof(SelectedLineTerm));
            }
        }
        public string UnchangedCode
        {
            get { return _unchangedCode; }
            set
            {
                _unchangedCode = value;
                NotifyOfPropertyChange(nameof(UnchangedCode));
            }
        }
        public string CleanedCode
        {
            get { return _cleanedCode; }
            set
            {
                _cleanedCode = value;
                NotifyOfPropertyChange(nameof(CleanedCode));
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
