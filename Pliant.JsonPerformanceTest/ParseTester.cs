﻿using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Runtime;

namespace Pliant.JsonPerformanceTest
{
    public class ParseTester
    {
        public IGrammar Grammar { get; private set; }
        public IParseEngine ParseEngine { get; private set; }
        public IParseRunner ParseRunner { get; private set; }

        public ParseTester(GrammarExpression expression)
            : this(expression.ToGrammar())
        {
        }

        public ParseTester(IGrammar grammar)
        {
            Grammar = grammar;
            ParseEngine = new ParseEngine(Grammar);
        }

        public ParseTester(IParseEngine parseEngine)
        {
            Grammar = parseEngine.Grammar;
            ParseEngine = parseEngine;
        }

        public void RunParse(string input)
        {
            ParseRunner = new ParseRunner(ParseEngine, input);
            InternalRunParse(ParseRunner);
        }

        public void RunParse(TextReader reader)
        {
            ParseRunner = new ParseRunner(ParseEngine, reader);
            InternalRunParse(ParseRunner);
        }

        private static void InternalRunParse(IParseRunner parseRunner)
        {
            while (!parseRunner.EndOfStream())
            {
                Assert.IsTrue(parseRunner.Read(), $"Parse Failed at Position {parseRunner.Position}");
            }
            Assert.IsTrue(parseRunner.ParseEngine.IsAccepted(), $"Parse was not accepted");
        }
    }
}