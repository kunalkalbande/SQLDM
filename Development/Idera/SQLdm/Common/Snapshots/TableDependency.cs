//------------------------------------------------------------------------------
// <copyright file="TableDependency.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents Table dependency sample
    /// </summary>
    [Serializable]
    public sealed class TableDependency
    {

        #region constants

        // Action Constants

        private const string Integrity = "integrity";
        private const string Read = "read";
        private const string SelectAll = "select *";
        private const string Update = "update";
        private const string UpdateSelectAll = "update/select *";

        // Type Constants
        private const string AfterTrigger = "after trigger";
        private const string AfterTriggerDisabled = "after trigger (disabled)";
        private const string CheckConstraint = "check constraint";
        private const string Default = "default";
        private const string FirstDeleteTrigger = "first delete trigger";
        private const string FirstDeleteTriggerDisabled = "first delete trigger (disabled)";
        private const string FirstInsertTrigger = "first insert trigger";
        private const string FirstInsertTriggerDisabled = "first insert trigger (disabled)";
        private const string FirstUpdateTrigger = "first update trigger";
        private const string FirstUpdateTriggerDisabled = "first update trigger (disabled)";
        private const string Function = "function";
        private const string InlineTableFunction = "inline table function";
        private const string InsteadOfTrigger = "instead of trigger";
        private const string InsteadOfTriggerDisabled = "instead of trigger (disabled)";
        private const string LastDeleteTrigger = "last delete trigger";
        private const string LastDeleteTriggerDisabled = "last delete trigger (disabled)";
        private const string LastInsertTrigger = "last insert trigger";
        private const string LastInsertTriggerDisabled = "last insert trigger (disabled)";
        private const string LastUpdateTrigger = "last update trigger";
        private const string LastUpdateTriggerDisabled = "last update trigger (disabled)";
        private const string PrimaryKey = "primary key";
        private const string StartupProcedure = "startup procedure";
        private const string StoredProcedure = "stored proc";
        private const string ForeignKey = "foreign key";
        private const string TableForeignKey = "table - foreign key";
        private const string TableFunction = "table function";
        private const string Trigger = "trigger";
        private const string View = "view";



        #endregion

        #region fields

        private DependencyAction? action = null;
        private bool? enabled = true;
        private string keyName = null;
        private string name = null;
        private DependencyType? type = null;
        private string schema = null;


        #endregion

        #region constructors


        // This constructor does not allow you to set enabled because the typefromBatch string is expected to contain this information
        internal TableDependency(string action, string name, string keyName, string typeFromBatch, string schema)
        {
            if (name == null || name.Length == 0) throw new ArgumentNullException("name");
            if (typeFromBatch == null || typeFromBatch.Length == 0) throw new ArgumentNullException("typeFromBatch");
            if (schema == null || schema.Length == 0) throw new ArgumentNullException("schema");

            this.action = ConvertToDependencyAction(action);
            this.keyName = keyName;
            this.name = name;
            this.type = ConvertToDependencyType(typeFromBatch);
            this.schema = schema;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the action of the dependency
        /// </summary>
        public DependencyAction? Action
        {
            get { return action; }
        }

        /// <summary>
        /// Gets whether the dependency is enabled
        /// </summary>		
        public bool? Enabled
        {
            get { return enabled; }
        }

        /// <summary>
        /// Gets the object's key name (if applicable)
        /// </summary>	
        public string KeyName
        {
            get { return keyName; }
        }

        /// <summary>
        /// Gets the object type of the dependency
        /// </summary>
        public DependencyType? Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the dependency name in [schema].[objectname] format
        /// </summary>
        public string Fullname
        {
            get { return  schema + "." + name; }
        }

        /// <summary>
        /// Gets the name of the dependency
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the schema
        /// </summary>
        public string Schema
        {
            get { return schema; }
        }

        #endregion

        #region methods

        // Convert dependency type from batch to DependencyType? and set _enabled
        // if necessary
        private DependencyType? ConvertToDependencyType(string type)
        {
            switch (type.ToLower())
            {
                case AfterTrigger:
                    return DependencyType.AfterTrigger;
                case AfterTriggerDisabled:
                    enabled = false;
                    return DependencyType.AfterTrigger;
                case CheckConstraint:
                    return DependencyType.CheckConstraint;
                case Default:
                    return DependencyType.Default;
                case FirstDeleteTrigger:
                    return DependencyType.FirstDeleteTrigger;
                case FirstDeleteTriggerDisabled:
                    enabled = false;
                    return DependencyType.FirstDeleteTrigger;
                case FirstInsertTrigger:
                    return DependencyType.FirstInsertTrigger;
                case FirstInsertTriggerDisabled:
                    enabled = false;
                    return DependencyType.FirstInsertTrigger;
                case FirstUpdateTrigger:
                    return DependencyType.FirstUpdateTrigger;
                case FirstUpdateTriggerDisabled:
                    enabled = false;
                    return DependencyType.FirstUpdateTrigger;
                case Function:
                    return DependencyType.Function;
                case InlineTableFunction:
                    return DependencyType.InlineTableFunction;
                case InsteadOfTrigger:
                    return DependencyType.InsteadOfTrigger;
                case InsteadOfTriggerDisabled:
                    enabled = false;
                    return DependencyType.InsteadOfTrigger;
                case LastDeleteTrigger:
                    return DependencyType.LastDeleteTrigger;
                case LastDeleteTriggerDisabled:
                    enabled = false;
                    return DependencyType.LastDeleteTrigger;
                case LastInsertTrigger:
                    return DependencyType.LastInsertTrigger;
                case LastInsertTriggerDisabled:
                    enabled = false;
                    return DependencyType.LastInsertTrigger;
                case LastUpdateTrigger:
                    return DependencyType.LastUpdateTrigger;
                case LastUpdateTriggerDisabled:
                    enabled = false;
                    return DependencyType.LastUpdateTrigger;
                case PrimaryKey:
                    return DependencyType.PrimaryKey;
                case StartupProcedure:
                    return DependencyType.StartupProcedure;
                case StoredProcedure:
                    return DependencyType.StoredProcedure;
                case ForeignKey:
                case TableForeignKey:
                    return DependencyType.ForeignKey;
                case TableFunction:
                    return DependencyType.TableFunction;
                case Trigger:
                    return DependencyType.Trigger;
                case View:
                    return DependencyType.View;
                default:
                    return DependencyType.Unknown;
            }
        }


        // Convert dependency action from batch to DependencyAction 
        private static DependencyAction ConvertToDependencyAction(string action)
        {
            switch (action.ToLower())
            {

                case Integrity:
                    return DependencyAction.Integrity;
                case Read:
                    return DependencyAction.Read;
                case SelectAll:
                    return DependencyAction.SelectAll;
                case Update:
                    return DependencyAction.Update;
                case UpdateSelectAll:
                    return DependencyAction.UpdateSelectAll;
                case null:
                case "":
                    return DependencyAction.None;
                default:
                    return DependencyAction.Unknown;

            }
        }


        #endregion

        #region nested types

        #endregion

    }
}
