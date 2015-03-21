﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for PulseRecognizerTests
    /// </summary>
    [TestClass]
    public class PulseRecognizerTests
    {

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Test_PulseRecognizer_That_Single_Token_Increments_Parse()
        {
            //  S   -> W
            //  W   -> [\s]+
            var grammar = new GrammarBuilder("S", p=>p
                .Production("S", r=>r
                    .Rule("whitespace`"))
                .Production("whitespace`", r=>r
                    .Rule(new WhitespaceTerminal(), "whitespace"))
                .Production("whitespace", r=>r
                    .Rule(new WhitespaceTerminal())
                    .Lambda()))
                .GetGrammar();
            var pulseRecognizer = new PulseRecognizer(grammar);

            const string input = "\t\f\r\n ";
            int i = 0;
            while (i < pulseRecognizer.Chart.Count)
            {
                var c = i==input.Length ? (char)0 : input[i];
                pulseRecognizer.Pulse(c);
                i++;
            }
            Assert.IsTrue(pulseRecognizer.IsAccepted());
        }

        [TestMethod]
        public void Test_PulseRecognizer_That_Ambiguous_Right_Recursive_Is_ReWritten()
        {
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("S", "L")
                        .Lambda())
                    .Production("L", r => r
                        .Rule(new RangeTerminal('a', 'z'), "L`"))
                    .Production("L`", r => r
                        .Rule(new RangeTerminal('a', 'z'), "L`")
                        .Lambda()))
                .GetGrammar();
            var input = "thisisonelonginputstring";
            var recognizer = new PulseRecognizer(grammar);
            foreach (var c in input)
            {
                recognizer.Pulse(c);
            }
            
            Assert.IsTrue(recognizer.IsAccepted());
            // when this count is < 10 we know that quasi complete items are being processed successfully
            Assert.IsTrue(recognizer.Chart.Earlemes[23].Completions.Count < 10);
        }

        [TestMethod]
        public void Test_PulseRecognizer_That_Right_Recursion_Is_Not_O_N_3()
        {
            var grammar = new GrammarBuilder("A", p => p
                .Production("A", r => r
                    .Rule('a', "A")
                    .Lambda()))
            .GetGrammar();

            const string input = "aaaaa";
            var recognizer = new PulseRecognizer(grammar);
            foreach (var c in input)
                if (!recognizer.Pulse(c))
                    break;
            
            Assert.IsTrue(recognizer.IsAccepted());
            
            var chart = recognizer.Chart;
            // -- 0 --
            // A ->.a A		    (0)	 # Start
            // A ->.			(0)	 # Start
            //
            // ...
            // -- n --
            // n	A -> a.A		(n-1)	 # Scan a
            // n	A ->.a A		(n)	 # Predict
            // n	A ->.			(n)	 # Predict
            // n	A -> a A.		(n)	 # Predict
            // n	A : A -> a A.	(0)	 # Transition
            // n	A -> a A.		(0)	 # Complete
            Assert.AreEqual(input.Length + 1, chart.Count);
            var lastEarleme = chart.Earlemes[chart.Earlemes.Count - 1];
            Assert.AreEqual(3, lastEarleme.Completions.Count);
            Assert.AreEqual(1, lastEarleme.Transitions.Count);
            Assert.AreEqual(1, lastEarleme.Predictions.Count);
            Assert.AreEqual(1, lastEarleme.Scans.Count);
        }        
    }
}
