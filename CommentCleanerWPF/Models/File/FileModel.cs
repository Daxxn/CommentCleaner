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
        #endregion

        #region - Constructors
        private FileModel( ) { }
        #endregion

        #region - Methods

        #region Open File Methods
        public List<CodeFile> OpenFiles( string[] paths )
        {
            List<CodeFile> output = new List<CodeFile>();
            foreach ( var path in paths )
            {
                if ( File.Exists(path) )
                {
                    try
                    {
                        using ( StreamReader reader = new StreamReader(path) )
                        {
                                output.Add(new CodeFile
                                {
                                    FileName = Path.GetFileName(path),
                                    FullPath = path,
                                    UnchangedCode = reader.ReadToEnd()
                                });
                        }
                    }
                    catch ( Exception e )
                    {
                        output.Add(new CodeFile
                        {
                            FileName = Path.GetFileName(path),
                            FullPath = path,
                            Error = e
                        });
                    }
                }
            }
            return output;
        }

        public (List<CodeFile>, string[]) OpenFilesFromDir( string dirPath, string[] filters, bool selectAll, bool shallowSearch, RegexModel selectedRegex )
        {
            string[] allPaths = GetFiles(dirPath, filters, selectAll, shallowSearch, selectedRegex);
            return (OpenFiles(allPaths), allPaths);
        }
        #endregion

        #region Cleaner Entry Methods
        public List<CodeFile> RunCleanerSync( string dirPath, string[] filters, bool selectAll, RegexModel selectedRegex )
        {
            string[] allFiles = GetFiles(dirPath, filters, selectAll, true, selectedRegex);

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
            string[] allFiles = GetFiles(dirPath, filters, selectAll, true, selectedRegex);

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

        public async Task RunCleanerAsync( List<CodeFile> codeFiles )
        {
            List<Task> tasks = new List<Task>();

            foreach ( var file in codeFiles )
            {
                tasks.Add(Task.Run(( ) =>
                {
                    try
                    {
                        RunCleanerRegex(file);
                    }
                    catch ( Exception e )
                    {
                        file.Error = e;
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
        #endregion

        #region File Filtering & Selection Methods
        public string[] GetFiles( string dirPath, string[] filters, bool selectAll, bool shallowSearch, RegexModel selectedRegexModel )
        {
            if ( !Directory.Exists(dirPath) )
            {
                throw new DirectoryNotFoundException();
            }
            string[] everyFile = shallowSearch ? GetFilesShallow(dirPath) : GetFilesDeep(dirPath);

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
        #endregion

        /// <summary>
        /// Matches comment strings in the file and removes them. May switch to a Split method for difference highlighting in the future.
        /// </summary>
        /// <param name="data">The RAW file string.</param>
        /// <param name="ext">The extension string used to match the Regex.</param>
        /// <returns>Returns a string with all found comments removed.</returns>
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

        private void RunCleanerRegex( CodeFile file )
        {
            var regex = RegexModel.AllRegexModels.First(reg => reg.Extension == Path.GetExtension(file.FullPath));
            string tempData = file.UnchangedCode;
            foreach ( var reg in regex.RegexCollection )
            {
                file.CleanedCode = reg.Regex.Replace(tempData, String.Empty);
            }
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
