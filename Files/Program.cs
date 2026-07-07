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

        Lexer lexer = new Lexer(
            
            
            """
            if(true) {
                println("Hello World, although it sounds like bits and bytes...")
            }
            
            load("nestest.nes")

            var("x", 0)
            while (x < 5) {
                set("x", x + 1)
                print("X is currently: ")
                println(x)
            }

            var("num", 2 + 2 * 2)
            if (num == (6)) {
                println(num)
            }           

           

            """
            

        );

        
        tokenList = lexer.ScanTokens();

        DisplayLexer(tokenList);

        Parser parser = new Parser(tokenList);
        astNodes = parser.ParseTokens();

        // DisplayParser(astNodes);

        Interpreter interpreter = new Interpreter(astNodes);
        interpreter.Execute();

        
       
        

    }
}