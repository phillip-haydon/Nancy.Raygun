using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Raygun.Messages
{
    public class RaygunUserMessage
    {
        public RaygunUserMessage(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; private set; }
    }
}
