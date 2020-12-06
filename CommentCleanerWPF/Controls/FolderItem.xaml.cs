using CommentCleanerWPF.Models.Folder;

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommentCleanerWPF.Controls
{
    /// <summary>
    /// Interaction logic for FolderItem.xaml
    /// </summary>
    public partial class FolderItem : UserControl
    {



        public FolderModel Folder
        {
            get { return ( FolderModel )GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Folder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(FolderItem), typeof(FolderItem), new PropertyMetadata(0));



        public FolderItem( )
        {
            InitializeComponent();
        }
    }
}
