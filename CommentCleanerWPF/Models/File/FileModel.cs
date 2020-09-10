using CommentCleanerWPF.Models.RegexModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

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
        public List<CodeFile> RunCleanerSync( string dirPath, string[] filters, bool selectAll, RegexModel selectedRegex )
        {
            string[] allFiles = GetFiles(dirPath, filters, selectAll, selectedRegex);

            List<CodeFile> output = new List<CodeFile>();

            foreach ( var file in allFiles )
            {
                CodeFile codeFile = new CodeFile();
                try
                {
                    codeFile.FullPath = file;
                    codeFile.FileName = Path.GetFileName(file);
                    using ( StreamReader reader = new StreamReader(file) )
                    {
                        string fileData = reader.ReadToEnd();
                        codeFile.UnchangedCode = fileData;
                        codeFile.CleanedCode = RunCleanerRegex(fileData, Path.GetExtension(file));
                    }
                }
                catch ( Exception e )
                {
                    codeFile.Error = e;
                }
                output.Add(codeFile);
            }

            return output;
        }

        public async Task<List<CodeFile>> RunCleanerAsync( string dirPath, string[] filters, bool selectAll, RegexModel selectedRegex )
        {
            string[] allFiles = GetFiles(dirPath, filters, selectAll, selectedRegex);

            List<Task<CodeFile>> tasks = new List<Task<CodeFile>>();

            foreach ( var file in allFiles )
            {
                tasks.Add(Task.Run(( ) =>
                {
                    CodeFile codeFile = new CodeFile();
                    try
                    {
                        codeFile.FullPath = file;
                        codeFile.FileName = Path.GetFileName(file);
                        using ( StreamReader reader = new StreamReader(file) )
                        {
                            string fileData = reader.ReadToEnd();
                            codeFile.UnchangedCode = fileData;
                            codeFile.CleanedCode = RunCleanerRegex(fileData, Path.GetExtension(file));
                        }
                    }
                    catch ( Exception e )
                    {
                        codeFile.Error = e;
                    }
                    return codeFile;
                }));
            }

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public string[] GetFiles( string dirPath, string[] filters, bool selectAll, RegexModel selectedRegexModel )
        {
            if ( !Directory.Exists(dirPath) )
            {
                throw new DirectoryNotFoundException();
            }
            string[] everyFile = GetFilesDeep(dirPath);

            return FilterFiles(everyFile, filters, selectAll, selectedRegexModel);
        }

        private string[] GetFilesDeep( string path )
        {
            string[] paths = Directory.GetDirectories(path);
            List<string> files = new List<string>(Directory.GetFiles(path));
            foreach ( var p in paths )
            {
                files.AddRange(GetFilesDeep(p));
            }
            return files.ToArray();
        }

        private string[] GetFilesShallow( string dirPath )
        {
            return Directory.GetFiles(dirPath);
        }

        private string[] FilterFiles( string[] allFiles, string[] filters, bool selectAll, RegexModel selectedRegex )
        {
            List<string> selectedFiles = new List<string>();

            if ( !selectAll )
            {
                foreach ( var file in allFiles )
                {
                    if ( Path.GetExtension(file) == selectedRegex.Extension  )
                    {
                        selectedFiles.Add(file);
                    }
                }
            }
            else
            {
                selectedFiles = GetAllFilesWithExtensions(allFiles);
            }
            return CustomFilter(selectedFiles, filters).ToArray();
        }

        private List<string> GetAllFilesWithExtensions( IEnumerable<string> everyFile )
        {
            string[] allExtensions = RegexModel.AllRegexModels.Select(reg => reg.Extension).ToArray();

            List<string> output = new List<string>();

            foreach ( var file in everyFile )
            {
                if ( allExtensions.Contains(Path.GetExtension(file)) )
                {
                    output.Add(file);
                }
            }
            return output;
        }

        private List<string> CustomFilter( IEnumerable<string> allFiles, string[] filters )
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
        /// VERY Temporary. Need to rework the Regex model.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string RunCleanerRegex( string data, string ext )
        {
            var regex = RegexModel.AllRegexModels.First(reg => reg.Extension == ext);
            string output = data;
            foreach ( var r in regex.RegexCollection )
            {
                output = r.Regex.Replace(output, "");
            }
            return output;
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
