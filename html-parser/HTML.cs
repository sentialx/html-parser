using html_parser.Enums;
using System;
using System.Collections.Generic;

namespace html_parser {
    public class HTML {
        public static List<string> selfClosingTags = new List<string>() {
            "AREA", "BASE", "BR", "COL", "COMMAND", "EMBED", "HR", "IMG", "INPUT",
            "KEYGEN", "LINK", "MENUITEM", "META", "PARAM", "SOURCE", "TRACK", "WBR"
        };

        /// <summary>
        /// Parses HTML code to a Document object.
        /// </summary>
        /// <param name="html"></param>
        /// <returns>Document</returns>
        public static Document Parse(string html) {
            var tokens = Tokenize(html);
            var elements = BuildTree(tokens);

            Document document = new Document();

            document.Children = elements;

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
                string tagName = GetTagName(token);
                if (token[1] == '/') {
                    return TagType.Closing;
                } else if (selfClosingTags.Contains(tagName)) {
                    return TagType.SelfClosed;
                } else {
                    return TagType.Opening;
                }
            } else {
                return TagType.Text;
            }
        }

        /// <summary>
        /// Gets tag name from tag code (e.g. <div>).
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Tag name</returns>
        public static string GetTagName(string source) {
            return source.Replace("<", "").Replace("/", "").Replace(">", "").Trim();
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

                // < is a tag starting char
                if (c == '<') {
                    if (capturing) {
                        // Add to tokens the captured text before the '<' char
                        // and continue capturing the text.
                        tokens.Add(capturedText);
                    } else {
                        // Start capturing text if it wasn't captured before.
                        capturing = true;
                    }
       
                    capturedText = "";
                } 
                // If the char is '>', it is the end of the tag.
                else if (c == '>' || i == html.Length - 1) {
                    // Stop capturing the text, and add the tag to tokens.
                    capturing = false;
                    capturedText += c;
                    tokens.Add(capturedText);
                } 
                // If the text isn't captured, and it's not a tag, start capturing it.
                else if (!capturing) {
                    capturedText = "";
                    capturing = true;
                }

                // Capture the text.
                if (capturing) {
                    capturedText += c;
                }
            }

            return tokens;
        }
        
        public static DOMElement GetOpeningTag(string tagName, DOMElement element) {
            if (element != null) {
                if (element.TagName == tagName) {
                    return element;
                } else {
                    return GetOpeningTag(tagName, element.ParentNode);
                }
            }

            return null;
        }

        /// <summary>
        /// Builds DOM tree using tokens generated from tokenizer.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>List of parent elements</returns>
        public static List<DOMElement> BuildTree(List<string> tokens) {
            List<DOMElement> elements = new List<DOMElement>();
            List<string> openedTags = new List<string>();

            DOMElement parent = null;

            for (int i = 0; i < tokens.Count; i++) {
                string token = tokens[i];

                string tagName = GetTagName(token).ToUpper();
                TagType tagType = GetTagType(token);

                if (tagType == TagType.SelfClosed || tagType == TagType.Opening || tagType == TagType.Text) {
                    DOMElement element = new DOMElement() {
                        NodeType = NodeType.Element,
                    };
                    
                    if (parent != null) {
                        element.ParentNode = parent;
                        parent.Children.Add(element);
                    } else {
                        elements.Add(element);
                    }

                    if (tagType == TagType.Opening) {
                        element.TagName = tagName;
                        parent = element;
                        openedTags.Add(tagName);
                    } else if (tagType == TagType.Text) {
                        element.NodeType = NodeType.Text;
                        element.NodeValue = token;
                    }
                } else if (tagType == TagType.Closing) {
                    if (parent != null) {
                        var openedTagIndex = openedTags.LastIndexOf(tagName);

                        if (openedTagIndex != -1) {
                            if (parent.TagName == tagName) {
                                parent = parent.ParentNode;
                            } else {
                                var el = GetOpeningTag(tagName, parent);
                                if (el != null) {
                                    parent = el.ParentNode;
                                }
                            }

                            openedTags.RemoveAt(openedTagIndex);
                        }
                       
                    }
                }
            }

            return elements;
        }
    }
}
