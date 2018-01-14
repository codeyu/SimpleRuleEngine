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

namespace RuleEngine.Evidence.EvidenceValue
{
    public interface IEvidenceValue : ICloneable
    {
        /// <summary>
        /// Value of this object.
        /// </summary>
        /// <returns></returns>
        object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Type of this value
        /// </summary>
        Type ValueType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        void Evaluate();

        /// <summary>
        /// Resets object to original state.
        /// </summary>
        void Reset();

        /// <summary>
        /// 
        /// </summary>
        string ModelId
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        event ModelLookupHandler ModelLookup;

        /// <summary>
        /// 
        /// </summary>
        event ChangedHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        event EvidenceLookupHandler EvidenceLookup;
    }
}
