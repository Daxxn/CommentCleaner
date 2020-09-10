using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommentCleanerWPF.Models.RegexModels
{
    public class RegexType
    {
        #region - Fields & Properties
        public string CommentType { get; set; }
        public string Expression { get; set; }
        public string Options { get; set; }
        public Regex Regex { get; set; }
        #endregion

        #region - Constructors
        public RegexType( ) { }
        #endregion

        #region - Methods
        public void BuildRegex( )
        {
            Regex = new Regex(Expression, ( RegexOptions )Enum.Parse(typeof(RegexOptions), Options));
        }
        #endregion

        #region - Full Properties

        #endregion
    }
}
