using System;

namespace html_parser {
    class Program {
        static void Main(string[] args) {
            string html = "<html><head></head><body><div>aha<a>a<b>b<c>c<d></b>xd</body></html>";

            Document document = HTML.Parse(html);

            Printer.Print(document.Children, true);

            Console.ReadKey();
        }
    }
}
