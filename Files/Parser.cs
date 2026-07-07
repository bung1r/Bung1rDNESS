using System.Collections;

public class Parser
{
    List<Token> tokens;
    private int index = 0;
    public Parser(List<Token> tokens)
    {
        this.tokens= tokens;
        index = 0;
    }
    private int ParseNum(string value)
    {
        return Convert.ToInt32(value, 10);
    }
    private int ParseHex(string value)
    {
        if (value.StartsWith("$"))
            return Convert.ToInt32(value[1..], 16);

        if (value.StartsWith("0x"))
            return Convert.ToInt32(value[2..], 16);

        throw new Exception("Invalid hex format");
    }
    private int ParseBin(string value)
    {
        if (value.StartsWith("0b"))
            return Convert.ToInt32(value[2..], 2);

        throw new Exception("Invalid binary format");
    }
    private string ParseString(string value)
    {
        return value.Trim('"');
    }
    private bool ParseBool(string value)
    {
        return bool.Parse(value);
    }
    private Token Current => tokens[index];
    private Token Advance()
    {
        return tokens[index++];
    }    
    public List<ASTNode> ParseTokens()
    {
        List<ASTNode> nodes = new List<ASTNode>();
        ASTNode ParseIdentifier() // runs when the Current == "Identifier" (for now)
        {
            string name = Current.lexeme;
            int lineNum = Current.lineNum;

            

            Advance(); // consume the identifier 
            if (Current.type == TokenType.LeftParen)
            {
                List<Expression> args = new List<Expression>();

                Advance(); // consume the (

                while (Current.type != TokenType.RightParen)
                {
                    // Console.WriteLine(Current.type);
                    if (Current.type != TokenType.Comma)
                    {
                        args.Add(ParseExpression()); // this will automatically advance
                    } else
                    {
                        Advance(); // force advance if there's a comma
                    }
                    
                    
                }

                Advance(); // consume the )

                return new FunctionCall(name, args);
            } else // can't find it? Try to make it a variable at least!
            {           
                return new IdentifierExpression(name);
            }

            
            
            
        }

        
        
        IfStatement ParseIfStatement()
        {
            Advance(); // consume the if

            if (Current.type != TokenType.LeftParen) 
                throw new Exception($"'(' expected at line {Current.lineNum}");

            Advance(); // consume the (

            Expression condition = ParseExpression();

            if (Current.type != TokenType.RightParen) 
                throw new Exception($"')' expected at line {Current.lineNum}");

            List<Statement> body = ParseBody(); // handles { to }

            return new IfStatement(condition, body);
        }

        WhileStatement ParseWhileStatement()
        {
            Advance(); // consume the if

            if (Current.type != TokenType.LeftParen) 
                throw new Exception($"'(' expected at line {Current.lineNum}");

            Advance(); // consume the (

            Expression condition = ParseExpression();

            if (Current.type != TokenType.RightParen) 
                throw new Exception($"')' expected at line {Current.lineNum}");

            List<Statement> body = ParseBody(); // handles { to }

            return new WhileStatement(condition, body);
        }  

        Expression ParseLogical()
        {
            Expression left = ParseComparison();

            while (Current.type == TokenType.AND || Current.type == TokenType.OR)
            {
                TokenType op = Current.type;
                Advance();
                Expression right = ParseComparison();

                left = new BinaryExpression(left, op, right);
            }

            return left;
        }

        Expression ParseComparison()
        {
            Expression left = ParseAddition();

            while (
                Current.type == TokenType.EQ || Current.type == TokenType.NEQ ||
                Current.type == TokenType.GT || Current.type == TokenType.GE ||
                Current.type == TokenType.LT || Current.type == TokenType.LE
                )
            {
                TokenType op = Current.type;
                Advance();
                Expression right = ParsePrimary();

                left = new BinaryExpression(left, op, right);
            }

            return left;
        }

        Expression ParseAddition()
        {
            Expression left = ParseMultiplication();

            while (Current.type == TokenType.ADD || Current.type == TokenType.SUB)
            {
                var op = Advance().type;
                var right = ParseMultiplication();

                left = new BinaryExpression(left, op, right);
            }

            return left;
        }

        Expression ParseMultiplication()
        {
            Expression left = ParsePrimary();

            while (Current.type == TokenType.MUL || Current.type == TokenType.DIV)
            {
                var op = Advance().type;
                var right = ParsePrimary();

                left = new BinaryExpression(left, op, right);
            }

            return left;
        }

        Expression ParsePrimary()
        {
            if (Current.type == TokenType.LeftParen)
            {
                Advance(); // consume the (

                Expression expression = ParseExpression();

                if (Current.type != TokenType.RightParen)
                    throw new Exception($"Expected ')' at line {Current.lineNum}");

                Advance(); // consume the )

                return expression;
            }

            Token token = Advance();

            switch (token.type)
            {
                case TokenType.Number:
                    return new NumLiteral(ParseNum(token.lexeme));

                case TokenType.HexNumber:
                    return new HexLiteral(ParseHex(token.lexeme));

                case TokenType.BiNumber:
                    return new BiLiteral(ParseBin(token.lexeme));

                case TokenType.String:
                    return new StringLiteral(ParseString(token.lexeme));
                
                case TokenType.Bool:
                    return new BoolLiteral(ParseBool(token.lexeme));

                case TokenType.Identifier:
                    return new IdentifierExpression(token.lexeme);

                

                default:
                    throw new Exception(
                        $"Unexpected token {token.type} at line {token.lineNum}");
            }
        }

        Expression ParseExpression()
        {
            return ParseLogical();
        }
        List<Statement> ParseBody() {
        
            List<Statement> body = new List<Statement>();

            Advance();

            if (Current.type != TokenType.LeftCurly)
            {
                char c = '{';
                throw new Exception($"Expected {c} at line {Current.lineNum}");
            }
            
            Advance(); // consume the '{'

            while (Current.type != TokenType.RightCurly)
            {
                ASTNode statement = ParseNode();
                if (statement is Statement s)
                {
                    body.Add(s);
                } else
                {
                    throw new Exception($"Expression {Current.type} found instead of Statement at line {Current.lineNum}");
                }
            }

            Advance(); // consume the '}'

            return body;
        }

        ASTNode ParseNode()
        {
            
            if (Current.type == TokenType.Identifier)
            {
                return ParseIdentifier();
            } else if (Current.type == TokenType.If)
            {
                return ParseIfStatement();
            } else if (Current.type == TokenType.While)
            {
                return ParseWhileStatement();
            } else 
            {
                throw new Exception($"Unexpected {Current.type} at line {Current.lineNum}");
            }
        }

        while (Current.type != TokenType.EOF)
        {
            nodes.Add(ParseNode());
        }
   
        return nodes;
    }
}