﻿using Pliant.Forest;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class InternalTreeNode : IInternalTreeNode
    {
        private INodeVisitorStateManager _stateManager;
        private IAndNode _currentAndNode;
        private IInternalNode _internalNode;

        public int Origin { get { return _internalNode.Origin; } }

        public int Location { get { return _internalNode.Location; } }

        public INonTerminal Symbol { get; private set; }

        public InternalTreeNode(
            IInternalNode internalNode,
            IAndNode currentAndNode,
            INodeVisitorStateManager stateManager)
        {
            _stateManager = stateManager;
            _currentAndNode = currentAndNode;
            _internalNode = internalNode;
            SetRule(_internalNode);
        }

        public InternalTreeNode(
            IInternalNode internalNode)
            : this(internalNode, new MultiPassNodeVisitorStateManager())
        {
        }

        public InternalTreeNode(
            IInternalNode internalNode,
            INodeVisitorStateManager stateManager)
            : this(internalNode, stateManager.GetCurrentAndNode(internalNode), stateManager)
        {
        }

        private void SetRule(IInternalNode node)
        {
            switch (node.NodeType)
            {
                case Forest.NodeType.Symbol:
                    Symbol = (node as ISymbolNode).Symbol as INonTerminal;
                    break;

                case Forest.NodeType.Intermediate:
                    Symbol = (node as IIntermediateNode).State.Production.LeftHandSide;
                    break;
            }
        }

        public IEnumerable<ITreeNode> Children
        {
            get
            {
                return EnumerateChildren(_currentAndNode);
            }
        }

        private IEnumerable<ITreeNode> EnumerateChildren(IAndNode andNode)
        {
            foreach (var child in andNode.Children)
            {
                switch (child.NodeType)
                {
                    case Forest.NodeType.Intermediate:
                        var intermediateNode = child as IIntermediateNode;
                        var currentAndNode = _stateManager.GetCurrentAndNode(intermediateNode);
                        foreach (var otherChild in EnumerateChildren(currentAndNode))
                            yield return otherChild;
                        break;

                    case Forest.NodeType.Symbol:
                        var symbolNode = child as ISymbolNode;
                        var childAndNode = _stateManager.GetCurrentAndNode(symbolNode);
                        yield return new InternalTreeNode(symbolNode, childAndNode, _stateManager);
                        break;

                    case Forest.NodeType.Token:
                        yield return new TokenTreeNode(child as ITokenNode);
                        break;

                    default:
                        throw new Exception("Unrecognized NodeType");
                }
            }
        }

        public TreeNodeType NodeType
        {
            get { return TreeNodeType.Internal; }
        }

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"{Symbol}({Origin}, {Location})";
        }
    }
}