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
    /// <summary>
    /// Comparable object that has no reference to the model
    /// </summary>
    public class Naked : IEvidenceValue
    {
        #region instance variables
        private object value;
        private Type valueType;
        private event ModelLookupHandler modelLookup;
        private event ChangedHandler changed;
        private event EvidenceLookupHandler evidenceLookup;
        #endregion
        #region constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <param name="parent"></param>
        ////[System.Diagnostics.DebuggerHidden]
        public Naked(object value, Type valueType)
        {
            this.value = value;
            this.valueType = valueType;
        }
        #endregion
        #region core
        /// <summary>
        /// 
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value==null || !this.value.Equals(value))
                {
                    this.value = value;
                    changed(this, new ChangedArgs());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public Type ValueType
        {
            get 
            {
                return valueType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Evaluate()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        public object Clone()
        {
            Naked xml = new Naked(value, valueType);
            return xml;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual event ModelLookupHandler ModelLookup
        {
            add
            {
                modelLookup = value;
            }
            remove
            {
                modelLookup = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual event ChangedHandler Changed
        {
            add
            {
                changed = value;
            }
            remove
            {
                changed = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual event EvidenceLookupHandler EvidenceLookup
        {
            add
            {
                evidenceLookup = value;
            }
            remove
            {
                evidenceLookup = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ModelId
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
