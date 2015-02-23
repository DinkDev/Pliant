﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public interface ITerminalBuilder
    {
        ITerminalBuilder Range(char start, char end);
        ITerminalBuilder Digit();
        ITerminalBuilder WhiteSpace();
    }
}
