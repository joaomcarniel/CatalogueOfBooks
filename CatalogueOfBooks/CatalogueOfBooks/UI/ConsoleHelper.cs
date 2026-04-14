namespace CatalogueOfBooks.UI
{
    public static class ConsoleHelper
    {
        public static string WriteString(string prompt, bool required = true)
        {
            while (true)
            {
                Console.Write($"  {prompt}: ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (!required || input.Length > 0) return input;
                Console.WriteLine("This field is required.");
            }
        }

        public static DateTime WriteDate(string prompt)
        {
            while (true)
            {
                string raw = WriteString(prompt + " (dd/MM/yyyy)");
                if (DateTime.TryParseExact(raw, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime dt))
                    return dt;
                Console.WriteLine("Invalid date. Format: dd/MM/yyyy");
            }
        }
        public static List<string> WriteList(string prompt)
        {
            var list = new List<string>();
            Console.WriteLine($"Enter {prompt} one per line. Empty line to finish.");
            while (true)
            {
                Console.Write("    -> ");
                string item = Console.ReadLine()?.Trim() ?? "";
                if (item.Length == 0) break;
                list.Add(item);
            }
            return list;
        }
        public static int WriteInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                string raw = WriteString(prompt);
                if (int.TryParse(raw, out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Please enter a whole number between {min} and {max}.");
            }
        }
    }
}
