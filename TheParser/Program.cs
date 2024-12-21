namespace TheParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter grammar rules (format: S->aS|bA, type 'END' to stop):");

            var grammar = new Grammar();
            var nonTerminals = new HashSet<string>();

            while (true)
            {
                string line = Console.ReadLine()?.Trim();
                if (line?.ToUpper() == "END")
                    break;

                // Split on -> and |
                string[] parts = line.Split(new[] { "->" }, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;

                string nonTerminal = parts[0].Trim();
                string[] rules = parts[1].Split('|').Select(r => r.Trim()).ToArray();

                grammar.AddRule(nonTerminal, rules);
                nonTerminals.Add(nonTerminal);
            }

            Console.WriteLine("\nGrammar successfully added:");
            if (grammar.IsSimple())
            {
                Console.WriteLine("The grammar is simple.");
            }
            else
            {
                Console.WriteLine("The grammar is not simple.");
            }

            while (true)
            {
                Console.Write("\nEnter the input string to validate (or 'exit' to quit): ");
                string input = Console.ReadLine()?.Trim();

                if (input?.ToLower() == "exit")
                    break;

                if (grammar.Parse(input))
                {
                    Console.WriteLine("Input string is ACCEPTED!");
                }
                else
                {
                    Console.WriteLine("Input string is NOT ACCEPTED!");
                }
            }
        }
    }
}
