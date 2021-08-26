//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.UI
{
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Dialogs.Config;

    public class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CollectionServiceConfigWizard());
        }
    }
}
