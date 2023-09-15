namespace BinObjCleaner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string initialDir = @" ";
            if (args.Length > 0)
            {
                initialDir = args[0];
            }

            while (true)
            {
                if (!AskForDirectory(ref initialDir))
                {
                    Console.WriteLine();
                    Console.WriteLine("Exit");
                    return;
                }

                ClearAndReport(initialDir);

                Console.WriteLine();
                Console.WriteLine("Clearing complete. Press Enter to repeat or Esc for exit:");

                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    return;
                }
                Console.Clear();
            }
        }

        private static bool AskForDirectory(ref string initialDir)
        {
            while (!Path.Exists(initialDir))
            {
                Console.WriteLine("Enter the path or empty string for exit:");
                initialDir = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(initialDir))
                {
                    return false;
                }
            }

            return true; 
        }

        private static void ClearAndReport(string initialDir)
        {
            DirectoryTree tree = new(initialDir);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Clear Binary and intermediate files from {tree.Path}");
            Console.WriteLine();

            WriteContent(tree);

            Console.WriteLine();
            Console.WriteLine();

            var toDel = tree.DirsToDel;

            if (toDel.Any())
            {
                Console.WriteLine($"Clearing list[{toDel.Count()}]:");

                foreach (string dir in toDel)
                {
                    string report;
                    try
                    {
                        Directory.Delete(dir, true);
                        report = "Removed";
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    catch (Exception ex)
                    {
                        report = $"Failed: {ex.Message}";
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.WriteLine($"{report}: {dir}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Complete");
        }

        private static void WriteContent(DirectoryTree tree)
        {
            foreach (var br in tree.Branches)
            {
                if (br.IsObjOrBin)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (br.HasObjOrBinDirectly)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                else if (br.ContainsObjOrBin)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.WriteLine(br);
            }
        }
    }
}