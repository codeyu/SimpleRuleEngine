/*
Simple Rule Engine
Copyright (C) 2005 by Sierra Digital Solutions Corp

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Evidence
{
    public class Rule : AEvidence, IRule
    {
        #region IRule Members
        private bool chainable = false;
        protected string equation;
        private List<RuleEngine.Evidence.ExpressionEvaluator.Symbol> postfixExpression;
        private List<EvidenceSpecifier> actions;
        #endregion
        #region constructor
        public Rule(string ID, string equation, List<EvidenceSpecifier> actions, int priority, bool chainable)
            : base(ID, priority)
        {
            if (actions == null || actions.Count < 1)
                throw new Exception("Rules must have at least one action.");
            foreach (EvidenceSpecifier action in actions)
            {
                if (!action.truthality && chainable)
                    throw new Exception("Chainable rules are not allowed to contain actions whos result is false.");
            }

            this.actions = actions;
            this.chainable = chainable;
            this.equation = equation;
            ArrayList al = new ArrayList();
            foreach (EvidenceSpecifier es in actions)
            {
                al.Add(es.evidenceID);
            }
            this.clauseEvidence = (string[])al.ToArray(typeof(string));

            //this is expensive and static, so compute now
            ExpressionEvaluator e = new ExpressionEvaluator();
            e.Parse(equation); //this method is slow, do it only when needed
            e.InfixToPostfix(); //this method is slow, do it only when needed
            this.postfixExpression = e.Postfix; //this method is slow, do it only when needed

            //determine the dependent facts
            string[] dependents = ExpressionEvaluator.RelatedEvidence(e.Postfix);
            dependentEvidence = dependents;

            //change event could set its value when a model is attached
            Naked naked = new Naked(false, typeof(bool));
            base.EvidenceValue = naked;
        }
        /// <summary>
        /// Constructor used by clone method. Do not use.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public Rule()
        {
        }
        #endregion
        #region core
        protected override int CalculateInternalPriority(int priority)
        {
            if (isChainable)
                return priority;
            else
                return 1000 * priority;
        }

        /// <summary>
        /// 
        /// </summary>
        public override event ChangedHandler Changed
        {
            add
            {
                base.Changed += value;
            }
            remove
            {
                base.Changed -= value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override event EvidenceLookupHandler EvidenceLookup
        {
            add
            {
                base.EvidenceLookup += value;
            }
            remove
            {
                base.EvidenceLookup -= value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override event ModelLookupHandler ModelLookup
        {
            add
            {
                base.ModelLookup += value;
            }
            remove
            {
                base.ModelLookup -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public bool isChainable
        {
            get { return chainable; }
        }
        
        public override void Evaluate()
        {
            ExpressionEvaluator e = new ExpressionEvaluator();
            e.GetEvidence += new EvidenceLookupHandler(RaiseEvidenceLookup);
            e.Postfix = this.postfixExpression;
            ExpressionEvaluator.Symbol o = e.Evaluate(); //PERFORMANCE: this method is slow.

            base.EvidenceValue.Reset(); //clear previous values

            //result must be of this type or the expression is invalid, throw exception
            IEvidenceValue result = o.value as IEvidenceValue;

            //exit if null returned
            if (o.type == ExpressionEvaluator.Type.Invalid)
            {
                return;
            }
            
            //see if its value has changed, if so then set the value and call the events
            if (base.Value.Equals(result.Value))
                return; //no change in value, dont raise an event

            base.Value = result.Value;
            RaiseChanged(this, new ChangedArgs());
        }
        
        protected override IEvidence Value_EvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            return RaiseEvidenceLookup(this, args);
        }
        protected override void Value_Changed(object sender, ChangedArgs args)
        {
            RaiseChanged(this, args);
        }
        protected override XmlNode Value_ModelLookup(object sender, ModelLookupArgs e)
        {
            return RaiseModelLookup(this, e);
        }
        public override string[] ClauseEvidence
        {
            get
            {
                List<string> list = new List<string>();

                foreach (EvidenceSpecifier es in actions)
                {
                    if ((bool)base.Value == es.truthality)
                        list.Add(es.evidenceID);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        public override object Clone()
        {
            Rule f = (Rule)base.Clone();
            f.postfixExpression = new List<ExpressionEvaluator.Symbol>(this.postfixExpression);
            f.actions = new List<EvidenceSpecifier>(this.actions);
            return f;
        }
        #endregion

    }
}
