//------------------------------------------------------------------------------
// <copyright file="AgentLogLine.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a single line of a SQL Server agent log
    /// </summary>
    public sealed class AgentLogLine
    {
        
        #region fields

        private string					_message = null;
		private	AgentLogMessageType?    _messageType = null;
		private int?					_messageNumber = null;
		//private DateTime				_timeStamp = System.DateTime.MinValue;
		private string					_timeStamp = null;

		#endregion

		#region constructors
		
        //internal SqlAgentLogLine(
        //    string message, 
        //    AgentLogMessageType messageType, 
        //    int messageNumber, 
        //    string timeStamp)
        //{
        //    _message = message;
        //    _messageType = messageType;
        //    _messageNumber = messageNumber;
        //    _timeStamp = timeStamp;
        //    parseMessage();
        //}

        //internal SqlAgentLogLine(
        //    string message, 
        //    AgentLogMessageType messageType, 
        //    string timeStamp)
        //{
        //    _message = message;
        //    _messageType = messageType;
        //    _timeStamp = timeStamp;
        //    parseMessageNumber();
        //    parseMessage();
        //}

        //internal SqlAgentLogLine(
        //    string message,
        //    string timeStamp)
        //{
        //    if (message == null || message.Length == 0) throw new ArgumentNullException("message");
        //    _timeStamp = timeStamp;
        //    _message = message;
        //    parseMessageNumber();
        //    parseMessageType();
        //    parseMessage();
        //}

        //internal SqlAgentLogLine(
        //    string message)
        //{
        //    if (message == null || message.Length == 0) throw new ArgumentNullException("message");
        //    _message = message;
        //    parseTimeStamp();
        //    parseMessageNumber();
        //    parseMessageType();	
        //    parseMessage();
        //}


		#endregion

		#region properties

		/// <summary>
		/// Gets the text of the error log message
		/// </summary>
		public string Message
		{
			get { return _message; }
		}

		/// <summary>
		/// Gets the message type of the error log entry
		/// </summary>
		public AgentLogMessageType? MessageType
		{
			get { return _messageType; }
		}

		/// <summary>
		/// Gets the message number of the error log entry
		/// </summary>
		public int? MessageNumber
		{
			get { return _messageNumber; }
		}

		/// <summary>
		/// Gets the timestamp for the error log entry
        /// <remarks>This is a string value due to internationalization problems in the web console.  This will be revisited.</remarks>
		/// </summary>
		public string TimeStamp
		{
			get { return _timeStamp; }
		}

        ///// <summary>
        ///// Returns the error log entry formatted as it appears in the raw file
        ///// </summary>
        //public string OriginalFormatting
        //{
        //    get
        //    {
        //        string originalFormat = String.Format("{0:MM-dd-yyyy HH:mm:ss}",_timeStamp) + " - ";
        //        switch(_messageType)
        //        {
        //            case AgentLogMessageType.Error:
        //                originalFormat += "! ";
        //                break;
        //            case AgentLogMessageType.Informational:
        //                originalFormat += "? ";
        //                break;
        //            case AgentLogMessageType.Warning:
        //                originalFormat += "+ ";
        //                break;
        //        }
        //        originalFormat += "[" + _messageNumber + "] " + _message;
        //        return originalFormat;
        //    }
        //}

		#endregion

		#region methods
		
        ///// <summary>
        ///// Dumps sample data to a string.
        ///// </summary>
        ///// <returns>Sample data.</returns>
        //public string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();
        //    dump.Append("----------"); dump.Append("\n");
        //    dump.Append("TimeStamp: " + TimeStamp.ToString()); dump.Append("\n");
        //    dump.Append("MessageType: " + MessageType.ToString()); dump.Append("\n");
        //    dump.Append("MessageNumber: " + MessageNumber); dump.Append("\n");
        //    dump.Append("Message: " + Message); dump.Append("\n");
        //    dump.Append("OriginalFormatting: " + OriginalFormatting); dump.Append("\n");
        //    return dump.ToString();
        //}

        ///// <summary>
        ///// Parses the timestamp from a SQL Agent Log line
        ///// </summary>
        //private void parseTimeStamp()
        //{
        //    try
        //    {
        //        _timeStamp = ParsingHelper.ParseSqlAgentLogLineTimeStamp(_message);
        //    }
        //    catch
        //    {
        //        _timeStamp = null;
        //    }
        //}

		/// <summary>
		/// Parses the message portion of a SQL Agent Log line by searching for the end of the message number [###]
		/// </summary>
		private void parseMessage()
		{
			try
			{
				int index = _message.IndexOf("]");
				if ((index > 0) && (index+1 < _message.Length))
				{
					_message = _message.Substring(_message.IndexOf("]")+1).Trim(new char[] {' '});
				}
				else
				{
					_message = "";
				}
			}
			catch
			{
				_message = "";	
			}
		}

		
		/// <summary>
		/// Parses the error type for the SQL Agent Log from the indicator +, -, or ?
		/// </summary>
		private void parseMessageType()
		{
			Regex messageTypePattern = new Regex(@"(?<=-\s)[?+!](?=\s\[)");
			switch(messageTypePattern.Match(_message).Value)
			{
				case "!":
					_messageType=AgentLogMessageType.Error;
					break;
				case "+":
					_messageType=AgentLogMessageType.Warning;
					break;
				case "?":
					_messageType=AgentLogMessageType.Informational;
					break;
				default:
					_messageType=AgentLogMessageType.Informational;
					break;
			}
		}

		/// <summary>
		/// Parses the message number from a SQL Agent Log line by searching for the first instance of [###]
		/// </summary>
		private void parseMessageNumber()
		{
			Regex messageNumberPattern = new Regex(@"(?<=\[)[0-9]+(?=\])");
			_messageNumber = messageNumberPattern.IsMatch(_message)?Convert.ToInt32(messageNumberPattern.Match(_message).Value):0;
		}


		#endregion

    }
}
