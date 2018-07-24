using System;

namespace html_parser {
    class Program {
        static void Main(string[] args) {
            string html = "<div><div><div><div>aha</div></div>";

            var tokens = HTML.Tokenize(html);
            var elements = HTML.BuildTree(tokens);
            HTML.PassHTMLToElements(ref elements);

            Console.ReadKey();
        }
    }
}
