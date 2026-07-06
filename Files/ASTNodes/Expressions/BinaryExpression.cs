public class BinaryExpression : Expression
{
    public Expression left;
    public TokenType operation;
    public Expression right;
    public BinaryExpression(Expression left, TokenType operation, Expression right)
    {
        this.left = left;
        this.right = right;
        this.operation = operation;
    }
}