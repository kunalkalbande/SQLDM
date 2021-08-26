using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    public partial class AddEditAzureSubscription : Form
    {
        private readonly IAzureSubscription azureSubscription;
        private readonly List<IAzureSubscription> subscriptions;

        public AddEditAzureSubscription(IAzureSubscription subscription,
            List<IAzureSubscription> subscriptions)
        {
            InitializeComponent();
            azureSubscription = subscription;
            this.subscriptions = subscriptions;
            LoadSubscriptionData();
        }

        private void LoadSubscriptionData()
        {
            if (azureSubscription != null)
            {
                textBoxSubscription.Text = azureSubscription.SubscriptionId;
                textBoxDescription.Text = azureSubscription.Description;
            }

            btnOK.Enabled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSubscription.Text))
            {
                return;
            }
            var subscriptionId = textBoxSubscription.Text;
            RepositoryHelper.InsertUpdateAzureSubscriptions(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                subscriptionId, textBoxDescription.Text, azureSubscription != null ? azureSubscription.Id : (long?)null);
            btnOK.Enabled = false;
        }

        private void subscriptionsValidator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            var value = e.Value == null ? null : e.Value.ToString().Trim();

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "subscriptionValidationGroup":
                    if (string.IsNullOrEmpty(value))
                    {
                        e.IsValid = false;
                        break;
                    }

                    var subscriptionId = textBoxSubscription.Text;
                    if (subscriptionId != null && subscriptions.Any(subs => subs.SubscriptionId == subscriptionId) &&
                        (azureSubscription == null || azureSubscription.SubscriptionId != subscriptionId))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text =
                            "The Subscription Id must be unique. This subscription is already present.";
                        break;
                    }

                    e.IsValid = true;
                    break;
                default:
                    e.IsValid = true;
                    break;
            }
        }

        private void AddEditAzureSubscription_Load(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            this.btnOK.Enabled = subscriptionsValidator.Validate(true, false).IsValid;
        }

        /// <summary>
        /// SQLDM-30696 Not able to create New Azure subscription as OK button is disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSubscription_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {

            UpdateControls();
        }
    }
}
