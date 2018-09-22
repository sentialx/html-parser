using System;

namespace html_parser {
    class Program {
        static void Main(string[] args) {
            string html = "<html><head> <title>Page Title</title></head><body> <div> <a> aha <b> boom </a> </a> under aha </div></div></b> <div> a div </div></body></html>";

            Document document = HTML.Parse(html);

            Printer.Print(document.Children, true);

            Console.ReadKey();
        }
    }
}
