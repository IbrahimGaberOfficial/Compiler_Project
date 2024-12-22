using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Grammar
    {
        private Dictionary<string, List<string>> productions = new Dictionary<string, List<string>>();
        private string inputString;
        private int position;

        public void AddRule(string nonTerminal, string[] rules)
        {
            productions[nonTerminal] = new List<string>(rules);
        }

        public bool IsSimple()
        {
            foreach (var prod in productions)
            {
                // Check for left recursion (rule starts with the same non-terminal).
                if (prod.Key == prod.Value[0])
                    return false;
                var firstChars = new HashSet<char>();
                foreach (var rule in prod.Value)
                {
                    if (rule.Length == 0 || !char.IsLower(rule[0]))
                        return false;
                    // Check for left recursion (rule starts with the same non-terminal).

                    if (!firstChars.Add(rule[0]))
                        return false;
                }
            }
            return true;
        }

        public bool Parse(string input)
        {
            inputString = input;
            position = 0;
            return ParseNonTerminal("S") && position == input.Length;
        }

        private bool ParseNonTerminal(string nonTerminal)
        {
            if (!productions.ContainsKey(nonTerminal))
                return false;

            foreach (string rule in productions[nonTerminal])
            {
                int savedPos = position;
                if (ParseRule(rule))
                    return true;
                position = savedPos;
            }
            return false;
        }

        private bool ParseRule(string rule)
        {
            if (position >= inputString.Length)
                return false;

            // Handle terminal
            if (rule.Length == 1)
            {
                if (position < inputString.Length && inputString[position] == rule[0])
                {
                    position++;
                    return true;
                }
                return false;
            }

            // Handle terminal followed by non-terminal (e.g., "aS")
            if (rule.Length == 2)
            {
                if (inputString[position] != rule[0])
                    return false;
                position++;
                return ParseNonTerminal(rule[1].ToString());
            }

            return false;
        }
    }

   
}
