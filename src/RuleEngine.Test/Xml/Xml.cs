using System;
using Xunit;
using System.Xml;
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
    
    public class Xml
    {
        #region internal
        private string modelname;
        private XmlNode model;
        private bool changed;

        public void Changed(object source, ChangedArgs args)
        {
            changed = true;
        }
        public XmlNode ModelLookup(object source, ModelLookupArgs args)
        {
            return model;
        }
        public IEvidence EvidenceLookup(object source, EvidenceLookupArgs args)
        {
            return null;
        }
        public Xml()
        {
        }
        #endregion
        #region string
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Fact]
        public void string1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load( AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname );
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal("Joe", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as text element
        /// </summary>
        [Fact]
        public void string2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();
            
            //change the value and see if it was updated
            changed = false;
            x.Value = "Bob";

            Assert.Equal("Bob", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read the first name as element into string
        /// </summary>
        [Fact]
        public void string3()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal("Joe", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as element from string
        /// </summary>
        [Fact]
        public void string4()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = "Bob";

            Assert.Equal("Bob", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into string
        /// </summary>
        [Fact]
        public void string5()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal("true", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from string
        /// </summary>
        [Fact]
        public void string6()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = "false";

            Assert.Equal("false", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into string
        /// </summary>
        [Fact]
        public void string7()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal("true", x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from string
        /// </summary>
        [Fact]
        public void string8()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();
 
            //change the value and see if it was updated
            changed = false;
            x.Value = "false";

            Assert.Equal("false", x.Value);
            Assert.Equal(true, changed);
        }
        #endregion
        #region double
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Fact]
        public void double1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/text()", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(456, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as text element
        /// </summary>
        [Fact]
        public void double2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/text()", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = 999;

            Assert.Equal(999, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read the first name as element into double
        /// </summary>
        [Fact]
        public void double3()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(456, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as element from double
        /// </summary>
        [Fact]
        public void double4()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();
 
            //change the value and see if it was updated
            changed = false;
            x.Value = 111;

            Assert.Equal(111, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into double
        /// </summary>
        [Fact]
        public void double5()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/@id", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(123, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from double
        /// </summary>
        [Fact]
        public void double6()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/@id", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = 234;

            Assert.Equal(234, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into double
        /// </summary>
        [Fact]
        public void double7()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/attribute::node()[name(.)='id']", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(123, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from double
        /// </summary>
        [Fact]
        public void double8()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/attribute::node()[name(.)='id']", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = 555;

            Assert.Equal(555, x.Value);
            Assert.Equal(true, changed);
        }
        #endregion
        #region boolean
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Fact]
        public void boolean1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(true, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as text element
        /// </summary>
        [Fact]
        public void boolean2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = false;

            Assert.Equal(false, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can read the first name as element into double
        /// </summary>
        [Fact]
        public void boolean3()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/dis", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(true, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as element from double
        /// </summary>
        [Fact]
        public void boolean4()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/dis", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = false;

            Assert.Equal(false, x.Value);
            Assert.Equal(true, changed);
        }


        /// <summary>
        /// Confirm we can read the first name as attribute into boolean
        /// </summary>
        [Fact]
        public void boolean5()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal(true, x.Value);
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as attribute from boolean
        /// </summary>
        [Fact]
        public void boolean6()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = false;

            Assert.Equal(false, x.Value);
            Assert.Equal(true, changed);
        }
        #endregion
        #region xmlnode
        /// <summary>
        /// Confirm we can read the first name as node
        /// </summary>
        [Fact]
        public void node1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address", typeof(XmlNode), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal("address", ((XmlNode)x.Value).Name );
            Assert.Equal(true, changed);
        }

        /// <summary>
        /// Confirm we cant write the first name as node
        /// </summary>
        [Fact]
        public void node2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address", typeof(XmlNode), modelname);
            RuleEngine.Evidence.EvidenceValue.Xml y = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/dis", typeof(XmlNode), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            y.Changed += Changed;
            y.ModelLookup += ModelLookup;
            x.Evaluate();
            y.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = y.Value;
            Assert.Equal("dis", ((XmlNode)x.Value).Name );
            Assert.Equal(true, changed);
        }
        #endregion
        #region model updates
        /// <summary>
        /// Confirm no change event was sent if the field was not updated
        /// </summary>
        [Fact]
        public void modelupdate1()
        {
            //init variables
            modelname = "xml.xml";
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            x.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("Joe", (string)x.Value);

            //change the model, dont update this xpath expression.
            changed = false;
            model["person"]["address"].InnerText = "bob";
            x.Evaluate();

            Assert.Equal(false, changed);
            Assert.Equal("Joe", (string)x.Value);
        }
        /// <summary>
        /// Confirm change event was sent if the field was updated
        /// </summary>
        [Fact]
        public void modelupdate2()
        {
            //init variables
            modelname = "xml.xml";
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            x.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("Joe", (string)x.Value);

            //change the model, update this xpath expression.
            changed = false;
            model["person"]["firstname"].InnerText = "bob";
            x.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("bob", (string)x.Value);
        }
        /// <summary>
        /// Confirm that when a model changes due to xml.value being set that other xml objects can get the new value too
        /// </summary>
        [Fact]
        public void modelUpdate3()
        {
            //init variables
            modelname = "xml.xml";
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;

            RuleEngine.Evidence.EvidenceValue.Xml y = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            y.Changed += Changed;
            y.ModelLookup += ModelLookup;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            x.Evaluate();
            y.Evaluate();
            changed = false;

            x.Value = "JOEJOE";

            Assert.True((string)x.Value == "JOEJOE");
            Assert.True((string)y.Value != "JOEJOE");

            if (changed)
            {
                x.Evaluate();
                y.Evaluate();
            }

            Assert.True((string)x.Value == "JOEJOE");
            Assert.True((string)y.Value == "JOEJOE");
        }
        #endregion
        #region clone
        [Fact]
        public void clone1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.Equal("Joe", x.Value);
            Assert.Equal(true, changed);
            
            //clone
            RuleEngine.Evidence.EvidenceValue.Xml y = (RuleEngine.Evidence.EvidenceValue.Xml)x.Clone();
            y.Changed += Changed;
            y.ModelLookup += ModelLookup;

            //change the model, update this xpath expression.
            changed = false;
            model["person"]["firstname"].InnerText = "bob";
            y.Evaluate();

            Assert.Equal(true, changed);
            Assert.Equal("bob", (string)y.Value);
        }
        #endregion
    }
}
