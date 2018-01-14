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
using System.Text;
using System.Xml;

namespace RuleEngine.Evidence.Actions
{
    public class ActionCallback : AEvidence, IAction
    {
        #region instance variables
        protected string equation;
        private string operatingId;
        private string callback;
        #endregion
        #region constructor
        public ActionCallback(string ID, string operatingId, int priority, string callback)
            : base(ID, priority)
        {
            this.operatingId = operatingId;
            this.callback = callback;
            //TODO: do the callback here
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
                return null; 
            }
        }
        public override void Evaluate()
        {
            if (!IsEvaluatable)
                throw new Exception("This action cannot currently be evaluated.");

            RaiseCallback(this, new CallbackArgs(callback));
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
        #endregion
    }
}
