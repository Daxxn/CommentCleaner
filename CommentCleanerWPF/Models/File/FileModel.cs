using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommentCleanerWPF.Models.FileStructures
{
    public class FileModel
    {
        #region - Fields & Properties
        private static FileModel _instance;
        public static Regex BasicCommentRegexCS { get; set; } = new Regex(@"[\t\s]+(//(?![!/])).*?$", RegexOptions.Multiline);
        public static Regex MultilineCommentRegexCS { get; set; } = new Regex(@"[\t\s]*/\*[^!].*?\*/", RegexOptions.Singleline);
        public static Regex MultilineCommentXAML { get; set; } = new Regex(@"[\t\s]*<!--[^#].*-->", RegexOptions.Multiline);

        public static Dictionary<string, Regex> CommentRegex { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public bool IsAll { get; set; } = false;
        #endregion

        #region - Constructors
        private FileModel( ) { }
        #endregion

        #region - Methods
        public List<CodeFile> RunCleaner( string dirPath, string[] filters )
        {
            if ( !Directory.Exists(dirPath) )
            {
                throw new DirectoryNotFoundException();
            }
            List<CodeFile> output = new List<CodeFile>();
            string[] everyFile = GetAllFiles(dirPath);

            List<string> allFiles = new List<string>(everyFile.Where(p =>
            {
                string ext = Path.GetExtension(p);
                return ext == ".cs" || ext == ".xaml";
            }));
            List<string> filteredFiles = CustomFilter(filters, allFiles);

            return output;
        }

        private string[] GetAllFiles( string path )
        {
            string[] paths = Directory.GetDirectories(path);
            List<string> files = new List<string>(Directory.GetFiles(path));
            foreach ( var p in paths )
            {
                files.AddRange(GetAllFiles(p));
            }
            return files.ToArray();
        }

        public List<string> CustomFilter( string[] filters, List<string> allFiles )
        {
            List<string> newList = new List<string>();
            foreach ( var file in allFiles )
            {
                bool pass = true;
                foreach ( var filter in filters )
                {
                    if ( file.Contains(filter) )
                    {
                        pass = false;
                    }
                }
                if ( pass )
                {
                    newList.Add(file);
                }
            }
            return newList;
        }

        /// <summary>
        /// VERY OLD - Doesnt work, will never work.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="lineTerm"></param>
        /// <returns></returns>
        //public CodeFile RunCleaner( StreamReader file, string lineTerm )
        //{
        //    CodeFile output = new CodeFile();
        //    string completeFile = file.ReadToEnd();
        //    StringBuilder allLines = new StringBuilder();
        //    string[] lines = completeFile.Split(new string[] { LineTerminators[lineTerm] }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach ( var line in lines )
        //    {
        //        if ( line.StartsWith("//!") )
        //        {
        //            continue;
        //        }
        //        if ( !line.StartsWith("//") )
        //        {
        //            allLines.Append(line);
        //        }
        //    }
        //    output.UnchangedCode = completeFile;
        //    output.CleanedCode = allLines.ToString();

        //    return output;
        //}

        /// <summary>
        /// VERY OLD - Doesnt work, will never work, Async
        /// </summary>
        /// <param name="file"></param>
        /// <param name="lineTerm"></param>
        /// <returns></returns>
        //public async Task<CodeFile> RunCleanerAsync( StreamReader file, string lineTerm )
        //{
        //    CodeFile output = new CodeFile();
        //    List<string> lines = await ReadLinesAsync(file);
        //    StringBuilder changedOutput = new StringBuilder();
        //    StringBuilder unchangedOutput = new StringBuilder();
        //    await Task.Run(( ) =>
        //    {
        //        foreach ( var line in lines )
        //        {
        //            if ( !line.Contains("//") || line.Contains("//!") )
        //            {
        //                changedOutput.Append(line);
        //                changedOutput.Append("\n");
        //            }
        //            unchangedOutput.Append(line);
        //            unchangedOutput.Append("\n");
        //        }

        //        output.UnchangedCode = unchangedOutput.ToString();
        //        output.CleanedCode = changedOutput.ToString();

        //        CleanMultilineComments(changedOutput.ToString());
        //    });

        //    return output;
        //}

        public CodeFile RunCleanerRegex( StreamReader file )
        {
            CodeFile output = new CodeFile();
            string fileData = file.ReadToEnd();
            output.UnchangedCode = fileData;
            var basicOut = BasicCommentRegexCS.Replace(fileData, "");
            var multiOut = MultilineCommentRegexCS.Replace(basicOut, "");
            output.CleanedCode = multiOut;
            return output;
        }

        public async Task<CodeFile> RunCleanerRegexAsync( StreamReader file )
        {
            CodeFile output = new CodeFile();
            string fileData = await file.ReadToEndAsync();
            output.UnchangedCode = fileData;
            await Task.Run(( ) =>
            {
                string regOut = BasicCommentRegexCS.Replace(fileData, "");
                output.CleanedCode = MultilineCommentRegexCS.Replace(regOut, "");
            });
            return output;
        }

        /// <summary>
        /// OLD - Would async read each line of the file. 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<List<string>> ReadLinesAsync( StreamReader file )
        {
            List<string> output = new List<string>();
            await Task.Run(( ) =>
            {
                while ( !file.EndOfStream )
                {
                    output.Add(file.ReadLine());
                }
            });
            return output;
        }

        /// <summary>
        /// OLD - Used the stringSplit method. Its really hard to use that for multi-line comments.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        //private string CleanMultilineComments( string lines )
        //{
        //    StringBuilder builder = new StringBuilder();
        //    string[] output = lines.Split(new string[] { StartComment, EndComment }, StringSplitOptions.RemoveEmptyEntries);
        //    if ( output.Length == 1 )
        //    {
        //        return lines;
        //    }
        //    else if ( output.Length > 1 && output.Length < 3 )
        //    {
        //        // Not sure what to do here yet.
        //        return lines;
        //    }
        //    if ( output.Length == 3 )
        //    {
        //        builder.Append(output[ 0 ]);
        //        builder.Append(output[ 2 ]);
        //        return builder.ToString();
        //    }
        //    else if ( output.Length > 3 )
        //    {
        //        var newOutput = CleanEmptyStrings(output);
        //        for ( int i = 0; i < newOutput.Length; i++ )
        //        {
        //            if ( i % 2 == 0 )
        //            {
        //                builder.Append(newOutput[ i ]);
        //            }
        //        }
        //        return builder.ToString();
        //    } 
        //    else
        //    {
        //        throw new Exception("Unknown Multiline Comments Parse Error");
        //    }
        //}

        /// <summary>
        /// OLD - Removes empty lines from the file. Still might be necessary.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private string[] CleanEmptyStrings( string[] strings )
        {
            List<string> output = new List<string>();
            Regex reg = new Regex(@"[\w\d]", RegexOptions.Multiline);
            foreach ( var str in strings )
            {
                if ( reg.Match(str).Success )
                {
                    output.Add(str);
                }
            }
            return output.ToArray();
        }

        public static void OnStartup( )
        {
            CommentRegex = new Dictionary<string, Regex>();
            var settingBase = RegexSettings.Synchronized(RegexSettings.Default);
            var props = settingBase.Properties.SyncRoot;
            var propCollection = props as SettingsPropertyCollection;
            foreach ( var prop in propCollection )
            {
                var p = prop as SettingsProperty;
                CommentRegex.Add(p.Name, ParseRegex(( string )p.DefaultValue));
            }
        }

        private static Regex ParseRegex( string settingValue )
        {
            string[] expressionSplt = settingValue.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

            RegexOptions options = RegexOptions.None;
            if ( expressionSplt.Length == 2 )
            {
                options = ( RegexOptions )Enum.Parse(typeof(RegexOptions), expressionSplt[ 1 ]);
            }

            return new Regex(expressionSplt[0], options);
        }
        #endregion

        #region - Full Properties
        public static FileModel Instance
        {
            get
            {
                if ( _instance is null )
                {
                    _instance = new FileModel();
                }
                return _instance;
            }
        }
        #endregion
    }
}
