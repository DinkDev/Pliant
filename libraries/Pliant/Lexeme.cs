﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    /// <summary>
    /// A Lexeme is something special. It acts like a token and a mini parser.
    /// </summary>
    public class Lexeme : IState
    {
        private StringBuilder _catpure;
        private PulseRecognizer _recognizer;
        private ILexerRule _lexerRule;
        
        public IDottedRule DottedRule { get; private set; }

        public IProduction Production
        {
            get { throw new NotImplementedException(); }
        }

        public int Origin
        {
            get { throw new NotImplementedException(); }
        }

        public StateType StateType
        {
            get { throw new NotImplementedException(); }
        }

        public IState NextState()
        {
            throw new NotImplementedException();
        }

        public IState NextState(int newOrigin)
        {
            throw new NotImplementedException();
        }

        public IState Parent
        {
            get { throw new NotImplementedException(); }
        }
    }
}
