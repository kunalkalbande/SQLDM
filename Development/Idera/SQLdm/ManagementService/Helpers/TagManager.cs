using System;
using System.Collections.Generic;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.Helpers
{
    internal static class TagManager
    {
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("TagManager");

        public static int AddTag(string name)
        {
            using (Log.InfoCall("AddTag"))
            {
                try
                {
                    return
                        RepositoryHelper.UpdateTagConfiguration(ManagementServiceConfiguration.ConnectionString,
                                                                new Tag(-1, name));
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while adding a tag.");
                    Log.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public static int UpdateTagConfiguration(Tag tag)
        {
            using (Log.InfoCall("UpdateTagConfiguration"))
            {
                int result;
                try
                {
                    result = 
                        RepositoryHelper.UpdateTagConfiguration(ManagementServiceConfiguration.ConnectionString, tag);
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while updating the tag configuration.");
                    Log.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }

                Management.QueueDelegate(delegate()
                    {
                        try
                        {
                            // update local monitored servers with new tag configuration
                            Management.ScheduledCollection.SyncServerTags(tag.Id == -1 ? result : tag.Id, tag.Instances, tag.CustomCounters);
                            
                            // update the collection service with new tag configuration
                            ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                            if (collSvc == null)
                                throw new ManagementServiceException("The collection service interface is not available.");
                            collSvc.UpdateTagConfiguration(tag.Id, tag.Instances, tag.CustomCounters);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Error updating collection service with removed tags: ", e);
                        }
                    });
                return result;
            }
        }

        public static void RemoveTags(IList<int> tagIds)
        {
            using (Log.InfoCall("RemoveTags"))
            {
                try
                {
                    RepositoryHelper.RemoveTags(ManagementServiceConfiguration.ConnectionString, tagIds);
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while removing tags.");
                    Log.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }

                Management.QueueDelegate(delegate()
                    {
                        try
                        {
                            // remove tag from all cached servers
                            Management.ScheduledCollection.RemoveServerTags(tagIds);
                            // remove tag from all servers in the collection service
                            ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                            if (collSvc == null)
                                throw new ManagementServiceException("The collection service interface is not available.");
                            collSvc.RemoveTags(tagIds);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Error updating collection service with removed tags: ", e);
                        }
                    });
            }
        }
    }
}
