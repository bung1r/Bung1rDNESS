using System.Collections;
using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using System.Xml;

public class Interpreter
{
    List<ASTNode> nodes;
    Dictionary<string, object> variables = new Dictionary<string, object>();
    public Interpreter(List<ASTNode> nodes) 
    {
        this.nodes = nodes;
    }

    private int EvalOperation(BinaryExpression be)
    {
        if (be.operation == TokenType.ADD || be.operation == TokenType.SUB || 
        be.operation == TokenType.MUL || be.operation == TokenType.DIV)
        {
            object left = be.left;
            if (be.left is BinaryExpression l)
            {
                left = EvalOperation(l);
            } else if (left is NumLiteral num)
            {
                left = num.value;
            } else if (left is BiLiteral bi)
            {
                left = bi.value;
            } else if (left is HexLiteral hex)
            {
                left = hex.value;
            } else if (left is IdentifierExpression ex && variables[ex.value].GetType() == typeof(int))
            {
                left = variables[ex.value];
            }


            object right = be.right;
            if (be.right is BinaryExpression r)
            {
                right = EvalOperation(r);
            } else if (right is NumLiteral num)
            {
                right = num.value;
            } else if (right is BiLiteral bi)
            {
                right = bi.value;
            } else if (right is HexLiteral hex)
            {
                right = hex.value;
            } else if (right is IdentifierExpression ex && variables[ex.value].GetType() == typeof(int))
            {
                right = variables[ex.value];
            }
            

            switch(be.operation)
            {
                case TokenType.ADD:
                    return (int)left + (int)right;
                case TokenType.SUB:
                    return (int)left - (int)right;
                case TokenType.MUL:
                    return (int)left * (int)right;
                case TokenType.DIV:
                    return (int)left / (int)right;

            }
            throw new Exception("In some way, you managed to get past all the operations. Congrats.");
        } else
        {
            throw new Exception($"Please don't process {be.operation} operation with the Evaluate function! Thanks!");
        }
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

        if (expr is IdentifierExpression ex)
        {
            return variables[ex.value];
        }

        if (expr is BinaryExpression be)
        {
            if (be.operation == TokenType.ADD || be.operation == TokenType.SUB || 
            be.operation == TokenType.MUL || be.operation == TokenType.DIV)
            {
                object left = be.left;
                if (be.left is BinaryExpression l)
                {
                    left = EvalOperation(l);
                } else if (left is NumLiteral num)
                {
                    left = num.value;
                } else if (left is BiLiteral bi)
                {
                    left = bi.value;
                } else if (left is HexLiteral hex)
                {
                    left = hex.value;
                } else if (left is IdentifierExpression exp && variables[exp.value].GetType() == typeof(int))
                {
                    left = variables[exp.value];
                }


                object right = be.right;
                if (be.right is BinaryExpression r)
                {
                    right = EvalOperation(r);
                } else if (right is NumLiteral num)
                {
                    right = num.value;
                } else if (right is BiLiteral bi)
                {
                    right = bi.value;
                } else if (right is HexLiteral hex)
                {
                    right = hex.value;
                } else if (right is IdentifierExpression exp && variables[exp.value].GetType() == typeof(int))
                {
                    right = variables[exp.value];
                }
                

                switch(be.operation)
                {
                    case TokenType.ADD:
                        return (int)left + (int)right;
                    case TokenType.SUB:
                        return (int)left - (int)right;
                    case TokenType.MUL:
                        return (int)left * (int)right;
                    case TokenType.DIV:
                        return (int)left / (int)right;

                }
                throw new Exception("In some way, you managed to get past all the operations. Congrats.");
            } else
            {
                throw new Exception($"Please don't process {be.operation} operation with the Evaluate function! Thanks!");
            }
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
                        if (l.operation == TokenType.ADD || l.operation == TokenType.SUB || 
                        l.operation == TokenType.MUL || l.operation == TokenType.DIV)
                        {
                            left = EvalOperation(l);
                        } else
                        {
                            left = EvalCondition(l);
                        }
                    }

                    object right = c.right;
                    if (c.right is BinaryExpression r)
                    {
                        if (r.operation == TokenType.ADD || r.operation == TokenType.SUB || 
                        r.operation == TokenType.MUL || r.operation == TokenType.DIV)
                        {
                            right = EvalOperation(r);
                        } else
                        {
                            left = EvalCondition(r);
                        }
                        
                    } 

                    if (c.operation == TokenType.GE || c.operation == TokenType.GT || c.operation == TokenType.LE || c.operation == TokenType.LT)
                    {
                        if (left.GetType() == typeof(bool) || right.GetType() == typeof(bool)) 
                                throw new Exception("It's a comparison expression, tf you doin with booleans! GET OUTTA HERE!");

                        if (left is not NumLiteral && left is not HexLiteral && left is not BiLiteral && left is not IdentifierExpression)
                            throw new Exception($"Invalid type to use comparison on: {left.GetType()}");

                        if (right is not NumLiteral && right is not HexLiteral && right is not BiLiteral && right is not IdentifierExpression)
                            throw new Exception($"Invalid type to use comparison on: {right.GetType()}");
                            
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
                        dynamic leftVal = left;
                        if (left is Expression e)
                        {
                            leftVal = Evaluate(e);
                        }

                        dynamic rightVal = right;
                        if (right is Expression f)
                        {
                            rightVal = Evaluate(f);
                        }

                        switch(c.operation)
                        {
                            case TokenType.EQ:
                                // Console.WriteLine($"{leftVal}, {rightVal}");
                                return leftVal == rightVal;
                            case TokenType.NEQ:
                                // Console.WriteLine($"{leftVal}, {rightVal}");
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
                    throw new Exception("Condition does not evaluate to a boolean");
                }
            }
        }
        
        if (node is FunctionCall func)
        {
            switch(func.name)
            {
                case "print":
                    Console.Write($"{Evaluate(func.arguments[0])}");
                    break;
                case "println":
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
                case "var":
                    // (name, initialValue)
                    if (func.arguments.Count != 2) Console.Write("'var' function must contain 2 arguments");
                    if (func.arguments[0] is not StringLiteral) Console.Write("First argument of 'var' must be a string");
                    
                    variables.Add((string)Evaluate(func.arguments[0]), Evaluate(func.arguments[1]));
                    break;
                case "set":
                    if (func.arguments.Count != 2) Console.Write("'var' function must contain 2 arguments");
                    if (func.arguments[0] is not StringLiteral) Console.Write("First argument of 'var' must be a string");
                    if (!variables.ContainsKey((string)Evaluate(func.arguments[0]))) Console.Write($"Variable name does not exist");

                    string varName = (string)Evaluate(func.arguments[0]);
                    variables[varName] = Evaluate(func.arguments[1]);
                    
                    
                    
                    break;
                    
            }
        } else if (node is IfStatement i)
        {
            if (EvalCondition(i.condition)) // if the condition is true
            {
                foreach (Statement statement in i.body)
                {
                    ProcessNode(statement);
                }
            }
        } else if (node is WhileStatement w)
        {
            while (EvalCondition(w.condition))
            {
                foreach (Statement statement in w.body)
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