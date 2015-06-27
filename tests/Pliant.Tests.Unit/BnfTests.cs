﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Dfa;
using Pliant.Grammars;
using Pliant.Tokens;
using System.IO;
using System.Text;

namespace Pliant.Tests.Unit
{
    /// <summary>
    ///     <see cref="http://cui.unige.ch/db-research/Enseignement/analyseinfo/AboutBNF.html"/>
    /// </summary>
    [TestClass]
    public class BnfTests
    {
        public TestContext TestContext { get; set; }

        private string _bnfText = @"
            <syntax>         ::= <rule> | <rule> <syntax>
            <rule>           ::= <identifier> ""::="" <expression> <line-end>
            <expression>     ::= <list> | <list> ""|"" <expression>
            <line-end>       ::= <EOL> | <line-end> <line-end>
            <list>           ::= <term > | <term> <list>
            <term>           ::= <literal > | <identifier>
            <identifier>     ::= ""<"" <rule-name> "">""
            <literal>        ::= '""' <text> '""' | ""'"" <text> ""'""";

        [TestMethod]
        public void Test_Bnf_That_String_Iterate_Sets_Baseline()
        {
            var stringReader = new StringReader(_bnfText);
            int c = 0;
            while ((c = stringReader.Read()) != -1) ;                
        }

        [TestMethod]
        public void Test_Bnf_That_Parse_Produces_Bnf_Chart()
        {
            /*  A -> B | C
             *  equals
             *  A -> B
             *  A -> C
             *  
             *  A -> { B }
             *  equals
             *  A -> A B | <null>
             *  
             *  A -> [ B ]
             *  equals
             *  A -> B | <null>
             *  
             *  A -> B { B }
             *  equals
             *  A -> B A | B
             * 
             *  Grammar
             *  -------
             *  <syntax>         ::= <rule> | <rule> <syntax>
             *  <rule>           ::= "<" <rule-name> ">" "::=" <expression> <line-end>
             *  <expression>     ::= <list> | <list> "|" <expression>
             *  <line-end>       ::= <EOL> | <line-end> <line-end>
             *  <list>           ::= <term> | <term> <list>
             *  <term>           ::= <literal> | "<" <rule-name> ">"
             *  <literal>        ::= '"' <text> '"' | "'" <text> 
             */
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddEdge(new DfaEdge(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddEdge(new DfaEdge(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, new TokenType("whitespace"));

            var ruleNameState = new DfaState();
            var zeroOrMoreLetterOrDigit = new DfaState(true);
            ruleNameState.AddEdge(new DfaEdge(new RangeTerminal('a', 'z'), zeroOrMoreLetterOrDigit));
            ruleNameState.AddEdge(new DfaEdge(new RangeTerminal('A', 'Z'), zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddEdge(new DfaEdge(new RangeTerminal('a', 'z'), zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddEdge(new DfaEdge(new RangeTerminal('A', 'Z'), zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddEdge(new DfaEdge(new DigitTerminal(), zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddEdge(new DfaEdge(new SetTerminal('-', '_'), zeroOrMoreLetterOrDigit));
            var ruleName = new DfaLexerRule(ruleNameState, new TokenType("rule-name"));
            
            var implements = new StringLiteralLexerRule("::=", new TokenType("implements"));
            var eol = new StringLiteralLexerRule("\r\n", new TokenType("eol"));

            var grammarBuilder = new GrammarBuilder("syntax")
                .Production("syntax", r => r
                    .Rule("syntax")
                    .Rule("rule", "syntax"))
                .Production("rule", r=>r
                    .Rule("identifier", implements, "expression", "line-end"))
                .Production("expression", r=>r
                    .Rule("list")
                    .Rule("list", '|', "expression"))
                .Production("line-end", r=>r
                    .Rule(eol)
                    .Rule("line-end", "line-end"))
                .Production("list", r=>r
                    .Rule("term")
                    .Rule("term", "list"))
                .Production("term", r=>r
                    .Rule("literal")
                    .Rule("identifier"))
                .Production("identifier", r=>r
                    .Rule('<', ruleName, '>'))
                .Production("literal", r=>r
                    .Rule('"', "doubleQuoteText", '"')
                    .Rule('\'', "singleQuoteText", '\''))
                .Production("doubleQuoteText", r=>r
                    .Rule("doubleQuoteText", new NegationTerminal(new Terminal('"')))
                    .Lambda())
                .Production("singleQuoteText", r=>r
                    .Rule("singleQuoteText", new NegationTerminal(new Terminal('\'')))
                    .Lambda())
                .Ignore(whitespace);
            var grammar = grammarBuilder.ToGrammar();
            Assert.IsNotNull(grammar);

            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, _bnfText);
            var stringReader = new StringReader(_bnfText);
            
            while (!parseInterface.EndOfStream())
            {                 
                if (!parseInterface.Read())
                {
                    var position = parseInterface.Position;
                    var startIndex = 0;
                    for (int i = position; i >= 0; i--)
                    {
                        if (_bnfText[i] == '\n' && i > 0)
                            if (_bnfText[i - 1] == '\r')
                            {
                                startIndex = i;
                                break;
                            }
                    }
                    var endIndex = _bnfText.IndexOf("\r\n", position);
                    endIndex = endIndex < 0 ? _bnfText.Length : endIndex;
                    var length = endIndex - startIndex;
                    var stringBuilder = new StringBuilder();
                    stringBuilder
                        .AppendFormat("Error parsing input string at position {0}.", parseInterface.Position)
                        .AppendLine()
                        .AppendFormat("start: {0}", startIndex)
                        .AppendLine()
                        .AppendLine(_bnfText.Substring(startIndex, length));

                    Assert.Fail(stringBuilder.ToString());
                }
            }
        }
    }
}
