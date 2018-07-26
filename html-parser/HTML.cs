using html_parser.Enums;
using System.Collections.Generic;

namespace html_parser {
    public class HTML {
        /// <summary>
        /// Parses HTML code to a Document object.
        /// </summary>
        /// <param name="html"></param>
        /// <returns>Document</returns>
        public static Document Parse(string html) {
            var tokens = Tokenize(html);
            var elements = BuildTree(tokens);
            PassHTMLToElements(ref elements);

            Document document = new Document();

            DOMElement documentElement = null;
            DOMElement bodyElement = null;
            DOMElement headElement = null;

            var otherChildren = new List<DOMElement>();

            foreach (DOMElement element in elements) {
                if (element.TagName == "html") {
                    documentElement = element;
                } else if (element.TagName == "head") {
                    headElement = element;
                } else if (element.TagName == "body") {
                    bodyElement = element;
                } else {
                    otherChildren.Add(element);
                }
            }

            if (documentElement == null) {
                documentElement = new DOMElement() {
                    TagName = "HTML",
                };

                if (headElement == null) {
                    headElement = new DOMElement() {
                        TagName = "HEAD",
                        OuterHTML = "<head></head>",
                    };
                }

                if (bodyElement == null) {
                    bodyElement = new DOMElement() {
                        TagName = "BODY",
                    };

                    string innerHTML = "";

                    foreach (DOMElement element in otherChildren) {
                        element.ParentNode = bodyElement;
                        innerHTML += element.OuterHTML;
                    }

                    bodyElement.Children = otherChildren;
                    bodyElement.InnerHTML = innerHTML;
                    bodyElement.OuterHTML = "<body>" + innerHTML + "</body>";
                }

                documentElement.Children.Add(headElement);
                documentElement.Children.Add(bodyElement);
            } else {
                var children = new List<DOMElement>();

                foreach (DOMElement element in documentElement.Children) {
                    if (element.TagName == "head") {
                        headElement = element;
                    } else if (element.TagName == "body") {
                        bodyElement = element;
                    } else {
                        children.Add(element);
                    }
                }

                if (headElement == null) {
                    headElement = new DOMElement() {
                        TagName = "HEAD",
                        OuterHTML = "<head></head>",
                    };
                    documentElement.Children.Insert(0, headElement);
                }

                if (bodyElement == null) {
                    bodyElement = new DOMElement() {
                        TagName = "BODY",
                    };

                    string innerHTML = "";

                    foreach (DOMElement element in children) {
                        documentElement.Children.Remove(element);
                        element.ParentNode = bodyElement;
                        innerHTML += element.OuterHTML;
                    }

                    bodyElement.Children = children;

                    bodyElement.InnerHTML = innerHTML;
                    bodyElement.OuterHTML = "<body>" + innerHTML + "</body>";

                    documentElement.Children.Insert(1, bodyElement);
                }
            }

            if (documentElement.OuterHTML == null) {
                string innerHTML = "";
                foreach (DOMElement element in documentElement.Children) {
                    innerHTML += element.OuterHTML;
                }
                documentElement.InnerHTML = innerHTML;
                documentElement.OuterHTML = "<html>" + innerHTML + "</html>";
            }

            document.Children.Add(documentElement);

            document.DocumentElement = documentElement;
            document.Body = bodyElement;
            document.Head = headElement;

            return document;
        }

        /// <summary>
        /// Minifies html code.
        /// </summary>
        /// <param name="html"></param>
        /// <returns>Minified html code</returns>
        public static string Minify(string[] html) {
            string htmlMin = "";

            for (int i = 0; i < html.Length; i++) {
                htmlMin += html[i].Trim();
            }

            return htmlMin;
        }

        /// <summary>
        /// Determines given tag code type.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Tag type</returns>
        public static TagType GetTagType(string token) {
            if (token[0] == '<') {
                if (token[1] == '/') {
                    return TagType.Closing;
                } else {
                    return TagType.Opening;
                }
            } else {
                return TagType.Text;
            }
        }

        /// <summary>
        /// Gets tag name from tag code (ie. <div>).
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Tag name</returns>
        public static string GetTagName(string source) {
            return source.Replace("<", "").Replace("/", "").Replace(">", "");
        }

        /// <summary>
        /// Separates a HTML code to tokens.
        /// </summary>
        /// <param name="html"></param>
        /// <returns>List of tokens</returns>
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
                } else if (c == '>' || i == html.Length - 1) {
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

        /// <summary>
        /// Builds DOM tree using tokens generated from tokenizer.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>List of parent elements</returns>
        public static List<DOMElement> BuildTree(List<string> tokens) {
            List<DOMElement> elements = new List<DOMElement>();
            DOMElement parent = null;
            DOMElement firstParent = null;

            for (int i = 0; i < tokens.Count; i++) {
                string token = tokens[i];

                string tagName = GetTagName(token).ToUpper();
                TagType tagType = GetTagType(token);

                if (tagType == TagType.Opening || tagType == TagType.Text) {
                    DOMElement element = new DOMElement();

                    bool newFirstParent = false;

                    if (parent != null) {
                        element.ParentNode = parent;
                        parent.Children.Add(element);
                    } else {
                        elements.Add(element);
                        firstParent = element;
                        newFirstParent = true;
                    }

                    if (tagType == TagType.Opening) {
                        element.TagName = tagName;
                        element.NodeType = NodeType.Element;

                        firstParent.OuterHTML += token;
                        if (!newFirstParent) {
                            firstParent.InnerHTML += token;
                        }

                        parent = element;
                    } else if (tagType == TagType.Text) {
                        element.NodeType = NodeType.Text;
                        element.NodeValue = token;

                        firstParent.OuterHTML += token;
                        firstParent.InnerHTML += token;
                    }
                } else if (tagType == TagType.Closing) {
                    if (parent != null) {
                        if (parent.TagName == tagName) {
                            firstParent.OuterHTML += token;
                            if (firstParent != parent) {
                                firstParent.InnerHTML += token;
                            }

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

        /// <summary>
        /// Passes recursively innerHTML and outerHTML to each element.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="tokens"></param>
        public static void PassHTMLToElements(ref List<DOMElement> elements, List<string> tokens = null) {
            foreach (DOMElement element in elements) {
                var newTokens = new List<string>();

                if (tokens == null) newTokens = Tokenize(element.InnerHTML);

                if (element.ParentNode != null) {
                    element.OuterHTML = element.ParentNode.InnerHTML;

                    string innerHTML = "";

                    int openingTags = 0;
                    int closingTags = 0;

                    for (int i = 0; i < tokens.Count; i++) {
                        var token = tokens[i];
                        var tagType = GetTagType(token);

                        if (tagType == TagType.Opening) openingTags++;
                        else if (tagType == TagType.Closing) closingTags++;

                        if (i != 0) {
                            if (i == tokens.Count - 1 && tagType == TagType.Closing && closingTags == openingTags) {
                                continue;
                            }

                            innerHTML += token;
                            newTokens.Add(token);
                        }
                    }

                    element.InnerHTML = innerHTML;
                }
                if (element.Children.Count > 0) {
                    PassHTMLToElements(ref element.Children, newTokens);
                }
            }
        }
    }
}
