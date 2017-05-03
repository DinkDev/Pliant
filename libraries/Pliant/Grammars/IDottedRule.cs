﻿using System;

namespace Pliant.Grammars
{
    public interface IDottedRule : IComparable<IDottedRule>
    {
        int Position { get; }
        IProduction Production { get; }
    }
}