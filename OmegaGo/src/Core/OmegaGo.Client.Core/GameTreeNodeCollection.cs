using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    /// <summary>
    /// A strongly typed collection used for storing GameTreeNode instances.
    /// Sets the Parent property of the GameTreeNode automatically.
    /// </summary>
    public sealed class GameTreeNodeCollection : IEnumerable
    {
        private GameTreeNode _owner;
        private List<GameTreeNode> _nodes;

        public int Count
        {
            get { return _nodes.Count; }
        }
        
        public GameTreeNodeCollection(GameTreeNode owner)
        {
            _owner = owner;
            _nodes = new List<GameTreeNode>();
        }

        public GameTreeNode this[int index]
        {
            get
            {
                return _nodes[index];
            }
        }
        
        public void AddNode(GameTreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node cant be null");

            if (node.Parent != null)
                ; // Throw error?

            _nodes.Add(node);
            node.Parent = _owner;
        }
        
        public bool RemoveNode(GameTreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node cant be null");

            bool removed = _nodes.Remove(node);

            //if (removed)
            //    node.Parent = null;

            return removed;
        }

        public IEnumerator GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
    }
}
