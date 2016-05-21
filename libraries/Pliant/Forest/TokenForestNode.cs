﻿using Pliant.Tokens;

namespace Pliant.Forest
{
    public class TokenForestNode : ForestNodeBase, ITokenForestNode
    {
        public IToken Token { get; private set; }

        public TokenForestNode(IToken token, int origin, int location)
            : base(origin, location)
        {
            Token = token;
            _hashCode = ComputeHashCode();
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Token; }
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var tokenNode = obj as TokenForestNode;
            if ((object)tokenNode == null)
                return false;

            return Location == tokenNode.Location
                && NodeType == tokenNode.NodeType
                && Origin == tokenNode.Origin
                && Token.Equals(tokenNode.Token);
        }

        private readonly int _hashCode;
        
        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Token.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}