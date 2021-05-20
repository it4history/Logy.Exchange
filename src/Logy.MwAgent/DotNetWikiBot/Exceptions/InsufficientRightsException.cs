using System;
using System.Runtime.InteropServices;

namespace Logy.MwAgent.DotNetWikiBot.Exceptions
{
    /// <summary>Exception for handling errors due to insufficient rights.</summary>
    /// <exclude/>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class InsufficientRightsException : WikiBotException
    {
        /// <exclude/>
        public InsufficientRightsException() { }

        /// <exclude/>
        public InsufficientRightsException(string msg) : base(msg) { }
        
        /// <exclude/>
        public InsufficientRightsException(string msg, Exception inner) : base(msg, inner) { }
    }
}
