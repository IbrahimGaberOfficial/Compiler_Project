namespace TheScanner
{
    internal class Program
    {
        public static void Main()
        {
            string fileName = "code.cpp";
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "files", fileName);

                // Verify file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                // Read and display the file content
                string code = File.ReadAllText(filePath);

                CppScanner scanner = new CppScanner();
                List<CppScanner.Token> tokens = scanner.Tokenize(code);

                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }
            }

            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Log error or handle specifically for missing files
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Error: Directory not found. {ex.Message}");
                // Handle invalid directory path
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: No permission to read file. {ex.Message}");
                // Handle access permission issues
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine($"Error: Path is too long. {ex.Message}");
                // Handle path length exceeding system limits
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: An I/O error occurred. {ex.Message}");
                // Handle other I/O related issues
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                // Handle any other unexpected errors
            }
        }



    }
}
