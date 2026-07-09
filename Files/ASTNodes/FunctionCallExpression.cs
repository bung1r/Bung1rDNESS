public class FunctionCallExpression : Statement
{
    public string name;
    public List<Expression> arguments;

    public FunctionCallExpression(string name, List<Expression> arguments)
    {
        this.name = name;
        this.arguments = arguments;
    }
}