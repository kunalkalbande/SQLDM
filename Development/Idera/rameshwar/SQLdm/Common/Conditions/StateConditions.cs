//------------------------------------------------------------------------------
// <copyright file="StateConditions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Checks the state against target flags
    /// </summary>
    [Serializable]
    public class StateIsCondition : ICondition
    {
        private string attribute;
        private MonitoredStateFlags target;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateIsCondition"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="target">The target.</param>
        public StateIsCondition(string attribute, MonitoredStateFlags target)
        {
            this.attribute = attribute;
            this.target = target;
        }

        private StateIsCondition(SerializationInfo info, StreamingContext context)
        {
            attribute = info.GetString("attribute");
            target = (MonitoredStateFlags) info.GetValue("target", typeof(MonitoredStateFlags));
        }

        #region ICondition Members

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool Matches(XmlElement element)
        {
            string state = element.GetAttribute(attribute);
            if (String.IsNullOrEmpty(state))
                return false;

            MonitoredStateFlags value = (MonitoredStateFlags)Enum.Parse(typeof(MonitoredStateFlags), state);
            if ((value & target) != MonitoredStateFlags.None)
            {
                element.SetAttribute("passed", "true");
                return true;
            }

            return false;
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("attribute", attribute);
            info.AddValue("target", target);
        }
    }
    
    /// <summary>
    /// Checks if a state is worse than the target state
    /// </summary>
    [Serializable]
    public class StateWorseThanCondition : ICondition
    {
        private string attribute;
        private MonitoredState target;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateWorseThanCondition"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="target">The target.</param>
        public StateWorseThanCondition(string attribute, MonitoredState target)
        {
            this.attribute = attribute;
            this.target = target;
        }

        public StateWorseThanCondition(SerializationInfo info, StreamingContext context)
        {
            attribute = info.GetString("attribute");
            target = (MonitoredState)info.GetValue("target", typeof(MonitoredState));
        }

        #region ICondition Members

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool Matches(XmlElement element)
        {
            string state = element.GetAttribute(attribute);
            if(String.IsNullOrEmpty(state))
                return false;

            MonitoredState value = (MonitoredState) Enum.Parse(typeof (MonitoredState), state);
            if (value > target)
            {
                element.SetAttribute("passed", "true");
                return true;
            }
            
            return false;
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("attribute", attribute);
            info.AddValue("target", target);
        }
    }
}
