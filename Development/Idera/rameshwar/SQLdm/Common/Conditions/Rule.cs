//------------------------------------------------------------------------------
// <copyright file="Rule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Xml;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Snapshots;

    public enum Operators
    {
        eq,
        ge,
        gt,
        le,
        lt,
        ne
    }
    
    public class Rule
    {
        private delegate bool EvalServerNames(string server);
        private delegate bool EvalThresholdViolations(Snapshot snapshot);
        private delegate bool EvalStateChanges(Snapshot snapshot);
        private delegate bool EvalTimestamp(DateTime timestamp);

        private int id;
        private string name;
        private string ruleXml;
        private List<string> serverNames;
        private EvalThresholdViolations evalThresholdViolations;
        private EvalStateChanges evalStateChanges;

        public static Rule Load(string fileName)
        {
            string xml = File.ReadAllText(fileName);
            return new Rule(xml);
        }

        public Rule(string xml)
        {
            this.ruleXml = xml;
        }
        
        public string Xml
        {
            get { return ruleXml; }
            set
            {
                ruleXml = value;
                serverNames = null;
                evalThresholdViolations = null;
                evalStateChanges = null;
            }
        }

        public XmlDocument DOMTree
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Xml);
                return doc;
            }
            set
            {
                Xml = value.ToString();
            }
        }


        internal bool Evaluate(Snapshot snapshot)
        {
            if (serverNames == null && evalThresholdViolations == null && evalStateChanges == null)
                Compile();

            if (serverNames != null)
            {
                if (!Contains(snapshot.ServerName, serverNames))
                    return false;
            }
            if (evalThresholdViolations != null)
            {
                if (!evalThresholdViolations(snapshot))
                    return false;
            }
            if (evalStateChanges != null)
            {
                if (!evalStateChanges(snapshot))
                    return false;
            }
            return true;
        }

        public void Compile()
        {
            using (XmlTextReader reader = new XmlTextReader(ruleXml, XmlNodeType.Element, null))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "Rule")
                        {
                            string v = reader["id"];
                            id = v == null ? -1 : Int32.Parse(v);
                            v = reader["name"];
                            name = v == null ? "" : v;
                            continue;
                        }

                        if (reader.Name == "ServerName")
                        {
                            compileServerNameCode(reader);
                        }
                        else
                            if (reader.Name == "ThresholdViolation")
                            {
                                compileThresholdViolationCode(reader);
                            }
                            else
                                if (reader.Name == "StateChange")
                                {
                                    compileStateChangeCode(reader);
                                }
                        //                              else
                        //                                throw new InvalidDataException("Invalid element: " + reader.Name);
                    }
                }
            }
        }

        private bool Contains(string server, IList<string> values)
        {
            foreach (string value in values)
            {
                if (value.Equals(server, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private void compileStateChangeCode(XmlTextReader reader)
        {

        }

        private void compileThresholdViolationCode(XmlTextReader reader)
        {
            List<MetricComparison> comparisons = new List<MetricComparison>();
            bool readingValue = false;
            MetricComparison comparison = new MetricComparison();
            // build a list of all the values to check for
            for (bool result = true; result; reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    readingValue = false;
                    if (reader.Name == "ThresholdViolation")
                        continue;
                    if (reader.Name == "Metric")
                    {
                        comparison = new MetricComparison();
                        comparison.Metric = reader["name"];
                        if (String.IsNullOrEmpty(comparison.Metric))
                            throw new InvalidDataException("Metric name is blank or null");
                        comparison.Operation = reader["op"];
                        if (String.IsNullOrEmpty(comparison.Operation))
                            throw new InvalidDataException("Metric operation is blank or null");
                        readingValue = true;
                    }
                }
                else
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (readingValue)
                        {
                            try
                            {
                                comparison.Value = reader.ReadContentAsLong();
                                comparisons.Add(comparison);
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.Message);
                                throw new InvalidDataException("Metric comparison value is invalid", e);
                            }
                        }
                    }
                    else
                        if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            readingValue = false;
                            if (reader.Name == "ThresholdViolation")
                                break;
                        }
            }

            if (comparisons.Count == 0)
                return;

            Type[] getMetricValueParmTypes = { typeof(Snapshot), typeof(Metric) };
            MethodInfo getMetricValue = GetType().GetMethod("GetMetricValue", getMetricValueParmTypes);

            Type[] argTypes = { typeof(Rule), typeof(Snapshot) };
            DynamicMethod method = new DynamicMethod("EvalThresholdViolations", typeof(bool), argTypes, typeof(Rule));
            ILGenerator generator = method.GetILGenerator();

            // local var to store intermediate results
            generator.DeclareLocal(typeof(bool));   // local var 0

            Label exitLabel = generator.DefineLabel();

            // store false as the default result
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Stloc_0);

            foreach (MetricComparison comp in comparisons)
            {
                Metric metric;
                // convert the metric name to an enum value
                try
                {
                    metric = (Metric)Enum.Parse(typeof(Metric), comp.Metric);
                }
                catch (Exception)
                {
                    throw;
                }

                Operators op;
                // convert the comparison name to an enum value
                try
                {
                    op = (Operators)Enum.Parse(typeof(Operators), comp.Operation);
                }
                catch (Exception)
                {
                    throw;
                }

                // retrieve the value of the metric from the snapshot
                generator.Emit(OpCodes.Ldarg_0);                // push this
                generator.Emit(OpCodes.Ldarg_1);                // push snapshot
                generator.Emit(OpCodes.Ldc_I4, (int)metric);    // push metric value
                generator.EmitCall(OpCodes.Call, getMetricValue, null);

                // push the right side of the comparison
                generator.Emit(OpCodes.Ldc_I8, comp.Value);

                Label nextTest = generator.DefineLabel();
                // add comparison instruction
                switch (op)
                {
                    case Operators.eq:
                        generator.Emit(OpCodes.Beq, nextTest);
                        break;
                    case Operators.ge:
                        generator.Emit(OpCodes.Bge, nextTest);
                        break;
                    case Operators.gt:
                        generator.Emit(OpCodes.Bgt, nextTest);
                        break;
                    case Operators.le:
                        generator.Emit(OpCodes.Ble, nextTest);
                        break;
                    case Operators.lt:
                        generator.Emit(OpCodes.Blt, nextTest);
                        break;
                    case Operators.ne:
                        generator.Emit(OpCodes.Ceq);
                        generator.Emit(OpCodes.Brfalse, nextTest);
                        break;
                }
                generator.Emit(OpCodes.Br, exitLabel);
                generator.MarkLabel(nextTest);
            }
            // if we get here then all the tests were true so set the local result to true
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stloc_0);
            // set the location of the exit label
            generator.MarkLabel(exitLabel);

            // push the result
            generator.Emit(OpCodes.Ldloc_0);
            // return
            generator.Emit(OpCodes.Ret);

            evalThresholdViolations = (EvalThresholdViolations)method.CreateDelegate(typeof(EvalThresholdViolations), this);
        }

        public long GetMetricValue(Snapshot snapshot, Metric metric)
        {
            ScheduledRefresh refresh = snapshot as ScheduledRefresh;

            switch (metric)
            {
                case Metric.AgentServiceStatus:
                    return (long)(refresh.Server.AgentServiceStatus.HasValue ? (long)refresh.Server.AgentServiceStatus : 1L);
                case Metric.DtcServiceStatus:
                    return (long)(refresh.Server.DtcServiceStatus.HasValue ? (long)refresh.Server.DtcServiceStatus : 1L);
                case Metric.FullTextServiceStatus:
                    return (long)(refresh.Server.FullTextServiceStatus.HasValue ? (long)refresh.Server.FullTextServiceStatus : 1L);
                case Metric.SqlServiceStatus:
                    return (long)(refresh.Server.SqlServiceStatus.HasValue ? (long)refresh.Server.SqlServiceStatus : 1L);
                //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get the value for the new SQL services
                case Metric.SQLBrowserServiceStatus:
                    return (long)(refresh.Server.SQLBrowserServiceStatus.HasValue ? (long)refresh.Server.SQLBrowserServiceStatus : 1L);
                case Metric.SQLActiveDirectoryHelperServiceStatus:
                    return (long)(refresh.Server.SQLActiveDirectoryHelperServiceStatus.HasValue ? (long)refresh.Server.SQLActiveDirectoryHelperServiceStatus : 1L);
                //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get the value for the new SQL services
            }

            return 999;
        }

        private void compileServerNameCode(XmlTextReader reader)
        {
            serverNames = new List<string>();
            string op = "eq";
            bool readingValue = false;
            // build a list of all the values to check for
            for (bool result = true; result; reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    readingValue = false;
                    if (reader.Name == "ServerName")
                    {
                        op = reader["op"];
                        if (op == null)
                            op = "eq";
                        continue;
                    }
                    if (reader.Name == "Value")
                        readingValue = true;
                }
                else
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (readingValue)
                        {
                            string v = reader.Value;
                            if (!String.IsNullOrEmpty(v))
                            {
                                if (!serverNames.Contains(v))
                                    serverNames.Add(v);
                            }
                        }
                    }
                    else
                        if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            readingValue = false;
                            if (reader.Name == "ServerName")
                                break;
                        }
            }
        }

        internal struct MetricComparison
        {
            internal string Metric;
            internal string Operation;
            internal long Value;
        }

    }

}
