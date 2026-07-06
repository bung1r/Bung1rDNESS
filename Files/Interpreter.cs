using System.Collections;
using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using System.Xml;

public class Interpreter
{
    List<ASTNode> nodes;
    public Interpreter(List<ASTNode> nodes) 
    {
        this.nodes = nodes;
    }

    public object Evaluate(Expression expr)
    {
        if (expr is StringLiteral s)
            return s.value;

        if (expr is HexLiteral h)
            return h.value;

        if (expr is NumLiteral n)
        {
            return n.value;
        }

        if (expr is BiLiteral b)
        {
            return b.value;
        }

        if (expr is BoolLiteral bo)
        {
            return bo.value;
        }


        return "Evaluation didn't work for this value, check Interpreter";
    }
    bool IsNumeric(object obj)
    {   
        Type type = obj.GetType();
        return type.IsPrimitive && type != typeof(bool) && type != typeof(char) && type != typeof(IntPtr) && type != typeof(UIntPtr);
    }
    public void ProcessNode(ASTNode node) 
    {
        if (node is FunctionCall func)
        {
            switch(func.name)
            {
                case "print":
                    Console.Write($"{Evaluate(func.arguments[0])}\n");
                    break;
                case "load":
                    Console.Write($"Attempted to load {Evaluate(func.arguments[0])}\n");
                    break;
                case "dump":
                    Console.Write($"Attempted to dump to {Evaluate(func.arguments[0])}\n");
                    break;
                case "run":
                    Console.Write("Attempted to run\n");
                    break;
                case "write":
                    Console.Write($"Attempted to write {Evaluate(func.arguments[1])} to address {Evaluate(func.arguments[0])}\n");
                    break;
            }
        } else if (node is IfStatement i)
        {
            bool EvalCondition(Expression condition)
            {
                if (condition is BinaryExpression c)
                {
                    if (c.operation == TokenType.AND || c.operation == TokenType.OR)
                    {
                        
                        bool left = EvalCondition(c.left);
                        bool right = EvalCondition(c.right);

                        switch (c.operation)
                        {
                            case TokenType.AND: return left && right;
                            case TokenType.OR: return left || right;
                        }
                        
                        throw new Exception("Uhh, so the weather, eh? It should literally be impossible to get here");
                    } else // >, >=, <, <=, ==, != stuff like that
                    {
                        object left = c.left;
                        if (c.left is BinaryExpression l)
                        {
                            left = EvalCondition(l);
                        }

                        object right = c.right;
                        if (c.right is BinaryExpression r)
                        {
                            right = EvalCondition(r);
                        } 

                        if (c.operation == TokenType.GE || c.operation == TokenType.GT || c.operation == TokenType.LE || c.operation == TokenType.LT)
                        {
                            if (left.GetType() == typeof(bool) || right.GetType() == typeof(bool)) 
                                    throw new Exception("It's a comparison expression, tf you doin with booleans! GET OUTTA HERE!");

                            if (left.GetType() != typeof(NumLiteral) && left.GetType() != typeof(HexLiteral) && left.GetType() != typeof(BiLiteral))
                                throw new Exception("Invalid type to use comparison on");

                            if (right.GetType() != typeof(NumLiteral) && right.GetType() != typeof(HexLiteral) && right.GetType() != typeof(BiLiteral))
                                throw new Exception("Invalid type to use comparison on");
                                
                            int leftVal = 0;
                            if (left is Expression e)
                            {
                                leftVal = (int)Evaluate(e);
                            }

                            int rightVal = 0;
                            if (right is Expression f)
                            {
                                rightVal = (int)Evaluate(f);
                            }

                            switch(c.operation)
                            {
                                case TokenType.GE:
                                    return leftVal >= rightVal;
                                case TokenType.GT:
                                    return leftVal > rightVal;
                                case TokenType.LE:
                                    return leftVal <= rightVal;
                                case TokenType.LT:
                                    return leftVal < rightVal;
                            }
                        } else
                        {   
                            dynamic leftVal = 0;
                            if (left is Expression e)
                            {
                                leftVal = Evaluate(e);
                            }

                            dynamic rightVal = 0;
                            if (right is Expression f)
                            {
                                rightVal = Evaluate(f);
                            }

                            switch(c.operation)
                            {
                                case TokenType.EQ:
                                    Console.WriteLine($"{leftVal}, {rightVal}");
                                    return leftVal == rightVal;
                                case TokenType.NEQ:
                                    Console.WriteLine($"{leftVal}, {rightVal}");
                                    return left != right;
                            }
                        }

                        throw new Exception("Congrats, you reached the backrooms!");
                    }
                } else // must be a bool literal or horrible things happen
                {
                    if (condition is BoolLiteral b)
                    {
                        return b.value;
                    } else // also allow functions that return 
                    {
                        throw new Exception("Something about not being a boolean value idk man");
                    }
                }
            }
            
            if (EvalCondition(i.condition)) // if the condition is true
            {
                foreach (Statement statement in i.body)
                {
                    ProcessNode(statement);
                }
            }
        } 
    }
    public void Execute()
    {
        foreach (ASTNode node in nodes)
        {
            ProcessNode(node);
        }
    }
}