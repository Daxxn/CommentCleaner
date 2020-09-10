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
            MainRoot.Loaded += vm.LoadedEvent;
            OpenFileButton.Click += vm.OpenFileEvent;
            RunCleanerButton.Click += vm.RunCleanerEventAsync;
            SaveFileButton.Click += vm.SaveFileEventAsync;
            SaveAllFilesButton.Click += vm.SaveAllFilesEventAsync;
            AddNewFileFilterButton.Click += vm.AddNewFileFilterEvent;
            FileFilterList.SelectionChanged += vm.DeleteFileFilterEvent;
        }
    }
}
