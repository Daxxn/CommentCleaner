using CommentCleanerWPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommentCleanerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow( MainViewModel vm )
        {
            InitializeComponent();
            DataContext = vm;
            InitializeEvents(vm);
        }

        public void InitializeEvents( MainViewModel vm )
        {
            //! Window
            MainRoot.Loaded += vm.LoadedEvent;

            //! File Management Buttons
            SelectFilesManuallyButton.Click += vm.SelectFilesEvent;
            SelectDirButton.Click += vm.SelectDirectoryEvent;

            // Change to open files and NOT run the cleaner
            OpenDirectoryButton.Click += vm.OpenDirectoryEvent;
            // Then use this to run the file.
            RunCleanerButton.Click += vm.RunCleanerEventAsync;

            //! Save Buttons
            SaveFileButton.Click += vm.SaveFileEventAsync;
            SaveAllFilesButton.Click += vm.SaveAllFilesEventAsync;

            //! Filter Controls
            AddNewFileFilterButton.Click += vm.AddNewFileFilterEvent;
            FileFilterList.SelectionChanged += vm.DeleteFileFilterEvent;
        }
    }
}
