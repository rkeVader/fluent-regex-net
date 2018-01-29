Option Strict On
Option Infer Off

Imports System.Text

''' <summary>
''' 2018.01.26 v1.2
''' 2018.01.25 v1.1
''' 2017.04.26 v1
''' CSA
''' An experiment in a Fluent interface for creating regular expression patterns via 'human-readable' code
''' this is inspired by a JavaScript implementations I ran across a few years ago.
''' </summary>
Public Class FluentRegex

    Private _sb As New StringBuilder

    Private Const RESERVED_CHARACTERS As String = "^$[](){}.|*+?\"

    Private Sub New()
        ' prevent direct instantiations of this class 
    End Sub

    ''' <summary>
    ''' Overridden to return the current regex pattern string
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return _sb.ToString()
    End Function

    ''' <summary>
    ''' Factory Accessor
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function StartRegex() As FluentRegex
        Return New FluentRegex()
    End Function

    ''' <summary>
    ''' This is just a pass-through function - syntactically doing nothing,
    ''' but is included to improve the Human-Readability of code using this class
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [Then]() As FluentRegex
        Get
            Return Me
        End Get
    End Property

#Region " Underlying Regex Method Accessors "

    ''' <summary>
    ''' Indicates whether or not the defined regex pattern is found in the provided string or not
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <param name="Options"></param>
    ''' <returns></returns>
    Public Function IsMatch(Value As String, Optional Options As RegularExpressions.RegexOptions = RegularExpressions.RegexOptions.None) As Boolean
        Return _compiledRegEx(Options).IsMatch(Value) 'RegularExpressions.Regex.IsMatch(Value, Me.ToString, Options)
    End Function

    ''' <summary>
    ''' Searches the provided string for all matches of the defined regex pattern
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <param name="Options"></param>
    ''' <returns></returns>
    Public Function Matches(Value As String, Optional Options As RegularExpressions.RegexOptions = RegularExpressions.RegexOptions.None) As RegularExpressions.MatchCollection
        Return _compiledRegEx(Options).Matches(Value)
    End Function

#End Region

#Region " Anchors "

    ''' <summary>
    ''' Start of Line/String Anchor ^
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchStartOfLine() As FluentRegex
        Get
            _sb.Append("^")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' End of Line/String Anchor $
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchEndOfLine() As FluentRegex
        Get
            _sb.Append("$")
            Return Me
        End Get
    End Property

#End Region

#Region " Literals "

    ''' <summary>
    ''' A literal pattern. The method handles any escaping any individual characters in the provided value may need.
    ''' </summary>
    ''' <param name="LiteralValue"></param>
    ''' <returns></returns>
    Public ReadOnly Property MatchLiteral(LiteralValue As String) As FluentRegex
        Get
            For Each c As Char In LiteralValue.ToCharArray
                If _characterNeedsEscaping(c) Then
                    _sb.AppendFormat("\{0}", c)
                Else
                    _sb.Append(c)
                End If
            Next

            Return Me
        End Get
    End Property

#End Region

#Region " Character Sets "
    ''' <summary>
    ''' Generates a character set based on any number of the predefined sets, plus any other characters provided.
    ''' </summary>
    ''' <param name="CharSets"></param>
    ''' <param name="OtherChars"></param>
    ''' <returns></returns>
    Public Function MatchCharacterSets(CharSets As PredefinedSets, ParamArray OtherChars() As Char) As FluentRegex
        Dim Pattern As String = "["
        If (CharSets And PredefinedSets.AnyCharacter) > 0 Then
            Pattern &= "."
        ElseIf (CharSets And PredefinedSets.Digit) > 0 Then
            Pattern &= "\d"
        ElseIf (CharSets And PredefinedSets.LowerCaseLetter) > 0 Then
            Pattern &= "a-z"
        ElseIf (CharSets And PredefinedSets.NonDigit) > 0 Then
            Pattern &= "\D"
        ElseIf (CharSets And PredefinedSets.NonWhiteSpace) > 0 Then
            Pattern &= "\S"
        ElseIf (CharSets And PredefinedSets.NonWordBreak) > 0 Then
            Pattern &= "\B"
        ElseIf (CharSets And PredefinedSets.NonWordCharacter) > 0 Then
            Pattern &= "\W"
        ElseIf (CharSets And PredefinedSets.UpperCaseLetter) > 0 Then
            Pattern &= "A-Z"
        ElseIf (CharSets And PredefinedSets.WhiteSpace) > 0 Then
            Pattern &= "\s"
        ElseIf (CharSets And PredefinedSets.WordBreak) > 0 Then
            Pattern &= "\b"
        ElseIf (CharSets And PredefinedSets.WordCharacter) > 0 Then
            Pattern &= "\w"
        End If

        If OtherChars IsNot Nothing Then
            For Each ch As Char In OtherChars
                If _characterNeedsEscaping(ch) Then
                    Pattern &= "\"
                End If
                Pattern &= ch
            Next
        End If

        Pattern &= "]"
        _sb.Append(Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Matches any character '.'
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchAnything() As FluentRegex
        Get
            _sb.Append(".")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a digit character \d
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchDigit() As FluentRegex
        Get
            _sb.Append("\d")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a non-digit character \D
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchNonDigit() As FluentRegex
        Get
            _sb.Append("\D")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a word character \w
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchWordCharacter() As FluentRegex
        Get
            _sb.Append("\w")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a non-word character \W
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchNonWordCharacter() As FluentRegex
        Get
            _sb.Append("\W")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a word break \b
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchWordBreak() As FluentRegex
        Get
            _sb.Append("\b")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a non-word-break \B
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchNonWordBreak() As FluentRegex
        Get
            _sb.Append("\B")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches any letter [a-zA-Z]
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchAnyLetter() As FluentRegex
        Get
            _sb.Append("[a-zA-Z]")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches any lowercase letter [a-z]
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchAnyLowerCaseLetter() As FluentRegex
        Get
            _sb.Append("[a-z]")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches any uppercase letter [A-Z]
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchAnyUpperCaseLetter() As FluentRegex
        Get
            _sb.Append("[A-Z]")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a white-space character \s
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchAnyWhiteSpaceCharacter() As FluentRegex
        Get
            _sb.Append("\s")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Matches a non-white-space character \S
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchAnyNonWhiteSpaceCharacter() As FluentRegex
        Get
            _sb.Append("\S")
            Return Me
        End Get
    End Property

#End Region

#Region " Non-Printable Characters "

    ''' <summary>
    ''' Match a Tab \t
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchTab() As FluentRegex
        Get
            _sb.Append("\t")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Match a Carriage Return \r
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchCarriageReturn() As FluentRegex
        Get
            _sb.Append("\r")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Match an ASCII Bell Character \a
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchASCIIBellCharacter() As FluentRegex
        Get
            _sb.Append("\a")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Match an ASCII Escape Character \e
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchASCIIEscapeCharacter() As FluentRegex
        Get
            _sb.Append("\e")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Match a Form Feed Character \f
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchFormFeed() As FluentRegex
        Get
            _sb.Append("\f")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Match a Vertical Tab \v
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchVerticalTab() As FluentRegex
        Get
            _sb.Append("\v")
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Match a Line Feed Character (alias for MatchNewLine) \n
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchLineFeed() As FluentRegex
        Get
            Return MatchNewLine()
        End Get
    End Property

    ''' <summary>
    ''' Match a New Line Character \n
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MatchNewLine() As FluentRegex
        Get
            _sb.Append("\n")
            Return Me
        End Get
    End Property

#End Region

#Region " Groups "

    ''' <summary>
    ''' Define a match group
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <returns></returns>
    Public ReadOnly Property Group(pattern As FluentRegex) As FluentRegex
        Get
            _sb.AppendFormat("({0})", pattern)
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Define a match group
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <returns></returns>
    Public ReadOnly Property Group(pattern As String) As FluentRegex
        Get
            _sb.AppendFormat("({0})", pattern)
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Define a named match group
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    Public ReadOnly Property NamedGroup(Pattern As FluentRegex, Name As String) As FluentRegex
        Get
            _sb.AppendFormat("(?<{1}>{0})", Pattern, Name)
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Define a named match group
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    Public ReadOnly Property NamedGroup(Pattern As String, Name As String) As FluentRegex
        Get
            _sb.AppendFormat("(?<{1}>{0})", Pattern, Name)
            Return Me
        End Get
    End Property

    ''' <summary>
    ''' Add a back-reference to a named match group
    ''' </summary>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    Public ReadOnly Property ReferenceNamedGroup(Name As String) As FluentRegex
        Get
            _sb.AppendFormat("\k<{0}>", Name)
            Return Me
        End Get
    End Property

#End Region

#Region " Sets "

    Public ReadOnly Property MatchAnyCharacterInSet(pattern As FluentRegex) As FluentRegex
        Get
            _sb.AppendFormat("[{0}]", pattern.ToString)
            Return Me
        End Get
    End Property

    Public ReadOnly Property MatchAnyCharacterInSet(pattern As String) As FluentRegex
        Get
            _sb.AppendFormat("[{0}]", pattern)
            Return Me
        End Get
    End Property

    Public ReadOnly Property MatchAnyCharacterNotInSet(pattern As FluentRegex) As FluentRegex
        Get
            _sb.AppendFormat("[^{0}]", pattern.ToString)
            Return Me
        End Get
    End Property

    Public ReadOnly Property MatchAnyCharacterNotInSet(pattern As String) As FluentRegex
        Get
            _sb.AppendFormat("[^{0}]", pattern)
            Return Me
        End Get
    End Property

    Public ReadOnly Property MatchOneOf(first As FluentRegex,
                                        second As FluentRegex,
                                        ParamArray additionalChoices() As FluentRegex) As FluentRegex
        Get
            _sb.AppendFormat("({0}|{1}", first, second)
            For Each choice As FluentRegex In additionalChoices
                _sb.AppendFormat("|{0}", choice)
            Next
            _sb.Append(")")
            Return Me
        End Get
    End Property

    Public ReadOnly Property MatchOneOf(first As String,
                                        second As String,
                                        ParamArray additionalChoices() As String) As FluentRegex
        Get
            _sb.AppendFormat("({0}|{1}", first, second)
            For Each choice As String In additionalChoices
                _sb.AppendFormat("|{0}", choice)
            Next
            _sb.Append(")")
            Return Me
        End Get
    End Property

#End Region

#Region " Positive and Negative Assertions "

    ''' <summary>
    ''' Pattern must be found ahead, but will not be included in the match.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function PositiveForwardAssertion(Pattern As FluentRegex) As FluentRegex
        _sb.AppendFormat("(?={0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must be found ahead, but will not be included in the match.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function PositiveForwardAssertion(Pattern As String) As FluentRegex
        _sb.AppendFormat("(?={0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must be found before the next pattern, but will not be included in the match.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function PositiveBackwardAssertion(Pattern As FluentRegex) As FluentRegex
        _sb.AppendFormat("(?<={0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must be found before the next pattern, but will not be included in the match.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function PositiveBackwardAssertion(Pattern As String) As FluentRegex
        _sb.AppendFormat("(?<={0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must NOT be found ahead.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function NegativeForwardAssertion(Pattern As FluentRegex) As FluentRegex
        _sb.AppendFormat("(?!{0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must NOT be found ahead.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function NegativeForwardAssertion(Pattern As String) As FluentRegex
        _sb.AppendFormat("(?!{0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must NOT be found before the next pattern.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function NegativeBackwardAssertion(Pattern As FluentRegex) As FluentRegex
        _sb.AppendFormat("(?<!{0})", Pattern)
        Return Me
    End Function

    ''' <summary>
    ''' Pattern must NOT be found before the next pattern.
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <returns></returns>
    Public Function NegativeBackwardAssertion(Pattern As String) As FluentRegex
        _sb.AppendFormat("(?<!{0})", Pattern)
        Return Me
    End Function

#End Region

#Region " Helpers "

    ''' <summary>
    ''' Returns true if the given character needs to be escaped in the regex
    ''' </summary>
    ''' <param name="Ch"></param>
    ''' <returns></returns>
    Private Shared Function _characterNeedsEscaping(Ch As Char) As Boolean
        Return RESERVED_CHARACTERS.Contains(Ch)
    End Function

    ''' <summary>
    ''' Returns whether or not the regex pattern built is valid
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsValid As Boolean
        Get
            Dim Result As Boolean = False
            If Not String.IsNullOrEmpty(Me.ToString()) Then
                Try
                    RegularExpressions.Regex.IsMatch("", Me.ToString())
                    Result = True
                Catch ex As Exception
                    Result = False
                End Try
            End If
            Return Result
        End Get
    End Property

    Private Function _addPattern(regexpattern As String) As FluentRegex
        _sb.Append(regexpattern)
        Return Me
    End Function
    Private Function _addPattern(template As String,
                                 ParamArray Params() As Object) As FluentRegex
        _sb.AppendFormat(template, Params)
        Return Me
    End Function

    ''' <summary>
    ''' returns the compiled regex pattern
    ''' </summary>
    ''' <param name="options"></param>
    ''' <returns></returns>
    Private Function _compiledRegEx(Optional options As RegularExpressions.RegexOptions = RegularExpressions.RegexOptions.None) As RegularExpressions.Regex
        options = options Or RegularExpressions.RegexOptions.Compiled
        Return New RegularExpressions.Regex(Me.ToString(), options)
    End Function

#End Region

#Region " Repetition "

    ''' <summary>
    ''' Modifies the preceding match with a repeat
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Repeated() As Repeat
        Get
            Return New Repeat(Me)
        End Get
    End Property

    Public Class Repeat

        Private _fRegex As FluentRegex

        Public Sub New(parentPattern As FluentRegex)
            _fRegex = parentPattern
        End Sub

        Public Function OneOrMoreTimes() As FluentRegex
            Return _fRegex._addPattern("+")
        End Function

        Public Function ZeroOrMoreTimes() As FluentRegex
            Return _fRegex._addPattern("*")
        End Function

        Public Function [Optional]() As FluentRegex
            Return _fRegex._addPattern("?")
        End Function

        Public Function XTimes(exactly As Integer) As FluentRegex
            Return _fRegex._addPattern("{{{0}}}", exactly)
        End Function

        Public Function X_to_Y_Times(min As Integer, max As Integer) As FluentRegex
            Return _fRegex._addPattern("{{{0},{1}}}", min, max)
        End Function

        Public Function AtLeast(x As Integer) As FluentRegex
            Return _fRegex._addPattern("{{{0},}}", x)
        End Function

        Public Function AtMost(x As Integer) As FluentRegex
            Return _fRegex._addPattern("{{,{0}}}", x)
        End Function

    End Class

#End Region

#Region " Enumerations "
    <Flags>
    Public Enum PredefinedSets As Short
        AnyCharacter = 1
        WhiteSpace = 2
        NonWhiteSpace = 4
        Digit = 8
        NonDigit = 16
        WordCharacter = 32
        NonWordCharacter = 64
        WordBreak = 128
        NonWordBreak = 256
        LowerCaseLetter = 512
        UpperCaseLetter = 1024
        AnyLetter = 512 + 1024
    End Enum
#End Region

End Class
