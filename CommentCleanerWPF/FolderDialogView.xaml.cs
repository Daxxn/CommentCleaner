using CommentCleanerWPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommentCleanerWPF
{
    /// <summary>
    /// Interaction logic for FolderDialogView.xaml
    /// </summary>
    public partial class FolderDialogView : Window
    {
        public FolderDialogView( FolderDialogViewModel vm )
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
