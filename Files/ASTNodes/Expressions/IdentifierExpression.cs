public class IdentifierExpression : Expression
{
    public string value;
    public IdentifierExpression(string value)
    {
        this.value = value;
    }
}