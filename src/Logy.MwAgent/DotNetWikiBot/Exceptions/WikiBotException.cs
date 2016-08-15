using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Logy.MwAgent.DotNetWikiBot.Exceptions
{
    /// <summary>Class establishes custom application exceptions.</summary>
    /// <exclude/>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class WikiBotException : Exception
    {
        /// <exclude/>
        public WikiBotException()
        {
        }

        /// <exclude/>
        public WikiBotException(string msg) : base(msg) {
            Console.Beep();

            // Console.ForegroundColor = ConsoleColor.Red;
        }

        /// <exclude/>
        public WikiBotException(string msg, Exception inner) : base(msg, inner)
        {
        }

        /// <exclude/>
        protected WikiBotException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
