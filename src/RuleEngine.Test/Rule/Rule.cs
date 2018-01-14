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
    
    public class Rule
    {
        #region instance variables
        private RuleEngine.Evidence.Actions.ActionExpression a1;
        private RuleEngine.Evidence.Actions.ActionExpression a2;
        private RuleEngine.Evidence.Fact f1;
        private RuleEngine.Evidence.Fact f2;
        private RuleEngine.Evidence.Rule r1;
        private RuleEngine.Evidence.Rule r2;

        private IEvidence EvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            if (args.Key == "f1")
            {
                return f1;
            }
            else if (args.Key == "f2")
            {
                return f2;
            }
            else if (args.Key == "a1")
            {
                return a1;
            }
            else if (args.Key == "a2")
            {
                return a2;
            }
            else if (args.Key == "r1")
            {
                return r1;
            }
            else if (args.Key == "r2")
            {
                return r2;
            }
            else
                throw new Exception("Unknown evidence");
        }
        private XmlNode ModelLookup(object sender, ModelLookupArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void CreateFact1()
        {
            f1 = new RuleEngine.Evidence.Fact("f1", 1,2d, typeof(double));
            f1.EvidenceLookup += new EvidenceLookupHandler(EvidenceLookup);
            f1.IsEvaluatable = true;
            f1.Evaluate();
        }
        private void CreateFact2()
        {
            f2 = new RuleEngine.Evidence.Fact("f2", 1, 3d, typeof(double));
            f2.EvidenceLookup += new EvidenceLookupHandler(EvidenceLookup);
            f2.IsEvaluatable = true;
            f2.Evaluate();
        }
        private void CreateActionA1()
        {
            a1 = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f2", "5", 1);
            a1.EvidenceLookup += new EvidenceLookupHandler(EvidenceLookup);
            a1.IsEvaluatable = true;
        }
        private void CreateActionA2()
        {
            a2 = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f2", "6", 1);
            a2.EvidenceLookup += new EvidenceLookupHandler(EvidenceLookup);
            a2.IsEvaluatable = true;
        }
        private System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier> CreateActionList()
        {
            System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier> actionList = new System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier>();
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a1"));
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a2"));
            return actionList;
        }
        private void CreateFact1(System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier> actionList)
        {
            r1 = new RuleEngine.Evidence.Rule("r1", "f1==f1", actionList, 1, true);
            r1.EvidenceLookup += new EvidenceLookupHandler(EvidenceLookup);
            r1.IsEvaluatable = true;
            r1.Evaluate();
        }
        #endregion
        #region constructor
        public Rule()
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
            CreateFact1();
            CreateFact2();
            CreateActionA1();
            CreateActionA2();
            CreateFact1(CreateActionList());
            Assert.Equal("true", (string)r1.Value.ToString().ToLower());
        }
        #endregion
    }
}
