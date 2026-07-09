public class Lexer
{
    private string source; // the source string 
    public Lexer(string source)
    {
        this.source = source;
    }
    
    public List<Token> ScanTokens()
    {
        int lineNum = 1;
        List<Token> tokenList = new List<Token>();
        TokenType tempToken = TokenType.Identifier;
        string tempLexeme = "";

        void ConcatLex(char c)
        {
            tempLexeme = string.Concat(tempLexeme, new ReadOnlySpan<char>(in c));
        }
        void DetermineTokenType()
        {
            switch(tempLexeme)
            {
                case "if": tempToken = TokenType.If; break;
                case "true": case "false": tempToken = TokenType.Bool; break;
                case "while": tempToken = TokenType.While; break;
            }
        }

        void ProcessLexeme()
        {
            if (tempLexeme.Length > 0)
            {
                DetermineTokenType();
                tokenList.Add(new Token(tempToken, tempLexeme, lineNum));
                tempLexeme = "";
            }
        }


        bool inString = false;
        bool inComment = false;
        for (int i = 0; i < source.Length; i++)
        {
            char c = source[i];
            char prevC = ' ';
            if (i > 0) prevC = source[i-1];
            
            switch (c)
            {
                case ' ':
                    if (inString) ConcatLex(' ');
                    break;
                
                case'\r':
                    break;

                case'\n':
                    lineNum++;
                    break;

                case '(':
                    if (!inString)
                    {
                        ProcessLexeme();

                        tokenList.Add(new Token(TokenType.LeftParen, "(", lineNum));
                        tempToken = TokenType.Identifier;
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case ')':
                    if (!inString || prevC == '(')
                    {
                        ProcessLexeme();

                        tokenList.Add(new Token(TokenType.RightParen, ")", lineNum));
                        tempToken = TokenType.Identifier;
                    } else
                    {
                        ConcatLex(c);
                    }
                    
                    break;

                case ',':
                    if (!inString)
                    {
                        ProcessLexeme();
                        tokenList.Add(new Token(TokenType.Comma, ",", lineNum));
                        tempToken = TokenType.Identifier;
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '{':
                    if (!inString)
                    {
                        tokenList.Add(new Token(TokenType.LeftCurly, "{", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '}':
                    if (!inString)
                    {
                        tokenList.Add(new Token(TokenType.RightCurly, "}", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '=':

                    if (!inString)
                    {
                        
                        if (tempLexeme == "=")
                        {
                            tokenList.Add(new Token(TokenType.EQ, "==", lineNum));
                            tempLexeme = "";
                        } else if (tempLexeme == ">")
                        {
                            tokenList.Add(new Token(TokenType.GE, ">=", lineNum));
                            tempLexeme = "";
                        } else if (tempLexeme == "<")
                        {
                            tokenList.Add(new Token(TokenType.LE, "<=", lineNum));
                            tempLexeme = "";
                        } else if (tempLexeme == "!")
                        {
                            tokenList.Add(new Token(TokenType.NEQ, "!=", lineNum));
                            tempLexeme = "";
                        } else
                        {
                            ProcessLexeme();
                            // do something when the equal sign is the only thing. 
                            ConcatLex(c);
                        }
                        tempToken = TokenType.Identifier;
                    } else
                    {
                        ConcatLex(c);
                    }
                    
                    
                    
                    break;

                case '>':
                    if (!inString)
                    {
                        ProcessLexeme();

                        if (source[i + 1] != '=')
                        {
                            tokenList.Add(new Token(TokenType.GT, ">", lineNum));
                        } else
                        {
                            ConcatLex(c);
                        }
                        
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '<':
                    if (!inString)
                    {
                        ProcessLexeme();

                        if (source[i + 1] != '=')
                        {
                            tokenList.Add(new Token(TokenType.LT, "<", lineNum));
                        } else
                        {
                            ConcatLex(c);
                        }
                        
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '!':
                    if (!inString)
                    {
                        ProcessLexeme();

                        if (source[i + 1] != '=')
                        {
                            tokenList.Add(new Token(TokenType.LT, "<", lineNum));
                        } else
                        {
                            ConcatLex(c);
                        }
                        
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '+':
                    if (!inString)
                    {
                        ProcessLexeme();
                        tokenList.Add(new Token(TokenType.ADD, "+", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;
                
                case '-':
                    if (!inString)
                    {
                        ProcessLexeme();
                        tokenList.Add(new Token(TokenType.SUB, "-", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '*':
                    if (!inString)
                    {
                        ProcessLexeme();
                        tokenList.Add(new Token(TokenType.MUL, "*", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '/':
                    if (!inString)
                    {
                        ProcessLexeme();
                        tokenList.Add(new Token(TokenType.DIV, "/", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '&':
                    if (!inString)
                    {
                        if (tempLexeme == "&")
                        {
                            tokenList.Add(new Token(TokenType.AND, "&&", lineNum));
                            tempToken = TokenType.Identifier;
                            tempLexeme = "";
                        } else
                        {
                            ProcessLexeme();
                            ConcatLex(c);
                        }
                    } else
                    {
                        ConcatLex(c);
                    }
                    break;

                case '|':
                    if (!inString)
                    {
                        if (tempLexeme == "|")
                        {
                            tokenList.Add(new Token(TokenType.OR, "||", lineNum));
                            tempToken = TokenType.Identifier;
                            tempLexeme = "";
                        } else
                        {
                            ProcessLexeme();
                            ConcatLex(c);
                        }
                    } else
                    {
                        ConcatLex(c);
                    }
                break;


                case '$': 
                    if (tempLexeme == "")
                    {
                        tempToken = TokenType.HexNumber;
                    }
                    ConcatLex(c);
                    break;

                case '"': 
            
                    if (tempLexeme == "")
                    {
                        tempToken = TokenType.String;
                        inString = true;
                        ConcatLex(c);
                    } else if (tempLexeme.StartsWith('"'))
                    {
                        ConcatLex(c);
                        tokenList.Add(new Token(tempToken, tempLexeme, lineNum));
                        tempLexeme = "";
                        tempToken = TokenType.Identifier;
                        inString = false;
                        
                    }

                

                    break;

                case '.':
                    if (!inString)
                    {
                        ProcessLexeme();
                        tokenList.Add(new Token(TokenType.Dot, ".", lineNum));
                    } else
                    {
                        ConcatLex(c);
                    }
                break;
                case 'x': // this means that it's hexadecimal
                    if (tempLexeme == "0")
                    {
                        tempToken = TokenType.HexNumber;
                    } 
                    ConcatLex(c);
                    break;

                case 'b':
                    if (tempLexeme == "0")
                    {
                        tempToken = TokenType.BiNumber;
                    } 
                    ConcatLex(c);
                    break;


                case '0': case '1': case '2': case '3': case '4': 
                case '5': case '6': case '7': case '8':case '9':
                    if (tempLexeme.Length == 0)
                    {
                        tempToken = TokenType.Number;
                    }
                    ConcatLex(c);

                    
                    break;
                
                case'#': // this is the comment character
                    if (tempLexeme == "")
                    {
                        Console.WriteLine(tempLexeme);
                        inString = true; // this prevents most things. !
                        tempLexeme = "#";
                    } else if (tempLexeme.StartsWith('#'))
                    {
                        inString = false;
                        tempLexeme = "";
                    } else
                    {
                        Console.WriteLine(tempLexeme);
                        throw new Exception($"Bro, why is there a random ahh hashtag at line {lineNum}");
                    }
                    break;


                default:
                    ConcatLex(c);
                    break;

            
            }

            if (i == source.Length - 1) // last char
            {
            tokenList.Add(new Token(TokenType.EOF, "", lineNum));
            }
        }

       

        return tokenList;
    }

    
}