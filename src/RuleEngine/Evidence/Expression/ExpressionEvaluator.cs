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

//TODO: 'true' and 'false' should always be of a boolean type


using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Evidence
{
    /// <summary>
    /// Summary description for ExpressionEvaluator2.
    /// </summary>
    public class ExpressionEvaluator
    {
        public event RuleEngine.EvidenceLookupHandler GetEvidence;

        #region RelatedEvidence
        public static string[] RelatedEvidence(List<Symbol> symbols)
        {
            ArrayList al = new ArrayList();
            foreach (Symbol symbol in symbols)
            {
                if (symbol.type != Type.Fact)
                    continue;

                al.Add(symbol.name);
            }
            return (string[])al.ToArray(typeof(string));
        }
        #endregion
        #region Evaluate
        #region instance varaibles
        private static readonly string LogicalRegEx = @"(\x29|\x28|>=|<=|!=|==|<|>|AND|OR|NOT|ISNULL|XOR|\x2b|\x2d|\x2a|\x2f)";
        protected double result = 0;
        protected List<Symbol> infix = new List<Symbol>();
        protected List<Symbol> postfix = new List<Symbol>();

        public enum Type
        {
            Fact,
            Value,
            Operator,
            Function,
            Result,
            OpenBracket,
            CloseBracket,
            Invalid //states the comparison could not be made and is invalid
        }
        public struct Symbol
        {
            public string name;
            public IEvidenceValue value;
            public Type type;
            public override string ToString()
            {
                return name;
            }
        }

        #endregion
        #region constructor
        public ExpressionEvaluator()
        {
        }

        #endregion
        #region core
        public List<Symbol> Infix
        {
            get
            {
                return new List<Symbol>(infix);
            }
            set
            {
                infix = value;
            }
        }
        public List<Symbol> Postfix
        {
            get
            {
                return new List<Symbol>(postfix);
            }
            set
            {
                postfix = value;
            }
        }

        #region parser
        public void Parse(string eq)
        {
            Debug.Write("Parsing to Infix: " + eq + " : ");

            //reset 
            infix.Clear();
            postfix.Clear();

            //tokinize string
            Regex regex = new Regex(LogicalRegEx);
            string[] rawTokins = regex.Split(eq);
            for (int x = 0; x < rawTokins.Length; x++)
            {
                string currentTokin = rawTokins[x].Trim();
                if (currentTokin == null || currentTokin == String.Empty) continue; //workaround: sometimes regex will bring back empty entries, skip these

                Symbol current = ParseToSymbol(currentTokin);

                //add the current to the collection
                infix.Add(current);
                Debug.Write(current.name + "|");
            }
            Debug.WriteLine("");
        }

        private Symbol ParseToSymbol(string s)
        {
            Symbol sym = new Symbol();
            if (IsOpenParanthesis(s))
            {
                sym.name = s;
                sym.type = Type.OpenBracket;
            }
            else if (IsCloseParanthesis(s))
            {
                sym.name = s;
                sym.type = Type.CloseBracket;
            }
            else if (IsFunction(s)) //isfunction must come b4 isvariable because its an exact match where the other isnt
            {
                sym.name = s;
                sym.type = Type.Function;
            }
            else if (IsOperator(s))
            {
                sym.name = s;
                sym.type = Type.Operator;
            }
            else if (IsBoolean(s))
            {
                Naked naked = new Naked(Boolean.Parse(s), typeof(bool));
                sym.name = s;
                sym.value = naked;
                sym.type = Type.Value;
            }
            else if (IsFact(s))
            {
                sym.name = s;
                sym.type = Type.Fact;
            }
            else if (IsNumber(s))
            {
                Naked naked = new Naked(Double.Parse(s), typeof(double));
                sym.name = s;
                sym.value = naked;
                sym.type = Type.Value;
            }
            else if (IsString(s))
            {
                Naked naked = new Naked(s.Substring(1, s.Length - 2), typeof(string));
                sym.name = s;
                sym.value = naked;
                sym.type = Type.Value;
            }
            else
            {
                //who knows what it is so throw an exception
                throw new Exception("Invalid tokin: " + s);
            }
            return sym;
        }

        private bool IsFact(string s)
        {
            if (s == null)
                return false;

            //variables must have the first digit as a letter and the remaining as numbers and letters
            bool result = true;
            if (!Char.IsLetter(s[0]))
            {
                result = false;
                return result;
            }

            foreach (char c in s.ToCharArray())
            {
                if (Char.IsLetter(c) || Char.IsNumber(c))
                    continue;

                result = false;
                break;
            }
            return result;
        }
        private bool IsBoolean(string s)
        {
            if (s != null && (s.ToLower() == "true" || s.ToLower() == "false"))
                return true;
            else
                return false;
        }
        private bool IsNumber(string s)
        {
            if (s == null)
                return false;

            //numbers must have all digits as numbers
            bool result = true;
            foreach (char c in s.ToCharArray())
            {
                if (Char.IsNumber(c))
                    continue;

                result = false;
                break;
            }
            return result;
        }
        private bool IsString(string s)
        {
            if (s == null)
                return false;

            bool result = false;
            if (s.StartsWith(@"""") && s.EndsWith(@""""))
                result = true;
            return result;
        }
        private bool IsOpenParanthesis(string s)
        {
            if (s == null)
                return false;

            //
            bool result = false;
            if (s == "(")
                result = true;

            return result;
        }
        private bool IsCloseParanthesis(string s)
        {
            if (s == null)
                return false;

            //
            bool result = false;
            if (s == ")")
                result = true;

            return result;
        }
        private bool IsOperator(string s)
        {
            if (s == null)
                return false;

            //must be an exact match
            bool result = false;
            switch (s)
            {
                case "+":
                case "-":
                case "/":
                case "*":
                case "^":
                case "==":
                case "!=":
                case ">=":
                case "<=":
                case ">":
                case "<":
                case "AND":
                case "OR":
                case "NOT":
                case "XOR":
                    result = true;
                    break;
            }
            return result;
        }
        private bool IsFunction(string s)
        {
            if (s == null)
                return false;

            //must be an exact match
            bool result = false;
            switch (s)
            {
                case "ISNULL":
                    result = true;
                    break;
            }
            return result;
        }
        #endregion
        #region infix to postfix
        public void InfixToPostfix()
        {
            Debug.Write("Parsing Infix to PostFix: ");

            postfix.Clear();
            Stack postfixStack = new Stack();
            foreach (Symbol s in infix)
            {
                if (s.type == Type.Value || s.type == Type.Fact)
                {
                    //push to result
                    Debug.Write(s.name + "|");
                    postfix.Add(s);
                }
                else if (s.type == Type.Operator || s.type == Type.Function)
                {

                    while (postfixStack.Count > 0 && !DeterminePrecidence(s, (Symbol)postfixStack.Peek()))
                    {
                        Debug.Write(((Symbol)postfixStack.Peek()).name + "|");
                        postfix.Add((Symbol)postfixStack.Pop());
                        
                    }
                    postfixStack.Push(s);

                }
                else if (s.type == Type.OpenBracket)
                {
                    postfixStack.Push(s);
                }
                else if (s.type == Type.CloseBracket)
                {
                    //pop off stack to '(', discard '('.
                    while (((Symbol)postfixStack.Peek()).type != Type.OpenBracket)
                    {
                        Debug.Write(((Symbol)postfixStack.Peek()).name + "|");
                        postfix.Add((Symbol)postfixStack.Pop());
                    }
                    postfixStack.Pop(); //discard '('
                }
                else
                {
                    throw new Exception("Illegal symbol: " + s.name);
                }
            }
            //now we pop off whats left on the stack
            while (postfixStack.Count > 0)
            {
                Debug.Write(((Symbol)postfixStack.Peek()).name + "|");
                postfix.Add((Symbol)postfixStack.Pop());
            }
            Debug.WriteLine("");
        }

        private bool DeterminePrecidence(Symbol higher, Symbol lower)
        {
            int s1 = Precidence(higher);
            int s2 = Precidence(lower);

            if (s1 > s2)
                return true;
            else
                return false;
        }

        private int Precidence(Symbol s)
        {
            int result = 0;

            switch (s.name)
            {
                case "*":
                case "/":
                case "%":
                    result = 32;
                    break;
                case "+":
                case "-":
                    result = 16;
                    break;
                case ">":
                case "<":
                case ">=":
                case "<=":
                    result = 8;
                    break;
                case "==":
                case "!=":
                    result = 4;
                    break;
                case "NOT":
                    result = 3;
                    break;
                case "AND":
                    result = 2;
                    break;
                case "XOR":
                case "OR":
                    result = 1;
                    break;
            }

            //functions have the highest priority
            if (s.type == Type.Function)
                result = 64;

            return result;
        }

        #endregion
        public Symbol Evaluate()
        {
            Stack operandStack = new Stack();

            foreach (Symbol s in postfix)
            {
                if (s.type == Type.Value)
                {
                    operandStack.Push(s);
                }
                else if (s.type == Type.Operator)
                {
                    Symbol op3 = new Symbol(); //result
                    Symbol op1;
                    Symbol op2;

                    switch (s.name)
                    {
                        case "+":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateAddition(op1, op2);
                            break;
                        case "-":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateSubtraction(op1, op2);
                            break;
                        case "*":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateMultiplication(op1, op2);
                            break;
                        case "/":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateDivision(op1, op2);
                            break;
                        case "==":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateEquals(op1, op2);
                            break;
                        case "!=":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateNEquals(op1, op2);
                            break;
                        case ">":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateGreaterThan(op1, op2);
                            break;
                        case "<":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateLessThan(op1, op2);
                            break;
                        case ">=":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateGreaterThanEqual(op1, op2);
                            break;
                        case "<=":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateLessThanEqual(op1, op2);
                            break;
                        case "AND":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateAnd(op1, op2);
                            break;
                        case "OR":
                            op2 = (Symbol)operandStack.Pop(); //this operation requires two parameters
                            op1 = (Symbol)operandStack.Pop();
                            op3 = EvaluateOr(op1, op2);
                            break;
                        case "NOT":
                            op1 = (Symbol)operandStack.Pop(); //this operation requires one parameters
                            op3 = EvaluateNot(op1);
                            break;
                        default:
                            throw new Exception(String.Format("Invalid operator: {0} of type", s.name, s.type));
                    }
                    operandStack.Push(op3);
                }
                else if (s.type == Type.Function)
                {
                    //Symbol[] parameters;
                    Symbol op3 = new Symbol();
                    op3.type = Type.Value;
                    //IEvidence fact;

                    switch (s.name)
                    {
                        case "ISNULL":
                            Symbol symbol = (Symbol)operandStack.Pop();

                            op3.value = new Naked(false, typeof(bool));

                            if (symbol.type == Type.Invalid || symbol.value.Value == null)
                            {
                                op3.value = new Naked(true, typeof(bool));
                            }

                            operandStack.Push(op3);
                            Debug.WriteLine(String.Format("ExpressionEvaluator ISNULL {0} = {1}", symbol.name, op3.value.Value));
                            break;

                        default:
                            throw new Exception(String.Format("Invalid function: {0} of type {1}", s.name, s.type));

                    }
                }
                else if (s.type == Type.Fact)
                {
                    Symbol op3 = new Symbol();
                    op3.type = Type.Value;
                    IEvidence fact;

                    fact = GetEvidence(this, new EvidenceLookupArgs(s.name));

                    op3.value = new Naked(fact.Value, fact.ValueType);
                    operandStack.Push(op3);
                    Debug.WriteLine(String.Format("ExpressionEvaluator FACT {0} = {1}", fact.ID, fact.Value));
                    continue;
                }
                else
                {
                    throw new Exception(String.Format("Invalid symbol type: {0} of type {1}", s.name, s.type));
                }
            }

            Symbol returnValue = (Symbol)operandStack.Pop();

            if (operandStack.Count > 0)
                throw new Exception("Invalid equation? OperandStack should have a count of zero.");
            return returnValue;
        }
        #region Evaluates
        private Symbol EvaluateAddition(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            object o1 = null;
            object o2 = null;

            object replacement;
            try
            {
                o1 = op1.value.Value;
                o2 = op2.value.Value;

                if (o1 is string || o2 is string)
                    replacement = o1.ToString() + o2.ToString();
                else if (o1 is double && o2 is double)
                    replacement = (double)o1 + (double)o2;
                else
                    throw new Exception("only to be caught");
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} + {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }

        private Symbol EvaluateSubtraction(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            object o1 = null;
            object o2 = null;

            object replacement;
            try
            {
                o1 = op1.value.Value;
                o2 = op2.value.Value;

                if (o1 is string || o2 is string)
                    throw new Exception("Cant subtract strings.");
                else if (o1 is double && o2 is double)
                    replacement = (double)o1 - (double)o2;
                else
                    throw new Exception("only to be caught");
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} - {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }

        private Symbol EvaluateMultiplication(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            object o1 = null;
            object o2 = null;

            object replacement;
            try
            {
                o1 = op1.value.Value;
                o2 = op2.value.Value;

                if (o1 is string || o2 is string)
                    throw new Exception("cant multiple strings");
                else if (o1 is double && o2 is double)
                    replacement = (double)o1 * (double)o2;
                else
                    throw new Exception("only to be caught");
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} * {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }

        private Symbol EvaluateDivision(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            object o1 = null;
            object o2 = null;

            object replacement;
            try
            {
                o1 = op1.value.Value;
                o2 = op2.value.Value;

                if (o1 is string || o2 is string)
                    throw new Exception("Cant divide strings");
                else if (o1 is double && o2 is double)
                    replacement = (double)o1 / (double)o2;
                else
                    throw new Exception("only to be caught");
            }
            catch
            {
                replacement = false;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} / {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }

        private Symbol EvaluateEquals(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            object o1 = null;
            object o2 = null;

            object replacement;
            try
            {
                o1 = op1.value.Value;
                o2 = op2.value.Value;

                bool result = o1.Equals(o2);
                replacement = result;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} == {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateNEquals(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            object o1 = null;
            object o2 = null;

            object replacement;
            try
            {
                o1 = op1.value.Value;
                o2 = op2.value.Value;

                bool result = !(o1.Equals(o2));
                replacement = result;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} != {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateAnd(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            bool b1 = false;
            bool b2 = false;

            //see if the facts are not equal
            object replacement;
            try
            {
                b1 = (bool)op1.value.Value;
                b2 = (bool)op2.value.Value;

                replacement = (b1 && b2);
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} AND {1} = {2}", b1, b2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateNot(Symbol op1)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            bool b1 = false;

            //see if the facts are not equal
            object replacement;
            try
            {
                b1 = (bool)op1.value.Value;

                replacement = (!b1);
            }
            catch //FUTURE: only catch specific errors and throw others up the stack
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator NOT {0} = {1}", b1, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateOr(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            bool b1 = false;
            bool b2 = false;

            //see if the facts are not equal
            object replacement;
            try
            {
                try
                {
                    b1 = (bool)op1.value.Value;
                }
                catch
                {
                }

                try
                {
                    b2 = (bool)op2.value.Value;
                }
                catch
                {
                }

                replacement = b1 || b2;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} OR {1} = {2}", b1, b2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateGreaterThan(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            IComparable o1 = null;
            IComparable o2 = null;

            //see if the facts are not equal
            object replacement;
            try
            {
                o1 = (IComparable)op1.value.Value;
                o2 = (IComparable)op2.value.Value;

                int result;
                result = o1.CompareTo(o2);

                if (result == 1)
                    replacement = true;
                else
                    replacement = false;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} > {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateLessThan(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            IComparable o1 = null;
            IComparable o2 = null;

            //see if the facts are not equal
            object replacement;
            try
            {
                o1 = (IComparable)op1.value.Value;
                o2 = (IComparable)op2.value.Value;

                int result;
                result = o1.CompareTo(o2);

                if (result == -1)
                    replacement = true;
                else
                    replacement = false;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionExaluator {0} < {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateGreaterThanEqual(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            IComparable o1 = null;
            IComparable o2 = null;

            //see if the facts are not equal
            object replacement;
            try
            {
                o1 = (IComparable)op1.value.Value;
                o2 = (IComparable)op2.value.Value;

                int result;
                result = o1.CompareTo(o2);

                if (result == 1 || result == 0)
                    replacement = true;
                else
                    replacement = false;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionEvaluator {0} >= {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        private Symbol EvaluateLessThanEqual(Symbol op1, Symbol op2)
        {
            Symbol op3 = new Symbol();
            op3.type = Type.Value;
            IComparable o1 = null;
            IComparable o2 = null;

            //see if the facts are not equal
            object replacement;
            try
            {
                o1 = (IComparable)op1.value.Value;
                o2 = (IComparable)op2.value.Value;

                int result;
                result = o1.CompareTo(o2);

                if (result == -1 || result == 0)
                    replacement = true;
                else
                    replacement = false;
            }
            catch
            {
                op3.type = Type.Invalid;
                op3.value = null;
                replacement = op3;
            }
            Debug.WriteLine(String.Format("ExpressionExaluator {0} <= {1} = {2}", o1, o2, replacement));

            op3.value = new Naked(replacement, typeof(bool));
            return op3;
        }
        #endregion

        #endregion
        #endregion


    }
}
