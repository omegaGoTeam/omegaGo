﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a collection of game in a SGF file
    /// </summary>
    internal class SgfCollection : IEnumerable<SgfGameTree>
    {
        /// <summary>
        /// Initializes an empty SGF game collection
        /// </summary>
        public SgfCollection()
        {
        }

        /// <summary>
        /// Initializes SGF game collection
        /// </summary>
        /// <param name="gameTrees">Games to add to the collection</param>
        public SgfCollection( IEnumerable<SgfGameTree> gameTrees )
        {
            if ( gameTrees == null ) throw new ArgumentNullException( nameof( gameTrees ) );
            GameTrees.AddRange( gameTrees );
        }

        /// <summary>
        /// List of games
        /// </summary>
        public List<SgfGameTree> GameTrees { get; } = new List<SgfGameTree>();

        public IEnumerator<SgfGameTree> GetEnumerator()
        {
            return GameTrees.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}