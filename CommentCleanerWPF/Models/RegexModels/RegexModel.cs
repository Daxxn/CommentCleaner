using CommentCleanerWPF.Models.FileStructures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CommentCleanerWPF.Models.RegexModels
{
    public class RegexModel
    {
        #region - Fields & Properties
        public static List<RegexModel> AllRegexModels { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public List<RegexType> RegexCollection { get; set; }
        #endregion

        #region - Constructors
        public RegexModel( ) { }
        #endregion

        #region - Methods

        public static void OnStartup( )
        {
            try
            {
                using ( StreamReader reader = new StreamReader(Settings.Default.RegexFilePath) )
                {
                    RegexCollection output = JsonConvert.DeserializeObject<RegexCollection>(reader.ReadToEnd());
                    AllRegexModels = output.RegexModelCollection;
                    AllRegexModels.ForEach(model => model.RegexCollection.ForEach(reg => reg.BuildRegex()));
                }
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public static void OnExit( )
        {
            //! Figure out how to save to Settings!
        }
        #endregion

        #region - Full Properties

        #endregion
    }
}
