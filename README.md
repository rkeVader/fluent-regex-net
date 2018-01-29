# fluent-regex-net
This project is an experiement in building a fluent interface for building Regular Expressioins in .NET  

I frequently find myself helping other developers to use Regular Expressions. 
When updating an 'intro' presentation and its support code and app, I decided a fluent interface would
enable newer users to leverage the power of RegExes.  

This class is not considered complete and will likely evolve as my needs do, and as I receive feedback from 
other developers.  

Example Usage:  
```
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
```
