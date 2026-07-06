public class IfStatement : Statement
{
    public Expression condition;
    public List<Statement> body;
    public IfStatement (Expression condition, List<Statement> body){
        this.condition = condition;
        this.body = body;
    }

}