//------------------------------------------------------------------------------
// <copyright file="DynamicParameterHelpGenerator.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Helpers
{
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;
    using System.Xml;

    public class DynamicParameterHelpUpdater
    {
        private XmlNamespaceManager namespaceManager;
        private XmlDocument document;
        private XmlElement helpItemElement;
        private XmlElement commandElement;
        private XmlElement detailsElement;
        
        private string commandName;
        private string path;
        private string verb;

        private const string commandNamespace = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        private const string mamlNamespace = "http://schemas.microsoft.com/maml/2004/10";
        private const string devNamespace = "http://schemas.microsoft.com/maml/dev/2004/10";
        private const string defaultNamespace = "http://msh";

        public DynamicParameterHelpUpdater()
        {
            document = new XmlDocument();
            document.Load("SQLdmSnapin.dll-Help.xml");

            namespaceManager = new XmlNamespaceManager(document.NameTable);
            namespaceManager.AddNamespace(String.Empty, defaultNamespace);
            namespaceManager.AddNamespace("maml", mamlNamespace);
            namespaceManager.AddNamespace("command", commandNamespace);
            namespaceManager.AddNamespace("dev", devNamespace);

            helpItemElement = document.DocumentElement;            
        }

        public static void Main(string[] args)
        {
            // read in the project help file, update it with new dynamic parameters, write it out with new name
            DynamicParameterHelpUpdater generator = new DynamicParameterHelpUpdater();
         
            generator.UpdateHelp();
            string xml = generator.document.OuterXml;
            Debug.WriteLine(xml);

            generator.document.Save("SQLdmSnapin.dll-XXXX.xml");
        }

        private void UpdateHelp()
        {
            SQLdmProvider provider = new SQLdmProvider();

            foreach (
                XmlNode nameElement in
                    helpItemElement.SelectNodes("command:command/command:details/command:name",
                                                         namespaceManager))
            {
                commandName = nameElement.InnerText;
                Debug.Print("Updating help for {0}", commandName);

                switch (commandName)
                {
                    case "Get-SqlServers":
                        continue;
                    case "New-SQLdmDrive":
                        continue;
                    case "New-SQLdmUser":
                        path = "/AppSecurity/bozo";
                        verb = VerbsCommon.New;
                        break;
                    case "New-SQLdmMonitoredInstance":
                        path = "/Instances/KGOOLSBEED";
                        verb = VerbsCommon.New;
                        break;
                    case "Remove-SQLdmMonitoredInstance":
                        path = "/Instances/KGOOLSBEED";
                        verb = VerbsCommon.Remove;
                        break;
                    case "Remove-SQLdmUser":
                        path = "/AppSecurity/bozo";
                        verb = VerbsCommon.Remove;
                        break;
                    case "Grant-SQLdmPermission":
                        path = "/AppSecurity/bozo/KGOOLSBEED";
                        verb = VerbsSecurity.Grant;
                        break;
                    case "Revoke-SQLdmPermission":
                        path = "/AppSecurity/bozo/KGOOLSBEED";
                        verb = VerbsSecurity.Revoke;
                        break;
                    case "Set-SQLdmUser":
                        path = "/AppSecurity/bozo";
                        verb = VerbsCommon.Set;
                        break;
                    case "Set-SQLdmAppSecurity":
                        path = "/AppSecurity";
                        verb = VerbsCommon.Set;
                        break;
                    case "Set-SQLdmMaintenanceMode":
                        path = "/Instances/KGOOLSBEED";
                        verb = VerbsCommon.Set;
                        break;
                    case "Set-SQLdmMonitoredInstance":
                        path = "/Instances/KGOOLSBEED";
                        verb = VerbsCommon.Set;
                        break;
                    case "Set-SQldmQueryMonitor":
                        path = "/Instances/KGOOLSBEED";
                        verb = VerbsCommon.Set;
                        break;
                    case "Set-SQLdmQueitTime":
                        path = "/Instances/KGOOLSBEED";
                        verb = VerbsCommon.Set;
                        break;
                }

                detailsElement = (XmlElement) nameElement.ParentNode;
                commandElement = (XmlElement) detailsElement.ParentNode;

                XmlElement syntaxElement = (XmlElement)
                    commandElement.GetElementsByTagName("syntax", namespaceManager.LookupNamespace("command"))[0];
                XmlElement parametersElement = (XmlElement)
                    commandElement.GetElementsByTagName("parameters", namespaceManager.LookupNamespace("command"))[0];

                List<RuntimeDefinedParameter> addList = new List<RuntimeDefinedParameter>();

                RuntimeDefinedParameterDictionary dynamicParameters = provider.GetDynamicParameters(verb, path);
                if (dynamicParameters != null)
                {
                    foreach (XmlElement syntaxItem in syntaxElement.SelectNodes("command:syntaxItem", namespaceManager))
                    {
                        foreach (RuntimeDefinedParameter rdp in dynamicParameters.Values)
                        {
                            // see if the parameter is already in the help doc
                            XmlElement parameterNode = (XmlElement)
                                                       syntaxItem.SelectSingleNode(
                                                           String.Format("command:parameter/maml:name[.=\"{0}\"]",
                                                                         rdp.Name), namespaceManager);
                            if (parameterNode == null)
                            {
                                addList.Add(rdp);
                            }
                        }
                        foreach (RuntimeDefinedParameter rdp in addList)
                        {
                            GenerateSyntax(rdp, syntaxItem);
                        }
                        addList.Clear();
                    }
                    foreach (RuntimeDefinedParameter rdp in dynamicParameters.Values)
                    {
                        // see if the parameter is already in the help doc
                        XmlElement parameterNode = (XmlElement)parametersElement.SelectSingleNode(
                                                                         String.Format("command:parameter/maml:name[.=\"{0}\"]",
                                                                         rdp.Name), namespaceManager);
                        if (parameterNode == null)
                        {
                            addList.Add(rdp);
                        }
                    }
                    foreach (RuntimeDefinedParameter rdp in addList)
                    {
                        GenerateParameter(rdp, parametersElement);
                    }
                    addList.Clear();
                }
            }
        }

        private void GenerateParameter(RuntimeDefinedParameter parameter, XmlElement parametersElement)
        {
            XmlElement p = GenerateParameterElement(parameter);
            parametersElement.AppendChild(p);
        }

        private void GenerateSyntax(RuntimeDefinedParameter parameter, XmlElement syntaxElement)
        {
            XmlElement p = GenerateParameterElement(parameter);

            // add a type element
            XmlElement typeElement = document.CreateElement("dev", "type", devNamespace);
            XmlElement nameElement = document.CreateElement("maml", "name", mamlNamespace);
            XmlText nameText = document.CreateTextNode(parameter.ParameterType.Name);
            XmlElement uriElement = document.CreateElement("maml", "uri", mamlNamespace);

            nameElement.AppendChild(nameText);
            typeElement.AppendChild(nameElement);
            typeElement.AppendChild(uriElement);
            p.AppendChild(typeElement);

            // append a default value node
            XmlElement dftElement = document.CreateElement("dev", "defaultValue", devNamespace);
            p.AppendChild(dftElement);

            syntaxElement.AppendChild(p);
        }

        private XmlElement GenerateParameterElement(RuntimeDefinedParameter parameter)
        {
            XmlElement result = document.CreateElement("command", "parameter", commandNamespace);
            ParameterAttribute pa = GetParameterAttribute(parameter);


            result.Attributes.Append(CreateAttribute("required", pa.Mandatory));
            result.Attributes.Append(CreateAttribute("position", pa.Position >= 1 ? pa.Position.ToString() : "named"));

            result.Attributes.Append(CreateAttribute("globbing", false));
            result.Attributes.Append(CreateAttribute("variableLength", parameter.ParameterType.IsArray));

            if (pa.ValueFromPipeline || pa.ValueFromPipelineByPropertyName)
            {
                StringBuilder pis = new StringBuilder("true (");
                if (pa.ValueFromPipeline)
                {
                    pis.Append("ByValue");
                    if (pa.ValueFromPipelineByPropertyName)
                        pis.Append(", ");
                }
                if (pa.ValueFromPipelineByPropertyName)
                    pis.Append("ByPropertyName");
                pis.Append(")");

                result.Attributes.Append(CreateAttribute("pipelineInput", pis.ToString()));
            }
            else
                result.Attributes.Append(CreateAttribute("pipelineInput", false));

            // add the name element
            XmlElement nameElement = document.CreateElement("maml", "name", mamlNamespace);
            result.AppendChild(nameElement);
            nameElement.AppendChild(document.CreateTextNode(parameter.Name));

            // add an empty description element
            XmlElement descElement = document.CreateElement("maml", "description", mamlNamespace);
            result.AppendChild(descElement);
            XmlElement paraElement = document.CreateElement("maml", "para", mamlNamespace);
            descElement.AppendChild(paraElement);

            // add a parameter value element
            XmlElement valueElement = document.CreateElement("command", "parameterValue", commandNamespace);
            valueElement.Attributes.Append(CreateAttribute("required", pa.Mandatory));
            valueElement.Attributes.Append(CreateAttribute("variableLength", parameter.ParameterType.IsArray));
            XmlText text = document.CreateTextNode(parameter.ParameterType.Name);
            valueElement.AppendChild(text);
            result.AppendChild(valueElement);

            return result;
        }

        private ParameterAttribute GetParameterAttribute(RuntimeDefinedParameter parameter)
        {
            foreach (Attribute a in parameter.Attributes)
            {
                if (a is ParameterAttribute)
                    return (ParameterAttribute)a;
            }
            return null;
        }

        private XmlAttribute CreateAttribute<T>(string name, T value)
        {
            XmlAttribute att = document.CreateAttribute(name);
            att.Value = value.ToString();
            return att;
        }

    }
}