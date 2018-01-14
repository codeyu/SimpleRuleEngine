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
using RuleEngine.Evidence;
using System.Collections;

namespace RuleEngine.Decisions
{
    public class ExecutionList
    {
        //List of evidences in order to run
        ArrayList list = new ArrayList();

        #region IComparer
        private class Comparer : IComparer
        {
            public int Compare(object x, object y)
            { //the higher the number the closer to the beginning of the collections
                int a = ((Item)x).priority;
                int b = ((Item)y).priority;

                if (a < b) return 1;
                else if (a > b) return -1;
                else return 0;
            }
        }
        private struct Item
        {
            public int priority;
            public string evidenceId;

            public Item(int priority, string evidenceId)
            {
                this.priority = priority;
                this.evidenceId = evidenceId;
            }
        }
        #endregion

        public ExecutionList()
        {
        }

        /// <summary>
        /// Reads the next evidence in the collection
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            Item e = (Item)list[0];
            list.RemoveAt(0);
            return e.evidenceId;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasNext
        {
            get
            {
                return (list.Count > 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evidence"></param>
        public void Add(IEvidence evidence)
        {
            //ensure this evidence has not already been added
            foreach(object value in list)
            {
                Item i = (Item)value;
                if (i.evidenceId == evidence.ID)
                    return;
            }

            //make the evidence evaluatable
            evidence.IsEvaluatable = true;

            //add it to the list
            list.Add(new Item(evidence.Priority, evidence.ID));
            list.Sort(new RuleEngine.Decisions.ExecutionList.Comparer());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(list.Count * 3);
            foreach (Item item in list.ToArray(typeof(Item)))
            {
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append(item.evidenceId);
            }

            return sb.ToString();
        }
    }
}
