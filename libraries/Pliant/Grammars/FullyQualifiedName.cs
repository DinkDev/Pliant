﻿namespace Pliant.Grammars
{
    public class FullyQualifiedName
    {
        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string FullName { get; private set; }

        public FullyQualifiedName(string @namespace, string name)
        {
            Namespace = @namespace;
            Name = name;
            FullName = !string.IsNullOrWhiteSpace(@namespace)
                ? $"{@namespace}.{name}"
                : $"{name}";
        }

        public override string ToString()
        {
            return FullName;
        }

        public static bool operator ==(FullyQualifiedName fullyQualifiedName, string value)
        {
            return fullyQualifiedName.FullName.Equals(value);
        }

        public static bool operator !=(FullyQualifiedName fullyQualifiedName, string value)
        {
            return !fullyQualifiedName.FullName.Equals(value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var otherFullyQualifiedName = obj as FullyQualifiedName;
            if (otherFullyQualifiedName == null)
                return false;
            return otherFullyQualifiedName.FullName == FullName;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}