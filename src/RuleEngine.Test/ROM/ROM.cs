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
    
    public class ROM
    {
        [Fact]
        public void TestEvaluateFunctionality()
        {
            RuleEngine.ROM rom = new RuleEngine.ROM();

            //facts and rules available to these text cases
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));

            RuleEngine.Evidence.Actions.ActionExpression a1 = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "3", 1);
            RuleEngine.Evidence.Actions.ActionExpression a2 = new RuleEngine.Evidence.Actions.ActionExpression("a2", "f2", "4", 1);
            RuleEngine.Evidence.Actions.ActionExecute a3 = new RuleEngine.Evidence.Actions.ActionExecute("a3", "R2", 1);

            System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier> actionList = new System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier>();
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a1"));
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a2"));
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a3"));

            RuleEngine.Evidence.Rule r1 = new RuleEngine.Evidence.Rule("R1", "f1==f1", actionList, 1, true);
            RuleEngine.Evidence.Rule r2 = new RuleEngine.Evidence.Rule("R2", "f1==f2", actionList, 1, false);

            rom.AddEvidence(f1);
            rom.AddEvidence(f2);
            rom.AddEvidence(a1);
            rom.AddEvidence(a2);
            rom.AddEvidence(a3);
            rom.AddEvidence(r1);
            rom.AddEvidence(r2);

            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            a1.IsEvaluatable = true;
            a2.IsEvaluatable = true;
            a3.IsEvaluatable = true;
            r1.IsEvaluatable = true;
            r2.IsEvaluatable = true;

            rom.Evaluate();

            Assert.Equal("3", f1.Value.ToString());
            Assert.Equal("4", f2.Value.ToString());
        }
        [Fact]
        public void TestCloneFunctionality()
        {
            RuleEngine.ROM rom = new RuleEngine.ROM();

            //facts and rules available to these text cases
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));

            RuleEngine.Evidence.Actions.ActionExpression a1 = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "3", 1);
            RuleEngine.Evidence.Actions.ActionExpression a2 = new RuleEngine.Evidence.Actions.ActionExpression("a2", "f2", "3", 1);
            RuleEngine.Evidence.Actions.ActionExecute a3 = new RuleEngine.Evidence.Actions.ActionExecute("a3", "R2", 1);

            System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier> actionList = new System.Collections.Generic.List<RuleEngine.Evidence.EvidenceSpecifier>();
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a1"));
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a2"));
            actionList.Add(new RuleEngine.Evidence.EvidenceSpecifier(true, "a3"));

            RuleEngine.Evidence.Rule r2 = new RuleEngine.Evidence.Rule("R2", "f1!=f2", actionList, 1, false);

            rom.AddEvidence(f1);
            rom.AddEvidence(f2);
            rom.AddEvidence(a1);
            rom.AddEvidence(a2);
            rom.AddEvidence(a3);
            rom.AddEvidence(r2);

            rom.AddDependentFact("f1", "R2");
            rom.AddDependentFact("f2", "R2");

            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            a1.IsEvaluatable = true;
            a2.IsEvaluatable = true;
            a3.IsEvaluatable = true;
            r2.IsEvaluatable = true;

            RuleEngine.ROM rom2 = (RuleEngine.ROM)rom.Clone();
            rom2.Evaluate();

            Assert.Equal("3", rom2["f1"].Value.ToString());
            Assert.Equal("3", rom2["f2"].Value.ToString());
            Assert.NotEqual(rom["f1"].Value.ToString(), rom2["f1"].Value.ToString());
        }
    }
}
