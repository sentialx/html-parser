using System;
using System.Collections.Generic;
using System.Text;

namespace html_parser
{
    enum NodeType
    {
        Element = 1,
        Attribute = 2,
        Text = 3,
        Comment = 8,
    }
}
