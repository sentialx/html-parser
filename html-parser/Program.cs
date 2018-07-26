using System;

namespace html_parser {
    class Program {
        static void Main(string[] args) {
            string html = "<div><div><div><div>aha<div /></div></div></div></div><div></div>";

            Document document = HTML.Parse(html);

            Console.ReadKey();
        }
    }
}
