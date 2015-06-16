﻿using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Lexemes
{
    public class TerminalLexeme : ILexeme
    {
        public ITerminal Terminal { get; private set; }

        public string Capture { get; private set; }

        public TokenType TokenType { get; private set; }

        public TerminalLexeme(ITerminal terminal, TokenType tokenType)
        {
            Terminal = terminal;
            TokenType = tokenType;
            Capture = string.Empty;
        }

        public TerminalLexeme(ITerminalLexerRule terminalRule)
            : this(terminalRule.Terminal, terminalRule.TokenType)
        {
        }

        public bool IsAccepted()
        {
            return Capture.Length > 0;
        }

        public bool Scan(char c)
        {
            if (!IsAccepted())
            {
                if (Terminal.IsMatch(c))
                {
                    Capture = c.ToString();
                    return true;
                }
            }
            return false;
        }
    }
}