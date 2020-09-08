using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommentCleanerWPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = ( o, e ) => { };

        public void NotifyOfPropertyChange( string propertyName ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
