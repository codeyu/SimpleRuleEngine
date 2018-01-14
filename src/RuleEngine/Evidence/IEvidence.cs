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

using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Evidence
{
    public interface IEvidence : IComparable, ICloneable
    {
        event RuleEngine.ChangedHandler Changed;
        event RuleEngine.EvidenceLookupHandler EvidenceLookup;
        event RuleEngine.ModelLookupHandler ModelLookup;
        event RuleEngine.CallbackHandler CallbackLookup;

        /// <summary>
        /// ID of this IEvidence
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// Value of IRuleEngineComparable of this IEvidence
        /// </summary>
        object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Value of IRuleEngineComparable of this IEvidence
        /// </summary>
        Type ValueType
        {
            get;
        }

        /// <summary>
        /// Actual object of value
        /// </summary>
        IEvidenceValue ValueObject
        {
            get;
        }

        /// <summary>
        /// States whether or not this evidence can be evaluated
        /// </summary>
        bool IsEvaluatable
        {
            get;
            set;
        }

        /// <summary>
        /// States the priority of this evidence in processing. Lower numbers will be processed first.
        /// </summary>
        int Priority
        {
            get;
        }

        /// <summary>
        /// Returns the truthality of this IEvidence
        /// </summary>
        /// <returns></returns>
        void Evaluate();


        /// <summary>
        /// 
        /// </summary>
        string[] DependentEvidence
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string[] ClauseEvidence
        {
            get;
        }

    }
}
