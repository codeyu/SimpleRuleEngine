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
using System.ComponentModel;
using System.Text;
using System.Xml;

using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Evidence
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AEvidence : IEvidence
    {
        #region instance variables
        private string id;
        private IEvidenceValue value;
        private bool isEvaluatable;
        private event ModelLookupHandler modelLookup;
        private event ChangedHandler changed;
        private event EvidenceLookupHandler evidenceLookup;
        private event CallbackHandler callbackLookup;

        protected string[] dependentEvidence=null;
        protected string[] clauseEvidence = null;
        private int priority=0;

        #endregion
        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="parent"></param>
        //[System.Diagnostics.DebuggerHidden]
        public AEvidence(string ID, int priority)
        {
            if (priority >= 1000 || priority <= 0)
                throw new Exception("Evidence priority must be greater than 0 and less than 1000. It was: " + priority);
            priority = CalculateInternalPriority(priority); //the evidence implementing this class must determine its true priority with respect to evidences

            this.id = ID;
            this.priority = priority;
        }

        /// <summary>
        /// Constructor. For use by clone method, must override and make public.
        /// </summary>
        protected AEvidence()
        {
        }
        #endregion
        #region core
        #region events
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
        public virtual event CallbackHandler CallbackLookup
        {
            add
            {
                callbackLookup = value;
            }
            remove
            {
                callbackLookup = null;
            }
        }
        #endregion

        protected abstract int CalculateInternalPriority(int priority);

        /// <summary>
        /// Identified of the evidence
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public string ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Contains the IRuleEngineComparable object that contains the value for this evidence
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public virtual object Value
        {
            get
            {
                if (!isEvaluatable)
                    throw new Exception("This fact currently is not evaluatable, it has no value: " + this.ID);
                return value.Value;
            }
            set
            {
                if (!isEvaluatable)
                    throw new Exception("This fact currently is not evaluatable, it has no value: " + this.ID);
                this.value.Value = value;
            }
        }

        /// <summary>
        /// Contains the IRuleEngineComparable object that contains the value for this evidence
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public virtual Type ValueType
        {
            get
            {
                return value.ValueType;
            }
        }

        public IEvidenceValue ValueObject
        {
            get { return value; }
        }

        /// <summary>
        /// IRuleEngineComparable thats responsible for the value
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        internal IEvidenceValue EvidenceValue
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;

                this.value.ModelLookup += Value_ModelLookup;
                this.value.Changed += Value_Changed;
                this.value.EvidenceLookup += Value_EvidenceLookup;
            }
        }

        /// <summary>
        /// Who this evidence is dependent on. Changes to these evidences could cause this evidence to be evaluated.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public string[] DependentEvidence
        {
            get
            {
                if (dependentEvidence == null)
                    return null;
                return dependentEvidence;
            }
        }

        /// <summary>
        /// Evidence, typically Actions, that are the clause statement to its evaluated conditional.
        /// Does not have a value until its been evaluated.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public abstract string[] ClauseEvidence
        {
            get;
        }

        /// <summary>
        /// Executes and sets truthality of evidence. Will clear previous value for new value.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public abstract void Evaluate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //TODO: why do we have this?
        //[System.Diagnostics.DebuggerHidden]
        public virtual int CompareTo(object obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ////[System.Diagnostics.DebuggerHidden]
        //[System.Diagnostics.DebuggerHidden]
        public virtual object Clone()
        {
            Type t = this.GetType();
            AEvidence x = (AEvidence)Activator.CreateInstance(t);
            x.id = id;
            x.priority = priority;
            if (dependentEvidence!=null)
                x.dependentEvidence = (string[])dependentEvidence.Clone();
            if (clauseEvidence!=null)
                x.clauseEvidence = (string[])clauseEvidence.Clone();
            if (EvidenceValue!=null)
                x.EvidenceValue = (IEvidenceValue)EvidenceValue.Clone();
            return x;
        }

        /// <summary>
        /// Whether or not this evidence is evaluatable or even has a value.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public bool IsEvaluatable
        {
            get
            {
                return isEvaluatable;
            }
            set
            {
                isEvaluatable = value;
                if (value && this.value!=null)
                {
                    this.value.Reset();
                }
            }
        }

        /// <summary>
        /// Priority of this evidence over other ones. lower value means it will be executed before higher values.
        /// </summary>
        public int Priority
        {
            get { return priority; }
        }

        #region eventhandlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual void RaiseChanged(object sender, ChangedArgs args)
        {
            if (changed!=null)
                changed(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual XmlNode RaiseModelLookup(object sender, ModelLookupArgs args)
        {
            //must always have a model lookup if one is needed
            return modelLookup(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual IEvidence RaiseEvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            //must always have an evidence lookup if one is needed.
            return evidenceLookup(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual void RaiseCallback(object sender, CallbackArgs args)
        {
            callbackLookup(sender, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract IEvidence Value_EvidenceLookup(object sender, EvidenceLookupArgs args);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected abstract void Value_Changed(object sender, ChangedArgs args);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected abstract XmlNode Value_ModelLookup(object sender, ModelLookupArgs e);
        #endregion
        #endregion

        #region IEvidence Members

        #endregion
    }

}
