﻿using CommentCleanerWPF.Models.FileStructures;
using CommentCleanerWPF.Models.RegexModels;
using CommentCleanerWPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CommentCleanerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup( StartupEventArgs e )
        {
            RegexModel.OnStartup();
            //FileModel.OnStartup();
            FileType.OnStartup();
            var window = new MainWindow(new MainViewModel());
            window.Show();
        }
    }
}
