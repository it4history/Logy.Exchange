using System;
using System.Runtime.InteropServices;

namespace Logy.MwAgent.DotNetWikiBot.Exceptions
{
    /// <summary>Exceptions for handling wiki edit conflicts.</summary>
    /// <exclude/>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class EditConflictException : WikiBotException
    {
        /// <exclude/>
        public EditConflictException() { }

        /// <exclude/>
        public EditConflictException(string msg) : base(msg) { }
        
        /// <exclude/>
        public EditConflictException(string msg, Exception inner) : base(msg, inner) { }
    }
}
