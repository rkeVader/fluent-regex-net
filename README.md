# fluent-regex-net
This project is an experiment in building a fluent interface for building Regular Expression (RegEx / RegExp) Patterns in .NET  
This is distributed under the MIT License.

I frequently find myself helping other developers to use Regular Expressions. 
When updating an 'intro' presentation and its support code and app, I decided a fluent interface would
enable newer users to leverage the power of RegExes.  

This class is not considered complete and will likely evolve as my needs do, and as I receive feedback from 
other developers. As is, the class currently supports the basics plus positive and negative assertions and named groups. I am looking at adding conditionals (if/then/else).  

Both VB.NET and C# versions of the same class are available (and will continue to be so unless upkeep becomes an issue). To use, just include the file of your choice in your project. Then...  

Example Usage (both generate the RegEx pattern /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{6,12}$/):  

### C#  
```csharp
            // Build RegEx pattern for password validation  
            // - Password must contain at least 1 Uppercase Letter  
            // - Password must contain at least 1 Lowercase Letter  
            // - Password must contain at least 1 Digit  
            // - Password may contain any other characters (assuming the above are met)  
            // - Password must be from 6 to 12 characters in length  
            FluentRegex fr = FluentRegex.StartRegex()  
                .MatchStartOfLine.Then  
                .PositiveForwardAssertion(  
                    FluentRegex.StartRegex()  
                        .MatchAnything.Repeated.ZeroOrMoreTimes.Then  
                        .MatchAnyUpperCaseLetter  
                ).Then  
                .PositiveForwardAssertion(  
                    FluentRegex.StartRegex()  
                        .MatchAnything.Repeated.ZeroOrMoreTimes.Then  
                        .MatchAnyLowerCaseLetter  
                ).Then  
                .PositiveForwardAssertion(  
                    FluentRegex.StartRegex()  
                        .MatchAnything.Repeated.ZeroOrMoreTimes.Then  
                        .MatchDigit  
                ).Then  
                .MatchAnything.Repeated.X_to_Y_Times(6, 12).Then  
                .MatchEndOfLine;  
                
            if (fr.IsMatch("AbcDef123"))
            {
                // We Have a Valid Password!
            }

            // ...

            // loop through all matches
            foreach (Match m in fr.Matches(somesamplestring))
            {

            }
```
### VB.NET  
```vb.net
    Dim p As FluentRegex = FluentRegex.StartRegex() _
            .MatchStartOfLine().Then _
            .PositiveForwardAssertion(
                FluentRegex.StartRegex() _
                    .MatchAnything.Repeated.ZeroOrMoreTimes.Then _
                    .MatchAnyUpperCaseLetter
            ).Then _
            .PositiveForwardAssertion(
                FluentRegex.StartRegex() _
                    .MatchAnything.Repeated.ZeroOrMoreTimes.Then _
                    .MatchAnyLowerCaseLetter
            ).Then _
            .PositiveForwardAssertion(
                FluentRegex.StartRegex() _
                    .MatchAnything.Repeated.ZeroOrMoreTimes.Then _
                    .MatchDigit
            ).Then _
            .MatchAnything.Repeated.X_to_Y_Times(6, 12).Then _
            .MatchEndOfLine

        If p.IsMatch("AbcDef123") Then
            ' We have a valid password
        End If

        ' ...

        ' Loop through all matchges
        For Each m As Match In p.Matches(SomeSampleString)

        Next        
```

## HTML Tag Matching  
So  parsing HTML isn't always the best use of RegExes, but they do pose an excellent example of group and backreferences.  
### C#
```csharp
            FluentRegex fr = FluentRegex.StartRegex()
                .MatchLiteral("<")
                .NamedGroup(FluentRegex.StartRegex().MatchWordCharacter.Repeated.OneOrMoreTimes, "tagname").Then
                .MatchAnyWhiteSpaceCharacter.Repeated.ZeroOrMoreTimes.Then
                .MatchAnyCharacterNotInSet(">").Repeated.ZeroOrMoreTimes.Then
                .MatchLiteral(">").Then
                .MatchAnyCharacterNotInSet("<").Repeated.ZeroOrMoreTimes.Then
                .MatchLiteral("</").Then
                .ReferenceNamedGroup("tagname").Then
                .MatchLiteral(">");

            foreach (Match m in fr.Matches("<abc>123</abc> <xyz style='color:white;'></xyz>"))
            {
                System.Diagnostics.Debug.Print(m.Value);
            }
```
