//------------------------------------------------------------------------------
// <copyright file="AlertConfigurationDialog.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Linq;
using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;
	using BBS.TracerX;
	using ChartFX.WinForms.Gauge;
	using ChartFX.WinForms.Gauge.HitDetection;
	using Common;
	using Helpers;
	using Common.Configuration;
	using Common.Data;
	using Common.Events;
	using Common.Objects;
	using Common.Objects.ApplicationSecurity;
	using Common.Thresholds;
	using Common.UI.Dialogs;
	using Infragistics.Win;
	using Infragistics.Win.UltraWinEditors;
	using Infragistics.Win.UltraWinGrid;
	using Microsoft.SqlServer.MessageBox;
	using Objects;
	using Properties;
	using Wintellect.PowerCollections;
	using Appearance = Infragistics.Win.Appearance;
	using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
	using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.DesktopClient.Controls;
    using Infragistics.Windows.Themes;

    public partial class AlertConfigurationDialog : BaseDialog
	{
		#region Private Fields
		private static readonly Logger LOG = Logger.GetLogger(typeof(AlertConfigurationDialog));

		private const string DialogTitle = "Alert {1} - {0}";

		private bool editingAlertTemplate;
		private string templateName;

		private string previousRankVal;
		private bool isCheckConfirmRankDialog = false;

		private Control focused = null;
		private AlertConfiguration configuration;
		private System.ComponentModel.PropertyChangedEventHandler listPropertyChangedEventHandler;
		// editors...
		private EditorWithText textEditor;
		private MonitoredSqlServer selectedInstance;

		//private List<string> jobCategories;
		//private List<string> databaseNames;
		private bool currentChanging;
		private bool gaugeChanging;
		private int? initialSelection;

		private Indicator infoIndicator;
		private Indicator warningIndicator;
		private Indicator criticalIndicator;
		private Section okSection;
		private Section warningSection;
		private Section criticalSection;
		private Section infoSection;
		private LinearStrip blueBar;
		private Dictionary<int, BaselineItemData> baselineData = new Dictionary<int, BaselineItemData>();
		private List<BaselineItemData> recommendations = new List<BaselineItemData>();

		private BindingList<AlertConfigurationItem> alertThresholds = new BindingList<AlertConfigurationItem>();

		private PermissionType permissionType = PermissionType.View;
		private Dictionary<InstanceType, List<string>> serverInstanceLists = new Dictionary<InstanceType, List<string>>();

		//The dictionary named "oldData" will contain a copy of all the “AlertConfigurationItem” before the user do any change into the UI.
		Dictionary<Pair<int, string>, AlertConfigurationItem> oldData = new Dictionary<Pair<int, string>, AlertConfigurationItem>();
		Type checktype;
		#endregion

		#region Constructors

		public AlertConfigurationDialog(int instanceId, bool editingAlertTemplate)
			: this(instanceId, editingAlertTemplate, "")
		{
		}

		public AlertConfigurationDialog(int instanceId, bool editingAlertTemplate, string template)
		{
			//[START] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --Retaining Size of Alert Configuration Dialog
			Size retainedSize = Settings.Default.AlertConfigurationDialogSize;
			//[END] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --Retaining Size of Alert Configuration Dialog
			InitializeComponent();
			//[START] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --Retaining Size of Alert Configuration Dialog
			if (retainedSize != null)
				Settings.Default.AlertConfigurationDialogSize = retainedSize;
			//[START] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --Retaining Size of Alert Configuration Dialog
			// Perform auto resize column.
			alertsGrid.DisplayLayout.Bands[0].Columns["Enabled"].PerformAutoResize();

			initialSelection = null;

			
			


			TemplateName = template;

			if (Size != Settings.Default.AlertConfigurationDialogSize)
				Size = Settings.Default.AlertConfigurationDialogSize;

			// make sure the minimum height of the dialog gets bumped at least once after an upgrade.
			if (Settings.Default.AlertConfigurationDialogSizeUpgradeMarker > 0)
			{
				// ensure the height is at least as big as the marker
				if (Size.Height < Settings.Default.AlertConfigurationDialogSizeUpgradeMarker)
					Height = Settings.Default.AlertConfigurationDialogSizeUpgradeMarker;
				Settings.Default.AlertConfigurationDialogSizeUpgradeMarker = -1 * Settings.Default.AlertConfigurationDialogSizeUpgradeMarker;
			}

			if (splitContainer.SplitterDistance != Settings.Default.AlertConfigurationDialogDividerLocation)
				splitContainer.SplitterDistance = Settings.Default.AlertConfigurationDialogDividerLocation;

			this.editingAlertTemplate = editingAlertTemplate;
			alertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
			alertConfigurationGrid.DrawFilter = new HideFocusRectangleDrawFilter();
			//10.0 Srishti purohit // For baseline alert
			//Making copy of State grid to give baseline alert functioanlity
			baselineAlertConfigurationGrid.DrawFilter = new HideFocusRectangleDrawFilter();
			alertTabControl.DrawFilter = new HideFocusRectangleDrawFilter();
			instanceConfigGrid.DrawFilter = new HideFocusRectangleDrawFilter();
			listPropertyChangedEventHandler = new System.ComponentModel.PropertyChangedEventHandler(list_PropertyChanged);


			// Set the dialog title
			if (editingAlertTemplate)
				if (string.IsNullOrEmpty(TemplateName))
					Text = string.Format(DialogTitle, "< Unknown >", "Template");
				else
					Text = string.Format(DialogTitle, TemplateName, "Template");
			else
				if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
			{
				selectedInstance = ApplicationModel.Default.ActiveInstances[instanceId];
				Text = string.Format(DialogTitle, selectedInstance.InstanceName, "Configuration");
			}
			else
				Text = string.Format(DialogTitle, "< Unknown >", "Configuration");

			// Populating the list of existing databases
			serverInstanceLists.Clear();
			existingDatabasesBackgroundWorker.RunWorkerAsync();

			int numberInstances = ApplicationModel.Default.ActiveInstances.Count;
			if (!editingAlertTemplate)
				numberInstances--;

			applyTemplateButton.Enabled = numberInstances > 0;

			LoadAlerts(instanceId);

			FillOldAuditData();

			instanceConfigPanel.Visible = false;
			configurationLayoutPanel.Visible = true;

			/* load user security token and set user permissions */
			UserToken token = ApplicationModel.Default.UserToken;

			// We don't have an instance id when editing the default alert configuration.
			// Have to check if user is a sysadmin otherwise they be read only.
			if (token.IsSecurityEnabled)
			{
				if (editingAlertTemplate)
				{
					permissionType = token.IsSQLdmAdministrator ? PermissionType.Administrator : PermissionType.View;
				}
				else
				{
					permissionType = ApplicationModel.Default.UserToken.GetServerPermission(selectedInstance.Id);
				}
			}
			else
			{
				permissionType = PermissionType.Administrator;
			}

			AdaptFontSize();
			SetGridTheme();
			updateLinearScaleFontAsPerTheme(this.linearScale1);
			ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
		}

		#endregion

		#region Properties

		public string TemplateName
		{
			get
			{
				if (String.IsNullOrEmpty(templateName))
					return "";
				else
					return templateName;
			}
			set { templateName = value; }
		}


		/// <summary>
		/// Select the given metric in the alerts grid
		/// </summary>
		/// <param name="metric"></param>
		public void Select(Metric metric)
		{
			Select((int)metric);
		}

		public AlertConfiguration Configuration
		{
			get { return configuration; }
			set
			{
				if (configuration != null)
				{
					configuration.PropertyChanged -= listPropertyChangedEventHandler;
					configuration.ItemPropertyChanged -= listPropertyChangedEventHandler;
				}

				configuration = value;

				configuration.PropertyChanged += listPropertyChangedEventHandler;
				configuration.ItemPropertyChanged += listPropertyChangedEventHandler;

				UpdateControls();
			}
		}

		public BindingList<AlertConfigurationItem> AlertThresholds
		{
			get { return alertThresholds; }
			set { alertThresholds = value; }
		}

		#endregion

		#region Helper Methods

		public void Select(int metricID)
		{
			initialSelection = metricID;
			foreach (UltraGridRow row in alertsGrid.Rows.GetAllNonGroupByRows())
			{
				AlertConfigurationItem item = row.ListObject as AlertConfigurationItem;
				if (item != null)
				{
					if (item.MetricID == metricID)
					{
						row.Activate();
						alertsGrid.Selected.Rows.Clear();
						alertsGrid.Selected.Rows.Add(row);
						break;
					}
				}
			}
		}

		private void UpdateControls()
		{
			bool permitted = permissionType > PermissionType.ReadOnlyPlus;
			applyButton.Enabled = configuration.IsChanged;
			createTemplateButton.Enabled = !configuration.IsChanged;

			var item = instanceConfigGrid.Selected.Rows.Count > 0 ? (AlertConfigurationItem)instanceConfigGrid.Selected.Rows[0].ListObject : null;
			btnAddInstance.Enabled = permitted ? true : false;
			btnEditInstance.Enabled = permitted && instanceConfigGrid.Selected.Rows.Count > 0;
			btnDeleteInstance.Enabled = permitted && (instanceConfigGrid.Selected.Rows.Count > 0) &&
										(item != null && !item.IsDefaultThreshold);
			instanceConfigGrid.Rows.Refresh(RefreshRow.RefreshDisplay);

			alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
		}

		private void LoadAlerts(int instanceId)
		{
			if (configuration == null)
			{
				if (editingAlertTemplate)
					Configuration = RepositoryHelper.GetDefaultAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId);
				else
					Configuration = RepositoryHelper.GetAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId);
			}
			
			LoadAlertThresholdList();

			configBindingSource.DataSource = configuration;
			instanceConfigGrid.DataSource = AlertThresholds;
		}

		private static Control GetFocusedControl(Control.ControlCollection controls)
		{
			Control focusedControl = null;

			foreach (Control control in controls)
			{
				if (control.Focused)
				{
					focusedControl = control;
				}
				else if (control.ContainsFocus)
				{
					return GetFocusedControl(control.Controls);
				}
			}

			return focusedControl ?? controls[0];
		}

		private bool IsMetricEnabled(Metric metric)
		{
			foreach (UltraGridRow row in alertsGrid.Rows)
			{
				if (row.IsGroupByRow)
				{
					UltraGridGroupByRow rowGroup = row as UltraGridGroupByRow;
					foreach (UltraGridRow element in rowGroup.Rows)
					{
						if ((int)element.Cells["MetricID"].Value == (int)metric)
						{
							LOG.Info("row is equal to metric");
							return (bool)element.Cells["Enabled"].Value;
						}
					}
				}
			}
			return false;
		}

		private void UpdateInformationLabel(object item)
		{
			if (item is FlattenedThreshold)
			{
				item = ((FlattenedThreshold)item).GetConfigurationItem();
			}

			if (item is AlertConfigurationItem)
			{
				bool show = false;
				MetricThresholdEntry threshold = ((AlertConfigurationItem)item).ThresholdEntry;
				if (threshold.Data != null)
				{
					AdvancedAlertConfigurationSettings settings = threshold.Data as AdvancedAlertConfigurationSettings;
					if (settings != null && settings.SnoozeInfo != null)
					{
						SnoozeInfo snoozeInfo = settings.SnoozeInfo;
						show = snoozeInfo.IsSnoozed(DateTime.UtcNow);
						if (show)
						{
							informationLabel.Text =
								String.Format("Alerts snoozed until {0} by {1}.", snoozeInfo.StopSnoozing.ToLocalTime(),
											  snoozeInfo.UnsnoozedBy);
						}
					}
				}
				informationLabelPanel.Visible = show;
			}
		}

		private bool GridHasScrollBar(UltraGrid grid)
		{
			UltraGridUIElement elem = grid.DisplayLayout.UIElement;
			if (elem != null)
				return elem.GetDescendant(typeof(RowScrollbarUIElement)) != null;

			return false;
		}

		// PropertyChanged event handler
		void list_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (sender is AlertConfiguration)
			{
				UpdateControls();
			}

			if (sender is AlertConfigurationItem)
			{
				alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
				instanceConfigGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
			}
		}

		private bool SaveChanges()
		{
			try
			{
				var managementService =
					ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

				IList<MetricThresholdEntry> thresholdList = new List<MetricThresholdEntry>();

				configuration.PrepareChangedItems();
				if (editingAlertTemplate)
					managementService.ChangeAlertTemplateConfiguration(configuration);
				else
					managementService.ChangeAlertConfiguration(configuration);

				bool thresholdsChanged = false;
				foreach (object o in configuration.ChangeItems)
				{
					if (o is MetricThresholdEntry)
					{
						if (!thresholdsChanged)
							thresholdsChanged = true;

						thresholdList.Add((MetricThresholdEntry)o);

					}
				}

				//if (!editingAlertTemplate)
				//{
				//    UpdateSharedAlertConfiguration(configuration.InstanceID, changedThresholds);
				//}

				int instanceCount = ApplicationModel.Default.ActiveInstances.Count;
				IList<int> targetServers = new List<int>();
				if (!editingAlertTemplate && (instanceCount > 1))
				{
					if (thresholdsChanged)
					{
						ApplicationMessageBox box = new ApplicationMessageBox();
						box.Text = editingAlertTemplate
									   ? "Would you like to apply this template to existing SQL Server instances?"
									   : "Would you like to apply these changes to other servers, tags or templates?";
						box.ShowCheckBox = false;
						box.Symbol = ExceptionMessageBoxSymbol.Question;
						box.Caption = Text;
						box.Buttons = ExceptionMessageBoxButtons.YesNo;

						if (box.Show(this) == DialogResult.Yes)
						{
							List<int> excluded = new List<int>();

							if (selectedInstance != null)
								excluded.Add(selectedInstance.Id);

							excluded.AddRange(
								from instancePermission in ApplicationModel.Default.UserToken.AssignedServers
								where instancePermission.PermissionType == (PermissionType.View | PermissionType.ReadOnlyPlus)
								select instancePermission.Server.SQLServerID);

							ArrayList metricsConfigurationListItems = configuration.GetAllAlertMetricChanged();
							SelectServersDialog dialog =
								new SelectServersDialog(
									"Select the template, tag, or servers to which you want to apply the alert configuration changes.",
									excluded, metricsConfigurationListItems);

							if (dialog.ShowDialog(this) == DialogResult.OK)
							{
								targetServers = dialog.SelectedServers;

								managementService.UpdateAlertConfigurations(thresholdList, targetServers);
							}
						}
					}
				}
				//Fill the logs about Alerts into the AuditingEngine
				FillAuditEntity(targetServers, thresholdList);
			}
			catch (Exception e)
			{
				ApplicationMessageBox.ShowError(this, "Unable to save alert configuration changes.", e);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Fill all the "AlertConfigurationItems" that the dialog shows in to the "oldData" dictionary in order to have all the alerts to the comparition process.
		/// This method will be called after load all the alerts into "AlertConfiguration" (configuration property)
		/// </summary>
		private void FillOldAuditData()
		{
			if (configuration.ItemList != null)
			{
				foreach (AlertConfigurationItem item in configuration.ItemList)
				{
					AddOldDataItem(ObjectHelper.Clone(item));
				}
			}
		}

		/// <summary>
		/// This method is the metod that will fill the oldData dictionary with an AlertConfigurationItem copy.
		/// It will remove the item if there is an old one before adding the new one.
		/// </summary>
		/// <param name="item">Is the AlertConfigurationItem that needs to add into the oldData dictionary</param>
		private void AddOldDataItem(AlertConfigurationItem item)
		{
			Pair<int, string> pair = new Pair<int, string>(item.MetricID, item.MetricInstance);
			if (oldData.ContainsKey(pair))
			{
				oldData.Remove(pair);
			}

			oldData.Add(pair, item);
		}

		/// <summary>
		/// Fill out the Auditable Entity with information of database
		/// </summary>
		/// <param name="item"> AlertConfigurationItem</param>
		/// <returns>AuditableEntity</returns>
		private AuditableEntity FillOutAuditableEntity(AlertConfigurationItem item)
		{
			AuditableEntity auditableEntity = item.GetAuditableEntity();

			auditableEntity.AddMetadataProperty(String.Format("{0} name", item.ThresholdEntry.MetricInstanceType.ToString()),
												item.ThresholdEntry.MetricInstanceName);
			auditableEntity.AddMetadataProperty(String.Format("{0} action", item.ThresholdEntry.MetricInstanceType.ToString()),
												item.ThresholdEntry.State.ToString());
			List<Pair<string, string>> properties = item.GetDetailProperties(item);

			if (properties.Count > 0)
			{
				auditableEntity.MetadataProperties.AddRange(properties);
			}

			return auditableEntity;
		}

		/// <summary>
		/// Check if we have changed in Alert Configuration
		/// </summary>
		/// <param name="oldAlertConfiguration"></param>
		/// <param name="alertConfiguration"></param>
		/// <returns></returns>
		private AuditableEntity AlertConfigurationDataBaseChange(AlertConfigurationItem oldAlertConfiguration, AlertConfigurationItem alertConfiguration)
		{
			AuditableEntity auditableThreshold = new AuditableEntity();
			auditableThreshold.Name = alertConfiguration.Name;

			if (alertConfiguration.Enabled)
			{
				if (!oldAlertConfiguration.Enabled)
				{
					// The state between old configuration and new configuration has been changed from
					// disable to enable.
					auditableThreshold.AddMetadataProperty("Alert status", "Enabled");
				}

				var temporaryAuditableEntity = FillOutAuditableEntity(alertConfiguration);
				foreach (Pair<string, string> metadataProperty in temporaryAuditableEntity.MetadataProperties)
				{
					auditableThreshold.AddMetadataProperty(metadataProperty.First, metadataProperty.Second);
				}
			}
			else
			{
				auditableThreshold.AddMetadataProperty("Alert status", "Disabled");
			}

			return auditableThreshold;
		}

		/// <summary>
		/// This method will log all the alerts that was changed by the user into the AuditingEngine.
		/// </summary>
		/// <param name="targetServers">The list of the servers that the user wants to apply the changes. It could be a new List<int> without any item</param>
		/// <param name="thresholdList">This is the MetricThresholdEntry list that contains only the items that was changed by the user</param>
		private void FillAuditEntity(IList<int> targetServers, IList<MetricThresholdEntry> thresholdList)
		{
			foreach (MetricThresholdEntry metricThresholdEntry in thresholdList)
			{
				var auditableEntity = new AuditableEntity();

				//using LINQ process in order to have the correct Item into configuration.ItemList. Note that to access to teh item it is necesary to iterate with a foreach.
				var alertConfigurationItems = from item in configuration.ItemList
											  where item.MetricID == metricThresholdEntry.MetricID
											  select item;

				foreach (AlertConfigurationItem item in alertConfigurationItems)
				{
					Pair<int, string> pairkey = new Pair<int, string>(item.MetricID,
																	  item.MetricInstance);
					if (oldData.ContainsKey(pairkey))
					{
						if (metricThresholdEntry.MetricInstanceType == InstanceType.Database || metricThresholdEntry.MetricInstanceType == InstanceType.Disk)
						{
							if (metricThresholdEntry.MetricInstanceName == item.MetricInstance && item.IsNotFakeThresholdChanged())
							{
								auditableEntity = AlertConfigurationDataBaseChange(oldData[pairkey], item);
							}
						}
						else
						{
							auditableEntity = item.GetAuditableEntity(oldData[pairkey]);
						}
					}
					else if (metricThresholdEntry.State == ThresholdState.Added)
					{
						auditableEntity = FillOutAuditableEntity(item);
					}
				}

				if (auditableEntity.MetadataProperties.Count > 0)
				{
					if (selectedInstance != null && !String.IsNullOrEmpty(selectedInstance.InstanceName))
					{
						auditableEntity.AddMetadataProperty("Affected Server", this.selectedInstance.InstanceName);
					}


					if (targetServers != null)
					{
						foreach (int targetId in targetServers)
						{
							auditableEntity.AddMetadataProperty("Affected Server", ApplicationModel.Default.ActiveInstances[targetId].ToString());
						}
					}

					AuditingEngine.Instance.ManagementService =
						ManagementServiceHelper.GetDefaultService(
							Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
					AuditingEngine.Instance.SQLUser =
						Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity
							? AuditingEngine.GetWorkstationUser()
							: Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
					AuditingEngine.Instance.LogAction(auditableEntity, AuditableActionType.AlertConfigurationChanged);

					//need to see if we applying to others servers if we do  will be audit copy to
					if (targetServers != null && targetServers.Count > 0)
					{
						AuditingEngine.Instance.LogAction(auditableEntity,
														  AuditableActionType.CopyAlertConfigFromServerToServer, this.selectedInstance.InstanceName);
					}
				}
			}
		}

		private void LoadAlertThresholdList()
		{
			if (AlertThresholds.Count > 0)
				AlertThresholds.Clear();

			foreach (AlertConfigurationItem item in configuration.ItemList)
			{
				AlertThresholds.Add(item);
			}
		}

		private void AddAlertThresholdItem(AlertConfigurationItem item)
		{
			try
			{
				AlertThresholds.Add(item);
				Configuration.AddEntry(item);

			}
			catch (Exception e)
			{
				LOG.Error("Error adding alert threshold item to the server alert configuration for {0} ",
						  item.ThresholdEntry.MonitoredServer.InstanceName, e);
				throw;
			}
		}

		#endregion

		#region splitContainer Event Methods

		private void splitContainer_MouseDown(object sender, MouseEventArgs e)
		{
			focused = GetFocusedControl(Controls);
		}

		private void splitContainer_MouseUp(object sender, MouseEventArgs e)
		{
			if (focused != null)
			{
				focused.Focus();
				focused = null;
			}
		}

		private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (splitContainer.SplitterDistance != Settings.Default.AlertConfigurationDialogDividerLocation)
				Settings.Default.AlertConfigurationDialogDividerLocation = splitContainer.SplitterDistance;
		}

		#endregion

		#region Alerts Grid Methods

		private void alertsGrid_MouseClick(object sender, MouseEventArgs e)
		{
			UltraGrid grid = sender as UltraGrid;
			if (e.Button == MouseButtons.Left)
			{
				UIElement selectedElement = grid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

				// see if the user clicked the ... button in the value column of the alert 
				// configuration grid.  This cell is set as non-editable so we have to look
				// for the mouse click in order to show the dialog.
				if (grid == alertConfigurationGrid)
				{
					object seco = selectedElement.GetContext();
					// only the Value column has an EditorButton
					if (seco is EditorButton)
					{
						object element = selectedElement.GetAncestor(typeof(CellUIElement)) as CellUIElement;
						if (element != null)
						{
							EditCell(((CellUIElement)element).Cell);
						}
						return;
					}
				}
				else if (grid == baselineAlertConfigurationGrid)
				{
					object seco = selectedElement.GetContext();
					// only the Value column has an EditorButton
					if (seco is EditorButton)
					{
						object element = selectedElement.GetAncestor(typeof(CellUIElement)) as CellUIElement;
						if (element != null)
						{
							EditCell(((CellUIElement)element).Cell);
						}
						return;
					}
				}

				if (permissionType < PermissionType.Modify || !(selectedElement is CheckIndicatorUIElement || selectedElement is ImageUIElement
					|| selectedElement is EditorWithTextDisplayTextUIElement))
					return;

				// logic to handle toggling a checkbox in a non-editable (no cell selection) column
				object contextObject = selectedElement.GetContext();
				if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
				{
					if (((UltraGridColumn)contextObject).Key == "Enabled")
					{
						UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;
                        selectedRow.Appearance.BackColor = Settings.Default.ColorScheme == "Dark" ?
                            ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;

                        if (selectedRow != null)
						{
							bool newValue = true;
							CurrencyManager cm =
								((ICurrencyManagerProvider)grid.DataSource).GetRelatedCurrencyManager(grid.DataMember);
							PropertyDescriptor descriptor = cm.GetItemProperties()["Enabled"];
							object selectedObject = selectedRow.ListObject;
							if (selectedObject is FlattenedThreshold && ((FlattenedThreshold)selectedObject).IsMutuallyExclusive)
							{
								// make checkbox work like a radio - clicking sets the value to true
								newValue = true;
							}
							else
							{
								object value = descriptor.GetValue(selectedRow.ListObject);
								if (value is bool)
								{
									if (grid == alertsGrid)
									{
										if ((bool)value == false)
										{

											if ((((Metric)selectedRow.Cells["MetricID"].Value == Metric.BombedJobs) && IsMetricEnabled(Metric.JobCompletion))
												|| (((Metric)selectedRow.Cells["MetricID"].Value == Metric.JobCompletion) && IsMetricEnabled(Metric.BombedJobs)))
											{
												DialogResult result = ApplicationMessageBox.ShowWarning(this,
														   "Both the SQL Server Agent Job Failure and SQL Server Agent Job Completion alerts are now enabled. To keep duplicate alerts from being generated, consider disabling the SQL Server Agent Job Failure alert.",
														   ExceptionMessageBoxButtons.OK);
											}
										}
									}
									newValue = !((bool)value);
								}
							}
							descriptor.SetValue(selectedRow.ListObject, newValue);
						}

						// if this is the alert configuration grid then force a refresh of the data
						if (grid == alertConfigurationGrid)
						{
							if (selectedRow != null)
							{
								CellEventArgs cea = new CellEventArgs(selectedRow.Cells["Enabled"]);
								alertConfigurationGrid_AfterCellUpdate(alertConfigurationGrid, cea);
							}
							else
								alertConfigurationGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
						}
						else if (grid == baselineAlertConfigurationGrid)
						{
							if (selectedRow != null)
							{
								CellEventArgs cea = new CellEventArgs(selectedRow.Cells["Enabled"]);
								baselineAlertConfigurationGrid_AfterCellUpdate(alertConfigurationGrid, cea);
							}
							else
								baselineAlertConfigurationGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
						}
					}
				}
			}
		}

		private static ImageBackgroundStretchMargins BG_Stretch_Margins = new ImageBackgroundStretchMargins(1, 1, 1, 1);
		private void alertConfigurationGrid_InitializeRow(object sender, InitializeRowEventArgs e)
		{
			FlattenedThreshold threshold = e.Row.ListObject as FlattenedThreshold;
			e.Row.Appearance.BackColor = Settings.Default.ColorScheme == "Dark" ?
							ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;

			// set activation for editable fields based on meta data for the row
			if (!e.Row.Cells["RangeStart"].Column.Hidden)
			{
				Activation activation =
					threshold.Enabled && threshold.IsEditable && permissionType >= PermissionType.Modify
						? Activation.AllowEdit
						: Activation.NoEdit;

				UltraGridCell rangeStartCell = e.Row.Cells["RangeStart"];
				rangeStartCell.Activation = activation;

				if (activation == Activation.AllowEdit)
				{
					Appearance appearance = rangeStartCell.Appearance;
					appearance.ImageBackground = DesktopClient.Properties.Resources.CellBorder;
					appearance.ImageBackgroundStretchMargins = BG_Stretch_Margins;
					appearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
				}
			}
			if (threshold.ThresholdItemType == ThresholdItemType.OK)
			{
				UltraGridCell cell = e.Row.Cells["Enabled"];
				cell.Hidden = true;
			}
			MetricDefinition metaData = threshold.GetConfigurationItem().GetMetaData();
			if (threshold.IsNumeric)
			{
				Type valueType = metaData.ValueType;
				ColumnStyle style = ColumnStyle.IntegerNonNegative;
				if (valueType == typeof(double) || valueType == typeof(float))
					style = ColumnStyle.DoubleNonNegative;
				e.Row.Cells["RangeStart"].Style = style;
				e.Row.Cells["RangeStart"].Column.MaskInput = (metaData.IsCustom) ? "999999999999999" : null;

				e.Row.Cells["RangeStart"].Hidden = !threshold.Enabled;
				e.Row.Cells["RangeEnd"].Hidden = !threshold.Enabled;
			}
			checktype = checktype = metaData.ValueType;
		}

		private void configBindingSource_CurrentChanged(object sender, System.EventArgs e)
		{
			try
			{
				currentChanging = true;

				// whenever the current row changes reconfigure the grid to show the correct columns
				UltraGridColumn valueColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["Value"];
				UltraGridColumn startColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];
				UltraGridColumn endColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeEnd"];
				UltraGridColumn enabledColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["Enabled"];

				// whenever the current row changes reconfigure the grid to show the correct columns
				//10.0 SQLdm Srishti Purohit -- for making key press validation work for baselinealertgrid
				UltraGridColumn startColumnBaseline = baselineAlertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];

				//SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- capturing the default column in the alert grid
				UltraGridColumn alertConfigValueColumnFake = alertsGrid.DisplayLayout.Bands[0].Columns["ConfiguredAlertValue"];


				AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
				if (item != null)
				{
					
					MetricDefinition metaData = item.GetMetaData();
					ThresholdOptions options = metaData.Options;
					System.Type valueType = metaData.ValueType;
					bool multiValued = (metaData.Options & ThresholdOptions.ContainedValueList) == ThresholdOptions.ContainedValueList;
					bool isSingleAlertTypeConfigured = (metaData.Options & ThresholdOptions.SingleAlertConfig) == ThresholdOptions.SingleAlertConfig; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- if the current value has onyl one alert type enable for one instance

					if (item.IsMultiInstance)
					{

						UltraGridBand band = instanceConfigGrid.DisplayLayout.Bands[0];
						band.ColumnFilters["MetricID"].ClearFilterConditions();

						band.ColumnFilters["MetricID"].FilterConditions.Add(FilterComparisionOperator.Equals,
																			item.MetricID);
						band.Columns["Rank"].Hidden = true;//30242
						//SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- Hide the deault instance row for preferred Node Metric
						//if (item.MetricID == (int)Metric.PreferredNodeUnavailability)
						//{
						//    band.ColumnFilters["MetricInstance"].ClearFilterConditions();
						//    band.ColumnFilters["MetricInstance"].FilterConditions.Add(FilterComparisionOperator.NotEquals,
						//        MetricThresholdEntry.DEFAULT_THRESHOLD_NAME);
						//}
						//else
						//{
						//    band.ColumnFilters["MetricInstance"].ClearFilterConditions();
						//    band.ColumnFilters["MetricInstance"].FilterConditions.Add(FilterComparisionOperator.Equals,
						//        MetricThresholdEntry.DEFAULT_THRESHOLD_NAME);
						//}
						//if (instanceConfigGrid.ActiveRowScrollRegion.FirstRow !=null)
						instanceConfigGrid.ActiveRowScrollRegion.FirstRow.Selected = true;

						instanceConfigGrid.Rows.Band.Columns["MetricInstance"].Header.Caption =
							item.InstanceType.ToString();

						var range1Column = instanceConfigGrid.DisplayLayout.Bands[0].Columns["RangeStart1"];
						var range2Column = instanceConfigGrid.DisplayLayout.Bands[0].Columns["RangeStart2"];
						var range3Column = instanceConfigGrid.DisplayLayout.Bands[0].Columns["RangeStart3"];
						var infoEnabledColumn = instanceConfigGrid.DisplayLayout.Bands[0].Columns["IsInfoEnabled"];
						var warnEnabledColumn = instanceConfigGrid.DisplayLayout.Bands[0].Columns["IsWarningEnabled"];
						var critEnabledColumn = instanceConfigGrid.DisplayLayout.Bands[0].Columns["IsCriticalEnabled"];
						var alertConfigValueColumn = instanceConfigGrid.DisplayLayout.Bands[0].Columns["ConfiguredAlertValue"]; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- capturing the configalertvalue column in the alert grid

						if (multiValued)
						{
							range1Column.Hidden = range2Column.Hidden = range3Column.Hidden = true;
							infoEnabledColumn.Hidden = warnEnabledColumn.Hidden = critEnabledColumn.Hidden = false;
						}
						else
						{
							range1Column.Hidden = range2Column.Hidden = range3Column.Hidden = false;
							infoEnabledColumn.Hidden = warnEnabledColumn.Hidden = critEnabledColumn.Hidden = true;
						}

						//SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- hiding all the other column and displaying configalertvalue column accordingly
						if (isSingleAlertTypeConfigured)
						{
							range1Column.Hidden = range2Column.Hidden = range3Column.Hidden = true;
							infoEnabledColumn.Hidden = warnEnabledColumn.Hidden = critEnabledColumn.Hidden = true;
							alertConfigValueColumn.Hidden = false;

						}
						else
						{
							alertConfigValueColumn.Hidden = true;
						}

						configurationLayoutPanel.Visible = false;
						instanceConfigPanel.Visible = true;
						UpdateControls();
					}
					else
					{
						configurationLayoutPanel.Visible = true;
						instanceConfigPanel.Visible = false;
					}


					if (!multiValued)
					{
						alertConfigurationGauge.Parent.Visible = true;

						if (metaData.IsConfigurable)
						{
							// hide value and show Range start and end
							startColumn.Hidden = false;
							endColumn.Hidden = false;
							valueColumn.Hidden = true;

							//if (item.MetricID != 8)
							//{
							//    alertConfigurationGauge.Parent.Visible = true;
							//}

							if (metaData.ComparisonType == ComparisonType.GE)
							{
								okSection = alertConfigurationGauge.MainScale.Sections[0];
								infoSection = alertConfigurationGauge.MainScale.Sections[1];
								warningSection = alertConfigurationGauge.MainScale.Sections[2];
								criticalSection = alertConfigurationGauge.MainScale.Sections[3];
								infoIndicator = alertConfigurationGauge.MainScale.Indicators[0];
								warningIndicator = alertConfigurationGauge.MainScale.Indicators[1];
								criticalIndicator = alertConfigurationGauge.MainScale.Indicators[2];
							}
							else
							{
								okSection = alertConfigurationGauge.MainScale.Sections[3];
								infoSection = alertConfigurationGauge.MainScale.Sections[2];
								warningSection = alertConfigurationGauge.MainScale.Sections[1];
								criticalSection = alertConfigurationGauge.MainScale.Sections[0];
								infoIndicator = alertConfigurationGauge.MainScale.Indicators[2];
								warningIndicator = alertConfigurationGauge.MainScale.Indicators[1];
								criticalIndicator = alertConfigurationGauge.MainScale.Indicators[0];
							}
							if (blueBar == null)
							{
								blueBar = alertConfigurationGauge.MainScale.Stripes[0];
							}

							// calc the high end of the gauge
							decimal gaugeMax = metaData.MaxValue;
							if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
							{
								long criticalThreshold = (long)Convert.ChangeType(item.RangeStart3, typeof(long));
								gaugeMax = metaData.GetVisualUpperBound(criticalThreshold);
							}
							alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
							alertConfigurationGauge.MainScale.Sections[3].Max = alertConfigurationGauge.MainScale.Max;
							// set the indicators to match the threshold values
							infoIndicator.Value = item.RangeStart1;
							warningIndicator.Value = item.RangeStart2;
							criticalIndicator.Value = item.RangeStart3;

							startColumn.MinValue = metaData.MinValue;
							startColumn.MaxValue = metaData.MaxValue;
							startColumnBaseline.MinValue = metaData.MinValue;
							startColumnBaseline.MaxValue = metaData.MaxValueBaseline;

							ConfigureGauge(item, true);
							UpdateBlueBar(item);
						}
						else
						{
							// allow changing enabled/disabled only
							startColumn.Hidden = true;
							endColumn.Hidden = true;
							valueColumn.Hidden = true;
							alertConfigurationGauge.Parent.Visible = false;
						}
					}
					else
					{
						alertConfigurationGauge.Parent.Visible = false;
						// hide the Range start and end columns and show value
						valueColumn.Hidden = false;
						startColumn.Hidden = true;
						endColumn.Hidden = true;
					}

					//SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- hiding all the other column when not needed
					if (isSingleAlertTypeConfigured)
					{
						valueColumn.Hidden = true;
						startColumn.Hidden = true;
						endColumn.Hidden = true;
					}


					if ((options & ThresholdOptions.MutuallyExclusive) == ThresholdOptions.MutuallyExclusive)
						enabledColumn.ValueList = alertConfigurationGrid.DisplayLayout.ValueLists["RadioButtons"];
					else
						enabledColumn.ValueList = alertConfigurationGrid.DisplayLayout.ValueLists["CheckBoxes"];

					advancedPanel.Visible = item.MetricID != (int)Metric.IndexRowHits &&
											item.MetricID != (int)Metric.FullTextRefreshHours &&
											item.MetricID != (int)Metric.Deadlock &&
											item.MetricID != (int)Metric.OldestUnsentMirroringTran;
					// Pruthviraj Nikam: Done changes for 5.1.7 Baseline Alerts     Date: 22-Jan-2019
					//isBaselineEnabledCheckBox.Visible = checkMetricSupportBaselineAlert(item.MetricID);
					//if (isBaselineEnabledCheckBox.Visible && isBaselineEnabledCheckBox.Checked != item.IsBaselineEnabled) //Change state of checkbox only if it is visible else it should alwys be false
					//    isBaselineEnabledCheckBox.Checked = item.IsBaselineEnabled;

					//SQLDM-30314
					item.IsBaselineEnabled = false;
					editButton.Visible = permissionType >= PermissionType.Modify && !valueColumn.Hidden;
				}
				alertConfigValueColumnFake.Hidden = true; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- hiding the unnecessary Column
				UpdateInformationLabel(item);
				//LoadBaselineThresholds();           // Pruthviraj Nikam: Done changes for 5.1.7 Baseline Alerts     Date: 22-Jan-2019
			}
			finally
			{
				currentChanging = false;
			}
		}
		private bool checkMetricSupportBaselineAlert(int metricId)
		{
			bool isMerticSupportBaselineAlert = false;
			try
			{
				LOG.Info("setting visibility of isBaselineEnabledCheck.");
				switch (metricId)
				{
					case (int)Metric.SQLCPUUsagePct:
					case (int)Metric.UserConnectionPct:
					case (int)Metric.NonDistributedTrans:
					case (int)Metric.NonSubscribedTransNum:
					case (int)Metric.OldestOpenTransMinutes:
					case (int)Metric.SQLMemoryUsagePct:
					case (int)Metric.NonSubscribedTransTime:
					case (int)Metric.ServerResponseTime:
					case (int)Metric.OSMemoryUsagePct:
					case (int)Metric.OSMemoryPagesPerSecond:
					case (int)Metric.OSCPUUsagePct:
					case (int)Metric.OSCPUPrivilegedTimePct:
					case (int)Metric.OSUserCPUUsagePct:
					case (int)Metric.OSCPUProcessorQueueLength:
					case (int)Metric.OSDiskPhysicalDiskTimePct:
					case (int)Metric.OSDiskAverageDiskQueueLength:
					case (int)Metric.ClientComputers:
					case (int)Metric.BlockedSessions:
					case (int)Metric.DataUsedPct:
					case (int)Metric.LogUsedPct:
					case (int)Metric.PageLifeExpectancy:
					case (int)Metric.ProcCacheHitRatio:
					case (int)Metric.VmCPUUtilization:
					case (int)Metric.AlwaysOnEstimatedDataLossTime:
					case (int)Metric.AlwaysOnEstimatedRecoveryTime:
					case (int)Metric.AlwaysOnSynchronizationPerformance:
					case (int)Metric.AlwaysOnLogSendQueueSize:
					case (int)Metric.AlwaysOnRedoQueueSize:
					case (int)Metric.AlwaysOnRedoRate:
						isMerticSupportBaselineAlert = true;
						break;
					default:
						isMerticSupportBaselineAlert = false;
						break;
				}
			}
			catch (Exception ex)
			{
				LOG.Error("Error while setting visibility of isBaselineEnabledCheck. " + ex);
			}
			return isMerticSupportBaselineAlert;
		}

		#endregion

		#region AlertConfigurationDialog Form Event Methods

		private void AlertConfigurationDialog_Load(object sender, System.EventArgs e)
		{
			// changes to the default 

			commentsTextBox.ReadOnly = permissionType < PermissionType.Modify;
			RankTextBox.ReadOnly = permissionType < PermissionType.Modify;
			informationLabelPanel.Enabled =
				applyButton.Visible = permissionType >= PermissionType.Modify;

			applyTemplateButton.Visible = (!editingAlertTemplate && permissionType >= PermissionType.Modify);

			// configure baseline not used on default config and only available to modify types
			configureBaselineButton.Visible = !editingAlertTemplate && permissionType >= PermissionType.Modify;

			// set default button not used on default config and only available to admins
			createTemplateButton.Visible = !editingAlertTemplate && permissionType == PermissionType.Administrator;

			// indicators on gauge can only be moved by modifiers
			alertConfigurationGauge.MainScale.Indicators[0].Draggable =
			   alertConfigurationGauge.MainScale.Indicators[1].Draggable =
			   alertConfigurationGauge.MainScale.Indicators[2].Draggable =
				  permissionType >= PermissionType.Modify;

			textEditor = new EditorWithText();

			// these options don't retain the value set in the designer.  Setting them causes the row
			// to be selected when clicked rather than the default which is cell selection.
			UltraGridColumn column = alertsGrid.DisplayLayout.Bands[0].Columns["Enabled"];

			column.CellClickAction = CellClickAction.RowSelect;
			column.Editor = textEditor;
			column.CellAppearance.ImageHAlign = HAlign.Center;

			column = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["Enabled"];
			column.CellClickAction = CellClickAction.RowSelect;
			column.Editor = textEditor;
			column.CellAppearance.ImageHAlign = HAlign.Center;
			column = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["ThresholdItemType"];
			column.CellClickAction = CellClickAction.RowSelect;
			column = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeEnd"];
			column.CellClickAction = CellClickAction.RowSelect;
			column = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["Value"];
			column.CellClickAction = CellClickAction.RowSelect;
			// set range start as editable
			column = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];
			column.CellClickAction = permissionType <= PermissionType.ReadOnlyPlus ? CellClickAction.RowSelect : CellClickAction.EditAndSelectText;

			//10.0 Srishti purohit // For baseline alert
			//Making copy of State grid to give baseline alert functioanlity
			column = baselineAlertConfigurationGrid.DisplayLayout.Bands[0].Columns["Enabled"];
			column.CellClickAction = CellClickAction.RowSelect;
			column.Editor = textEditor;
			column.CellAppearance.ImageHAlign = HAlign.Center;
			column = baselineAlertConfigurationGrid.DisplayLayout.Bands[0].Columns["ThresholdItemType"];
			column.CellClickAction = CellClickAction.RowSelect;
			column = baselineAlertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeEnd"];
			column.CellClickAction = CellClickAction.RowSelect;
			column = baselineAlertConfigurationGrid.DisplayLayout.Bands[0].Columns["Value"];
			column.CellClickAction = CellClickAction.RowSelect;
			// set range start as editable
			column = baselineAlertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];
			column.CellClickAction = permissionType <= PermissionType.ReadOnlyPlus ? CellClickAction.RowSelect : CellClickAction.EditAndSelectText;


			ultraCheckEditor1_CheckedChanged(checkBox1, null); //CustomCheckbox 4.12 DarkTheme  Babita Manral 

            UltraGridRow[] rows = alertsGrid.Rows.GetAllNonGroupByRows();
			if (rows.Length > 0)
			{
				if (initialSelection != null)
					Select(initialSelection.Value);
				else
				{
					currentChanging = true;
					rows[0].Activate();
					alertsGrid.Selected.Rows.Clear();
					alertsGrid.Selected.Rows.Add(rows[0]);
					currentChanging = false;
				}
			}

			UltraGridBand alertsBand = alertsGrid.DisplayLayout.Bands[0];
			alertsBand.ColumnFilters["MetricInstance"].ClearFilterConditions();
			alertsBand.ColumnFilters["MetricInstance"].FilterConditions.Add(FilterComparisionOperator.Equals, MetricThresholdEntry.DEFAULT_THRESHOLD_NAME);
			alertsBand.Columns["Rank"].AllowRowFiltering = DefaultableBoolean.False;
			alertsBand.Columns["Rank"].Swap(alertsBand.Columns["Name"]);
			alertsBand.Columns["Name"].Width = 175;//30242
			// see if there are any recommendations
			if (!editingAlertTemplate)
				alertRecommendationsBackgroundWorker.RunWorkerAsync();

		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			applyButton.Enabled = false;
			//if (!String.IsNullOrEmpty(RankTextBox.Text))
			//{
			//    if (previousRankVal != RankTextBox.Text)
			//    {
			//        var confirmResult = ApplicationMessageBox.ShowWarning(this, "Important: Rank is a global setting. Editing it will affect all instances, not just one template.", ExceptionMessageBoxButtons.YesNo);
			//        if (confirmResult == DialogResult.No)
			//        {
			//            AlertConfigurationItem item = alertsGrid.ActiveRow.ListObject as AlertConfigurationItem;

			//            item.Rank = Convert.ToInt32(previousRankVal);
			//            alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
			//            RankTextBox.Text = previousRankVal;
			//        }
			//        if (confirmResult == DialogResult.Yes)
			//        {
			//            if (SaveChanges())
			//            {
			//                configuration.ChangedApplied();
			//                UpdateControls();
			//            }
			//            isCheckConfirmRankDialog = true;
			//            return;
			//        }
			//        else
			//        {
			//            isCheckConfirmRankDialog = true;
			//            return;
			//        }
			//    }
			//}

			if (SaveChanges())
			{
				configuration.ChangedApplied();
				UpdateControls();
			}
		}

		private void advancedButton_Click(object sender, EventArgs args)
		{
			AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;

			if (item != null)
			{
				AdvancedAlertConfigurationSettings settings =
					item.ThresholdEntry.Data as AdvancedAlertConfigurationSettings;

				if (settings == null)
				{
					settings =
						new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(item.MetricID),
															   item.ThresholdEntry.Data);
				}

				AdvancedAlertConfigurationDialog aacd = new AdvancedAlertConfigurationDialog(selectedInstance != null ? selectedInstance.Id : 0, item.Name, settings, editingAlertTemplate, true, checktype);

				if (aacd.ShowDialog(this) == DialogResult.OK)
				{
					item.SetData(settings);
				}

			}
			else
			{
				ApplicationMessageBox.ShowError(this, "The alert configuration is null.");
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (configuration.IsChanged)
			{
				//if (!String.IsNullOrEmpty(RankTextBox.Text))
				//{
				//    if (previousRankVal != RankTextBox.Text)
				//    {

				//            var confirmResult = ApplicationMessageBox.ShowWarning(this, "Important: Rank is a global setting. Editing it will affect all instances, not just one template.", ExceptionMessageBoxButtons.YesNo);
				//            if (confirmResult == DialogResult.No)
				//            {
				//                AlertConfigurationItem item = alertsGrid.ActiveRow.ListObject as AlertConfigurationItem;

				//                item.Rank = Convert.ToInt32(previousRankVal);
				//                alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
				//                RankTextBox.Text = previousRankVal;
				//            }
				//            if (confirmResult == DialogResult.Yes)
				//            {
				//                if (!SaveChanges())
				//                    this.DialogResult = DialogResult.None;
				//            }
				//        }
				//}

				if (!SaveChanges())
					this.DialogResult = DialogResult.None;
			}
		}

		private void applyTemplateButton_Click(object sender, EventArgs args)
		{
			SelectAlertTemplateForm selectAlertTemplate = new SelectAlertTemplateForm(selectedInstance.Id);

			if (DialogResult.OK == selectAlertTemplate.ShowDialog(this))
			{
				configuration = null;
				LoadAlerts(selectedInstance.Id);
			}

		}

		private void createTemplateButton_Click(object sender, EventArgs e)
		{
			AddEditAlertTemplate addEditAlertTemplate = new AddEditAlertTemplate(selectedInstance.Id);
			if (DialogResult.OK == addEditAlertTemplate.ShowDialog(this))
			{
				configuration = null;
				LoadAlerts(selectedInstance.Id);
			}
		}

		private void AlertConfigurationDialog_Resize(object sender, EventArgs e)
		{
			if (Size != Settings.Default.AlertConfigurationDialogSize)
				Settings.Default.AlertConfigurationDialogSize = Size;
		}

		private void ultraCheckEditor1_CheckedChanged(object sender, EventArgs e)
		{
			UltraGridColumn categoryColumn = alertsGrid.DisplayLayout.Bands[0].Columns["Category"];
			SortedColumnsCollection sortColumns = alertsGrid.DisplayLayout.Bands[0].SortedColumns;
			if (sortColumns.Exists("Category"))
			{
				categoryColumn.Hidden = true;
				sortColumns.Remove(categoryColumn);
			}
            if (checkBox1.Checked) //CustomCheckbox 4.12 DarkTheme  Babita Manral 
            {
                categoryColumn.Hidden = false;
                sortColumns.Add(categoryColumn, false, true);
            }
        }

        private void AlertConfigurationDialog_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			if (e != null) e.Cancel = true;
			if (editingAlertTemplate)
				Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureAlertTemplates);
			else
				Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertsConfiguration);
		}

		private void AlertConfigurationDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			if (hlpevent != null) hlpevent.Handled = true;
			if (editingAlertTemplate)
				Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DefaultAlertsConfiguration);
			else
				Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertsConfiguration);
		}
		private void RankTextBox_Leave(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(RankTextBox.Text))
			{
				if (previousRankVal != RankTextBox.Text)
				{

					var confirmResult = ApplicationMessageBox.ShowWarning(this, "Important: Rank is a global setting. Editing it will affect all instances, not just one template.", ExceptionMessageBoxButtons.YesNo);
					if (confirmResult == DialogResult.No)
					{
						AlertConfigurationItem item = alertsGrid.ActiveRow.ListObject as AlertConfigurationItem;

						item.Rank = Convert.ToInt32(previousRankVal);
						alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
						RankTextBox.Text = previousRankVal;
					}
					if (confirmResult == DialogResult.Yes)
					{
						SaveChanges();
					}
				}
			}
		}
		private void RankTextBox_TextChanged(object sender, System.EventArgs e)
		{
			int test = 0;
			applyButton.Enabled = true;
			if (!String.IsNullOrEmpty(RankTextBox.Text))
			{
				if (!(Convert.ToInt32(RankTextBox.Text) > 0 && Convert.ToInt32(RankTextBox.Text) <= 100))
				{
					RankTextBox.Text = "";

					if (!currentChanging)
					{
						AlertConfigurationItem item = alertsGrid.ActiveRow.ListObject as AlertConfigurationItem;
						if (item != null && RankTextBox.Text != "")
						{
							test = Convert.ToInt32(RankTextBox.Text);
							item.Rank = test;
						}
					}
				}
			}
			if (alertsGrid.ActiveRow != null)
			{
				AlertConfigurationItem itemAlert = alertsGrid.ActiveRow.ListObject as AlertConfigurationItem;
				if (itemAlert != null && RankTextBox.Text != "")
					previousRankVal = Convert.ToString(itemAlert.Rank);
			}
		}

		private void RankTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((!char.IsDigit(e.KeyChar)) || char.IsControl(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void commentsTextBox_TextChanged(object sender, EventArgs e)
		{
			if (!currentChanging)
			{
				AlertConfigurationItem item = alertsGrid.ActiveRow.ListObject as AlertConfigurationItem;
				if (item != null)
				{
					item.Comments = commentsTextBox.Text;
				}
			}
		}

		/// <summary>
		/// Calculated as a percentage of Info,Critical and Warning according to the baseline.
		/// </summary>
		private void LoadBaselineThresholds()
		{
			AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
			if (item.IsBaselineEnabled)
			{
				baselineAlertConfigurationGrid.Visible = true;
				alertConfigurationGrid.Visible = false;
			}
			else
			{
				baselineAlertConfigurationGrid.Visible = false;
				alertConfigurationGrid.Visible = true;
			}
			ConfigureGauge(item, true);
			UpdateControls();
		}

		//10.0 Srishti purohit // For baseline alert
		//Event trigger to change state of baseline alert enabling
		//private void isBaselineEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
		//{
		//    AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
		//    if (item.IsBaselineEnabled != isBaselineEnabledCheckBox.Checked)
		//        item.IsBaselineEnabled = isBaselineEnabledCheckBox.Checked;
		//    if (item.IsBaselineEnabled)
		//    {
		//        baselineAlertConfigurationGrid.Visible = true;
		//        alertConfigurationGrid.Visible = false;
		//        if (baselineAlertConfigurationGrid.Rows != null && baselineAlertConfigurationGrid.Rows.Count > 0)
		//        {
		//            //UltraGridCell criticalCellValue = alertConfigurationGrid.Rows[alertConfigurationGrid.Rows.Count - 1].Cells["RangeEnd"];
		//            //if (criticalCellValue != null)
		//            //    criticalCellValue.Value = "999999999999999";
		//        }
		//    }
		//    else
		//    {
		//        baselineAlertConfigurationGrid.Visible = false;
		//        alertConfigurationGrid.Visible = true;
		//    }
		//    ConfigureGauge(item, true);
		//    UpdateControls();
		//}


		#endregion

		#region Value Editor Methods

		private void dialogEditor_EditorButtonClick(object sender, EditorButtonEventArgs e)
		{
			EditCell(e.Context as UltraGridCell);
		}

		private void EditCell(UltraGridCell cell)
		{
			if (cell != null)
			{
				if (cell.Column.Key == "Value")
					EditValueCell(cell);

				UpdateInformationLabel(cell.Row.ListObject);
			}
		}

		private void EditValueCell(UltraGridCell cell)
		{
			FlattenedThreshold threshold = cell.Row.ListObject as FlattenedThreshold;
			MetricDefinition metaData = threshold.GetConfigurationItem().GetMetaData();
			if (metaData.ValueType == typeof(string))
				EditStringValueCell(cell, threshold);
			else
				EditListValueCell(cell, threshold);
		}

		private void EditListValueCell(UltraGridCell cell, FlattenedThreshold threshold)
		{
			CheckedListDialog<object> dialog = new CheckedListDialog<object>();
			Threshold.ComparableList comparableList = threshold.Value as Threshold.ComparableList;
			if (EditRestrictedValueListCell(cell, threshold, threshold.GetPossibleValues<object>()) == System.Windows.Forms.DialogResult.OK)
			{
				alertConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.ReloadData);

				//10.0 Srishti purohit // For baseline alert
				//Making copy of State grid to give baseline alert functioanlity
				if (baselineAlertConfigurationGrid.DisplayLayout.Rows != null)
					baselineAlertConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.ReloadData);
			}
		}

		private DialogResult EditRestrictedValueListCell(UltraGridCell cell, FlattenedThreshold threshold, IEnumerable listItems)
		{
			using (AlertEnumeratedValuesConfigDialog dialog = new AlertEnumeratedValuesConfigDialog())
			{
				dialog.SetThresholdEntry(threshold.GetConfigurationItem());
				DialogResult result = dialog.ShowDialog(this);
				if (permissionType >= PermissionType.Modify && result == DialogResult.OK)
				{
					threshold.SetComparableList(threshold.Value as Threshold.ComparableList, true);
				}
				return result;
			}
		}

		private void EditStringValueCell(UltraGridCell cell, FlattenedThreshold threshold)
		{
			Debug.Assert(false, "AlretConfigurationDialog.EditStringValueCell has no string valued cells to edit.");
		}

		#endregion

		#region Alert Configuration Gauge methods

		private void ConfigureGauge(AlertConfigurationItem item, bool adjustMaxValue)
		{
			if (alertConfigurationGrid.Visible)
				ConfigureGaugeAlert(item, adjustMaxValue);
			else
				ConfigureGaugeBaseline(item, adjustMaxValue);
		}
		private void ConfigureGaugeAlert(AlertConfigurationItem item, bool adjustMaxValue)
		{
			MetricDefinition metaData = item.GetMetaData();
			ThresholdOptions options = metaData.Options;

			Threshold infoThreshold = null;
			Threshold warningThreshold = null;
			Threshold criticalThreshold = null;

			infoThreshold = item.ThresholdEntry.InfoThreshold;
			warningThreshold = item.ThresholdEntry.WarningThreshold;
			criticalThreshold = item.ThresholdEntry.CriticalThreshold;

			SectionCollection sections = alertConfigurationGauge.MainScale.Sections;

			if (metaData.ComparisonType == ComparisonType.GE)
			{
				// only enable indicators (triangles) for enabled thresholds
				alertConfigurationGauge.MainScale.Indicators[0].Visible = infoThreshold.Enabled;
				alertConfigurationGauge.MainScale.Indicators[1].Visible = warningThreshold.Enabled;
				alertConfigurationGauge.MainScale.Indicators[2].Visible = criticalThreshold.Enabled;
				// only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
				alertConfigurationGauge.MainScale.Sections[1].Visible = infoThreshold.Enabled;
				alertConfigurationGauge.MainScale.Sections[2].Visible = warningThreshold.Enabled;
				alertConfigurationGauge.MainScale.Sections[3].Visible = criticalThreshold.Enabled;
			}
			else
			{
				// only enable indicators (triangles) for enabled thresholds
				alertConfigurationGauge.MainScale.Indicators[2].Visible = infoThreshold.Enabled;
				alertConfigurationGauge.MainScale.Indicators[1].Visible = warningThreshold.Enabled;
				alertConfigurationGauge.MainScale.Indicators[0].Visible = criticalThreshold.Enabled;
				// only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
				alertConfigurationGauge.MainScale.Sections[2].Visible = infoThreshold.Enabled;
				alertConfigurationGauge.MainScale.Sections[1].Visible = warningThreshold.Enabled;
				alertConfigurationGauge.MainScale.Sections[0].Visible = criticalThreshold.Enabled;
			}

			// force in the section colors
			okSection.Color = Color.Green;
			infoSection.Color = Color.Blue;
			warningSection.Color = Color.Gold;
			criticalSection.Color = Color.Red;

			infoIndicator.Color = Color.Blue;
			warningIndicator.Color = Color.Gold;
			criticalIndicator.Color = Color.Red;

			okSection.Visible = true;
			infoSection.Visible = infoThreshold.Enabled;
			warningSection.Visible = warningThreshold.Enabled;
			criticalSection.Visible = criticalThreshold.Enabled;

			long rs1 = Convert.ToInt64(item.RangeStart1);
			long rs2 = Convert.ToInt64(item.RangeStart2);
			long rs3 = Convert.ToInt64(item.RangeStart3);

			decimal gaugeMax = metaData.MaxValue;
			if (adjustMaxValue)
			{
				if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
				{
					if (infoThreshold.Enabled || warningThreshold.Enabled || criticalThreshold.Enabled)
					{
						BaselineItemData baselineItem;

						long maxValue = criticalThreshold.Enabled ? rs3 : warningThreshold.Enabled ? rs2 : rs1;
						if (metaData.ComparisonType == ComparisonType.LE)
							maxValue = infoThreshold.Enabled ? rs1 : warningThreshold.Enabled ? rs2 : rs3;

						// see if there is a baseline item for this sucker
						if (baselineData.TryGetValue(item.MetricID, out baselineItem))
						{
							// make sure auto adjust items will show the entire reference range 
							if (baselineItem.ReferenceRangeEnd.HasValue && baselineItem.ReferenceRangeEnd.Value > maxValue)
								maxValue = Convert.ToInt64(baselineItem.ReferenceRangeEnd.Value);
						}

						gaugeMax = metaData.GetVisualUpperBound(maxValue);
					}
				}
			}
			

			if (gaugeMax == 0m)
				gaugeMax = 100m;

			alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
			alertConfigurationGauge.MainScale.Min = metaData.MinValue;

			if (metaData.ComparisonType == ComparisonType.GE)
			{
				double nextBoundary = alertConfigurationGauge.MainScale.Max;
				if (criticalThreshold.Enabled)
				{
					criticalSection.Max = nextBoundary;
					criticalSection.Min = rs3;
					nextBoundary = rs3;
				}
				if (warningThreshold.Enabled)
				{
					warningSection.Max = nextBoundary;
					warningSection.Min = rs2;
					nextBoundary = rs2;
				}
				if (infoThreshold.Enabled)
				{
					infoSection.Max = nextBoundary;
					infoSection.Min = rs1;
					nextBoundary = rs1;
				}
				okSection.Min = alertConfigurationGauge.MainScale.Min;
				okSection.Max = nextBoundary;
			}
			else
			{
				double nextBoundary = alertConfigurationGauge.MainScale.Min;
				if (criticalThreshold.Enabled)
				{
					criticalSection.Min = nextBoundary;
					criticalSection.Max = rs3;
					nextBoundary = rs3;
				}
				if (warningThreshold.Enabled)
				{
					warningSection.Max = rs2;
					warningSection.Min = nextBoundary;
					nextBoundary = rs2;
				}
				if (infoThreshold.Enabled)
				{
					infoSection.Max = rs1;
					infoSection.Min = nextBoundary;
					nextBoundary = rs1;
				}
				okSection.Min = nextBoundary;
				okSection.Max = alertConfigurationGauge.MainScale.Max;
			}
			try
			{
				gaugeChanging = true;
				infoIndicator.Value = rs1;
				warningIndicator.Value = rs2;
				criticalIndicator.Value = rs3;
			}
			finally
			{
				gaugeChanging = false;
			}
		}
		//10.0 srishti purohit -- for baseline alert modifications
		//Gauge for baseline to check max value and change grid values
		private void ConfigureGaugeBaseline(AlertConfigurationItem item, bool adjustMaxValue)
		{
			try
			{
				LOG.Info("In ConfigureGaugeBaseline");
				MetricDefinition metaData = item.GetMetaData();
				ThresholdOptions options = metaData.Options;

				Threshold infoThresholdBaseline = null;
				Threshold warningThresholdBaseline = null;
				Threshold criticalThresholdBaseline = null;

				infoThresholdBaseline = item.ThresholdEntry.BaselineInfoThreshold;
				warningThresholdBaseline = item.ThresholdEntry.BaselineWarningThreshold;
				criticalThresholdBaseline = item.ThresholdEntry.BaselineCriticalThreshold;

				SectionCollection sections = alertConfigurationGauge.MainScale.Sections;

				if (metaData.ComparisonType == ComparisonType.GE)
				{
					// only enable indicators (triangles) for enabled thresholds
					alertConfigurationGauge.MainScale.Indicators[0].Visible = infoThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Indicators[1].Visible = warningThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Indicators[2].Visible = criticalThresholdBaseline.Enabled;
					// only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
					alertConfigurationGauge.MainScale.Sections[1].Visible = infoThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Sections[2].Visible = warningThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Sections[3].Visible = criticalThresholdBaseline.Enabled;
				}
				else
				{
					// only enable indicators (triangles) for enabled thresholds
					alertConfigurationGauge.MainScale.Indicators[2].Visible = infoThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Indicators[1].Visible = warningThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Indicators[0].Visible = criticalThresholdBaseline.Enabled;
					// only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
					alertConfigurationGauge.MainScale.Sections[2].Visible = infoThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Sections[1].Visible = warningThresholdBaseline.Enabled;
					alertConfigurationGauge.MainScale.Sections[0].Visible = criticalThresholdBaseline.Enabled;
				}

				// force in the section colors
				okSection.Color = Color.Green;
				infoSection.Color = Color.Blue;
				warningSection.Color = Color.Gold;
				criticalSection.Color = Color.Red;

				infoIndicator.Color = Color.Blue;
				warningIndicator.Color = Color.Gold;
				criticalIndicator.Color = Color.Red;

				okSection.Visible = true;
				infoSection.Visible = infoThresholdBaseline.Enabled;
				warningSection.Visible = warningThresholdBaseline.Enabled;
				criticalSection.Visible = criticalThresholdBaseline.Enabled;

				long rs1 = Convert.ToInt64(item.RangeStartBaselineInfo1);
				long rs2 = Convert.ToInt64(item.RangeStartBaselineWarning2);
				long rs3 = Convert.ToInt64(item.RangeStartBaselineCritical3);

				decimal gaugeMax = metaData.MaxValueBaseline;
				if (adjustMaxValue)
				{
					if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
					{
						if (infoThresholdBaseline.Enabled || warningThresholdBaseline.Enabled || criticalThresholdBaseline.Enabled)
						{
							BaselineItemData baselineItem;

							long maxValue = criticalThresholdBaseline.Enabled ? rs3 : warningThresholdBaseline.Enabled ? rs2 : rs1;
							if (metaData.ComparisonType == ComparisonType.LE)
								maxValue = infoThresholdBaseline.Enabled ? rs1 : warningThresholdBaseline.Enabled ? rs2 : rs3;

							// see if there is a baseline item for this sucker
							if (baselineData.TryGetValue(item.MetricID, out baselineItem))
							{
								// make sure auto adjust items will show the entire reference range 
								if (baselineItem.ReferenceRangeEnd.HasValue && baselineItem.ReferenceRangeEnd.Value > maxValue)
									maxValue = Convert.ToInt64(baselineItem.ReferenceRangeEnd.Value);
							}

							gaugeMax = metaData.GetVisualUpperBound(maxValue);
						}
					}
				}
				

				if (gaugeMax == 0m)
					gaugeMax = 100m;

				alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
				alertConfigurationGauge.MainScale.Min = metaData.MinValue;

				if (metaData.ComparisonType == ComparisonType.GE)
				{
					double nextBoundary = alertConfigurationGauge.MainScale.Max;
					if (criticalThresholdBaseline.Enabled)
					{
						criticalSection.Max = nextBoundary;
						criticalSection.Min = rs3;
						nextBoundary = rs3;
					}
					if (warningThresholdBaseline.Enabled)
					{
						warningSection.Max = nextBoundary;
						warningSection.Min = rs2;
						nextBoundary = rs2;
					}
					if (infoThresholdBaseline.Enabled)
					{
						infoSection.Max = nextBoundary;
						infoSection.Min = rs1;
						nextBoundary = rs1;
					}
					okSection.Min = alertConfigurationGauge.MainScale.Min;
					okSection.Max = nextBoundary;
				}
				else
				{
					double nextBoundary = alertConfigurationGauge.MainScale.Min;
					if (criticalThresholdBaseline.Enabled)
					{
						criticalSection.Min = nextBoundary;
						criticalSection.Max = rs3;
						nextBoundary = rs3;
					}
					if (warningThresholdBaseline.Enabled)
					{
						warningSection.Max = rs2;
						warningSection.Min = nextBoundary;
						nextBoundary = rs2;
					}
					if (infoThresholdBaseline.Enabled)
					{
						infoSection.Max = rs1;
						infoSection.Min = nextBoundary;
						nextBoundary = rs1;
					}
					okSection.Min = nextBoundary;
					okSection.Max = alertConfigurationGauge.MainScale.Max;
				}
				try
				{
					gaugeChanging = true;
					infoIndicator.Value = rs1;
					warningIndicator.Value = rs2;
					criticalIndicator.Value = rs3;
				}
				finally
				{
					gaugeChanging = false;
				}
			}
			catch (Exception ex)
			{
				LOG.Error(" Error in configuring gauge baseline. " + ex);
				throw;
			}
		}

		private void alertConfigurationGauge_ValueChanged(object sender, ChartFX.WinForms.Gauge.IndicatorEventArgs e)
		{
			if (gaugeChanging)
				return;

			AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
			if (item == null)
				return;

			if (!currentChanging)
			{
				IndicatorCollection indicators = alertConfigurationGauge.MainScale.Indicators;
				double indicatorValue = e.Indicator.ValueDisplayed;

				// compensate for conversion issues going between double and long

				//SQLDM 10.1 (Pulkit Puri)-- Gauge should change the value when baseline is enabled
				//Sqldm-19204 fix
				if (!item.IsBaselineEnabled)
				{
					if (e.Indicator == infoIndicator)
					{
						item.RangeStart1 = indicatorValue;
						if (item.GetMetaData().ComparisonType == ComparisonType.GE)
						{
							if (indicatorValue > warningIndicator.ValueDisplayed)
								item.RangeStart2 = indicatorValue;
							if (indicatorValue > criticalIndicator.ValueDisplayed)
								item.RangeStart3 = indicatorValue;
						}
						else
						{
							if (indicatorValue < warningIndicator.ValueDisplayed)
								item.RangeStart2 = indicatorValue;
							if (indicatorValue < criticalIndicator.ValueDisplayed)
								item.RangeStart3 = indicatorValue;
						}
					}
					else if (e.Indicator == warningIndicator)
					{
						item.RangeStart2 = indicatorValue;
						if (item.GetMetaData().ComparisonType == ComparisonType.GE)
						{
							if (indicatorValue < infoIndicator.ValueDisplayed)
								item.RangeStart1 = indicatorValue;
							if (indicatorValue > criticalIndicator.ValueDisplayed)
								item.RangeStart3 = indicatorValue;
						}
						else
						{
							if (indicatorValue > infoIndicator.ValueDisplayed)
								item.RangeStart1 = indicatorValue;
							if (indicatorValue < criticalIndicator.ValueDisplayed)
								item.RangeStart3 = indicatorValue;
						}
					}
					else if (e.Indicator == criticalIndicator)
					{
						item.RangeStart3 = indicatorValue;
						if (item.GetMetaData().ComparisonType == ComparisonType.GE)
						{
							if (indicatorValue < infoIndicator.ValueDisplayed)
								item.RangeStart1 = indicatorValue;
							if (indicatorValue < warningIndicator.ValueDisplayed)
								item.RangeStart2 = indicatorValue;
						}
						else
						{
							if (indicatorValue > infoIndicator.ValueDisplayed)
								item.RangeStart1 = indicatorValue;
							if (indicatorValue > warningIndicator.ValueDisplayed)
								item.RangeStart2 = indicatorValue;
						}
					}
				}
				else
				{
					//Making copy of State grid to give baseline alert functioanlity

					if (e.Indicator == infoIndicator)
					{
						item.RangeStartBaselineInfo1 = indicatorValue;
						if (item.GetMetaData().ComparisonType == ComparisonType.GE)
						{
							if (indicatorValue > warningIndicator.ValueDisplayed)
								item.RangeStartBaselineWarning2 = indicatorValue;
							if (indicatorValue > criticalIndicator.ValueDisplayed)
								item.RangeStartBaselineCritical3 = indicatorValue;
						}
						else
						{
							if (indicatorValue < warningIndicator.ValueDisplayed)
								item.RangeStartBaselineWarning2 = indicatorValue;
							if (indicatorValue < criticalIndicator.ValueDisplayed)
								item.RangeStartBaselineCritical3 = indicatorValue;
						}
					}
					else if (e.Indicator == warningIndicator)
					{
						item.RangeStartBaselineWarning2 = indicatorValue;
						if (item.GetMetaData().ComparisonType == ComparisonType.GE)
						{
							if (indicatorValue < infoIndicator.ValueDisplayed)
								item.RangeStartBaselineInfo1 = indicatorValue;
							if (indicatorValue > criticalIndicator.ValueDisplayed)
								item.RangeStartBaselineCritical3 = indicatorValue;
						}
						else
						{
							if (indicatorValue > infoIndicator.ValueDisplayed)
								item.RangeStartBaselineInfo1 = indicatorValue;
							if (indicatorValue < criticalIndicator.ValueDisplayed)
								item.RangeStartBaselineCritical3 = indicatorValue;
						}
					}
					else if (e.Indicator == criticalIndicator)
					{
						item.RangeStartBaselineCritical3 = indicatorValue;
						if (item.GetMetaData().ComparisonType == ComparisonType.GE)
						{
							if (indicatorValue < infoIndicator.ValueDisplayed)
								item.RangeStartBaselineInfo1 = indicatorValue;
							if (indicatorValue < warningIndicator.ValueDisplayed)
								item.RangeStartBaselineWarning2 = indicatorValue;
						}
						else
						{
							if (indicatorValue > infoIndicator.ValueDisplayed)
								item.RangeStartBaselineInfo1 = indicatorValue;
							if (indicatorValue > warningIndicator.ValueDisplayed)
								item.RangeStartBaselineWarning2 = indicatorValue;
						}
					}
				}
				// see if the grids active cell is currently in edit mode
				UltraGridCell activeCell = alertConfigurationGrid.ActiveCell;
				if (activeCell != null && activeCell.IsInEditMode)
				{
					EmbeddableEditorBase editor = activeCell.EditorResolved;
					// editor should sync its value with the grid
					if (editor != null)
						editor.Value = activeCell.Value;
				}
				//10.0 Srishti purohit // For baseline alert
				//Making copy of State grid to give baseline alert functioanlity
				// see if the grids active cell is currently in edit mode
				activeCell = baselineAlertConfigurationGrid.ActiveCell;
				if (activeCell != null && activeCell.IsInEditMode)
				{
					EmbeddableEditorBase editor = activeCell.EditorResolved;
					// editor should sync its value with the grid
					if (editor != null)
						editor.Value = activeCell.Value;
				}
				alertConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.RefreshDisplay);
				//10.0 Srishti purohit // For baseline alert
				//Making copy of State grid to give baseline alert functioanlity
				baselineAlertConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.RefreshDisplay);
				ConfigureGauge(item, false);
				//               RefilterRecommendations();
			}
		}

		private void alertConfigurationGauge_GetTip(object sender, GetTipEventArgs e)
		{
			BaselineItemData baselineItem;

			string tooltip = String.Empty;
			IHitTestTarget target = e.FoundTargets.TopMostNoneEmptyTarget;
			if (target != null && target is LinearStrip)
			{
				AlertConfigurationItem alertItem = configBindingSource.Current as AlertConfigurationItem;
				if (alertItem != null && baselineData.TryGetValue(alertItem.MetricID, out baselineItem))
				{
					tooltip = String.Format(BASELINE_TOOLTIP_TEMPLATE,
											((LinearStrip)target).Min,
											((LinearStrip)target).Max,
											baselineItem.Average,
											baselineItem.Maximum,
											baselineItem.Deviation,
											baselineItem.Count);
				}
			}
			e.SelectedTarget.ToolTip = tooltip;
		}

		private void alertConfigurationGauge_MouseDown(object sender, MouseEventArgs e)
		{
			HitTestTargetStack hit = alertConfigurationGauge.HitTest(e.Location);
			IHitTestTarget target = hit.TopMostNoneEmptyTarget;
			if (target != null && target is LinearStrip)
			{
				Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertsConfigurationBlueBar);
			}
		}

		#endregion

		#region Alert Configuration Grid Methods

		private void alertConfigurationGrid_BeforeCellActivate(object sender, CancelableCellEventArgs e)
		{
			// if the column is not RangeStart then cancel the activation and select the row
			if (e.Cell.Column.Key != "RangeStart")
			{
				e.Cancel = true;
				e.Cell.Row.Selected = true;
				e.Cell.Row.Activate();
			}
		}

		private void alertConfigurationGrid_AfterCellUpdate(object sender, CellEventArgs e)
		{
			// update the values of the alertConfigurationGauge
			AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
			MetricDefinition metaData = item.GetMetaData();
			long rs1 = Convert.ToInt64(item.RangeStart1); // infoThreshold starting Range
			long rs2 = Convert.ToInt64(item.RangeStart2); // warningThreshold starting Range
			long rs3 = Convert.ToInt64(item.RangeStart3); // criticalThreshold starting Range

			int rowIndex = e.Cell.Row.Index;

			if (metaData.ComparisonType == ComparisonType.GE)
			{
				switch (rowIndex)
				{
					case 1:   // Info Start Range was changed
						if (rs1 > rs3)
							item.RangeStart3 = item.RangeStart2 = rs1;
						else if (rs1 > rs2)
							item.RangeStart2 = rs1;
						break;
					case 2:  // Warning start range was changed
						if (rs2 < rs1)
							item.RangeStart1 = rs2;
						else if (rs2 > rs3)
							item.RangeStart3 = rs2;
						break;
					case 3:
						if (rs3 < rs1)
							item.RangeStart1 = item.RangeStart2 = rs3;
						else if (rs3 < rs2)
							item.RangeStart2 = rs3;
						break;
					default:
						break;
				}
				//if (rs1 > rs2)
				//{
				//    if (rowIndex == 1)
				//        item.RangeStart2 = rs1;
				//    else
				//        item.RangeStart1 = rs2;
				//}
			}
			if (metaData.ComparisonType == ComparisonType.LE)
			{
				switch (rowIndex)
				{
					case 1:   // Info Start Range was changed
						if (rs1 < rs3)
							item.RangeStart3 = item.RangeStart2 = rs1;
						else if (rs1 < rs2)
							item.RangeStart2 = rs1;
						break;
					case 2:  // Warning start range was changed
						if (rs2 > rs1)
							item.RangeStart1 = rs2;
						else if (rs2 < rs3)
							item.RangeStart3 = rs2;
						break;
					case 3:
						if (rs3 > rs1)
							item.RangeStart1 = item.RangeStart2 = rs3;
						else if (rs3 > rs2)
							item.RangeStart2 = rs3;
						break;
					default:
						break;
				}
				//if (rs2 > rs1)
				//{
				//    if (rowIndex == 1)
				//        item.RangeStart2 = rs1;
				//    else
				//        item.RangeStart1 = rs2;
				//}
			}

			ConfigureGauge(item, (metaData.Options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue);

			//            if ((metaData.Options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
			//            {
			//                int criticalThreshold = (int)Convert.ChangeType(item.RangeStart2, typeof(int));

			//                long gaugeMax = metaData.GetVisualUpperBound(criticalThreshold);
			//                alertConfigurationGauge.MainScale.Max = gaugeMax;
			//                alertConfigurationGauge.MainScale.Sections[2].Max = gaugeMax;
			//            }

			//            IndicatorCollection indicators = alertConfigurationGauge.Scales[0].Indicators;
			//            indicators[0].Value = item.RangeStart1;
			//            indicators[1].Value = item.RangeStart2;

			// because some edits change the values of other rows - refresh the grid after editing a cell
			alertConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.ReloadData);
			//           RefilterRecommendations();
		}

		private void baselineAlertConfigurationGrid_AfterCellUpdate(object sender, CellEventArgs e)
		{
			try
			{
				LOG.Info("baselineAlertConfigurationGrid_AfterCellUpdate");
				// update the values of the alertConfigurationGauge
				AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
				MetricDefinition metaData = item.GetMetaData();
				long rs1 = Convert.ToInt64(item.RangeStartBaselineInfo1); // infoThreshold starting Range
				long rs2 = Convert.ToInt64(item.RangeStartBaselineWarning2); // warningThreshold starting Range
				long rs3 = Convert.ToInt64(item.RangeStartBaselineCritical3); // criticalThreshold starting Range

				int rowIndex = e.Cell.Row.Index;

				if (metaData.ComparisonType == ComparisonType.GE)
				{
					switch (rowIndex)
					{
						case 1:   // Info Start Range was changed
							if (rs1 > rs3)
								item.RangeStartBaselineCritical3 = item.RangeStartBaselineWarning2 = rs1;
							else if (rs1 > rs2)
								item.RangeStartBaselineWarning2 = rs1;
							break;
						case 2:  // Warning start range was changed
							if (rs2 < rs1)
								item.RangeStartBaselineInfo1 = rs2;
							else if (rs2 > rs3)
								item.RangeStartBaselineCritical3 = rs2;
							break;
						case 3:
							if (rs3 < rs1)
								item.RangeStartBaselineInfo1 = item.RangeStartBaselineWarning2 = rs3;
							else if (rs3 < rs2)
								item.RangeStartBaselineWarning2 = rs3;
							break;
						default:
							break;
					}
				}
				if (metaData.ComparisonType == ComparisonType.LE)
				{
					switch (rowIndex)
					{
						case 1:   // Info Start Range was changed
							if (rs1 < rs3)
								item.RangeStartBaselineCritical3 = item.RangeStartBaselineWarning2 = rs1;
							else if (rs1 < rs2)
								item.RangeStartBaselineWarning2 = rs1;
							break;
						case 2:  // Warning start range was changed
							if (rs2 > rs1)
								item.RangeStartBaselineInfo1 = rs2;
							else if (rs2 < rs3)
								item.RangeStartBaselineCritical3 = rs2;
							break;
						case 3:
							if (rs3 > rs1)
								item.RangeStartBaselineInfo1 = item.RangeStartBaselineWarning2 = rs3;
							else if (rs3 > rs2)
								item.RangeStartBaselineWarning2 = rs3;
							break;
						default:
							break;
					}
				}

				ConfigureGauge(item, (metaData.Options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue);

				// because some edits change the values of other rows - refresh the grid after editing a cell
				//10.0 Srishti purohit // For baseline alert
				//Making copy of State grid to give baseline alert functioanlity
				baselineAlertConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.ReloadData);
			}
			catch (Exception ex)
			{
				LOG.Error("Error in baseline after cell update. " + ex);
				throw;
			}
		}

		private void alertConfigurationGrid_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
		{
			EditCell(e.Cell);
		}

		private void editButton_Click(object sender, EventArgs e)
		{
			UltraGridRow row = alertConfigurationGrid.ActiveRow;
			if (row == null)
			{
				row = alertConfigurationGrid.Rows[2];
				EditCell(row.Cells["Value"]);
			}
			else
				EditCell(row.Cells["Value"]);
		}

		#endregion

		#region Information Label Methods

		private void informationLabel_MouseEnter(object sender, EventArgs e)
		{
			informationLabel.ForeColor = Color.DimGray;
		}

		private void informationLabel_MouseLeave(object sender, EventArgs e)
		{
			informationLabel.ForeColor = Color.Black;
		}

		private void informationLabel_MouseClick(object sender, MouseEventArgs e)
		{
			// add code here to show the unsnooze dialog
			AlertConfigurationItem item = configBindingSource.Current as AlertConfigurationItem;
			if (item != null)
			{
				SnoozeAlertsDialog.UnSnoozeAlerts(this, configuration);
				informationLabelPanel.Visible = item.ThresholdEntry.IsAlertSnoozed(DateTime.UtcNow);
			}
		}

		#endregion

		#region Alert Recommendations Label

		private void alertRecommendationsLabel_MouseEnter(object sender, EventArgs e)
		{
			alertRecommendationsLabel.ForeColor = Color.Black;
			alertRecommendationsLabel.BackColor = Color.FromArgb(255, 189, 105);
			alertRecommendationsWarningImage.BackColor = Color.FromArgb(255, 189, 105);
		}

		private void alertRecommendationsLabel_MouseLeave(object sender, EventArgs e)
		{
			alertRecommendationsLabel.ForeColor = Color.Black;
			alertRecommendationsLabel.BackColor = Color.Empty;
			alertRecommendationsWarningImage.BackColor = Color.Empty;
		}

		private void alertRecommendationsLabel_MouseDown(object sender, MouseEventArgs e)
		{
			alertRecommendationsLabel.ForeColor = Color.White;
			alertRecommendationsLabel.BackColor = Color.FromArgb(251, 140, 60);
			alertRecommendationsWarningImage.BackColor = Color.FromArgb(251, 140, 60);
		}

		private void alertRecommendationsLabel_MouseUp(object sender, MouseEventArgs e)
		{
			alertRecommendationsLabel.ForeColor = Color.Black;
			alertRecommendationsLabel.BackColor = Color.FromArgb(255, 189, 105);
			alertRecommendationsWarningImage.BackColor = Color.FromArgb(255, 189, 105);
			ShowAlertRecommendations();
		}

		private void ShowAlertRecommendations()
		{
			if (configuration != null)
			{
				AlertRecommendationsDialog dialog = new AlertRecommendationsDialog(configuration, recommendations);
				DialogResult result = dialog.ShowDialog(this);

				// even if they cancel the recommendation dialog we need to update
				// the recommendations because the configuration could have changed.
				if (!alertRecommendationsBackgroundWorker.IsBusy)
				{
					alertRecommendationsPanel.Visible = false;
					alertRecommendationsBackgroundWorker.RunWorkerAsync();
				}

				if (result == DialogResult.OK)
				{
					ConfigureGauge(configBindingSource.Current as AlertConfigurationItem, false);
				}
			}
		}

		private void alertRecommendationsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			bool useDefaults;
			DateTime startDate;
			DateTime endDate;
			DateTime? earliestAvailableData;
			short days;

			List<BaselineItemData> newRecommendations = null;

			SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
			using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
			{
				connection.Open();

				Dictionary<int, BaselineItemData> newBaseline = new Dictionary<int, BaselineItemData>();
				foreach (BaselineItemData item in BaselineHelpers.GetBaseline(connection, configuration.InstanceID, true))
				{
					int? metricId = item.GetMetaData().MetricId;
					if (metricId.HasValue)
					{
						newBaseline.Add(metricId.Value, item);
					}
				}

				BaselineHelpers.GetBaselineParameters(connection,
											configuration.InstanceID,
											out useDefaults,
											out startDate,
											out endDate,
											out days,
											out earliestAvailableData);
				// don't bother getting recommendations if we don't have 24 hours worth of data collected                
				if (earliestAvailableData.HasValue && (DateTime.UtcNow - earliestAvailableData.Value) >= TimeSpan.FromHours(24))
				{
					newRecommendations = BaselineHelpers.FilterRecommendations(configuration, newBaseline.Values);
				}

				e.Result =
					new Pair<Dictionary<int, BaselineItemData>, List<BaselineItemData>>(newBaseline,
																	newRecommendations);
			}
		}

		private void alertRecommendationsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				ApplicationMessageBox.ShowError(this, e.Error);
				return;
			}
			if (e.Cancelled)
			{
				LOG.Debug("Alert recommendations worker cancelled.");
			}

			Pair<Dictionary<int, BaselineItemData>, List<BaselineItemData>> result =
				(Pair<Dictionary<int, BaselineItemData>, List<BaselineItemData>>)e.Result;

			if (result.First != null)
				baselineData = result.First;
			else
				baselineData.Clear();

			if (result.Second != null)
				recommendations = result.Second;
			else
				recommendations.Clear();

			MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(configuration.InstanceID);
			UpdateAlertRecommendationsVisibility();
			UpdateGauge(configBindingSource.Current as AlertConfigurationItem);
		}

		public void RefilterRecommendations()
		{
			recommendations = BaselineHelpers.FilterRecommendations(configuration, baselineData.Values);
			UpdateAlertRecommendationsVisibility();
		}

		private void UpdateGauge(AlertConfigurationItem item)
		{
			if (item != null)
			{
				ConfigureGauge(item, (item.GetMetaData().Options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue);
				UpdateBlueBar(item);
			}
		}

		public void UpdateAlertRecommendationsVisibility()
		{
			bool visible = false;
			// no reason to tease view only people...
			if (!editingAlertTemplate && permissionType >= PermissionType.Modify)
			{
				if (recommendations.Count > 0)
				{
					visible = true;
				}
			}
			alertRecommendationsPanel.Visible = visible;

			//START: SQLdm 10.0 (Tarun Sapra) - Raise an operational alert for "alert recommendations"
			if (visible)
			{
				try
				{
					IManagementService defaultManagementService =
							ManagementServiceHelper.GetDefaultService(
								Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

					defaultManagementService.LogOperationalAlert(selectedInstance.DisplayInstanceName, MonitoredState.Warning, "Recommendations Are Available", "Recommendations for alerts are available for SQLServer = " + selectedInstance.DisplayInstanceName + ", please review thresholds");
				}
				catch (Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException ex)
				{
					ApplicationMessageBox.ShowError(null,
													"An error occurred while attempting to contact the Management Service.",
													ex as Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException);
				}
				catch (Exception genericEx)
				{
					ApplicationMessageBox.ShowError(null,
													 Idera.SQLdm.DesktopClient.Properties.Resources.ExceptionUnhandled,
													genericEx as Exception);
				}
			}
			//END: SQLdm 10.0 (Tarun Sapra) - Raise an operational alert for "alert recommendations"

		}

		public void UpdateBlueBar(AlertConfigurationItem alertItem)
		{
			BaselineItemData baselineItem;
			bool showBlueBar = false;

			if (blueBar == null)
				blueBar = alertConfigurationGauge.MainScale.Stripes[0];

			if (blueBar != null)
			{
				if (alertItem != null && baselineData.TryGetValue(alertItem.MetricID, out baselineItem))
				{
					if (baselineItem.ReferenceRangeStart.HasValue && baselineItem.ReferenceRangeEnd.HasValue)
					{
						blueBar.Min = Convert.ToDouble(baselineItem.ReferenceRangeStart.Value);
						blueBar.Max = Convert.ToDouble(baselineItem.ReferenceRangeEnd.Value);
						// If values are exactly the same... blueBar doesn't display
						if (baselineItem.ReferenceRangeStart.Value == baselineItem.ReferenceRangeEnd.Value)
						{
							blueBar.Max = blueBar.Max + .0001;
						}
						showBlueBar = true;
					}
				}
				blueBar.Visible = showBlueBar;
			}
		}

		#endregion

		#region Baseline Methods

		private const string BASELINE_TOOLTIP_TEMPLATE =
			"Baseline Statistics\nRange: {0} - {1}\nAverage: {2}\nMaximum: {3}\nStdev: {4}\nCount: {5}";

		private void configureBaselineButton_Click(object sender, EventArgs e)
		{
			using (MonitoredSqlServerInstancePropertiesDialog dialog = new MonitoredSqlServerInstancePropertiesDialog(configuration.InstanceID))
			{
				dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Baseline;

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					if (!alertRecommendationsBackgroundWorker.IsBusy)
					{
						alertRecommendationsPanel.Visible = false;
						alertRecommendationsBackgroundWorker.RunWorkerAsync();
					}
				}
			}
		}

		#endregion

		#region Threshold Instance Alerting

		private void btnAddInstance_Click(object sender, EventArgs e)
		{
			try
			{
				AlertConfigurationItem defaultItem =
					(from row in instanceConfigGrid.Rows.GetFilteredInNonGroupByRows() where ((AlertConfigurationItem)row.ListObject).IsDefaultThreshold select (AlertConfigurationItem)row.ListObject).FirstOrDefault();

				AlertConfigurationItem newItem = new AlertConfigurationItem(defaultItem);
				while (true)
				{
					if (ShowInstanceThresholdDialog(newItem, InstanceAction.Add) == DialogResult.OK)
					{
						if (configuration.ItemExists(newItem))
						{
							string message =
								string.Format(
									"Alert thresholds for {1} {0} already exist, select another {1}, or click Cancel and edit the existing thresholds.",
									newItem.MetricInstance, newItem.InstanceType.ToString().ToLower());

							ApplicationMessageBox.ShowQuestion(this, message, ExceptionMessageBoxButtons.OK);

							continue;
						}

						AddAlertThresholdItem(newItem);
						if (defaultItem != null)
						{
							defaultItem.IsThresholdChanged = true;

							// No real change has performed.
							defaultItem.EnableFakeThresholdChanged();
						}

						break;
					}
					else
					{
						break;
					}
				}

				//instanceConfigGrid.Rows.Refresh(RefreshRow.ReloadData);
				//alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
			}
			catch (Exception except)
			{
				LOG.Error("Error adding new Alert Instance", except);
			}
			finally
			{
				UpdateControls();
			}
		}

		private void btnEditInstance_Click(object sender, EventArgs e)
		{
			try
			{
				AlertConfigurationItem aci = (AlertConfigurationItem)instanceConfigGrid.Selected.Rows[0].ListObject;

				//AlertConfigurationItem editItem = aci.IsThresholdNew ? 
				//                                  aci : 
				//                                  configuration[aci.MetricID, aci.IsDefaultThreshold ? String.Empty : aci.MetricInstance];

				if (ShowInstanceThresholdDialog(aci, InstanceAction.Edit) == DialogResult.OK)
				{
					AlertConfigurationItem defaultItem =
						(from row in instanceConfigGrid.Rows.GetFilteredInNonGroupByRows() where ((AlertConfigurationItem)row.ListObject).IsDefaultThreshold select (AlertConfigurationItem)row.ListObject).FirstOrDefault();
					if (defaultItem != null)
					{
						defaultItem.IsThresholdChanged = true;

						if (!aci.IsDefaultThreshold)
						{
							// If the edited 'threshold' is not the default 'threshold'
							defaultItem.EnableFakeThresholdChanged();
						}
					}

					LOG.Info("Item Updated");
				}
				else
				{
					LOG.Info("Item NOT updated");
				}
			}
			catch (Exception except)
			{
				LOG.Error("Error editing threshold instance ", except);
			}
			finally
			{
				UpdateControls();
			}

		}

		private void btnDeleteInstance_Click(object sender, EventArgs e)
		{
			try
			{
				AlertConfigurationItem item = (AlertConfigurationItem)instanceConfigGrid.ActiveRow.ListObject;

				if (!item.IsDefaultThreshold)
				{
					DialogResult dialogResult =
							ApplicationMessageBox.ShowWarning(ParentForm,
															  String.Format("Are you sure you want to delete the alert thresholds for {0}?", item.MetricInstance),
															  ExceptionMessageBoxButtons.YesNo);

					if (dialogResult == DialogResult.Yes)
					{
						AlertThresholds.Remove(item);
						configuration.RemoveEntry(item.MetricID, item.MetricInstance);

						AlertConfigurationItem defaultItem =
							(from row in instanceConfigGrid.Rows.GetFilteredInNonGroupByRows() where ((AlertConfigurationItem)row.ListObject).IsDefaultThreshold select (AlertConfigurationItem)row.ListObject).FirstOrDefault();

						if (defaultItem != null)
						{
							defaultItem.IsThresholdChanged = true;

							// No real change has been performed in the 'default threshold'.
							defaultItem.EnableFakeThresholdChanged();
						}
					}
				}
				else
				{
					ApplicationMessageBox.ShowError(this, "The default instance cannot be deleted.");
				}
			}
			catch (Exception except)
			{
				LOG.Error("Error deleting threshold instance ", except);
			}
			finally
			{
				UpdateControls();
			}

		}

		private DialogResult ShowInstanceThresholdDialog(AlertConfigurationItem item, InstanceAction action)
		{
			DialogResult result = DialogResult.None;

			List<string> existingInstances = new List<string>();
			foreach (UltraGridRow row in instanceConfigGrid.Rows.GetFilteredInNonGroupByRows())
			{
				AlertConfigurationItem aci = row.ListObject as AlertConfigurationItem;
				if (aci != null && !aci.IsDefaultThreshold && !existingInstances.Contains(aci.MetricInstance))
				{
					existingInstances.Add(aci.MetricInstance);
				}
			}

			instanceThresholdDialog itd = new instanceThresholdDialog(item, action, editingAlertTemplate, existingInstances, permissionType);

			result = itd.ShowDialog();

			instanceConfigGrid.DisplayLayout.Rows.Refresh(RefreshRow.FireInitializeRow);

			return result;
		}

		private void instanceConfigGrid_InitializeRow(object sender, InitializeRowEventArgs e)
		{
			AlertConfigurationItem aci = e.Row.ListObject as AlertConfigurationItem;
			//isBaselineEnabledCheckBox.Checked = aci.IsBaselineEnabled;

			if (aci == null)
				return;

			e.Row.Cells["RangeStart1"].Hidden = !aci.ThresholdEntry.InfoThreshold.Enabled;
			e.Row.Cells["RangeStart2"].Hidden = !aci.ThresholdEntry.WarningThreshold.Enabled;
			e.Row.Cells["RangeStart3"].Hidden = !aci.ThresholdEntry.CriticalThreshold.Enabled;

			if ((!editingAlertTemplate) && serverInstanceLists.ContainsKey(aci.InstanceType))
			{
				e.Row.Appearance.ForeColor = (aci.InstanceType == InstanceType.Disk ||
											  aci.InstanceType == InstanceType.Database)
											 && !aci.IsDefaultThreshold
											 && !serverInstanceLists[aci.InstanceType].Contains(aci.MetricInstance)
												 ? Color.Gray
												 : e.Row.Appearance.ForeColor;
			}
		}

		private void instanceConfigGrid_ClickCell(object sender, ClickCellEventArgs e)
		{
			if (e.Cell.Column.Key == "ThresholdEnabled")
			{
				if (permissionType >= PermissionType.Modify)
				{
					AlertConfigurationItem aci = e.Cell.Row.ListObject as AlertConfigurationItem;
					if (aci != null)
						aci.ThresholdEnabled = !aci.ThresholdEnabled;

					e.Cell.Row.Refresh(RefreshRow.RefreshDisplay);
				}
			}
		}

		private void instanceConfigGrid_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
		{
			if (e.Cell.Column.Key == "Enabled")
				return;

			if (permissionType > PermissionType.ReadOnlyPlus)
			{
				AlertConfigurationItem aci = e.Cell.Row.ListObject as AlertConfigurationItem;

				ShowInstanceThresholdDialog(aci, InstanceAction.Edit);
			}
		}

		#endregion

		private void instanceConfigGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
		{
			UpdateControls();
		}

		private void existingDatabasesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (!editingAlertTemplate)
			{
				int instanceId = selectedInstance.Id;
				serverInstanceLists.Add(InstanceType.Database, GetDatabaseList(instanceId));
				serverInstanceLists.Add(InstanceType.Disk, GetDiskList(instanceId));
			}
		}

		private List<string> GetDatabaseList(int sqlServerId)
		{
			return RepositoryHelper.GetDatabaseList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, sqlServerId);
		}

		private List<string> GetDiskList(int sqlServerId)
		{
			List<string> result = new List<string>();

			foreach (string disk in RepositoryHelper.GetDriveList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, sqlServerId))
			{
				if (disk.Length == 2 && disk.EndsWith(":"))
					result.Add(disk.TrimEnd(':'));
				else
					result.Add(disk);
			}

			return result;
		}

		private void existingDatabasesBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//Adding null check to handle exception
			if (instanceConfigGrid.Rows != null)
				instanceConfigGrid.Rows.Refresh(RefreshRow.FireInitializeRow);
		}

		/// <summary>
		/// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
		/// </summary>
		private void AdaptFontSize()
		{
			AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
		}

		void OnCurrentThemeChanged(object sender, EventArgs e)
		{
			SetGridTheme();
			if (Settings.Default.ColorScheme == "Dark")
			{
				appearance18.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
				appearance18.BackColor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
			}
			else
            {
				appearance18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
				appearance18.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
			}
			updateLinearScaleFontAsPerTheme(this.linearScale1);
		}

		private void SetGridTheme()
		{
			// Update UltraGrid Theme
			var themeManager = new GridThemeManager();
			themeManager.updateGridTheme(this.alertsGrid);
			themeManager.updateGridTheme(this.alertConfigurationGrid);
			themeManager.updateGridTheme(this.instanceConfigGrid);
			themeManager.updateGridTheme(this.baselineAlertConfigurationGrid);
		}

	
		public void updateLinearScaleFontAsPerTheme(LinearScale linearscale)
		{
			ThemeSetter ts = new ThemeSetter();
			ts.SetLinearScale(linearscale);
		}
	}
}
