public class FunctionCall : Statement
{
    public string name;
    public List<Expression> arguments;

    public FunctionCall(string name, List<Expression> arguments)
    {
        this.name = name;
        this.arguments = arguments;
    }
}