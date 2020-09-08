using System;
using System.Collections.Generic;
using System.Text;

namespace CommentCleanerWPF.Models.FileStructures
{
    public class FileType
    {
        #region - Fields & Properties
        public static List<FileType> Types { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        #endregion

        #region - Constructors
        public FileType( ) { }
        #endregion

        #region - Methods
        public static void OnStartup( )
        {
            Types = new List<FileType>();
            try
            {
                var settingStrings = Settings.Default.FileTypes.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                foreach ( var settingStr in settingStrings )
                {
                    var settings = settingStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var temp = new FileType();
                    foreach ( var setting in settings )
                    {
                        var keyValues = setting.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if ( keyValues.Length == 2 )
                        {
                            switch ( keyValues[0] )
                            {
                                case "Name":
                                    temp.Name = keyValues[ 1 ];
                                    break;
                                case "Extension":
                                    temp.Extension = keyValues[ 1 ];
                                    break;
                                default:
                                    throw new ArgumentException("Unknown Key");
                            }
                        }
                    }
                    Types.Add(temp);
                }
            }
            catch ( Exception )
            {
                throw;
            }
        }
        #endregion

        #region - Full Properties

        #endregion
    }
}
