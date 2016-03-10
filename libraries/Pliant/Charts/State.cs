﻿using Pliant.Ast;
using Pliant.Grammars;
using System;
using System.Text;

namespace Pliant.Charts
{
    public class State : IState
    {
        public IProduction Production { get; private set; }

        public int Origin { get; private set; }

        public ISymbol PreDotSymbol { get; private set; }

        public ISymbol PostDotSymbol { get; private set; }

        public int Length { get; private set; }

        public virtual StateType StateType { get { return StateType.Normal; } }

        public INode ParseNode { get; set; }

        private readonly int _hashCode;

        public State(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            Production = production;
            Origin = origin;
            Length = position;
            PostDotSymbol = GetPostDotSymbol(position, production);
            PreDotSymbol = GetPreDotSymbol(position, production);
            _hashCode = ComputeHashCode();
        }

        public State(IProduction production, int position, int origin, INode parseNode)
            : this(production, position, origin)
        {
            ParseNode = parseNode;
        }

        public IState NextState()
        {
            return NextState(null as INode);
        }

        public IState NextState(INode node)
        {
            if (IsComplete)
                return null;
            return new State(
                Production,
                Length + 1,
                Origin,
                node);
        }

        public IState NextState(int newOrigin)
        {
            return NextState(newOrigin, null);
        }

        public IState NextState(int newOrigin, INode parseNode)
        {
            if (IsComplete)
                return null;
            return new State(
                Production,
                Length + 1,
                newOrigin,
                parseNode);
        }

        public bool IsComplete
        {
            get { return Length == Production.RightHandSide.Count; }
        }

        public bool IsSource(ISymbol searchSymbol)
        {
            if (IsComplete)
                return false;
            return PostDotSymbol.Equals(searchSymbol);
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var state = obj as State;
            if ((object)state == null)
                return false;
            // PERF: Hash Codes are Cached, so equality performance is cached as well
            return GetHashCode() == state.GetHashCode();
        }


        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                Length.GetHashCode(),
                Origin.GetHashCode(),
                Production.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", Production.LeftHandSide.Value);
            const string Dot = "\u25CF";

            for (int p = 0; p < Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == Length ? Dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Length == Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            stringBuilder.AppendFormat("\t\t({0})", Origin);
            return stringBuilder.ToString();
        }

        private static ISymbol GetPreDotSymbol(int position, IProduction production)
        {
            if (position == 0 || production.IsEmpty)
                return null;
            return production.RightHandSide[position - 1];
        }

        private static ISymbol GetPostDotSymbol(int position, IProduction production)
        {
            if (position >= production.RightHandSide.Count)
                return null;
            return production.RightHandSide[position];
        }
    }
}