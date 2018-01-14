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
using System.Diagnostics;

using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Evidence.Actions
{
    public class ActionExpression : AEvidence, IAction
    {
        #region instance variables
        protected string equation;
        private ExpressionEvaluator evaluator;
        private string operatingId;
        #endregion
        #region constructor
        public ActionExpression(string ID, string operatingId, string equation, int priority) : base(ID, priority)
        {
            this.equation = equation;
            this.operatingId = operatingId;

            //determine the dependent facts
            ExpressionEvaluator e = new ExpressionEvaluator();
            e.Parse(equation); //this method is slow, do it only when needed

            string[] dependents = ExpressionEvaluator.RelatedEvidence(e.Infix);
            dependentEvidence = dependents;

            //assing a value
            Naked naked = new Naked(null, typeof(string));
            this.EvidenceValue = naked;

            //this is expensive and static, so compute now
            evaluator = new ExpressionEvaluator();
            evaluator.Parse(equation); //this method is slow, do it only when needed
            evaluator.InfixToPostfix(); //this method is slow, do it only when needed
            evaluator.GetEvidence += delegate(object sender, EvidenceLookupArgs args)
            {
                return RaiseEvidenceLookup(sender, args);
            };
        }
        
        /// <summary>
        /// Constructor used by clone method. Do not use.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public ActionExpression()
        {
        }
        #endregion
        #region core
        protected override int CalculateInternalPriority(int priority)
        {
            return 1000 * 1000 * priority;
        }

        public override void Evaluate()
        {
            if (!IsEvaluatable)
                throw new Exception("This action cannot currently be evaluated.");

            RuleEngine.Evidence.ExpressionEvaluator.Symbol result = evaluator.Evaluate();

            //throw exception up stack if an error was present
            if (result.type == ExpressionEvaluator.Type.Invalid)
            {
                throw new Exception(String.Format("Invalid expression for action:{0}, equation:{1}", ID, equation));
            }

            //get the fact that should be assigned too, throw exception if IFact is not returned
            IFact fact = RaiseEvidenceLookup(this, new EvidenceLookupArgs(operatingId)) as IFact;
            if (fact == null)
                throw new Exception(String.Format("operatingId was not of type IFact: {0}", operatingId));

            //set the value
            Trace.WriteLine( "FACT " + operatingId + "=" + result.value.Value.ToString());
            fact.Value = result.value.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override IEvidence Value_EvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            return RaiseEvidenceLookup(this, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void Value_Changed(object sender, ChangedArgs args)
        {
            RaiseChanged(this, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override XmlNode Value_ModelLookup(object sender, ModelLookupArgs e)
        {
            return RaiseModelLookup(this, e);
        }
        /// <summary>
        /// 
        /// </summary>
        public override string[] ClauseEvidence
        {
            get 
            { 
                return new String[1] {operatingId} ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        public override object Clone()
        {
            ActionExpression f = (ActionExpression)base.Clone();
            f.equation = this.equation;
            f.operatingId = this.operatingId;
            f.evaluator = this.evaluator;
            return f;
        }
        #endregion

    }
}
