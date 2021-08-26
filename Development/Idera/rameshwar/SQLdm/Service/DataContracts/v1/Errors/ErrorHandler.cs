﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using BBS.TracerX;
using System.ServiceModel.Web;

namespace Idera.SQLdm.Service.DataContracts.v1.Errors
{
    internal class ErrorHandler : IErrorHandler
    {
        private static Logger _logX = Logger.GetLogger("ErrorHandler");

        public bool HandleError(Exception x)
        {
            using (_logX.ErrorCall(string.Format("HandleError({0})", null == x ? "null exception!" : x.Message)))
            {
                _logX.Error("Exception:", x);

                // indicates to the system that the error was handled and no further error condition exists
                return true;
            }
        }


        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var rmp = new HttpResponseMessageProperty();
            if (error is LicenseManagerException)
            {
                string error_to_publish = error.Message;
                fault = Message.CreateMessage(version, "", error_to_publish, new DataContractJsonSerializer(error_to_publish.GetType()));

                // tell WCF to use JSON encoding rather than default XML
                fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
                if (((LicenseManagerException)error).IsAuthenticationFailed)
                {
                    rmp.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                }
                else
                    rmp.StatusCode = System.Net.HttpStatusCode.ExpectationFailed;

                // put appropraite description here..
                rmp.StatusDescription = error.Message.Replace("\n", "&#10;").Replace("\r", "&#13;");
                // rmp.StatusDescription = error.Message + "  See fault object for more information.";
                fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
                return;
            }
            if (error is FaultException)
            {
                var error_to_publish = new JsonExceptionWrapper(error);
                try
                {
                    // extract the our FaultContract object from the exception object.
                    System.Reflection.PropertyInfo pInfo = error.GetType().GetProperty("Detail");
                    var detail = string.Empty;
                    if (pInfo != null)
                    {
                        detail = Convert.ToString(pInfo.GetGetMethod().Invoke(error, null));
                    }
                    else
                    {
                        detail = error.Message;
                    }


                    // create a fault message containing our FaultContract object
                    fault = Message.CreateMessage(version, "", error_to_publish, new DataContractJsonSerializer(error_to_publish.GetType()));

                    // tell WCF to use JSON encoding rather than default XML
                    fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
                    // SQLdm 10.0 (Swati Gogia): to display status code in case where error is WebFaultException
                    if (error is WebFaultException)
                    {
                        rmp.StatusCode = ((WebFaultException)error).StatusCode;
                    }
                    else
                    {
                        rmp.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    }

                    // put appropraite description here..
                    rmp.StatusDescription = error.Message + "  See fault object for more information.";
                    fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
                    return;
                }
                catch { }
            }

            fault = Message.CreateMessage(version, "", new JsonExceptionWrapper(error), new DataContractJsonSerializer(typeof(JsonExceptionWrapper)));

            fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));

            // return custom error code.
            rmp = new HttpResponseMessageProperty();
            rmp.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            // put appropraite description here..
            rmp.StatusDescription = error.Message.Replace("\n", "&#10;").Replace("\r", "&#13;");

            fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
        }
    }
}
