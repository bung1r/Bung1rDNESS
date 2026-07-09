public class IdentifierExpression : Expression
{
    public string value;
    public List<string> path;
    public IdentifierExpression(string value, List<string>? path = null)
    {
        this.value = value;
        this.path = path ?? new List<string>();
    }
}