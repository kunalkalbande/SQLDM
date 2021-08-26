//------------------------------------------------------------------------------
// <copyright file="NotificationDataManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.ManagementService.Notification.Data;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Notification;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Idera.SQLdm.ManagementService.Notification
{
    /*
    public class NotificationDataManager : SqlDataManager, INotificationDataManager
    {

        private NotificationProviderInfo[]      notificationProviders;
        private NotificationDestinationInfo[]   notificationDestinations;
        private NotificationRule[]              notificationRules;

        public NotificationDataManager(string connectInfo) : base(connectInfo)
        {
        }

        public System.Collections.Generic.IEnumerable<Idera.SQLdm.Notification.NotificationProviderInfo> GetNotificationProviders()
        {
            if (notificationProviders == null)
                LoadNotificationProviders();

            return notificationProviders;
        }

        private void LoadNotificationProviders()
        {
            NotificationProviderInfo npi = null;
            IDictionary<Guid,NotificationProviderInfo> result = new Dictionary<Guid,NotificationProviderInfo>();

            using (SqlDataSession session = base.GetSession())
            {
                session.Open();
                using (SqlCommand command = session.CreateCommand("p_NotificationProviders_Get"))
                {
                    command.Parameters.AddWithValue("@Id", DBNull.Value);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        object[] values = new object[reader.FieldCount];

                        while (reader.Read())
                        {
                            try
                            {
                                reader.GetValues(values);
                                npi = new NotificationProviderInfo();
                                npi.Id = (Guid)values[0];
                                npi.Name = (string)values[1];
                                npi.Description = (string)values[2];
                                npi.ProviderTypeName = (string)values[3];
                                npi.Enabled = (bool)values[4];

                                result[npi.Id] = npi;
                            }
                            catch (Exception e)
                            {
                                // log the message but continue 
                            }
                        }
                    }
                }

                using (SqlCommand command = session.CreateCommand("p_NotificationProviderProperties_Get"))
                {
                    command.Parameters.AddWithValue("@PropertyId", DBNull.Value);
                    command.Parameters.AddWithValue("@ProviderId", DBNull.Value);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                Guid providerId = reader.GetGuid(3);
                                if (npi == null || npi.Id != providerId)
                                {
                                    if (!result.TryGetValue(providerId, out npi))
                                    {
                                        // log message but continue with the next row 
                                        continue;
                                    }
                                }

                                NotificationProviderProperty property = new NotificationProviderProperty();
                                property.Id = reader.GetGuid(0);
                                property.Name = reader.GetString(1);
                                property.SerializedValue = reader.GetSqlBytes(2).Buffer;

                                npi.Properties.Add(property);
                            }
                            catch (Exception e)
                            {
                                // log the message but continue 
                            }
                        }
                    }
                }
            }

            notificationProviders = new NotificationProviderInfo[result.Count];
            result.Values.CopyTo(notificationProviders, 0);
        }

        /// <summary>
        /// Return the Type object represented by the string.  Throws an exception 
        /// if the type cannot be found.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>Type</returns>
        private Type GetProviderType(string typeName)
        {
            return Type.GetType(typeName, true, true);
        }

        public NotificationProviderInfo GetNotificationProvider(Guid notificationProviderId)
        {
            for (int i = 0; i < notificationProviders.Length; i++)
            {
                if (notificationProviders[i].Id == notificationProviderId)
                    return notificationProviders[i];
            }

            return null;
        }

        public NotificationDestinationInfo GetNotificationDestination(Guid notificationDestinationId)
        {
            for (int i = 0; i < notificationDestinations.Length; i++)
            {
                if (notificationDestinations[i].Id == notificationDestinationId)
                    return notificationDestinations[i];
            }

            return null;
        }

        /// <summary>
        /// Updates the notification provider data in the database.  New providers can't be added using 
        /// this method.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool SaveNotificationProvider(Idera.SQLdm.Notification.NotificationProviderInfo provider)
        {
            return SaveNotificationProviders(new NotificationProviderInfo[] { provider });

        }

        /// <summary>
        /// Updates the notification provider data in the database.  New providers can't be added using 
        /// this method.  Properties are replaced.
        /// </summary>
        /// <param name="providers"></param>
        /// <returns></returns>
        public bool SaveNotificationProviders(System.Collections.Generic.IEnumerable<Idera.SQLdm.Notification.NotificationProviderInfo> providers)
        {
            NotificationProviderInfo currentProvider;
            List<NotificationProviderProperty> toBeDeleted = new List<NotificationProviderProperty>(5);

            bool result = true;

            using (SqlDataSession session = base.GetSession())
            {
                session.Open();

                using (SqlCommand npiUpdate = session.CreateCommand("p_NotificationProviders_Update"),
                        nppUpdate = session.CreateCommand("p_NotificationProviderProperty_Update"),
                        nppAdd = session.CreateCommand("p_NotificationProviderProperty_Add"),
                        nppDelete = session.CreateCommand("p_NotificationProviderProperty_Delete")
                    )
                {
                    IDataTransaction transaction = session.BeginTransaction();

                    try
                    {
                        SqlParameter npiUpdate_id = npiUpdate.Parameters.Add("@PropertyId", SqlDbType.UniqueIdentifier);
                        SqlParameter npiUpdate_name = npiUpdate.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                        SqlParameter npiUpdate_description = npiUpdate.Parameters.Add("@Description", SqlDbType.NVarChar, 512);
                        SqlParameter npiUpdate_type = npiUpdate.Parameters.Add("@ProviderType", SqlDbType.NVarChar, 512);
                        SqlParameter npiUpdate_enabled = npiUpdate.Parameters.Add("@Enabled", SqlDbType.Bit);

                        SqlParameter nppUpdate_propertyId = nppUpdate.Parameters.Add("@PropertyId", SqlDbType.UniqueIdentifier);
                        SqlParameter nppUpdate_name = nppUpdate.Parameters.Add("@Name", SqlDbType.NVarChar, 256);
                        SqlParameter nppUpdate_value = nppUpdate.Parameters.Add("@Value", SqlDbType.VarBinary, 1024);

                        SqlParameter nppAdd_providerId = nppAdd.Parameters.Add("@ProviderId", SqlDbType.UniqueIdentifier);
                        SqlParameter nppAdd_name = nppAdd.Parameters.Add("@Name", SqlDbType.NVarChar, 256);
                        SqlParameter nppAdd_value = nppAdd.Parameters.Add("@Value", SqlDbType.VarBinary, 1024);
                        SqlParameter nppAdd_id = nppAdd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ReturnPropertyId", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, 10, 0, null, System.Data.DataRowVersion.Current, false, null, "", "", ""));

                        SqlParameter nppDelete_id = nppDelete.Parameters.Add("@Id", SqlDbType.UniqueIdentifier);

                        foreach (NotificationProviderInfo npi in providers)
                        {
                            currentProvider = GetNotificationProvider(npi.Id);

                            npiUpdate_id.Value = npi.Id;
                            npiUpdate_name.Value = npi.Name;
                            npiUpdate_description.Value = npi.Description;
                            npiUpdate_type.Value = npi.ProviderTypeName;
                            npiUpdate_enabled.Value = npi.Enabled;
                            
                            npiUpdate.ExecuteNonQuery();

                            foreach (NotificationProviderProperty property in npi.Properties)
                            {     
                                // make a copy of the current properties 
                                toBeDeleted.Clear();
                                toBeDeleted.AddRange(currentProvider.Properties);

                                Guid propertyId = property.Id;
                                NotificationProviderProperty existingProperty = currentProvider.GetProperty(propertyId);
                                if (existingProperty != null)
                                {   // update an existing property
                                    toBeDeleted.Remove(existingProperty);
                                    nppUpdate_propertyId.Value = propertyId;
                                    nppUpdate_name.Value = property.Name;
                                    nppUpdate_value.Value = property.SerializedValue;
                                    nppUpdate.ExecuteNonQuery();
                                }
                                else
                                {   // add a new property
                                    nppAdd_providerId.Value = propertyId;
                                    nppAdd_name.Value = property.Name;
                                    nppAdd_value.Value = property.SerializedValue;
                                    nppAdd.ExecuteNonQuery();
                                    property.Id = (Guid)nppAdd_id.Value;
                                }
                            }
                            if (toBeDeleted.Count > 0)
                            {
                                foreach (NotificationProviderProperty property in toBeDeleted)
                                {
                                    nppDelete_id.Value = property.Id;
                                    nppDelete.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();

                        // now that the values have been persisted - update the cached values
                        foreach (NotificationProviderInfo npi in providers)
                        {
                            currentProvider = GetNotificationProvider(npi.Id);
                            currentProvider.Enabled = npi.Enabled;
                            currentProvider.Properties = npi.Properties;
                        }
                    }
                    catch (Exception e)
                    {
                        result = false;
                        transaction.Rollback();
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
            }
            return result;
        }

        public System.Collections.Generic.IEnumerable<Idera.SQLdm.Notification.NotificationDestinationInfo> GetNotificationDestinations()
        {
            if (notificationDestinations == null)
                LoadNotificationDestinations();

            return notificationDestinations;
        }

        private void LoadNotificationDestinations()
        {
            NotificationDestinationInfo ndi = null;
            Dictionary<Guid,NotificationDestinationInfo> result = new Dictionary<Guid,NotificationDestinationInfo>(5);

            using (SqlDataSession session = base.GetSession())
            {
                session.Open();
                using (SqlCommand command = session.CreateCommand("p_NotificationDestinations_Get"))
                {
                    command.Parameters.AddWithValue("@DestinationId", DBNull.Value);
                    command.Parameters.AddWithValue("@ProviderId", DBNull.Value);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                Guid providerId = reader.GetGuid(3);
                                NotificationProviderInfo npi = GetNotificationProvider(providerId);
                                if (npi == null)
                                    continue;

                                ndi = npi.CreateNewDestination(reader.GetString(1));
                                ndi.Id = reader.GetGuid(0);
                                ndi.Name = reader.GetString(1);
                                ndi.DestinationTypeName = reader.GetString(2);
                                result.Add(ndi.Id, ndi);
                            }
                            catch (Exception e)
                            {
                                // log the message but continue 
                            }
                        }
                    }
                }
                    using (SqlCommand command = session.CreateCommand("p_NotificationDestinationProperties_Get"))
                    {
                        command.Parameters.AddWithValue("@PropertyId", DBNull.Value);
                        command.Parameters.AddWithValue("@DestinationId", DBNull.Value);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    Guid destinationId = reader.GetGuid(3);
                                    if (ndi == null || ndi.Id != destinationId)
                                    {
                                        if (!result.TryGetValue(destinationId, out ndi))
                                        {
                                            // log message but continue with the next row 
                                            continue;
                                        }
                                    }

                                    NotificationDestinationProperty property = new NotificationDestinationProperty();
                                    property.Id = reader.GetGuid(0);
                                    property.Name = reader.GetString(1);
                                    property.SerializedValue = reader.GetSqlBytes(2).Buffer;

                                    ndi.Properties.Add(property);
                                }
                                catch (Exception e)
                                {
                                    // log the message but continue 
                                }
                            }

                        }
                }
            }

            notificationDestinations = new NotificationDestinationInfo[result.Count];
            result.Values.CopyTo(notificationDestinations, 0);
        }


        private bool AddNotificationDestination(NotificationDestinationInfo newDestination, SqlDataSession session)
        {
            using (SqlCommand ndiAdd = session.CreateCommand("p_NotificationDestination_Add"),
                    nppAdd = session.CreateCommand("p_NotificationProviderProperty_Add"))
            {
                SqlParameter ndiAdd_providerId = npiUpdate.Parameters.Add("@ProviderId", SqlDbType.UniqueIdentifier);
                SqlParameter ndiAdd_name = npiUpdate.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                SqlParameter ndiAdd_type = npiUpdate.Parameters.Add("@DestinationType", SqlDbType.NVarChar, 512);
                SqlParameter ndiAdd_id = nppAdd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ReturnDestinationId", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, 10, 0, null, System.Data.DataRowVersion.Current, false, null, "", "", ""));

                SqlParameter nppAdd_destinationId = nppAdd.Parameters.Add("@DestinationId", SqlDbType.UniqueIdentifier);
                SqlParameter nppAdd_name = nppAdd.Parameters.Add("@Name", SqlDbType.NVarChar, 256);
                SqlParameter nppAdd_value = nppAdd.Parameters.Add("@Value", SqlDbType.VarBinary, 1024);
                SqlParameter nppAdd_id = nppAdd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ReturnPropertyId", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, 10, 0, null, System.Data.DataRowVersion.Current, false, null, "", "", ""));

                ndiAdd_providerId.Value = newDestination.Provider.Id;
                ndiAdd_name = newDestination.Name;
                ndiAdd_type = newDestination.DestinationTypeName;
                ndiAdd.ExecuteNonQuery();
                Guid destinationId = (Guid)ndiAdd_id.Value;
                newDestination.Id = destinationId;

                foreach (NotificationDestinationProperty property in newDestination.Properties)
                {
                    nppAdd_destinationId.Value = destinationId;
                    nppAdd_name.Value = property.Name;
                    nppAdd_value.Value = property.SerializedValue;
                    nppAdd.ExecuteNonQuery();
                    property.Id = (Guid)nppAdd_id.Value;
                }
            }
            return true;
        }

        public bool SaveNotificationDestination(NotificationDestinationInfo destination)
        {
            NotificationDestinationInfo currentDestination;
            List<NotificationDestinationProperty> toBeDeleted = new List<NotificationDestinationProperty>(5);
            
            bool result = true;

            using (SqlDataSession session = base.GetSession())
            {
                session.Open();

                currentProvider = GetNotificationProvider(destination.Id);
                if (currentProvider == null)
                    return AddNotificationDestination(destination, session);

                using (SqlCommand ndiUpdate = session.CreateCommand("p_NotificationDestination_Update"),
                        nppUpdate = session.CreateCommand("p_NotificationProviderProperty_Update"),
                        nppAdd = session.CreateCommand("p_NotificationProviderProperty_Add"),
                        nppDelete = session.CreateCommand("p_NotificationProviderProperty_Delete")
                    )
                {
                    IDataTransaction transaction = session.BeginTransaction();

                    try
                    {
                        SqlParameter npiUpdate_id = npiUpdate.Parameters.Add("@PropertyId", SqlDbType.UniqueIdentifier);
                        SqlParameter npiUpdate_name = npiUpdate.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                        SqlParameter npiUpdate_description = npiUpdate.Parameters.Add("@Description", SqlDbType.NVarChar, 512);
                        SqlParameter npiUpdate_type = npiUpdate.Parameters.Add("@ProviderType", SqlDbType.NVarChar, 512);
                        SqlParameter npiUpdate_enabled = npiUpdate.Parameters.Add("@Enabled", SqlDbType.Bit);

                        SqlParameter nppUpdate_propertyId = nppUpdate.Parameters.Add("@PropertyId", SqlDbType.UniqueIdentifier);
                        SqlParameter nppUpdate_name = nppUpdate.Parameters.Add("@Name", SqlDbType.NVarChar, 256);
                        SqlParameter nppUpdate_value = nppUpdate.Parameters.Add("@Value", SqlDbType.VarBinary, 1024);

                        SqlParameter nppAdd_providerId = nppAdd.Parameters.Add("@ProviderId", SqlDbType.UniqueIdentifier);
                        SqlParameter nppAdd_name = nppAdd.Parameters.Add("@Name", SqlDbType.NVarChar, 256);
                        SqlParameter nppAdd_value = nppAdd.Parameters.Add("@Value", SqlDbType.VarBinary, 1024);
                        SqlParameter nppAdd_id = nppAdd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ReturnPropertyId", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, 10, 0, null, System.Data.DataRowVersion.Current, false, null, "", "", ""));

                        SqlParameter nppDelete_id = nppDelete.Parameters.Add("@Id", SqlDbType.UniqueIdentifier);

                        foreach (NotificationProviderInfo npi in providers)
                        {
                            currentProvider = GetNotificationProvider(npi.Id);

                            npiUpdate_id.Value = npi.Id;
                            npiUpdate_name.Value = npi.Name;
                            npiUpdate_description.Value = npi.Description;
                            npiUpdate_type.Value = npi.ProviderTypeName;
                            npiUpdate_enabled.Value = npi.Enabled;

                            npiUpdate.ExecuteNonQuery();

                            foreach (NotificationProviderProperty property in npi.Properties)
                            {
                                // make a copy of the current properties 
                                toBeDeleted.Clear();
                                toBeDeleted.AddRange(currentProvider.Properties);

                                Guid propertyId = property.Id;
                                NotificationProviderProperty existingProperty = currentProvider.GetProperty(propertyId);
                                if (existingProperty != null)
                                {   // update an existing property
                                    toBeDeleted.Remove(existingProperty);
                                    nppUpdate_propertyId.Value = propertyId;
                                    nppUpdate_name.Value = property.Name;
                                    nppUpdate_value.Value = property.SerializedValue;
                                    nppUpdate.ExecuteNonQuery();
                                }
                                else
                                {   // add a new property
                                    nppAdd_providerId.Value = propertyId;
                                    nppAdd_name.Value = property.Name;
                                    nppAdd_value.Value = property.SerializedValue;
                                    nppAdd.ExecuteNonQuery();
                                    property.Id = (Guid)nppAdd_id.Value;
                                }
                            }
                            if (toBeDeleted.Count > 0)
                            {
                                foreach (NotificationProviderProperty property in toBeDeleted)
                                {
                                    nppDelete_id.Value = property.Id;
                                    nppDelete.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();

                        // now that the values have been persisted - update the cached values
                        foreach (NotificationProviderInfo npi in providers)
                        {
                            currentProvider = GetNotificationProvider(npi.Id);
                            currentProvider.Enabled = npi.Enabled;
                            currentProvider.Properties = npi.Properties;
                        }
                    }
                    catch (Exception e)
                    {
                        result = false;
                        transaction.Rollback();
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
            }
            return result;
        }

        public bool DeleteNotificationDestination(Idera.SQLdm.Notification.NotificationDestinationInfo destination)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.Collections.Generic.IEnumerable<Idera.SQLdm.Notification.NotificationRule> GetNotificationRules()
        {
            if (notificationRules == null)
                LoadNotificationRules();

            return notificationRules;
        }

        /// <summary>
        /// Load NotificationRules and associate them with a notification destination.
        /// </summary>
        private void LoadNotificationRules()
        {
            NotificationRule rule = null;

            IDictionary<Guid,NotificationRule> result = new Dictionary<Guid,NotificationRule>();

            using (SqlDataSession session = base.GetSession())
            {
                session.Open();
                using (SqlCommand command = session.CreateCommand("p_NotificationRules_Get"))
                {
                    command.Parameters.AddWithValue("@Id", DBNull.Value);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                rule = new NotificationRule();
                                rule.Id = reader.GetGuid(0);
                                rule.SerializedCondition = reader.GetSqlBytes(1).Buffer;
                                result[rule.Id] = rule;
                            }
                            catch (Exception e)
                            {
                                // log the message but continue 
                            }
                        }
                    }
                }
                using (SqlCommand command = session.CreateCommand("p_NotificationRuleDestinations_Get"))
                {
                    command.Parameters.AddWithValue("@RuleId", DBNull.Value);
                    command.Parameters.AddWithValue("@DestinationId", DBNull.Value);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                Guid ruleId = reader.GetGuid(0);
                                Guid destinationId = reader.GetGuid(1);

                                if (rule == null || rule.Id != ruleId)
                                {
                                    if (!result.TryGetValue(ruleId, out rule))
                                    {
                                        // log message but continue with next row 
                                        continue;
                                    }
                                }
                            
                                NotificationDestinationInfo destination = GetNotificationDestination(destinationId);
                                if (destination != null)
                                {
                                    // log message but continue with next row 
                                    continue;
                                }
                                rule.Destinations.Add(destination);
                            }
                            catch (Exception e)
                            {
                                // log the message but continue 
                            }
                        }
                    }
                }
            }

            notificationRules = new NotificationRule[result.Count];
            result.Values.CopyTo(notificationRules, 0);
        }

        public bool SaveNotificationRule(Idera.SQLdm.Notification.NotificationRule rule)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool DeleteNotificationRule(Idera.SQLdm.Notification.NotificationRule rule)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public interface ICachedObjectDescriptor<I,T>
    {
        I GetIdentifier(T cachableObject);
        ICachedObjectWriter<T> Writer   { get; }
        ICachedObjectReader<I,T> Reader { get; }
    }

    public interface ICachedObjectWriter<T>
    {
        bool Save(T item);
        bool Save(IEnumerable<T> items);
    }

    public interface ICachedObjectReader<I,T>
    {
        IEnumerable<T> Get();
        T Get(I id);
    }

    public class NotificationProviderPropertyDescriptor : ICachedObjectDescriptor<Guid, NotificationProviderProperty>
    {
        private static NotificationProviderPropertyWriter writer = new NotificationProviderPropertyWriter();
        private static NotificationProviderPropertyReader reader = new NotificationProviderPropertyReader();

        public Guid GetIdentifier(NotificationProviderProperty cachableObject)
        {
            return cachableObject.Id;    
        }

        public ICachedObjectWriter<NotificationProviderProperty> Writer
        {
            get { return writer; }
        }

        public ICachedObjectReader<Guid, NotificationProviderProperty> Reader
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }


        public class NotificationProviderPropertyReader : ICachedObjectReader<Guid, NotificationProviderProperty>
        {
            public IEnumerable<NotificationProviderProperty> Get()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public NotificationProviderProperty Get(Guid id)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public class NotificationProviderPropertyWriter : ICachedObjectWriter<NotificationProviderProperty>
        {

            public bool Save(NotificationProviderProperty item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool Save(IEnumerable<NotificationProviderProperty> items)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
    /* */
}
