namespace TheScanner
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine("Enter C++ code (press Enter twice to end input):");

            // Read multiple lines from user input until a blank line is entered
            string code = string.Empty;
            string line;
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                code += line + "\n";
            }

            CppScanner scanner = new CppScanner();
            List<CppScanner.Token> tokens = scanner.Tokenize(code);

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}
