using System;
using Xunit;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;

using RuleEngine.Evidence;
using RuleEngine;

namespace UnitTests
{
    
    public class ActionExpression
    {
        #region internal
        public ActionExpression()
        {
        }
        #endregion
        #region strings
        /// <summary>
        /// Confirm we can write a new value to a double
        /// </summary>
        [Fact]
        public void double1()
        {
            //init variables
            string modelname = "1.xml";
            bool changed = false;
            XmlNode model1 = null;
            XmlNode model2 = null;
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));
            #region delegates
            f1.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f1.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f1.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            #region delegates
            f2.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f2.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f2.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            f1.Evaluate();
            f2.Evaluate();

            RuleEngine.Evidence.Actions.ActionExpression f = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "3", 1);
            #region delegates
            f.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f.IsEvaluatable = true;

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.Equal(true, changed);
            Assert.True(f1.Value is double);
            Assert.Equal(3, (double)f1.Value);
        }
        /// <summary>
        /// Confirm we can read from one fact and write its value to another.
        /// </summary>
        [Fact]
        public void double2()
        {
            //init variables
            string modelname = "1.xml";
            bool changed = false;
            XmlNode model1 = null;
            XmlNode model2 = null;
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));
            #region delegates
            f1.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f1.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f1.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            #region delegates
            f2.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f2.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f2.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            f1.Evaluate();
            f2.Evaluate();

            RuleEngine.Evidence.Actions.ActionExpression f = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "f2", 1);
            #region delegates
            f.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f.IsEvaluatable = true;

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.Equal(true, changed);
            Assert.True(f1.Value is double);
            Assert.Equal(4, (double)f1.Value);
        }
        /// <summary>
        /// Confirm we can read from one fact, add one, and write its value to another.
        /// </summary>
        [Fact]
        public void double3()
        {
            //init variables
            string modelname = "1.xml";
            bool changed = false;
            XmlNode model1 = null;
            XmlNode model2 = null;
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));
            #region delegates
            f1.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f1.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f1.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            #region delegates
            f2.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f2.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f2.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            f1.Evaluate();
            f2.Evaluate();

            RuleEngine.Evidence.Actions.ActionExpression f = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "f2+1", 1);
            #region delegates
            f.Changed += delegate(object source, ChangedArgs args)
            {
                changed = true;
            };
            f.ModelLookup += delegate(object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f.EvidenceLookup += delegate(object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f.IsEvaluatable = true;

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.Equal(true, changed);
            Assert.True(f1.Value is double);
            Assert.Equal(5, (double)f1.Value);
        }
        #endregion
    }
}
