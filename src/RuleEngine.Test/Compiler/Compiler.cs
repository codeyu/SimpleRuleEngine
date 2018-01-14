using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.IO;

using Xunit;
using RuleEngine.Evidence;
using RuleEngine;

namespace UnitTests.Compiler
{
    
    public class Compiler
    {
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Fact]
        public void TestHarness()
        {
            //get a datareader stream from the xml file
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ConformanceLevel = ConformanceLevel.Document;
            xrs.IgnoreComments = true;
            xrs.IgnoreProcessingInstructions = true;
            xrs.IgnoreWhitespace = true;
            Stream s = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Compiler\Compiler.xml").BaseStream;
            XmlReader reader = XmlReader.Create(s, xrs);

            string comment;
            string ruleset;
            ArrayList model = null;
            RuleEngine.ROM rom = null;

            bool inTest = false;
            while(!reader.EOF)
            {
                if (reader.IsStartElement("test"))
                {
                    Debug.WriteLine("START TEST");
                    inTest = true;
                    model = new ArrayList();
                    reader.Read();
                }

                else if (inTest && reader.Name == "comment")
                {
                    comment = reader.ReadElementContentAsString();
                    Debug.WriteLine(comment);
                }

                else if (inTest && reader.Name == "ruleset")
                {
                    ruleset = reader.ReadElementContentAsString();
                    Debug.WriteLine(ruleset);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ruleset);
                    rom = RuleEngine.Compiler.Compiler.Compile(doc);
                }

                else if (inTest && reader.Name == "model")
                {
                    string mid = reader.GetAttribute("modelId");
                    string m = reader.ReadElementContentAsString();
                    XmlDocument mod = new XmlDocument();
                    mod.Load(m);
                    model.Add(mod);
                    rom.AddModel(mid, mod);
                }

                else if (inTest && reader.Name == "evaluate")
                {
                    //evaluate
                    Debug.WriteLine("Evaluate");
                    rom.Evaluate();
                    reader.Read();
                }

                else if (inTest && reader.Name=="assign")
                {
                    Debug.WriteLine("Assign");
                    string mid = reader.GetAttribute("factId");
                    string m = reader.ReadElementContentAsString();

                    object value;
                    //determine value type
                    switch (rom[mid].ValueType.ToString()) //deterrmine the type of value returned by xpath
                    {
                        case "System.Double":
                            value = Double.Parse(m);
                            break;
                        case "System.Boolean":
                            value = Boolean.Parse(m);
                            break;
                        case "System.String":
                            value = m;
                            break;
                        default:
                            throw new Exception("Invalid type: " + m );
                    }
                    rom[mid].Value = value;
                }

                else if (inTest && reader.Name == "result")
                {
                    Debug.WriteLine("Assert");
                    string mid = reader.GetAttribute("factId");
                    string m = reader.ReadElementContentAsString();

                    Assert.Equal(m, rom[mid].Value.ToString());
                }

                else if (inTest && reader.Name == "test")
                {
                    rom = null;
                    model = null;
                    comment = null;
                    ruleset = null;
                    inTest = false;
                    reader.Read();
                    Debug.WriteLine("END TEST");
                }
                else
                {
                    reader.Read();
                }
                
            }

        }
    }
}
