﻿using System.Collections.Generic;

namespace Pliant.Collections
{
    public class ReadWriteList<T> : List<T>, IReadOnlyList<T>
    {
        public ReadWriteList()
        {
        }

        public ReadWriteList(IList<T> list) : base(list)
        {
        }
    }
}