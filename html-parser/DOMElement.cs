using html_parser.Enums;
using System.Collections.Generic;

namespace html_parser {
    public class DOMElement {
        public DOMElement ParentNode;
        public List<DOMElement> Children = new List<DOMElement>();
        public NodeType NodeType = NodeType.Element;
        public string InnerHTML;
        public string OuterHTML;
        public string NodeValue;
        public string TagName;
    }
}
