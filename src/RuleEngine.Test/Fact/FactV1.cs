using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;

using Xunit;
using RuleEngine.Evidence;
using RuleEngine;

namespace UnitTests
{
    
    public class FactV1
    {
        #region internal
        private bool changed = false;
        private XmlNode model1;
        private XmlNode model2;
        private EventArgs lastEventArgs;
        private object lastSource;

        public void Changed(object source, ChangedArgs args)
        {
            changed = true;
        }
        public XmlNode ModelLookup(object source, ModelLookupArgs args)
        {
            if (((ModelLookupArgs)args).Key == "1.xml")
                return model1;
            else if (((ModelLookupArgs)args).Key == "2.xml")
                return model2;
            else
                throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
        }
        public IEvidence EvidenceLookup(object source, EvidenceLookupArgs args)
        {
            return null;
        }
        public FactV1()
        {
        }
        #endregion
        #region strings
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Fact]
        public void string1()
        {
            //init variables
            string modelname = "1.xml";
            changed = false;

            RuleEngine.Evidence.Fact f = new RuleEngine.Evidence.Fact("f1", 1, "/root/person/firstname/text()", typeof(string), modelname);
            f.Changed += Changed;
            f.ModelLookup += ModelLookup;
            f.EvidenceLookup += EvidenceLookup;
            f.IsEvaluatable = true;
            f.Evaluate();

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("Joe", (string)f.Value);
        }
        /// <summary>
        /// Confirm we can change the first name as text element
        /// </summary>
        [Fact]
        public void string2()
        {
            //init variables
            string modelname = "1.xml";
            changed = false;

            RuleEngine.Evidence.Fact f = new RuleEngine.Evidence.Fact("f1", 1, "/root/person/firstname/text()", typeof(string), modelname);
            f.Changed += Changed;
            f.ModelLookup += ModelLookup;
            f.EvidenceLookup += EvidenceLookup;
            f.IsEvaluatable = true;
            f.Evaluate();

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("Joe", (string)f.Value);

            //change the model, dont update this xpath expression.
            changed = false;
            model1["person"]["firstname"].InnerText = "bob";
            f.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("bob", (string)f.Value);
        }
        /// <summary>
        /// Confirm that a fact can retrieve a value off an arbitrary nodeset and different facts can point to different models
        /// </summary>
        [Fact]
        public void string3()
        {
            string modelname1 = "1.xml";
            string modelname2 = "2.xml";
            changed = false;

            RuleEngine.Evidence.Fact b = new RuleEngine.Evidence.Fact("f2", 1, "/root/address", typeof(XmlNode), modelname1);
            b.Changed += Changed;
            b.ModelLookup += ModelLookup;
            b.EvidenceLookup += EvidenceLookup;
            b.IsEvaluatable = true;
            b.Evaluate();

            RuleEngine.Evidence.Fact f = new RuleEngine.Evidence.Fact("f1", 1, "@pobox", typeof(bool), modelname2);
            f.Changed += Changed;
            f.ModelLookup += ModelLookup;
            f.EvidenceLookup += EvidenceLookup;
            f.IsEvaluatable = true;
            f.Evaluate();

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname1);
            model1 = doc.DocumentElement;
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname2);
            model2 = doc["root"]["person"]["address"];
            f.Evaluate();
            b.Evaluate();

            Assert.Equal(true, changed);
            Assert.True(b.Value is XmlNode);

            //change the model, update this xpath expression.
            changed = false;
            f.Value = true;

            Assert.Equal(true, changed);
            Assert.True(Boolean.Parse(model2.Attributes["pobox"].Value));
            Assert.False(Boolean.Parse(model1["person"]["address"].Attributes["pobox"].Value));
        }
        /// <summary>
        /// Confirm that a fact can retrieve a value off an arbitrary nodeset and different facts can point to same models
        /// </summary>
        [Fact]
        public void string4()
        {
            //init variables
            string modelname1 = "1.xml";
            string modelname2 = "2.xml";
            changed = false;

            RuleEngine.Evidence.Fact b = new RuleEngine.Evidence.Fact("f1", 1, "/root/address", typeof(XmlNode), modelname1);
            b.Changed += Changed;
            b.ModelLookup += ModelLookup;
            b.EvidenceLookup += EvidenceLookup;
            b.IsEvaluatable = true;
            b.Evaluate();

            RuleEngine.Evidence.Fact f = new RuleEngine.Evidence.Fact("f1", 1, "@pobox", typeof(bool), modelname2);
            f.Changed += Changed;
            f.ModelLookup += ModelLookup;
            f.EvidenceLookup += EvidenceLookup;
            f.IsEvaluatable = true;
            f.Evaluate();

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname1);
            model1 = doc.DocumentElement;
            model2 = doc["root"]["person"]["address"];
            f.Evaluate();
            b.Evaluate();

            Assert.Equal(true, changed);
            Assert.True(b.Value is XmlNode);

            //change the model, update this xpath expression.
            changed = false;
            f.Value = true;

            Assert.Equal(true, changed);
            Assert.True(Boolean.Parse(model2.Attributes["pobox"].Value));
            Assert.True(Boolean.Parse(model1["person"]["address"].Attributes["pobox"].Value));
        }

        /// <summary>
        /// Confirm that cloning method returns a clone of fact and xml
        /// </summary>
        [Fact]
        public void string6()
        {
            string modelname = "1.xml";
            changed = false;

            RuleEngine.Evidence.Fact f = new RuleEngine.Evidence.Fact("f1", 1, "/root/person/firstname/text()", typeof(string), modelname);
            f.Changed += Changed;
            f.ModelLookup += ModelLookup;
            f.EvidenceLookup += EvidenceLookup;
            f.IsEvaluatable = true;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            //clone
            RuleEngine.Evidence.Fact a = (RuleEngine.Evidence.Fact)f.Clone();
            a.Changed += Changed;
            a.ModelLookup += ModelLookup;
            a.EvidenceLookup += EvidenceLookup;
            a.IsEvaluatable = true;

            model1["person"]["firstname"].InnerText = "Bob";
            a.Evaluate();

            Assert.True((string)f.Value == "Joe");
            Assert.True((string)a.Value == "Bob");
            Assert.True(changed == true);
        }

        /// <summary>
        /// Confirm that cloning method returns a clone of fact and naked
        /// </summary>
        [Fact]
        public void string7()
        {
            //init variables
            string modelname = "1.xml";
            changed = false;

            RuleEngine.Evidence.Fact f = new RuleEngine.Evidence.Fact("f1", 1, "Joe", typeof(string));
            f.Changed += Changed;
            f.ModelLookup += ModelLookup;
            f.EvidenceLookup += EvidenceLookup;
            f.IsEvaluatable = true;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            //clone
            RuleEngine.Evidence.Fact a = (RuleEngine.Evidence.Fact)f.Clone();
            a.Changed += Changed;
            a.ModelLookup += ModelLookup;
            a.EvidenceLookup += EvidenceLookup;
            a.IsEvaluatable = true;

            a.Value = "Bob";
            a.Evaluate();
            f.Evaluate();

            Assert.True((string)f.Value == "Joe");
            Assert.True((string)a.Value == "Bob");
            Assert.True(changed == true);
        }
        #endregion
    }
}
