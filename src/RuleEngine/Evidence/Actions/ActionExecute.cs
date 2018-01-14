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

namespace RuleEngine.Evidence.Actions
{
    public class ActionExecute : AEvidence, IAction
    {
        #region instance variables
        protected string equation;
        private string operatingId;
        #endregion
        #region constructor
        public ActionExecute(string ID, string operatingId, int priority)
            : base(ID, priority)
        {
            this.operatingId = operatingId;
        }

        /// <summary>
        /// Constructor used by clone method. Do not use.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public ActionExecute()
        {
        }
        #endregion
        #region core
        protected override int CalculateInternalPriority(int priority)
        {
            return 1000 * 1000 * priority;
        }

        public override string[] ClauseEvidence
        {
            get 
            {
                return new String[1] {operatingId}; 
            }
        }
        public override void Evaluate()
        {
            if (!IsEvaluatable)
                throw new Exception("This action cannot currently be evaluated.");
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        public override object Clone()
        {
            ActionExecute f = (ActionExecute)base.Clone();
            f.equation = this.equation;
            f.operatingId = this.operatingId;
            return f;
        }

        #endregion
    }
}
