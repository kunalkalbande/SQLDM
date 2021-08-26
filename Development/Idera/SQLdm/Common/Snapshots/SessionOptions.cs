//------------------------------------------------------------------------------
// <copyright file="SessionOptions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Represents the options that may be set on a session
    /// </summary>
    [Serializable]
    public sealed class SessionOptions : ISerializable
    {
        #region constants

        private const int AnsiNullDefaultOffBit = 1073741824;
        private const int AnsiNullDefaultOnBit = 536870912;
        private const int ArithabortBit = 64;
        private const int ArithignoreBit = 16;
        private const int FmtonlyBit = 16777216;
        private const int ForceplanBit = 8388608;
        private const int NocountBit = 2097152;
        private const int NoexecBit = 8;
        private const int NumericRoundabortBit = 32768;
        private const int ParseOnlyBit = 16384;
        private const int PartialResultsBit = 262144;
        private const int ProcIdOnBit = 65536;
        private const int QuotedIdentifierBit = 134217728;
        private const int ShowplanBit = 4;
        private const int StatisticsBit = 8192;
        private const int TraceFlagOnBit = 4194304;
        private const int XactAbortBit = 131072;

        private const string comma = ", ";
        private const string on = " On";
        private const string off = " Off";

        private const string AnsiDefaultsString = "Ansi Defaults";
        private const string AnsiNullDefaultOffString = "Ansi Null Default";
        private const string AnsiNullDefaultOnString = "Ansi Null Default";
        private const string AnsiNullsString = "Ansi Nulls";
        private const string AnsiPaddingString = "Ansi Padding";
        private const string AnsiWarningsString = "Ansi Warnings";
        private const string ArithabortString = "Arithabort";
        private const string ArithignoreString = "Arithignore";
        private const string ConcatNullYieldsNullString = "Concat Null Yields Null";
        private const string FmtonlyString = "Fmtonly";
        private const string ForceplanString = "Forceplan";
        private const string NocountString = "Nocount";
        private const string NoexecString = "Noexec";
        private const string NumericRoundabortString = "Numeric Roundabort";
        private const string ParseOnlyString = "Parse Only";
        private const string PartialResultsString = "RowCount <> 0(Partial Results)";
        private const string ProcIdOnString = "ProcID";
        private const string QuotedIdentifierString = "Quoted Identifier";
        private const string ShowplanString = "Showplan";
        private const string StatisticsString = "Statistics";
        private const string TraceFlagOnString = "Trace Flag";
        private const string XactAbortString = "Transaction Abort";

        #endregion

        #region fields

        private bool? ansiDefaults = null;
        private bool? ansiNullDefaultOff = null;
        private bool? ansiNullDefaultOn = null;
        private bool? ansiNulls = null;
        private bool? ansiPadding = null;
        private bool? ansiWarnings = null;
        private bool? arithabort = null;
        private bool? arithignore = null;
        private bool? concatNullYieldsNull = null;
        private bool? fmtonly = null;
        private bool? forceplan = null;
        private bool? nocount = null;
        private bool? noexec = null;
        private bool? numericRoundabort = null;
        private bool? parseOnly = null;
        private bool? partialResults = null;
        private bool? procIdOn = null;
        private bool? quotedIdentifier = null;
        private bool? showplan = null;
        private bool? statistics = null;
        private int statusMask = 0;
        private bool? traceFlagOn = null;
        private bool? xactAbort = null;

        #endregion

        #region constructors

        public SessionOptions() { }

        public SessionOptions(SerializationInfo info, StreamingContext context)
        {
            ansiDefaults = (bool?)info.GetValue("ansiDefaults", typeof(bool?));
            ansiNullDefaultOff = (bool?)info.GetValue("ansiNullDefaultOff", typeof(bool?));
            ansiNullDefaultOn = (bool?)info.GetValue("ansiNullDefaultOn", typeof(bool?));
            ansiNulls = (bool?)info.GetValue("ansiNulls", typeof(bool?));
            ansiPadding = (bool?)info.GetValue("ansiPadding", typeof(bool?));
            ansiWarnings = (bool?)info.GetValue("ansiWarnings", typeof(bool?));
            arithabort = (bool?)info.GetValue("arithabort", typeof(bool?));
            arithignore = (bool?)info.GetValue("arithignore", typeof(bool?));
            concatNullYieldsNull = (bool?)info.GetValue("concatNullYieldsNull", typeof(bool?));
            fmtonly = (bool?)info.GetValue("fmtonly", typeof(bool?));
            forceplan = (bool?)info.GetValue("forceplan", typeof(bool?));
            nocount = (bool?)info.GetValue("nocount", typeof(bool?));
            noexec = (bool?)info.GetValue("noexec", typeof(bool?));
            numericRoundabort = (bool?)info.GetValue("numericRoundabort", typeof(bool?));
            parseOnly = (bool?)info.GetValue("parseOnly", typeof(bool?));
            partialResults = (bool?)info.GetValue("partialResults", typeof(bool?));
            procIdOn = (bool?)info.GetValue("procIdOn", typeof(bool?));
            quotedIdentifier = (bool?)info.GetValue("quotedIdentifier", typeof(bool?));
            showplan = (bool?)info.GetValue("showplan", typeof(bool?));
            statistics = (bool?)info.GetValue("statistics", typeof(bool?));
            statusMask = info.GetInt32("statusMask");
            traceFlagOn = (bool?)info.GetValue("traceFlagOn", typeof(bool?));
            xactAbort = (bool?)info.GetValue("xactAbort", typeof(bool?));
        }

        #endregion

        #region properties

        /// <summary>
        /// Collectively set several ANSI settings
        /// </summary>
        public bool? AnsiDefaults
        {
            get { return ansiDefaults; }
            internal set { ansiDefaults = value; }
        }


        /// <summary>
        /// When on, new columns created by Alter Table and Create Table are not nullable
        /// </summary>
        public bool? AnsiNullDefaultOff
        {
            get { return ansiNullDefaultOff; }
            internal set { ansiNullDefaultOff = value; }
        }

        /// <summary>
        /// When on, new columns created by Alter Table and Create Table are nullable
        /// </summary>
        public bool? AnsiNullDefaultOn
        {
            get { return ansiNullDefaultOn; }
            internal set { ansiNullDefaultOn = value; }
        }

        /// <summary>
        /// Use SQL-92 nullability equivalence rules
        /// </summary>
        public bool? AnsiNulls
        {
            get { return ansiNulls; }
            internal set { ansiNulls = value; }
        }

        /// <summary>
        /// Controls the way columns store values shorter than the defined size of the column
        /// </summary>
        public bool? AnsiPadding
        {
            get { return ansiPadding; }
            internal set { ansiPadding = value; }
        }

        /// <summary>
        /// Controls whether warnings appear in aggregates when null values are present
        /// </summary>
        public bool? AnsiWarnings
        {
            get { return ansiWarnings; }
            internal set { ansiWarnings = value; }
        }

        /// <summary>
        /// Terminates a query when an overflow or divide-by-zero error occurs during query execution.
        /// </summary>
        public bool? Arithabort
        {
            get { return arithabort; }
            internal set { arithabort = value; }
        }

        /// <summary>
        /// Controls whether error messages are returned from overflow or divide-by-zero errors during a query.
        /// </summary>
        public bool? Arithignore
        {
            get { return arithignore; }
            internal set { arithignore = value; }
        }

        /// <summary>
        /// Controls whether concatenation results are treated as null or empty string values.
        /// </summary>
        public bool? ConcatNullYieldsNull
        {
            get { return concatNullYieldsNull; }
            internal set { concatNullYieldsNull = value; }
        }

        /// <summary>
        /// Returns only metadata to the client.
        /// </summary>
        public bool? Fmtonly
        {
            get { return fmtonly; }
            internal set { fmtonly = value; }
        }

        /// <summary>
        /// Makes the Microsoft SQL Server query optimizer process a join in the same order as tables appear in the FROM clause of a SELECT statement only.
        /// </summary>
        public bool? Forceplan
        {
            get { return forceplan; }
            internal set { forceplan = value; }
        }

        /// <summary>
        /// Stops the message that shows the number of rows affected by a Transact-SQL statement from being returned as part of the results.
        /// </summary>
        public bool? Nocount
        {
            get { return nocount; }
            internal set { nocount = value; }
        }

        /// <summary>
        /// Compiles each query but does not execute it.
        /// </summary>
        public bool? Noexec
        {
            get { return noexec; }
            internal set { noexec = value; }
        }

        /// <summary>
        /// Specifies the level of error reporting generated when rounding in an expression causes a loss of precision.
        /// </summary>
        public bool? NumericRoundabort
        {
            get { return numericRoundabort; }
            internal set { numericRoundabort = value; }
        }

        /// <summary>
        /// Examines the syntax of each Transact-SQL statement and returns any error messages without compiling or executing the statement.
        /// </summary>
        public bool? ParseOnly
        {
            get { return parseOnly; }
            internal set { parseOnly = value; }
        }

        public bool? PartialResults
        {
            get { return partialResults; }
            internal set { partialResults = value; }
        }

        public bool? ProcIdOn
        {
            get { return procIdOn; }
            internal set { procIdOn = value; }
        }

        /// <summary>
        /// Causes SQL Server 2005 to follow the SQL-92 rules regarding quotation mark delimiting identifiers and literal strings.
        /// </summary>
        public bool? QuotedIdentifier
        {
            get { return quotedIdentifier; }
            internal set { quotedIdentifier = value; }
        }

        /// <summary>
        /// After this SET statement is executed, SQL Server returns the execution plan information for each query in text. 
        /// </summary>
        public bool? Showplan
        {
            get { return showplan; }
            internal set { showplan = value; }
        }

        /// <summary>
        /// Return diagnostics data 
        /// </summary>
        public bool? Statistics
        {
            get { return statistics; }
            internal set { statistics = value; }
        }

        /// <summary>
        /// Returns a formatted list of current statuses
        /// </summary>
        public string StatusList
        {
            get
            {
                StringBuilder statusList = new StringBuilder();
                if (AnsiDefaults.HasValue && AnsiDefaults.Value) { statusList.Append(comma); statusList.Append(AnsiDefaultsString); statusList.Append(on); }
                if (AnsiNullDefaultOff.HasValue && AnsiNullDefaultOff.Value) { statusList.Append(comma); statusList.Append(AnsiNullDefaultOffString); statusList.Append(on); }
                if (AnsiNullDefaultOn.HasValue && AnsiNullDefaultOn.Value) { statusList.Append(comma); statusList.Append(AnsiNullDefaultOnString); statusList.Append(on); }
                if (AnsiNulls.HasValue && AnsiNulls.Value) { statusList.Append(comma); statusList.Append(AnsiNullsString); statusList.Append(on); }
                if (AnsiPadding.HasValue && AnsiPadding.Value) { statusList.Append(comma); statusList.Append(AnsiPaddingString); statusList.Append(on); }
                if (AnsiWarnings.HasValue && AnsiWarnings.Value) { statusList.Append(comma); statusList.Append(AnsiWarningsString); statusList.Append(on); }
                if (Arithabort.HasValue && Arithabort.Value) { statusList.Append(comma); statusList.Append(ArithabortString); statusList.Append(on); }
                if (Arithignore.HasValue && Arithignore.Value)
                {
                    statusList.Append(comma); statusList.Append(ArithignoreString); statusList.Append(on);
                    if (Arithabort.HasValue && Arithabort.Value)
                    {
                        statusList.Append("(Overridden by Arithabort)");
                    }
                }
                if (ConcatNullYieldsNull.HasValue && ConcatNullYieldsNull.Value) { statusList.Append(comma); statusList.Append(ConcatNullYieldsNullString); statusList.Append(on); }
                if (Fmtonly.HasValue && Fmtonly.Value) { statusList.Append(comma); statusList.Append(FmtonlyString); statusList.Append(on); }
                if (Forceplan.HasValue && Forceplan.Value) { statusList.Append(comma); statusList.Append(ForceplanString); statusList.Append(on); }
                if (Nocount.HasValue && Nocount.Value) { statusList.Append(comma); statusList.Append(NocountString); statusList.Append(on); }
                if (Noexec.HasValue && Noexec.Value) { statusList.Append(comma); statusList.Append(NoexecString); statusList.Append(on); }
                if (NumericRoundabort.HasValue && NumericRoundabort.Value) { statusList.Append(comma); statusList.Append(NumericRoundabortString); statusList.Append(on); }
                if (ParseOnly.HasValue && ParseOnly.Value) { statusList.Append(comma); statusList.Append(ParseOnlyString); statusList.Append(on); }
                if (PartialResults.HasValue && PartialResults.Value) { statusList.Append(comma); statusList.Append(PartialResultsString); statusList.Append(on); }
                if (ProcIdOn.HasValue && ProcIdOn.Value) { statusList.Append(comma); statusList.Append(ProcIdOnString); statusList.Append(on); }
                if (QuotedIdentifier.HasValue && QuotedIdentifier.Value) { statusList.Append(comma); statusList.Append(QuotedIdentifierString); statusList.Append(on); }
                if (Showplan.HasValue && Showplan.Value) { statusList.Append(comma); statusList.Append(ShowplanString); statusList.Append(on); }
                if (Statistics.HasValue && Statistics.Value) { statusList.Append(comma); statusList.Append(StatisticsString); statusList.Append(on); }
                if (TraceFlagOn.HasValue && TraceFlagOn.Value) { statusList.Append(comma); statusList.Append(TraceFlagOnString); statusList.Append(on); }
                if (XactAbort.HasValue && !XactAbort.Value) { statusList.Append(comma); statusList.Append(XactAbortString); statusList.Append(off); }

                if (statusList.Length > 2)
                {
                    return statusList.ToString().Substring(2);
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Mask from DBCC PSS on SQL 2000 only
        /// </summary>
        public int StatusMask
        {
            get { return statusMask; }
            internal set
            {
                statusMask = value;
                ParseStatusMask();
            }
        }

        /// <summary>
        /// Indicates that a trace flag has been set
        /// </summary>
        public bool? TraceFlagOn
        {
            get { return traceFlagOn; }
            internal set { traceFlagOn = value; }
        }

        /// <summary>
        /// Returns whether there is an option set that may be a problem
        /// Warning settings taken from DM 4.6
        /// </summary>
        public bool IsInWarning
        {
            get
            {
                if (
                    (Fmtonly.HasValue && Fmtonly.Value)
                    || (Noexec.HasValue && Noexec.Value)
                    || (XactAbort.HasValue && !XactAbort.Value)
                    || (PartialResults.HasValue && !PartialResults.Value)
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Specifies whether SQL Server automatically rolls back the current transaction when a Transact-SQL statement raises a run-time error.
        /// </summary>
        public bool? XactAbort
        {
            get { return xactAbort; }
            internal set { xactAbort = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        private void ParseStatusMask()
        {
            AnsiNullDefaultOff = (AnsiNullDefaultOffBit & StatusMask) == AnsiNullDefaultOffBit;
            AnsiNullDefaultOn = (AnsiNullDefaultOnBit & StatusMask) == AnsiNullDefaultOnBit;
            Arithabort = (ArithabortBit & StatusMask) == ArithabortBit;
            Arithignore = (ArithignoreBit & StatusMask) == ArithignoreBit;
            Fmtonly = (FmtonlyBit & StatusMask) == FmtonlyBit;
            Forceplan = (ForceplanBit & StatusMask) == ForceplanBit;
            Nocount = (NocountBit & StatusMask) == NocountBit;
            Noexec = (NoexecBit & StatusMask) == NoexecBit;
            NumericRoundabort = (NumericRoundabortBit & StatusMask) == NumericRoundabortBit;
            ParseOnly = (ParseOnlyBit & StatusMask) == ParseOnlyBit;
            PartialResults = (PartialResultsBit & StatusMask) == PartialResultsBit;
            ProcIdOn = (ProcIdOnBit & StatusMask) == ProcIdOnBit;
            QuotedIdentifier = (QuotedIdentifierBit & StatusMask) == QuotedIdentifierBit;
            Showplan = (ShowplanBit & StatusMask) == ShowplanBit;
            Statistics = (StatisticsBit & StatusMask) == StatisticsBit;
            TraceFlagOn = (TraceFlagOnBit & StatusMask) == TraceFlagOnBit;
            XactAbort = !((XactAbortBit & StatusMask) == XactAbortBit);
        }

        #endregion

        #region interface implementations

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ansiDefaults", ansiDefaults);
            info.AddValue("ansiNullDefaultOff", ansiNullDefaultOff);
            info.AddValue("ansiNullDefaultOn", ansiNullDefaultOn);
            info.AddValue("ansiNulls", ansiNulls);
            info.AddValue("ansiPadding", ansiPadding);
            info.AddValue("ansiWarnings", ansiWarnings);
            info.AddValue("arithabort", arithabort);
            info.AddValue("arithignore", arithignore);
            info.AddValue("concatNullYieldsNull", concatNullYieldsNull);
            info.AddValue("fmtonly", fmtonly);
            info.AddValue("forceplan", forceplan);
            info.AddValue("nocount", nocount);
            info.AddValue("noexec", noexec);
            info.AddValue("numericRoundabort", numericRoundabort);
            info.AddValue("parseOnly", parseOnly);
            info.AddValue("partialResults", partialResults);
            info.AddValue("procIdOn", procIdOn);
            info.AddValue("quotedIdentifier", quotedIdentifier);
            info.AddValue("showplan", showplan);
            info.AddValue("statistics", statistics);
            info.AddValue("statusMask", statusMask);
            info.AddValue("traceFlagOn", traceFlagOn);
            info.AddValue("xactAbort", xactAbort);
        }

        #endregion

        #region nested types

        #endregion
    }
}
