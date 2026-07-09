public class WhileStatement : Statement
{
    public Expression condition;
    public List<ASTNode> body;
    public WhileStatement (Expression condition, List<ASTNode> body){
        this.condition = condition;
        this.body = body;
    }

}