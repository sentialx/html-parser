using System;

namespace html_parser {
    class Program {
        static void Main(string[] args) {
            string html = "<html><head><title>Page Title</title></head><body><div><a>Text<b>b</a>aha</a>...</div><section>Section</section><footer>Foo</footer></body></html>";

            Document document = HTML.Parse(html);

            Printer.Print(document.Children, true);

            Console.ReadKey();
        }
    }
}
