using System;
using System.Collections.Generic;
using System.Text;

namespace html_parser
{
    class DOMElement
    {
        public List<DOMElement> Children = new List<DOMElement>();
        public NodeType NodeType;
        public string InnerHTML;
        public string InnerText;
        public string OuterHTML;
        public string NodeValue;
        public string TagName;
        public DOMElement ParentNode;
    }
}
