using System.Collections;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Xml;

public class Interpreter
{
    List<ASTNode> nodes;
    MainForm form;
    Bus nes;

    Dictionary<string, object> variables = new Dictionary<string, object>();
    Dictionary<string, object> cpuValues = new Dictionary<string, object>();
    Dictionary<string, object> ppuValues = new Dictionary<string, object>();
    Dictionary<string, object> apuValues = new Dictionary<string, object>();
    Dictionary<string, object> nesValues = new Dictionary<string, object>();
    Dictionary<string, object> formValues = new Dictionary<string, object>();
    public Interpreter(List<ASTNode> nodes, MainForm form) 
    {
        this.nodes = nodes;
        this.form = form;
        nes = form.nes;
 
        cpuValues = new()
        {
            ["a"] = Wrap(() => nes.cpu.A),
            ["x"] = Wrap(() => nes.cpu.X),
            ["y"] = Wrap(() => nes.cpu.Y),
            ["pc"] = Wrap(() => nes.cpu.PC),
            ["sr"] = Wrap(() => nes.cpu.SR),
            ["sp"] = Wrap(() => nes.cpu.SP),
            

            ["aset"] = WrapSetter<byte>(value => nes.cpu.A = value),
            ["xset"] = WrapSetter<byte>(value => nes.cpu.X = value),
            ["yset"] = WrapSetter<byte>(value => nes.cpu.Y = value),
            ["pcset"] = WrapSetter<ushort>(value => nes.cpu.PC = value),
            ["srset"] = WrapSetter<byte>(value => nes.cpu.SR = value),
            ["spset"] = WrapSetter<byte>(value => nes.cpu.SP = value),
        };

        ppuValues = new()
        {
            
        };

        apuValues = new()
        {
            ["p1"] = Wrap(() => nes.apu.p1),
            ["p2"] = Wrap(() => nes.apu.p2),
            ["t"] = Wrap(() => nes.apu.t),
            ["n"] = Wrap(() => nes.apu.n),
            ["d"] = Wrap(() => nes.apu.d), 
        };

        nesValues = new()
        {
            ["clockcounter"] = Wrap(() => nes.clockCounter),

            ["clockcounterset"] = WrapSetter<int>(value => throw new Exception("clockcounter cannot be set.")),
        };
    }
    private Func<object> Wrap<T>(Func<T> func)
    {
        return () => func();
    }
    private Action<object> WrapSetter<T>(Action<T> setter)
    {
        return value =>
        {
            try
            {
                T converted = (T)Convert.ChangeType(value, typeof(T));
                setter(converted);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Cannot convert {value?.GetType().Name ?? "null"} to {typeof(T).Name}.",
                    ex);
            }
        };
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
            } else if (left is FunctionCall fc)
            {
                left = Evaluate(fc);
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
            } else if (right is FunctionCall fc)
            {
                right = Evaluate(fc);
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
            if (ex.path.Count == 0)
            {
                return variables[ex.value];
            } else
            {  
                object? values = null;
                switch(ex.value)
                {
                    case "cpu":
                    values = cpuValues;
                    break;
                    case "ppu":
                    values = ppuValues;
                    break;
                    case "apu":
                    values = apuValues;
                    break;
                    case "form":
                    values = formValues;
                    break;
                    case "nes":
                    values = nesValues;
                    break;
                }
                if (nes == null) throw new Exception($"A ROM must be loaded before being able to access {ex.value}");
                for (int i = 0; i < ex.path.Count; i++)
                {
                    if (values is Dictionary<string, object> dict)
                    {
                        values = dict[ex.path[i]];
                    } else
                    {
                        throw new Exception($"Path from {ex.value} to {ex.path[ex.path.Count - 1]} is invalid at {ex.path[i]}");
                    }
                }


                if (values is Func<object> f) 
                {
                    return f();
                } else if (values is Action<object> a)
                {
                    return a;
                } else 
                {
                    return new Exception("bruh");
                }
            }
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

        if (expr is FunctionCall fc) // for functions that return a value
        {
            switch(fc.name)
            {
                case "cpuread":
                    if (fc.arguments.Count < 1) throw new Exception($"cpuread method requires at least 1 argument");
                    ushort cpuaddress = Convert.ToUInt16(Evaluate(fc.arguments[0]));
                    bool cpubRead = false;
                    if (fc.arguments.Count == 2 && fc.arguments[1] is BoolLiteral) cpubRead = (bool)Evaluate(fc.arguments[1]);

                    return nes.cpuRead(cpuaddress, cpubRead);
                case "pushort cpuaddress = (ushort)Evaluate(fc.arguments[0]);puread":
                    if (fc.arguments.Count < 1) throw new Exception($"ppuread method requires 1 arguments, {fc.arguments.Count} were provided.");
                    ushort ppuaddress = Convert.ToUInt16(Evaluate(fc.arguments[0]));
                    bool ppubRead = false;
                    if (fc.arguments.Count == 2 && fc.arguments[1] is BoolLiteral) ppubRead = (bool)Evaluate(fc.arguments[1]);
                    return nes.ppu.ppuRead(ppuaddress, ppubRead);
            }
        }
        return "Evaluation didn't work for this value, check Interpreter";
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
                                return leftVal != rightVal;
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
            ushort address = 0;
            byte data = 0;
            switch(func.name)
            {
                case "print":
                    Console.Write($"{Evaluate(func.arguments[0])}");
                    break;
                case "println":
                    
                    Console.Write($"{Evaluate(func.arguments[0])}\n");
                    break;
                case "load":
                    // Console.Write($"Attempted to load {Evaluate(func.arguments[0])}\n");
                    form.LoadROM((string)Evaluate(func.arguments[0]));
                    nes = form.nes;
                    break;
                case "dump": 
                    Console.Write($"Attempted to dump to {Evaluate(func.arguments[0])}\n");
                    break;
                case "run": // runs if a valid cartridge is inside. 
                    if (nes == null) throw new Exception("A cartridge has not been loaded. Please use the load method, or use loadrun to do both at once.");
                    form.RunROM();
                    break;
                case "loadrun": // loads and runs, not much to say here!
                    form.LoadROMAndRun((string)Evaluate(func.arguments[0]));
                    nes = form.nes;
                    break;
                case "write": // writes memory addresses, what do you want me to say?
                    if (func.arguments.Count <= 1) throw new Exception("write method not given enough arguments!!");
                    if (func.arguments[0] is StringLiteral s)
                    {
                        string source = (string)Evaluate(s);
                        switch(source)
                        {
                            case "cpu":
                                address = Convert.ToUInt16(Evaluate(func.arguments[1]));
                                data = Convert.ToByte(Evaluate(func.arguments[2]));
                                nes.cpuWrite(address, data);
                                break;
                            case "ppu":
                                address = Convert.ToUInt16(Evaluate(func.arguments[1]));
                                data = Convert.ToByte(Evaluate(func.arguments[2]));
                                nes.ppu.ppuWrite(address, data);
                                break;
                        }
                    } else if (func.arguments[0] is IdentifierExpression ie)
                    {
                        if (ie.path.Count == 0) throw new Exception("This file path specified has a length of zero. If you meant to access 'cpu' and 'ppu' memory, simply enter cpu or ppu as a string as this method's first argument.");
                        ie.path[ie.path.Count - 1] = ie.path[ie.path.Count - 1] + "set";
                        object action = Evaluate(ie);
                        if (action is Action<object> a)
                        {
                            a(Evaluate(func.arguments[1]));
                        } else
                        {
                            throw new Exception($"Action {ie.path[ie.path.Count - 1]} not found or invalid");
                        }
                    } else
                    {
                        throw new Exception($"Invalid type {func.arguments[0].GetType()} used.");
                    }
                    break;
                case "cpuwrite":
                    address = Convert.ToUInt16(Evaluate(func.arguments[0]));
                    data = Convert.ToByte(Evaluate(func.arguments[1]));
                    nes.cpuWrite(address, data);
                    break;
                case "ppuwrite":
                    address = Convert.ToUInt16(Evaluate(func.arguments[0]));
                    data = Convert.ToByte(Evaluate(func.arguments[1]));
                    nes.ppu.ppuWrite(address, data);
                    break;
                case "var": // creates a variable
                    // (name, initialValue)
                    if (func.arguments.Count != 2) Console.Write("'var' function must contain 2 arguments");
                    if (func.arguments[0] is not StringLiteral) Console.Write("First argument of 'var' must be a string");
                    
                    variables.Add((string)Evaluate(func.arguments[0]), Evaluate(func.arguments[1]));
                    break;
                case "set": // sets a varible made using var()
                    if (func.arguments.Count != 2) Console.Write("'var' function must contain 2 arguments");
                    if (func.arguments[0] is not StringLiteral) Console.Write("First argument of 'var' must be a string");
                    if (!variables.ContainsKey((string)Evaluate(func.arguments[0]))) Console.Write($"Variable name does not exist");

                    string varName = (string)Evaluate(func.arguments[0]);
                    variables[varName] = Evaluate(func.arguments[1]);
                    break;
                case "save": // saves the game to same folder as the .nes or at a specified file path
                    if (nes == null) throw new Exception("NES does not exist");
                    if (func.arguments.Count == 0)
                    {
                        form.SaveGame();
                    } else
                    {
                        form.SaveGame((string)Evaluate(func.arguments[0]));
                    }
                    break;
                case "wait": // waits for a specific number of millseconds
                    Thread.Sleep((int)Evaluate(func.arguments[0]));
                    break;
                case "waituntil": // waits until the clockcounter is a certain amount
                    int waitGoal = (int)Evaluate(func.arguments[0]);
                    while(nes.clockCounter < waitGoal)
                    {
                        Thread.SpinWait(100);
                    }
                    break;
                case "waitsteps": // waits a certain number of steps
                    int waitStepGoal = (int)Evaluate(func.arguments[0]) + nes.clockCounter;
                    while (nes.clockCounter < waitStepGoal)
                    {
                        Thread.SpinWait(100);
                    }
                    break;
                case "close": // closes the game instance, but does not remove the cartridge. 
                    form.CloseROM();
                    break;
                case "exit": // exits out of the form completely
                    form.Close();
                    break;
                case "read":
                    // this has technically already been implemented at in the Evaluation func.
                    break;
                case "pause": // pauses the program (no more clock cycles!)
                    form.paused = true;
                    break;
                case "play": // plays the program (more clock cycles!)
                    form.paused = false;
                    break;
                case "step": // steps the program a certain amount of times (1 step = 1 ppu, 2 step = 1 cpu)
                    if (func.arguments.Count >= 1)
                    {
                        int amt = (int)Evaluate(func.arguments[0]);
                        for (int i = 0; i < amt; i++)
                        {
                            nes.clock();
                        }
                    } else
                    {
                        nes.clock();
                    }

                    break;
                case "stepuntil":
                    int stepAmt =(int)Evaluate(func.arguments[0]) - nes.clockCounter;
                    
                    for (int i = 0; i < stepAmt; i++)
                    {
                        nes.clock();
                    }
                    break;
                case "stepuntilasync":
                    new Thread(() => {
                        int stepAmt =(int)Evaluate(func.arguments[0]) - nes.clockCounter;
                        
                        for (int i = 0; i < stepAmt; i++)
                        {
                            nes.clock();
                        }

                        form.paused = false;
                    }).Start();
                    break;
                case "press": // press + release
                    // press("A", 5, "steps", true)
                    void PressKeyFunc()
                    {
                        string mode = (string)Evaluate(func.arguments[2]);
                        Keys key = DetermineKey((string)Evaluate(func.arguments[0]));
                        int duration = (int)Evaluate(func.arguments[1]);
                        form.InputKeyDownLogic(key);
                        if (mode == "step" || mode == "steps") {
                            int goal = nes.clockCounter + duration;
                            while (nes.clockCounter < goal) {Thread.SpinWait(100);}
                        } else
                        {
                            // else then just assume seconds. 
                            Thread.Sleep(duration);
                        }
                        form.InputKeyUpLogic(key);
                    }

                    if (func.arguments.Count == 4 && (bool)Evaluate(func.arguments[3]) == true)
                    {
                       Thread e = new Thread(() => PressKeyFunc());
                       e.Start();

                    } else
                    {
                       PressKeyFunc();
                    }
                break;

                case "hold": // press but no relase
                    int holdtemp = nes.clockCounter;
                    Keys holdkey = DetermineKey((string)Evaluate(func.arguments[0]));
                    int whenhold = (int)Evaluate(func.arguments[1]);
                    int holdoffset = 1000;
                    while (nes.clockCounter + holdoffset < whenhold) { Thread.SpinWait(100);}
                    form.InputKeyDownLogic(holdkey);
                    break;

                case "release": // release if 
                    int releasetemp = nes.clockCounter;
                    Keys releasekey = DetermineKey((string)Evaluate(func.arguments[0]));
                    int whenrelease = (int)Evaluate(func.arguments[1]);
                    int releaseoffset = 1000;
                    while (nes.clockCounter + releaseoffset < whenrelease) { Thread.SpinWait(100);}
                    form.InputKeyUpLogic(releasekey);
                    break;

                case "record":
                    form.recordInputs = (bool)Evaluate(func.arguments[0]);
                    break;

                    
            }
        } else if (node is IfStatement i)
        {
            if (EvalCondition(i.condition)) // if the condition is true
            {
                foreach (ASTNode statement in i.body)
                {
                    ProcessNode(statement);
                }
            }
        } else if (node is WhileStatement w)
        {
            while (EvalCondition(w.condition))
            {
                foreach (ASTNode statement in w.body)
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
    Keys DetermineKey(string keystring)
    {
        switch(keystring)
        {
            case "X": return Keys.X; 
            case "Z": return Keys.Z; 
            case "N": return Keys.N; 
            case "M": return Keys.M; 

            case "W": 
            case "up": return Keys.W; 

            case "S": 
            case "down": return Keys.S;

            case "A":
            case "left": return Keys.A; 

            case "D": 
            case "right": return Keys.D;

            default:
            throw new Exception("key was invalid!");
        }
    }
    
}

