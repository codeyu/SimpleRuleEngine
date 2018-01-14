using System;
using Xunit;
using System.Xml;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;

using RuleEngine;
using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace UnitTests
{
    
    public class ExpressionEvaluator
    {
        [Fact]
        public void TestEvaluateFunctionality()
        {
            //get a datareader stream from the xml file
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ConformanceLevel = ConformanceLevel.Document;
            xrs.IgnoreComments = true;
            xrs.IgnoreProcessingInstructions = true;
            xrs.IgnoreWhitespace = true;
            Stream s = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Expression\ExpressionEvaluator.xml").BaseStream;
            XmlReader reader = XmlReader.Create(s, xrs);

            //advance the reader to the first test element
            string comment="";
            string condition="";
            string result="";
            
            while (!reader.EOF)
            {
                Debug.Flush();

                if (reader.IsStartElement("Tests"))
                {
                    reader.Read();
                    continue;
                }

                if (reader.IsStartElement("Test"))
                {
                    comment = "";
                    condition = "";
                    result = "";
                    reader.Read();
                    continue;
                }

                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Comment":
                            comment = reader.ReadElementString();
                            break;
                        case "Condition":
                            condition = reader.ReadElementString();
                            break;
                        case "Result":
                            result = reader.ReadElementString();
                            break;
                    }
                    continue;
                }

                //run the test on test end element
                if (reader.Name == "Test" && !reader.IsStartElement())
                {
                    Debug.WriteLine("");
                    Debug.Write("Test Case: ");
                    Debug.WriteLine(comment);
                    Debug.WriteLine(condition);
                    Debug.WriteLine(result);

                    //facts and rules available to these text cases
                    RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("F1", 1, 2d, typeof(double));
                    RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("F2", 1, 4d, typeof(double));
                    RuleEngine.Evidence.Fact s1 = new RuleEngine.Evidence.Fact("String", 1, "String", typeof(string));
                    RuleEngine.Evidence.Fact in1 = new RuleEngine.Evidence.Fact("In", 1, 2d, typeof(double));
                    RuleEngine.Evidence.Fact out1 = new RuleEngine.Evidence.Fact("Out", 1, null, typeof(double));
                    f1.IsEvaluatable = true;
                    f2.IsEvaluatable = true;
                    s1.IsEvaluatable = true;

                    RuleEngine.Evidence.ExpressionEvaluator e = new RuleEngine.Evidence.ExpressionEvaluator();
                    e.Parse(condition);
                    e.InfixToPostfix();
                    e.GetEvidence += delegate(object source, EvidenceLookupArgs args)
                        {
                            if (args.Key == "F1")
                            {
                                return f1;
                            }
                            else if (args.Key == "F2")
                            {
                                return f2;
                            }
                            else if (args.Key == "String")
                            {
                                return s1;
                            }
                            else if (args.Key == "In")
                            {
                                return in1;
                            }
                            else if (args.Key == "Out")
                            {
                                return out1;
                            }
                            else
                                throw new Exception("Unknown evidence: " + args.Key);
                        };
                    RuleEngine.Evidence.ExpressionEvaluator.Symbol r = e.Evaluate();

                    //throw exception up stack if an error was present
                    Assert.False(r.type == RuleEngine.Evidence.ExpressionEvaluator.Type.Invalid && result != "Invalid");
                    Assert.Equal(result, r.value.Value.ToString().ToLower());

                    reader.Read();
                    continue;
                }

                //junk gos here
                reader.Read();
            }
        }
    }
}
