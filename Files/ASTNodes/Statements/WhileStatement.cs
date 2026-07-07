public class WhileStatement : Statement
{
    public Expression condition;
    public List<Statement> body;
    public WhileStatement (Expression condition, List<Statement> body){
        this.condition = condition;
        this.body = body;
    }

}