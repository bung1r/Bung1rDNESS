using System.Runtime.CompilerServices;

public class Token
{
    public TokenType type;
    public string lexeme; // the text
    public int lineNum; // for debugging purposes
    
    public Token(TokenType type, string lexeme, int lineNum = 0)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.lineNum = lineNum;
    }
}

public enum TokenType
{
    Identifier,
    If, 
    GE,
    GT,
    LE,
    LT,
    EQ,
    NEQ,
    AND,
    OR,
    ADD,
    SUB,
    MUL,
    DIV,
    Number,
    HexNumber,
    BiNumber,
    String,
    Bool,
    LeftParen,
    RightParen,
    LeftCurly,
    RightCurly,
    Comma,
    Comment,
    Unknown,
    EOF,
}