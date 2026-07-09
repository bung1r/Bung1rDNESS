public class IfStatement : Statement
{
    public Expression condition;
    public List<ASTNode> body;
    public IfStatement (Expression condition, List<ASTNode> body){
        this.condition = condition;
        this.body = body;
    }

}