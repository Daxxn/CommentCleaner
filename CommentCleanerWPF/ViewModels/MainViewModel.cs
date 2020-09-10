using CommentCleanerWPF.Exceptions;
using CommentCleanerWPF.Models;
using CommentCleanerWPF.Models.FileStructures;
using CommentCleanerWPF.Models.RegexModels;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private ObservableCollection<RegexModel> _fileTypes;

        private RegexModel _selectedFIleType;

        private bool _selectAll;

        private CodeFile _selectedCodeFile;
        private ObservableCollection<CodeFile> _allCodeFiles;
        #endregion

        #region - Constructors
        public MainViewModel( )
        {
            FileModel = FileModel.Instance;
            DirPath = @"C:\Users\Cody\source\repos\CommentTestApp\CommentTestApp";
            FileTypes = new ObservableCollection<RegexModel>(RegexModel.AllRegexModels);
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
                AllCodeFiles = new ObservableCollection<CodeFile>(await FileModel.RunCleanerAsync(DirPath, FilterItem.ToStringArray(FileFilters), SelectAll, SelectedFileType));
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

        /// <summary>
        /// Might not work as intended. Possible problem with Parallel Getting out of sync with the output List
        /// </summary>
        public async void SaveAllFilesEventAsync( object sender, EventArgs e )
        {
            try
            {
                List<Exception> allExceptions = new List<Exception>();
                await Task.Run(( ) =>
                {
                    Parallel.ForEach(AllCodeFiles, ( codeFile ) =>
                    {
                        try
                        {
                            using ( StreamWriter writer = new StreamWriter(codeFile.FullPath) )
                            {
                                writer.Write(codeFile.CleanedCode);
                                writer.Flush();
                            }
                        }
                        catch ( Exception innerExe )
                        {
                            var newInnerExe = new Exception($"-- {innerExe.GetType().Name} : {innerExe.Message}\n\t{codeFile.FileName} : {codeFile.FullPath}");
                            allExceptions.Add(innerExe);
                        }
                    });
                });

                if ( allExceptions.Count > 0 )
                {
                    throw new MultiException(allExceptions);
                }
            }
            catch ( MultiException exe )
            {
                StringBuilder builder = new StringBuilder("There were some Errors:");
                builder.AppendLine($"\nCount: {exe.Exceptions.Count()}\n");

                foreach ( var exeption in exe.Exceptions )
                {
                    builder.AppendLine(exeption.Message);
                }

                MessageBox.Show(builder.ToString(), "Multiple Exceptions");
            }
            catch ( Exception exe )
            {
                MessageBox.Show($"Unknown save error: {exe.Message}", "Error");
            }
        }

        public async void SaveAllFilesAsyncBackup( object sender, EventArgs eve )
        {
            try
            {
                List<Exception> allExceptions = new List<Exception>();
                List<Task> tasks = new List<Task>();

                foreach ( var codeFile in AllCodeFiles )
                {
                    tasks.Add(Task.Run(( ) =>
                    {
                        try
                        {
                            using ( StreamWriter writer = new StreamWriter(codeFile.FullPath) )
                            {
                                writer.Write(codeFile.CleanedCode);
                                writer.Flush();
                            }
                        }
                        catch ( Exception innerExe )
                        {
                            var newInnerExe = new Exception($"-- {innerExe.GetType().Name} : {innerExe.Message}\n\t{codeFile.FileName} : {codeFile.FullPath}");
                            allExceptions.Add(innerExe);
                        }
                    }));
                }

                await Task.WhenAll(tasks);

                if ( allExceptions.Count > 0 )
                {
                    throw new MultiException(allExceptions);
                }
            }
            catch ( MultiException exe )
            {
                StringBuilder builder = new StringBuilder("There were some Errors:");
                builder.AppendLine($"\nCount: {exe.Exceptions.Count()}\n");

                foreach ( var exeption in exe.Exceptions )
                {
                    builder.AppendLine(exeption.Message);
                }

                MessageBox.Show(builder.ToString(), "Multiple Exceptions");
            }
            catch ( Exception exe )
            {
                MessageBox.Show($"Unknown save error: {exe.Message}", "Error");
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

        public ObservableCollection<RegexModel> FileTypes
        {
            get { return _fileTypes; }
            set
            {
                _fileTypes = value;
                NotifyOfPropertyChange(nameof(FileTypes));
            }
        }

        public RegexModel SelectedFileType
        {
            get { return _selectedFIleType; }
            set
            {
                _selectedFIleType = value;
                NotifyOfPropertyChange(nameof(SelectedFileType));
            }
        }

        public bool SelectAll
        {
            get { return _selectAll; }
            set
            {
                _selectAll = value;
                NotifyOfPropertyChange(nameof(SelectAll));
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
