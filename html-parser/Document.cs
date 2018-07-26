using System;
using System.Collections.Generic;
using System.Text;

namespace html_parser {
    public class Document {
        public DOMElement DocumentElement;
        public DOMElement Body;
        public DOMElement Head;
        public List<DOMElement> Children = new List<DOMElement>();
    }
}
