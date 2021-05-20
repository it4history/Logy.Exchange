using System;
using System.Runtime.InteropServices;

namespace Logy.MwAgent.DotNetWikiBot.Exceptions
{
    /// <summary>Exception for handling situations when bot operation is disallowed.</summary>
    /// <exclude/>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class BotDisallowedException : WikiBotException
    {
        /// <exclude/>
        public BotDisallowedException() { }

        /// <exclude/>
        public BotDisallowedException(string msg) : base(msg) { }

        /// <exclude/>
        public BotDisallowedException(string msg, Exception inner) : base(msg, inner) { }
    }
}
