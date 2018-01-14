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
using RuleEngine.Evidence;

namespace RuleEngine
{
    public class ROM : ICloneable
    {
        #region instance variables
        /// <summary>
        /// collection of all evidence objects
        /// </summary>
        private Dictionary<string, IEvidence> evidenceCollection = new Dictionary<string, IEvidence>();
        /// <summary>
        /// specifies the models to be used
        /// </summary>
        private Dictionary<string, XmlDocument> models = new Dictionary<string, XmlDocument>();
        /// <summary>
        /// specifies for a given evidence all evidence thats dependent on it
        /// </summary>
        private Dictionary<string, List<string>> dependentEvidence = new Dictionary<string, List<string>>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, Delegate> callback = new Dictionary<string, Delegate>();
        #endregion
        #region constructor
        public ROM()
        {
        }
        #endregion
        #region core
        /// <summary>
        /// Add a model to the ROM. The name given to the model must match those specified in the ruleset.
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="model"></param>
        public void AddModel(string modelId, XmlDocument model)
        {
            //add model to collection
            models.Add(modelId, model);
        }

        /// <summary>
        /// Add a fact, action, or rule to the ROM.
        /// </summary>
        /// <param name="Action"></param>
        public void AddEvidence(IEvidence evidence)
        {
            //add evidence to collection
            evidenceCollection.Add(evidence.ID, evidence); 
        }

        /// <summary>
        /// specifies for a given evidence all evidence thats dependent on it
        /// </summary>
        /// <param name="evidence"></param>
        /// <param name="dependentEvidence"></param>
        public void AddDependentFact(string evidence, string dependentEvidence)
        {
            if (!this.dependentEvidence.ContainsKey(evidence))
                this.dependentEvidence.Add(evidence, new List<string>());
            this.dependentEvidence[evidence].Add(dependentEvidence);
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, IEvidence> Evidence
        {
            get
            {
                return evidenceCollection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEvidence this[string id]
        {
            get
            {
                try
                {
                    return evidenceCollection[id];
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                evidenceCollection[id] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Evaluate()
        {
            Decisions.Decision decision = (new Decisions.Decision());
            decision.EvidenceLookup += evidence_EvidenceLookup;
            decision.ModelLookup += evidence_ModelLookup;
            decision.Evaluate(evidenceCollection, dependentEvidence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            ROM rom = new ROM();
            rom.callback = new Dictionary<string, Delegate>(this.callback);

            rom.dependentEvidence = new Dictionary<string, List<string>>();
            foreach (string key in this.dependentEvidence.Keys)
            {
                rom.dependentEvidence.Add(key, new List<string>(this.dependentEvidence[key]));
            }

            rom.evidenceCollection = new Dictionary<string, IEvidence>();
            foreach (string key in this.evidenceCollection.Keys)
            {
                IEvidence evidence = (IEvidence)this.evidenceCollection[key].Clone();
                rom.evidenceCollection.Add(key, evidence);
            }

            rom.models = new Dictionary<string, XmlDocument>(this.models);

            return rom;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterCallback(string name, Delegate callback)
        {
            this.callback.Add(name, callback);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private IEvidence evidence_EvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            try
            {
                return evidenceCollection[args.Key];
            }
            catch (Exception e)
            {
                throw new Exception("Could not find evidence: " + args.Key, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private XmlNode evidence_ModelLookup(object sender, ModelLookupArgs args)
        {
            try
            {
                return models[args.Key];
            }
            catch
            {
                throw new Exception("Could not find model: " + args.Key);
            }
        }

        #endregion
    }
}