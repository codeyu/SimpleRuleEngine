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

using RuleEngine;
using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;
using RuleEngine.Evidence.Actions;

namespace RuleEngine.Compiler
{ 
    public class Compiler
    {
        public static ROM Compile(XmlDocument document)
        {
            //TODO: validate against schema
            ROM rom = CreateRom();
            LoadFacts(rom, document);
            LoadRules(rom, document);
            FactRelationship(rom, document);
            return rom;
        }

        private static ROM CreateRom()
        {
            ROM rom = new ROM();
            return rom;
        }

        private static void LoadFacts(ROM rom, XmlDocument document)
        {
            XmlNodeList facts = document.SelectNodes("RuleEngine/Facts//Fact");

            foreach (XmlNode factNode in facts)
            {
                string id = factNode.Attributes["id"].Value;
                string type = factNode.Attributes["type"].Value;
                string desc = factNode.Attributes["desc"].Value;
                string modelId = factNode.Attributes["modelId"].Value;
                Type valueType = null;

                //ensure same rule wont be entered twice
                if (rom[id]!=null)
                    throw new Exception("Fact has already been loaded: " + id);

                //determine priority
                int priority = 500;
                if (factNode.Attributes["priority"]!=null)
                    priority = Int32.Parse(factNode.Attributes["priority"].Value);


                IFact fact = null;
                if (factNode["xpath"].InnerText != String.Empty)
                {
                    //determine xpath
                    string xpath = factNode["xpath"].InnerText;

                    //determine value type
                    switch (type) //deterrmine the type of value returned by xpath
                    {
                        case "double":
                            valueType = typeof(double);
                            break;
                        case "boolean":
                            valueType = typeof(bool);
                            break;
                        case "string":
                            valueType = typeof(string);
                            break;
                        default:
                            throw new Exception("Invalid type: " + id + " " + type);
                    }

                    //determine value
                    Xml x = new Xml(xpath, valueType, modelId);

                    //create fact and add it to the rom
                    fact = new Fact(id, priority, xpath, valueType, modelId);
                }
                else
                {
/*
                    //determine value type
                    switch (type) //deterrmine the type of value returned by xpath
                    {
                        case "double":
                            valueType = typeof(double);
                            break;
                        case "boolean":
                            valueType = typeof(bool);
                            break;
                        case "string":
                            valueType = typeof(string);
                            break;
                        default:
                            throw new Exception("Invalid type: " + id + " " + type);
                    }

                    //determine value
                    Naked x = new Naked(value, valueType);

                    //create fact and add it to the rom
                    fact = new Fact(id, priority, valueType,);
 */
                }


                rom.AddEvidence(fact);
            }
        }

        private static void LoadRules(ROM rom, XmlDocument document)
        {
            XmlNodeList rules = document.SelectNodes("RuleEngine/Rules//Rule");

            foreach (XmlNode ruleNode in rules)
            {
                string id = ruleNode.Attributes["id"].Value;
                bool inference = false;
                if (ruleNode.Attributes["chainable"] != null)
                    inference = Boolean.Parse(ruleNode.Attributes["chainable"].Value);
                int priority = 500;
                if (ruleNode.Attributes["priority"] != null)
                    priority = Int32.Parse(ruleNode.Attributes["priority"].Value);

                //ensure this has not already been entered
                if (rom[id] != null)
                    throw new Exception("Rule Id's must be unique: " + id);

                //expression
                string condition = ruleNode["Condition"].InnerText;

                //actions
                int actionCounter = 0;
                List<EvidenceSpecifier> actions = new List<EvidenceSpecifier>();
                XmlNodeList actionList;
                #region Evaluate
                actionList = ruleNode.SelectNodes("Actions//Evaluate");
                foreach (XmlNode a in actionList)
                {
                    try
                    {
                        string actionOperatingName = a.Attributes["factId"].Value;
                        string aValue = a.InnerText;
                        bool result = true;
                        int actionPriority = 500;
                        if (a.Attributes["priority"] != null)
                            actionPriority = Int32.Parse(a.Attributes["priority"].Value);
                        if (a.Attributes["result"] != null)
                            result = Boolean.Parse(a.Attributes["result"].Value);

                        string actionId = id + "-" + actionOperatingName + "-" + actionCounter++;
                        rom.AddEvidence(new ActionExpression(actionId, actionOperatingName, aValue, actionPriority));
                        actions.Add(new EvidenceSpecifier(result, actionId));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid action: " + a.OuterXml, e);
                    }
                }
                #endregion
                #region Execute
                actionList = ruleNode.SelectNodes("Actions//Execute");
                foreach (XmlNode a in actionList)
                {
                    try
                    {
                        string actionOperatingName = a.Attributes["factId"].Value;
                        bool result = true;
                        int actionPriority = 500;
                        if (a.Attributes["priority"] != null)
                            actionPriority = Int32.Parse(a.Attributes["priority"].Value);
                        if (a.Attributes["result"] != null)
                            result = Boolean.Parse(a.Attributes["result"].Value);

                        string actionId = id + "-" + actionOperatingName + "-" + actionCounter++;
                        rom.AddEvidence(new ActionExecute(actionId, actionOperatingName, actionPriority));
                        actions.Add(new EvidenceSpecifier(result, actionId));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid action: " + a.OuterXml, e);
                    }
                }
                #endregion

                //now create the rule
                IRule r = new Rule(id, condition, actions, priority, inference);
                rom.AddEvidence(r);
            }
        }

        private static void LoadActions(ROM rom, XmlDocument document)
        {
        }

        private static void FactRelationship(ROM rom, XmlDocument doc)
        {
            //go though each chainable rule and determine who they are dependent on
            foreach(IEvidence evidence in rom.Evidence.Values)
            {
                if (!(evidence is IRule))
                    continue;

                IRule rule = (IRule)evidence;

                if (!rule.isChainable)
                    continue;

                foreach (string depFacts in rule.DependentEvidence)
                {
                    rom.AddDependentFact(depFacts, rule.ID);
                }
            }
        }

    }
}
