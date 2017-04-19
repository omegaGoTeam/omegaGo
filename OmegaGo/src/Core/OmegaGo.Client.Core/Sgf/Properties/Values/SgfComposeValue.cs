using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Represents a SGF compose value
    /// </summary>
    /// <typeparam name="TLeft">Left type</typeparam>
    /// <typeparam name="TRight">Right type</typeparam>
    public class SgfComposeValue<TLeft,TRight>
    {
        /// <summary>
        /// Creates a compose value
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        public SgfComposeValue( TLeft left, TRight right )
        {
            Left = left;
            Right = right;
        }        

        /// <summary>
        /// Left value
        /// </summary>
        public TLeft Left { get; set; }

        /// <summary>
        /// Right value
        /// </summary>
        public TRight Right { get; set; }
    }
}
