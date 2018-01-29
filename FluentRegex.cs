using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FluentRegexCSharp
{
    class FluentRegex
    {

        private StringBuilder _sb = new StringBuilder();

        private const string RESERVED_CHARACTERS = @"^$[](){}.|*+?\";

        private FluentRegex()
        {
            // prevent direct instantiations of this class 
        }

        /// <summary>
        /// Overridden to return the current regex pattern string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _sb.ToString();
        }

        /// <summary>
        /// Factory Accessor
        /// </summary>
        /// <returns></returns>
        public static FluentRegex StartRegex()
        {
            return new FluentRegex();
        }

        /// <summary>
        /// This is just a pass-through function - syntactically doing nothing,
        /// but is included to improve the Human-Readability of code using this class
        /// </summary>
        /// <returns></returns>
        public FluentRegex Then
        {
            get
            {
                return this;
            }
        }

        #region " Underlying Regex Method Accessors "

        /// <summary>
        /// Indicates whether or not the defined regex pattern is found in the provided string or not
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public Boolean IsMatch(string Value, System.Text.RegularExpressions.RegexOptions Options = System.Text.RegularExpressions.RegexOptions.None)
        {
            return _compiledRegEx(Options).IsMatch(Value);
        }

        /// <summary>
        /// Searches the provided string for all matches of the defined regex pattern
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public System.Text.RegularExpressions.MatchCollection Matches(string Value, System.Text.RegularExpressions.RegexOptions Options = System.Text.RegularExpressions.RegexOptions.None)
        {
            return _compiledRegEx(Options).Matches(Value);
        }

        #endregion

        #region " Anchors "

        /// <summary>
        /// Start of Line/String Anchor ^
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchStartOfLine
        {
            get
            {
                _sb.Append(@"^");
                return this;
            }
        }

        /// <summary>
        /// End of Line/String Anchor $
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchEndOfLine
        {
            get
            {
                _sb.Append(@"$");
                return this;
            }
        }

        #endregion

        #region " Literals "

        /// <summary>
        /// A literal pattern. The method handles any escaping any individual characters in the provided value may need.
        /// </summary>
        /// <param name="LiteralValue"></param>
        /// <returns></returns>
        public FluentRegex MatchLiteral(string LiteralValue)
        {
            foreach (Char c in LiteralValue.ToCharArray())
            {
                if (_characterNeedsEscaping(c))
                {
                    _sb.AppendFormat(@"\{0}", c);
                }
                else
                {
                    _sb.Append(c);
                }
            }

            return this;
        }

        #endregion

        #region " Character Sets "
        /// <summary>
        /// Generates a character set based on any number of the predefined sets, plus any other characters provided.
        /// </summary>
        /// <param name="CharSets"></param>
        /// <param name="OtherChars"></param>
        /// <returns></returns>
        public FluentRegex MatchCharacterSets(PredefinedSets CharSets, params Char[] OtherChars)
        {
            string Pattern = @"[";
            if ((CharSets & PredefinedSets.AnyCharacter) > 0)
            {
                Pattern += ".";
            }
            else
            {
                if ((CharSets & PredefinedSets.Digit) > 0)
                {
                    Pattern += @"\d";
                }
                else
                {
                    if ((CharSets & PredefinedSets.LowerCaseLetter) > 0)
                    {
                        Pattern += @"a-z";
                    }
                    else
                    {
                        if ((CharSets & PredefinedSets.NonDigit) > 0)
                        {
                            Pattern += @"\D";
                        }
                        else if ((CharSets & PredefinedSets.NonWhiteSpace) > 0)
                        {
                            Pattern += @"\S";
                        }
                        else if ((CharSets & PredefinedSets.NonWordBreak) > 0)
                        {
                            Pattern += @"\B";
                        }
                        else if ((CharSets & PredefinedSets.NonWordCharacter) > 0)
                        {
                            Pattern += @"\W";
                        }
                        else if ((CharSets & PredefinedSets.UpperCaseLetter) > 0)
                        {
                            Pattern += @"A-Z";
                        }
                        else if ((CharSets & PredefinedSets.WhiteSpace) > 0)
                        {
                            Pattern += @"\s";
                        }
                        else if ((CharSets & PredefinedSets.WordBreak) > 0)
                        {
                            Pattern += @"\b";
                        }
                        else if ((CharSets & PredefinedSets.WordCharacter) > 0)
                        {
                            Pattern += @"\w";
                        }

                        if (OtherChars != null)
                        {
                            foreach (Char ch in OtherChars)
                            {
                                if (_characterNeedsEscaping(ch))
                                {
                                    Pattern += @"\";
                                }
                                Pattern += ch;
                            }
                        }

                        Pattern += @"]";
                        _sb.Append(Pattern);
                        return this;
                    }
                }
            }

            ///??????
            return null;
        }

        /// <summary>
        /// Matches any character '.'
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchAnything
        {
            get
            {
                _sb.Append(@".");
                return this;
            }
        }

        /// <summary>
        /// Matches a digit character \d
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchDigit
        {
            get
            {
                _sb.Append(@"\d");
                return this;
            }
        }

        /// <summary>
        /// Matches a non-digit character \D
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchNonDigit
        {
            get
            {
                _sb.Append(@"\D");
                return this;
            }
        }

        /// <summary>
        /// Matches a word character \w
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchWordCharacter
        {
            get
            {
                _sb.Append(@"\w");
                return this;
            }
        }

        /// <summary>
        /// Matches a non-word character \W
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchNonWordCharacter
        {
            get
            {
                _sb.Append(@"\W");
                return this;
            }
        }

        /// <summary>
        /// Matches a word break \b
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchWordBreak
        {
            get
            {
                _sb.Append(@"\b");
                return this;
            }
        }

        /// <summary>
        /// Matches a non-word-break \B
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchNonWordBreak
        {
            get
            {
                _sb.Append(@"\B");
                return this;
            }
        }

        /// <summary>
        /// Matches any letter [a-zA-Z]
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchAnyLetter
        {
            get
            {
                _sb.Append(@"[a-zA-Z]");
                return this;
            }
        }

        /// <summary>
        /// Matches any lowercase letter [a-z]
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchAnyLowerCaseLetter
        {
            get
            {
                _sb.Append(@"[a-z]");
                return this;
            }
        }

        /// <summary>
        /// Matches any uppercase letter [A-Z]
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchAnyUpperCaseLetter
        {
            get
            {
                _sb.Append(@"[A-Z]");
                return this;
            }
        }

        /// <summary>
        /// Matches a white-space character \s
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchAnyWhiteSpaceCharacter
        {
            get
            {
                _sb.Append(@"\s");
                return this;
            }
        }

        /// <summary>
        /// Matches a non-white-space character \S
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchAnyNonWhiteSpaceCharacter
        {
            get
            {
                _sb.Append(@"\S");
                return this;
            }
        }

        #endregion

        #region " Non-Printable Characters "

        /// <summary>
        /// Match a Tab \t
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchTab
        {
            get
            {
                _sb.Append(@"\t");
                return this;
            }
        }

        /// <summary>
        /// Match a Carriage return \r
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchCarriagereturn
        {
            get
            {
                _sb.Append(@"\r");
                return this;
            }
        }

        /// <summary>
        /// Match an ASCII Bell Character \a
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchASCIIBellCharacter
        {
            get
            {
                _sb.Append(@"\a");
                return this;
            }
        }

        /// <summary>
        /// Match an ASCII Escape Character \e
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchASCIIEscapeCharacter
        {
            get
            {
                _sb.Append(@"\e");
                return this;
            }
        }

        /// <summary>
        /// Match a Form Feed Character \f
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchFormFeed
        {
            get
            {
                _sb.Append(@"\f");
                return this;
            }
        }

        /// <summary>
        /// Match a Vertical Tab \v
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchVerticalTab
        {
            get
            {
                _sb.Append(@"\v");
                return this;
            }
        }

        /// <summary>
        /// Match a Line Feed Character (alias for MatchNewLine) \n
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchLineFeed
        {
            get
            {
                return MatchNewLine;
            }
        }

        /// <summary>
        /// Match a New Line Character \n
        /// </summary>
        /// <returns></returns>
        public FluentRegex MatchNewLine
        {
            get
            {
                _sb.Append(@"\n");
                return this;
            }
        }

        #endregion

        #region " Groups "

        /// <summary>
        /// Define a match group
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public FluentRegex Group(FluentRegex pattern)
        {
            _sb.AppendFormat(@"({0})", pattern);
            return this;
        }

        /// <summary>
        /// Define a match group
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public FluentRegex Group(string pattern)
        {
            _sb.AppendFormat(@"({0})", pattern);
            return this;
        }

        /// <summary>
        /// Define a named match group
        /// </summary>
        /// <param name="Pattern"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public FluentRegex NamedGroup(FluentRegex Pattern, string Name)
        {
            _sb.AppendFormat(@"(?<{1}>{0})", Pattern, Name);
            return this;
        }

        /// <summary>
        /// Define a named match group
        /// </summary>
        /// <param name="Pattern"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public FluentRegex NamedGroup(string Pattern, string Name)
        {
            _sb.AppendFormat(@"(?<{1}>{0})", Pattern, Name);
            return this;
        }

        /// <summary>
        /// Add a back-reference to a named match group
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public FluentRegex ReferenceNamedGroup(string Name)
        {
            _sb.AppendFormat(@"\k<{0}>", Name);
            return this;
        }

        #endregion

        #region " Sets "

        public FluentRegex MatchAnyCharacterInSet(FluentRegex pattern)
        {
            _sb.AppendFormat(@"[{0}]", pattern.ToString());
            return this;
        }

        public FluentRegex MatchAnyCharacterInSet(string pattern)
        {
            _sb.AppendFormat(@"[{0}]", pattern);
            return this;
        }

        public FluentRegex MatchAnyCharacterNotInSet(FluentRegex pattern)
        {
            _sb.AppendFormat(@"[^{0}]", pattern.ToString());
            return this;
        }

        public FluentRegex MatchAnyCharacterNotInSet(string pattern)
        {
            _sb.AppendFormat(@"[^{0}]", pattern);
            return this;
        }

        public FluentRegex MatchOneOf(FluentRegex first,
                                            FluentRegex second,
                                            params FluentRegex[] additionalChoices)
        {
            _sb.AppendFormat(@"({0}|{1}", first, second);
            foreach (FluentRegex choice in additionalChoices)
            {
                _sb.AppendFormat(@"|{0}", choice);
            }
            _sb.Append(@")");
            return this;
        }

        public FluentRegex MatchOneOf(string first,
                                            string second,
                                            params string[] additionalChoices)
        {
            _sb.AppendFormat(@"({0}|{1}", first, second);
            foreach (string choice in additionalChoices)
                _sb.AppendFormat(@"|{0}", choice);
            _sb.Append(")");
            return this;
        }


        #endregion

        #region " Positive and Negative Assertions "

        /// <summary>
        /// Pattern must be found ahead, but will not be included in the match.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex PositiveForwardAssertion(FluentRegex Pattern)
        {
            _sb.AppendFormat(@"(?={0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must be found ahead, but will not be included in the match.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex PositiveForwardAssertion(string Pattern)
        {
            _sb.AppendFormat(@"(?={0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must be found before the next pattern, but will not be included in the match.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex PositiveBackwardAssertion(FluentRegex Pattern)
        {
            _sb.AppendFormat(@"(?<={0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must be found before the next pattern, but will not be included in the match.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex PositiveBackwardAssertion(string Pattern)
        {
            _sb.AppendFormat(@"(?<={0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must NOT be found ahead.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex NegativeForwardAssertion(FluentRegex Pattern)
        {
            _sb.AppendFormat(@"(?!{0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must NOT be found ahead.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex NegativeForwardAssertion(string Pattern)
        {
            _sb.AppendFormat(@"(?!{0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must NOT be found before the next pattern.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex NegativeBackwardAssertion(FluentRegex Pattern)
        {
            _sb.AppendFormat(@"(?<!{0})", Pattern);
            return this;
        }

        /// <summary>
        /// Pattern must NOT be found before the next pattern.
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public FluentRegex NegativeBackwardAssertion(string Pattern)
        {
            _sb.AppendFormat(@"(?<!{0})", Pattern);
            return this;
        }

        #endregion

        #region " Helpers "

        /// <summary>
        /// returns true if the given character needs to be escaped in the regex
        /// </summary>
        /// <param name="Ch"></param>
        /// <returns></returns>
        private static bool _characterNeedsEscaping(Char Ch)
        {
            return RESERVED_CHARACTERS.Contains(Ch);
        }

        /// <summary>
        /// returns whether or not the regex pattern built is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get
            {
                bool Result = false;
                if (!String.IsNullOrEmpty(this.ToString()))
                {
                    try
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch("", this.ToString()))
                        {

                        }
                        Result = true;
                    }
                    catch
                    {
                        Result = false;
                    }
                }
                return Result;
            }
        }

        private FluentRegex _addPattern(string regexpattern)
        {
            _sb.Append(regexpattern);
            return this;
        }
        private FluentRegex _addPattern(string template,
                                        params Object[] parms)
        {
            _sb.AppendFormat(template, parms);
            return this;
        }

        /// <summary>
        /// returns the compiled regex pattern
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private System.Text.RegularExpressions.Regex _compiledRegEx(System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.None)
        {
            options = options | System.Text.RegularExpressions.RegexOptions.Compiled;
            return new System.Text.RegularExpressions.Regex(this.ToString(), options);
        }

        #endregion

        #region " Repetition "

        /// <summary>
        /// Modifies the preceding match with a repeat
        /// </summary>
        /// <returns></returns>
        public Repeat Repeated
        {
            get
            {
                return new Repeat(this);
            }
        }

        public class Repeat
        {

            private FluentRegex _fRegex;

            public Repeat(FluentRegex parentpattern)
            {
                _fRegex = parentpattern;
            }

            public FluentRegex OneOrMoreTimes
            {
                get
                {
                    return _fRegex._addPattern(@"+");
                }
            }

            public FluentRegex ZeroOrMoreTimes
            {
                get
                {
                    return _fRegex._addPattern(@"*");
                }
            }

            public FluentRegex Optional
            {
                get
                {
                    return _fRegex._addPattern(@"?");
                }
            }

            public FluentRegex XTimes(int exactly)
            {
                return _fRegex._addPattern(@"{{{0}}}", exactly);
            }

            public FluentRegex X_to_Y_Times(int min, int max)
            {
                return _fRegex._addPattern(@"{{{0},{1}}}", min, max);
            }

            public FluentRegex AtLeast(int x)
            {
                return _fRegex._addPattern(@"{{{0},}}", x);
            }

            public FluentRegex AtMost(int x)
            {
                return _fRegex._addPattern(@"{{,{0}}}", x);
            }

        }

        #endregion

        #region " Enumerations "
        [Flags]
        public enum PredefinedSets : short
        {
            AnyCharacter = 1,
            WhiteSpace = 2,
            NonWhiteSpace = 4,
            Digit = 8,
            NonDigit = 16,
            WordCharacter = 32,
            NonWordCharacter = 64,
            WordBreak = 128,
            NonWordBreak = 256,
            LowerCaseLetter = 512,
            UpperCaseLetter = 1024,
            AnyLetter = 512 + 1024
        }
        #endregion


    }
}
