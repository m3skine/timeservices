using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Instinct_
{
    /// <summary>
    /// CommandBinaryList
    /// </summary>
    public class CommandBinaryLog
    {
        private LinkedList<Block> _blockList = new LinkedList<Block>();

        #region Class Types
        /// <summary>
        /// Block
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Block
        {
            /// <summary>
            /// 
            /// </summary>
            public byte[] Bytes; //  = new byte[1 << 256]
        }
        #endregion Class Types

        /// <summary>
        /// Applies this instance.
        /// </summary>
        public void Apply()
        {
        }
    }
}