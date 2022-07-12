using ReflectiveUI.Core.ObjectGraph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectiveUI.Core.NodeMatching
{
    public class NodeMatchPredicate
    {
        private Type? _matchType;

        public bool IsMatch(IInteractNode node)
        {
            // If the match type isn't null, and the node type is assignable to it.
            if (_matchType != null && node.GetType().IsAssignableTo(_matchType))
            {
                return true;
            }
            return false;
        }

        public NodeMatchPredicate IsObject()
        {
            _matchType = typeof(InteractNode.Object);
            return this;
        }
    }
}
