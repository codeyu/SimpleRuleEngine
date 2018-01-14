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
using System.Xml.XPath;

namespace RuleEngine.Evidence.EvidenceValue
{
    public class Xml : IEvidenceValue
    {
        #region instance variables
        private object previousValue=null;
        private string xPath;
        private Type valueType;
        private string modelId;
        public event ModelLookupHandler ModelLookup;
        public event ChangedHandler Changed;
        public event EvidenceLookupHandler EvidenceLookup;
        #endregion
        #region constructor
        //[System.Diagnostics.DebuggerHidden]
        public Xml(string xPath, Type valueType, string modelId)
        {
            if (modelId==null)
                throw new Exception("ModelId cannot be null.");
            if (xPath == null)
                throw new Exception("xPath cannot be null.");
            if (valueType==null)
                throw new Exception("valueType cannot be null.");

            this.modelId = modelId;
            this.xPath = xPath;
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
                return previousValue;
            }
            set
            {
                //if the incoming value is the previous value we have nothing to do
                if (previousValue != value)
                {
                    XmlNode model = null;
                    model = ModelLookup(this, new ModelLookupArgs(modelId));

                    setValue(model, value);
                    previousValue = value;
                    Changed(this, new ChangedModelArgs(modelId));
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
        /// Does the actual work of getting the value of the xpath expression.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        private object getValue(XmlNode model)
        {
		    //empty string is equivalent to null
		    if (xPath=="")
			    return null;

		    //XPathNavigator requires an xmldocument to operate on, a model is not required therefore we must supply an empty one
		    if (model==null)
			    model = new XmlDocument().DocumentElement;

		    //PERFORMANCE: big performance hit here, can we use compiled xpath?
		    //execute the xpath
		    object result=null;
            try
            {
                //Create XPathNavigator
                XPathNavigator xpathNav = model.CreateNavigator();

                //Compile the XPath expression
                XPathExpression xpathExpr = xpathNav.Compile(xPath);

                //Display the results depending on type of result
                switch (xpathExpr.ReturnType)
                {
                    case XPathResultType.Number:
                    case XPathResultType.Boolean:
                    case XPathResultType.String:
                        result = xpathNav.Evaluate(xpathExpr);
                        break;
                    case XPathResultType.NodeSet:
                        XPathNodeIterator nodeIter = xpathNav.Select(xpathExpr);
                        nodeIter.MoveNext();
                        result = ((IHasXmlNode)nodeIter.Current).GetNode();
                        break;
                    case XPathResultType.Error:
                        break;
                }
            }
            catch
            {
            }

            //returned nothing
            if (result == null) return null;

            //if its an xmlnode and the return type if of xmlnode
            if (result is XmlNode && valueType.IsAssignableFrom(typeof(XmlNode)))
                return result;
            else if (valueType.IsAssignableFrom(typeof(XmlNode)))
                throw new Exception("Type of XmlNode but its xpath expression did not return a node.");

            //if we have an xmlnode then then find its value
            if (result is XmlNode)
            {
                XmlNode node = (XmlNode)result;
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        result = node.Value;
                        break;
                    case XmlNodeType.Element:
                        result = node.InnerText;
                        break;
                    case XmlNodeType.Text:
                        result = node.Value;
                        break;
                    default:
                        throw new Exception(String.Format("Node type '{0}' not implemented", node.NodeType.ToString() ) );
                }
            }

            //cast the result to the expected type
            if (valueType == typeof(string))
            {
                return result.ToString();
            }
            else if (valueType == typeof(double))
            {
                if (result.ToString() == String.Empty) return 0; //no value means default value
                return Double.Parse(result.ToString());
            }
            else if (valueType == typeof(bool))
            {
                if (result.ToString() == String.Empty) return false; //no value means default value
                return Boolean.Parse(result.ToString());
            }
            else
            {
                throw new Exception("unsupported type: " + typeof(ValueType).ToString());
            }

        }

        /// <summary>
        /// Does the actual work of setting the value of the xpath expression.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        private void setValue(XmlNode model, object value)
        {
            try
            {
                //empty string is equivalent to null
                if (xPath == "" || model == null)
                    return;

                XmlNode y = null;
                try
                {
                    //suppress all events being processed or else we will end up in an infinite loop
                    y = model.SelectSingleNode(xPath);
                }
                catch(Exception e)
                {
                    //not setable
                    throw e;
                }

                if (valueType.IsAssignableFrom(typeof(XmlNode)))
                    throw new Exception("Type of XmlNode cannot be set.");


                switch (y.NodeType)
                {
                    case XmlNodeType.Element:
                        y.InnerText = value.ToString();
                        break;
                    case XmlNodeType.Attribute:
                        y.Value = value.ToString();
                        break;
                    case XmlNodeType.Text:
                        y.Value = value.ToString();
                        break;
                    default:
                        throw new Exception("Not a supported node type.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Invalid xPath: " + xPath, e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Evaluate()
        {
            XmlNode model = null;
            model = ModelLookup(this, new ModelLookupArgs(modelId));

            object x = getValue(model);

            if (x == null)
                throw new Exception("no value for fact");

            if (previousValue==null || !x.Equals(previousValue))
            {
                previousValue = x;
                Changed(this, new ChangedModelArgs(modelId));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        public object Clone()
        {
            Xml xml = new Xml(xPath, valueType, modelId);
            return xml;
        }

        /// <summary>
        /// 
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public void Reset()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string ModelId
        {
            get
            {
                return modelId;
            }
        }
        #endregion
    }
}
