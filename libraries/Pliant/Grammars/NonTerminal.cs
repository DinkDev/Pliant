﻿namespace Pliant.Grammars
{
    public class NonTerminal : Symbol, INonTerminal
    {
        public string Value { get; private set; }

        public string Namespace { get; private set; }

        public string Name { get; private set; }

        public NonTerminal(string name)
            : this(string.Empty, name)
        {
            Name = name;
        }

        public NonTerminal(string @namespace, string name)
            : base(SymbolType.NonTerminal)
        {
            Namespace = @namespace;
            Name = name;

            // precompute to same time on property execution
            if(string.IsNullOrEmpty(@namespace))
                Value = Name;
            else 
                Value = $"{Namespace}.{Name}";
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            
            var nonTerminal = obj as INonTerminal;
            if ((object)nonTerminal == null)
                return false;

            return Value.Equals(nonTerminal.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}