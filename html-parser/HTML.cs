using System;
using System.Collections.Generic;
using System.Text;

namespace html_parser {
    class HTML {
        public static string Minify(string[] html) {
            string htmlMin = "";

            for (int i = 0; i < html.Length; i++) {
                htmlMin += html[i].Trim();
            }

            return htmlMin;
        }

        public static List<string> Tokenize(string html) {
            List<string> tokens = new List<string>();

            bool capturing = false;
            string capturedText = "";

            for (int i = 0; i < html.Length; i++) {
                char c = html[i];

                if (c == '<') {
                    if (capturing) {
                        tokens.Add(capturedText);
                    } else {
                        capturing = true;
                    }
                    capturedText = "";
                } else if (c == '>') {
                    capturing = false;
                    capturedText += c;
                    tokens.Add(capturedText);
                } else if (!capturing) {
                    capturedText = "";
                    capturing = true;
                }

                if (capturing) {
                    capturedText += c;
                }
            }

            return tokens;
        }

        public static List<DOMElement> BuildTree(List<string> tokens) {
            List<DOMElement> elements = new List<DOMElement>();
            DOMElement parent = null;

            for (int i = 0; i < tokens.Count; i++) {
                string token = tokens[i];

                DOMElement element = new DOMElement();

                string elementType = "text";
                string tagName = GetTagName(token);

                if (token[0] == '<') {
                    if (token[1] == '/') {
                        elementType = "closing";
                    } else {
                        elementType = "opening";
                    }
                }

                if (elementType == "opening" || elementType == "text") {
                    if (parent != null) {
                        element.ParentNode = parent;
                        parent.Children.Add(element);
                    } else {
                        elements.Add(element);
                    }

                    if (elementType == "opening") {
                        element.TagName = tagName;
                        element.NodeType = NodeType.Element;
                        parent = element;
                    } else {
                        element.NodeType = NodeType.Text;
                        element.NodeValue = token;
                    }
                } else if (elementType == "closing") {
                    if (parent != null) {
                        if (parent.TagName == tagName) {
                            parent = parent.ParentNode;
                        } else {
                            if (parent.ParentNode != null && tokens[i - 1][1] != '/' && parent.Children.Count != 0) {
                                parent = parent.ParentNode.ParentNode;
                            }
                        }
                    }
                }
            }

            return elements;
        }

        public static string GetTagName(string source) {
            return source.Replace("<", "").Replace("/", "").Replace(">", "");
        }
    }
}
