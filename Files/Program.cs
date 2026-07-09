

using System.Windows.Forms;

public class Program
{
    static void DisplayParser(List<ASTNode> astNodes)
    {
        Console.Write("---------- PARSER INFO -----------\n");
        foreach (ASTNode node in astNodes)
        {
            if (node is FunctionCall n)
            {
                Console.Write($"FunctionCall: {n.name} Args: ");

                foreach (Expression arg in n.arguments)
                {
                    Console.Write($"{arg.GetType().Name}()  ");
                }

                Console.Write($"\n");
            } else if (node is IfStatement i)
            {
                Console.Write($"There be an if statemetn!\n");
            }
        }
        Console.Write("---------------------------------\n");
    }

    static void DisplayLexer(List<Token> tokenList)
    {
        Console.Write("---------- LEXER INFO -----------\n");
        foreach (Token token in tokenList)
        {
            Console.Write($"Line {token.lineNum}: {token.type}={token.lexeme} \n");
        }
        Console.Write("---------------------------------\n");
    }

    static void Main()
    {
        List<Token> tokenList;
        List<ASTNode> astNodes;

        string source = File.ReadAllText("./Files/DNESSFiles/script.dness");

        Lexer lexer = new Lexer(source);

        
        tokenList = lexer.ScanTokens();

        DisplayLexer(tokenList);

        Parser parser = new Parser(tokenList);
        astNodes = parser.ParseTokens();

        // DisplayParser(astNodes);

        MainForm form = new MainForm();
        
        Interpreter interpreter = new Interpreter(astNodes, form);

        Thread formThread = new Thread(() => Application.Run(form));
        formThread.Start();
        
        interpreter.Execute();
    }
}