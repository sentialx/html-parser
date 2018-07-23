using System;

namespace html_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            string html = "<div></div>";

            var tokens = HTML.Tokenize(html);
            HTML.BuildTree(tokens);

            Console.ReadKey();
        }
    }
}
