﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// A strongly typed collection used for storing GameTreeNode instances.
    /// Sets the Parent property of the GameTreeNode automatically.
    /// </summary>
    public sealed class GameTreeNodeCollection : IEnumerable, IEnumerable<GameTreeNode>
    {
        private readonly GameTreeNode _owner;
        private readonly List<GameTreeNode> _nodes;

        public int Count => _nodes.Count;

        public GameTreeNodeCollection( GameTreeNode owner )
        {
            _owner = owner;
            _nodes = new List<GameTreeNode>();
        }

        public GameTreeNode this[ int index ] => _nodes[ index ];

        public void AddNode( GameTreeNode node )
        {
            if ( node == null )
                throw new ArgumentNullException( nameof( node ) );

            if ( node.Parent != null )
                throw new ArgumentException( "Given node already has a parent", nameof( node ) );

            _nodes.Add( node );
            node.Parent = _owner;
        }

        public bool RemoveNode( GameTreeNode node )
        {
            if ( node == null )
                throw new ArgumentNullException( nameof( node ) );

            bool removed = _nodes.Remove( node );

            //if (removed)
            //    node.Parent = null;

            return removed;
        }

        IEnumerator<GameTreeNode> IEnumerable<GameTreeNode>.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public IEnumerator GetEnumerator() => _nodes.GetEnumerator();
    }
}