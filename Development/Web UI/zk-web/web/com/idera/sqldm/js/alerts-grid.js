jq2 = jQuery.noConflict();
jq2(function ($) {
	userSavedFilters = new Object();
	productRestUrlHTTPS="";
	var showAdvancedFilterDialog = function(gridObject, isEdit, savedFilterName,isEditingUnSavedFilter,productRestUrl,header,dmProducts) {
		
		prodIns=[];
		dmProductsArr=eval(dmProducts);
		for(j=0;j<dmProductsArr.length;j++){
			prodIns.push({"InstanceName":dmProductsArr[j].InstanceName,"name":dmProductsArr[j].InstanceName,"value":dmProductsArr[j].InstanceName});
		}
		dmProducts=prodIns;
		var filterName = "";
		var topItems = [];
		var columns = gridObject.getColumns();
		
		if(isEdit) {
			/* if the advanced filter is open for edit, then the if_applied_filter flag is set to false to prompt save */
			gridObject.if_applied_filter_saved = false;
		}
		else {
			/* if advanced filter window is open for create, then save prompt should not be shown unless its applied */
			gridObject.if_applied_filter_saved = true;
		}
		
		// Store for populating the second operand in the filter if the value chosen
		// in the first operand is 'Status'.
		
		var dmInstancesStore = Ext.create('Ext.data.Store', {
			storeId : 'dmInstancesStore',
			autoLoad : 'true',
			fields : ['InstanceName'],
			data :eval(dmProducts),
			pageSize : 15,
			remoteFilter : false,
			autoLoad : true
		});
		
		var metricsStore = Ext.create('Ext.data.Store', {
			storeId : 'metricsStore',
			fields : [ 'MetricId', 'Name' ],
			proxy : {
				type : 'ajax',
				url : productRestUrlHTTPS+'/Metrics/tzo/0',
				headers: {"Authorization": header},
				reader : {
					type : 'json',
					root : 'data'
				},
				noCache:false,
			},
			autoLoad : true,
			remoteFilter : false,
			//sqldm-29940. Sort the metrics alphabetically.
			sorters: [{
				property: 'Name',
				direction: 'ASC'
			}],
			sortRoot: 'Name',
			sortOnLoad: true,
		});

        var instanceStore = Ext.create('Ext.data.Store', {
            storeId : 'instanceStore',
            fields:[{
                "name": "Overview.InstanceName",
                "type": "string",
                "mapping": "Overview.InstanceName"
            },{
                "name": "Overview.SQLServerId",
                "type": "string",
                "mapping": "Overview.SQLServerId"
            }],
            proxy : {
                type : 'ajax',
                url : productRestUrlHTTPS+'/Instances',
                headers: {"Authorization": header},
                reader : {
                    type : 'json',
                    root : 'data'
                },
                noCache:false
            },
            autoLoad : true
        });
		
		var columnsStore = Ext.create('Ext.data.Store', {
			fields : [ 'dataIndex', 'text' ],
			storeId : 'columnsStore',
			data : [ {
				"dataIndex" : "instance",
				"text" : "Instance"
			}, {
				"dataIndex" : "metric",
				"text" : "Metric"
			}, {
				"dataIndex" : "severity",
				"text" : "Severity"
			}, {
				"dataIndex" : "sqldm",
				"text" : "SQLDM"
			}, {
				"dataIndex" : "span",
				"text" : "Alerts Time Span"
			} ]
		});

		// Store for populating the operations combo box in each
		// filter.
		var operatorsStore = Ext.create('Ext.data.Store', {
			fields : [ 'operator', 'value' ],
			storeId : 'operatorsStore',
			data : [ {
				"operator" : "Contains",
				"value" : "contains"
			},{
				"operator" : "Does Not Contain",
				"value" : "notContains"
			},{
				"operator" : "Between",
				"value" : "between"
			}]
		});
		

		// Store for populating the logical operator drop down combo box
		// at the end of each applied filter.
		var operatorsStore1 = Ext.create('Ext.data.Store', {
			fields : [ 'operator', 'value' ],
			data : [ {
				"operator" : "AND",
				"value" : "AND"
			}]
		});
		
		var severityStore = Ext.create('Ext.data.Store', {
			fields : [ 'name', 'value' ],
			data : [ {
				"name" : "Critical",
				"value" : "8"
			}, {
				"name" : "Warning",
				"value" : "4"
			}, {
				"name" : "Ok",
				"value" : "1"
			}, {
				"name" : "Informational",
				"value" : "2"
			} ]
		});

		var getNameFieldByColumnName = function(column_name){
			var nameField = "value";
			switch (column_name.toLowerCase()) {
			case "severity":
				nameField = "value";
				break;
			case "metric":
				nameField = "MetricId";
				break;
			case "instance":
				nameField = "Overview.SQLServerId";
				break;
			case "sqldm":
				nameField = "InstanceName";
				break;
			default :
				nameField = "value";
			}
			return nameField;
			
		}
		
		var getValueFieldByColumnName = function(column_name){
			var valueField = "name";
			switch (column_name.toLowerCase()) {
			case "severity":
				valueField = "name";
				break;
			case "metric":
				valueField = "Name";
				break;
			case "instance":
				valueField = "Overview.InstanceName";
				break;
			case "sqldm":
				valueField = "InstanceName";
				break;
			default :
				valueField = "name";
			}
			return valueField;
			
		}
		
		// This function maps the store to be chosen depending upon the 
		// column chosen in the first combo box.
		var getStoreByColumnName = function(column_name) {
			var store = severityStore;
			if (column_name == undefined) {
				return;
			}
			switch (column_name.toLowerCase()) {
				case "severity":
					store = severityStore;
					break;
				case "metric":
					store = metricsStore;
					break;
				case "instance":
					store = instanceStore;
					break;
				case "sqldm":
					store=dmInstancesStore;
					break;
			}
			return store;
		}
		
		// This function creates an array and populates it with all the sub components
		// needed for advanced filter.
		var initiateTopItems = function() {
			// This pushes the help tooltip for advanced filter component
			topItems.push({
		        xtype: 'component',
		        componentCls: 'idera-ext-advfilter-help',
		        id: 'help-tooltip',
		        autoEl: {
		            tag: 'a', 
		            href: 'javascript:void(0)',
		            html: 'Help'
		        }, 
		        listeners: {
		            click: function() {
		                // handle click here
		            	return;
		            }
		        }
		    });
			// This will add the text box for entering the custom filter name.
			topItems.push({
				xtype : 'textfield',
				labelAlign : 'top',
				width : 688,
				fieldLabel : "Custom Filter Name",
				id : 'customFilterName',
				value : filterName,
			});
			// This adds the separator component in the window body.
			topItems.push({
				xtype : 'tbseparator',
				componentCls : 'idera-ext-advanced-filter-seperator'
			});
		}

		// This function return a tagfield with whichever store is needed for
		// a particular column name.
		var getTagFieldForColumn = function(column_name) {
			return {
				xtype : 'tagfield',
				qtip : 'value',
				width : 267,
				store : getStoreByColumnName(column_name),
				valueField : getNameFieldByColumnName(column_name),
				displayField : getValueFieldByColumnName(column_name),
				queryMode : 'local',
				filterPickList : true,
				componentCls : 'idera-ext-advfilter-panel-operand2',
				listeners : {
					select:function(combo, value) {
						var allTags = jQuery('.x-tagfield-item', combo.el.dom.childNodes[1]),
						    numberOfTags = allTags.length,
						    lastTag = jQuery('.x-tagfield-item', combo.el.dom.childNodes[1])[numberOfTags - 1];
						jQuery(lastTag).attr('title', value[0].data.value);
					}
				}
			}
		}

		// This function renders the tag field(which is the second operand) 
		// whenever the value in the first combo box is changed.
		var renderTagFieldInFilter = function(that, column_name) {
			that.ownerCt.add(getTagFieldForColumn(column_name));
		}

		// This function renders the date field(which is the second operand) 
		// whenever the value in the first combo box is changed to between, after, before etc.
		var renderDateFieldsInFilter = function(that, startDateOnly, dateOnly) {
			var itemArray = []
			// This block is executed if only start date is needed.
			if (startDateOnly) {
				itemArray.push({
					xtype : 'panel',
					width : 274,
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-starttime-only',
					items : [ {
						xtype : 'datefield',
						width : 129
					}, {
						xtype : 'timefield',
						componentCls : 'idera-ext-advfilter-endtime-field',
						width : 129
					} ],
					layout : 'hbox'
				});
			} else if (dateOnly) {
				// This block is executed if only a single date field is needed.
				itemArray.push({
					xtype : 'panel',
					width : 274,
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-starttime-only',
					items : [ {
						xtype : 'datefield',
						width : 267
					} ],
					layout : 'hbox'
				});
			} else {
				// This will render start and end dates + times
				itemArray.push({
					xtype : 'panel',
					width : 320,
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-starttime-panel',
					items : [ {
						xtype : 'label',
						text : 'Start:'
					}, {
						xtype : 'datefield',
						width : 129
					}, {
						xtype : 'timefield',
						componentCls : 'idera-ext-advfilter-endtime-field',
						width : 129
					} ],
					layout : 'hbox'
				});

				itemArray.push({
					xtype : 'panel',
					width : 320,
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-endtime-panel',
					items : [ {
						xtype : 'label',
						text : 'End:'
					}, {
						xtype : 'datefield',
						width : 129
					}, {
						xtype : 'timefield',
						componentCls : 'idera-ext-advfilter-endtime-field',
						width : 129
					} ],
					layout : 'hbox'
				});
			}
			that.ownerCt.add({
				xtype : 'panel',
				cls : 'idera-no-border-panel',
				componentCls : 'idera-ext-advfilter-time-panel',
				items : itemArray,
				layout : 'vbox'
			});
		}

		// This function renders the combo box for the logical operator
		// which ties all the filters together.
		var renderDeleteAndLogicOperator = function(that) {
			that.ownerCt.add({
				xtype : 'combo',
				width : 81,
				store : operatorsStore1,
				queryMode : 'local',
				displayField : 'operator',
				valueField : 'value',
				value: 'AND',
				componentCls : 'idera-ext-advfilter-panel-logicoperator'
			});
			
			that.ownerCt.add({
				xtype : 'button',
				ui : 'round',
				width : 16,
				height : 16,
				text : '-',
				componentCls : 'idera-ext-round-button',
				listeners : {
					click : function() {
						var count = Ext.get('contentBox').component.items.items.length;
					    if(count - 3 == 1) {
					    	Ext.getCmp("saveFilterBtn").setDisabled(true);
			                Ext.getCmp("clearAllFiltersBtn").setDisabled(true);
			                Ext.getCmp("applyFilterBtn").setDisabled(true);
					    }
						this.findParentByType('panel').destroy();
					}
				}
			});
		}

		// This function creates the panel which contains all the components for a filter
		// one panel per filter is rendered every time add filter button is clicked.
		var createPanel = function(columnValue, operatorValue, userValue,
				relationOperator) {
			var widthOfOperator=138;
			var valueField = {
					xtype : 'tagfield',
					qtip : 'value',
					width : 267,
					store : getStoreByColumnName(columnValue),
					valueField : getNameFieldByColumnName(columnValue),
					displayField : getValueFieldByColumnName(columnValue),
					value : userValue,
					componentCls : 'idera-ext-advfilter-panel-operand2',
					listeners : {
						select:function(combo, value) {
							var allTags = jQuery('.x-tagfield-item', combo.el.dom.childNodes[1]),
						        numberOfTags = allTags.length,
						        lastTag = jQuery('.x-tagfield-item', combo.el.dom.childNodes[1])[numberOfTags - 1];
						    jQuery(lastTag).attr('title', value[0].data.value);
						}
					}
				}
			
			if("between"==operatorValue){
				for(i=0;i<userValue.length;i++){
					userValue[i]=new Date(userValue[i])
				}
				widthOfOperator=93;
				var itemArray = []
				itemArray.push({
					xtype : 'panel',
					width : 320,
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-starttime-panel',
					items : [ {
						xtype : 'label',
						text : 'Start:'
					}, {
						xtype : 'datefield',
						width : 129,
						value:userValue[0],
					}, {
						xtype : 'timefield',
						value:userValue[1],
						componentCls : 'idera-ext-advfilter-endtime-field',
						width : 129
					} ],
					layout : 'hbox'
				});

				itemArray.push({
					xtype : 'panel',
					width : 320,
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-endtime-panel',
					items : [ {
						xtype : 'label',
						text : 'End:'
					}, {
						xtype : 'datefield',
						value:userValue[2],
						width : 129
					}, {
						xtype : 'timefield',
						value:userValue[3],
						componentCls : 'idera-ext-advfilter-endtime-field',
						width : 129
					} ],
					layout : 'hbox'
				});
				
				valueField={
					xtype : 'panel',
					cls : 'idera-no-border-panel',
					componentCls : 'idera-ext-advfilter-time-panel',
					items : itemArray,
					layout : 'vbox'
				}
			}

			return Ext
					.create(
							'Ext.panel.Panel',
							{
								componentCls : 'idera-ext-advfilter-panel',
								layout : 'hbox',
								items : [
										{
											xtype : 'combo',
											width : 154,
											store : columnsStore,
											queryMode : 'local',
											displayField : 'text',
											valueField : 'dataIndex',
											value : columnValue,
											componentCls : 'idera-ext-advfilter-panel-operand1',
											listeners : {
												change : function() {
													// On change of the combo box, the filter components
													// have to be rendered again on the basis of the value chosen.
													value = this.ownerCt.items.items[1].value;
													// If the value is not set in the operation already
													// the default value equals will be chosen.
													if(value == undefined) {
														value = 'equals';
													}
													
													//The following block removes the operation, operand2
													// components from the filter so that it can be rerendered
													// on the basis of column chosen.
													this.ownerCt.remove(this.ownerCt.items.items[2],true);
													this.ownerCt.remove(this.ownerCt.items.items[2],true);
													this.ownerCt.remove(this.ownerCt.items.items[2],true);
													
													if ("between" == value
															.toLowerCase()) {
														renderDateFieldsInFilter(this);
													} else if ("after" == value
															.toLowerCase()) {
														renderDateFieldsInFilter(
																this, true);
													} else if ("before" == value
															.toLowerCase()) {
														renderDateFieldsInFilter(
																this, false, true);
													} else {
														var column_name = this.ownerCt.items.items[0].value;
														renderTagFieldInFilter(
																this, column_name);
													}
													renderDeleteAndLogicOperator(this);
													Ext.getCmp("saveFilterBtn").setDisabled(false);
													Ext.getCmp("clearAllFiltersBtn").setDisabled(false);
													Ext.getCmp("applyFilterBtn").setDisabled(false);
												}
											}
										},
										{
											xtype : 'combo',
											width : widthOfOperator,
											store : operatorsStore,
											queryMode : 'local',
											displayField : 'operator',
											valueField : 'value',
											componentCls : 'idera-ext-advfilter-panel-operation',
											listeners : {
												change : {
													fn : function() {
														value = this.getValue();

														this.ownerCt.remove(this.ownerCt.items.items[2],true);
														this.ownerCt.remove(this.ownerCt.items.items[2],true);
														this.ownerCt.remove(this.ownerCt.items.items[2],true);
														
														if ("between" == value
																.toLowerCase()) {
															this.setWidth(93);
															renderDateFieldsInFilter(this);
														} else if ("after" == value
																.toLowerCase()) {
															this.setWidth(138);
															renderDateFieldsInFilter(
																	this, true);
														} else if ("before" == value
																.toLowerCase()) {
															this.setWidth(138);
															renderDateFieldsInFilter(
																	this, false,
																	true);
														} else {
															this.setWidth(138);
															var column_name = this.ownerCt.items.items[0].value;
															renderTagFieldInFilter(
																	this,
																	column_name);
														}
														renderDeleteAndLogicOperator(this);
													}
												}
											},
											value : operatorValue,

										},
										valueField,
										{
											xtype : 'combo',
											width : 81,
											store : operatorsStore1,
											queryMode : 'local',
											displayField : 'operator',
											valueField : 'value',
											value : relationOperator ? relationOperator : 'AND',
											componentCls : 'idera-ext-advfilter-panel-logicoperator'
										},
										{
											xtype : 'button',
											ui : 'round',
											width : 16,
											height : 16,
											text : '-',
											componentCls : 'idera-ext-round-button',
											listeners : {
												click : function() {
													try {
													    var count = Ext.get('contentBox').component.items.items.length;
													    if(count - 3 == 1) {
													    	Ext.getCmp("saveFilterBtn").setDisabled(true);
											                Ext.getCmp("clearAllFiltersBtn").setDisabled(true);
											                Ext.getCmp("applyFilterBtn").setDisabled(true);
													    }
													}
													catch(err) {
														
													} 
													
													this.findParentByType('panel')
															.destroy();
												}
											}
										}]
							});
		}

		// Method that returns data
		var fetchFilterData = function() {
			var filters = [];
			contentBox = Ext.getCmp("contentBox").items.items;
			for (i = 3; i < contentBox.length; i++) {
				var filter = {};
				panel = contentBox[i];

				filter.property = panel.items.items[0].value;
				filter.op = panel.items.items[1].value;
				filter.logicOperator = panel.items.items[4].value;

				if (panel.items.items[2].xtype == 'panel') {

					panelsSize = panel.items.items[2].items.length;
					// When Two Panel is used, it is Both Start Date Time and End
					// Date Time
					if (panelsSize == 2) {
						date_time_panel = panel.items.items[2].items;
						startDatePanel = date_time_panel.items[0];
						endDatePanel = date_time_panel.items[1];
						var startDate = startDatePanel.items.items[1].getValue();
						var startTime = startDatePanel.items.items[2].getValue();
						var endDate = endDatePanel.items.items[1].getValue();
						var endTime = endDatePanel.items.items[2].getValue();
						filter.value = [startDate,startTime,endDate,endTime];
					} else if (panelsSize == 1) {
						// When only one Panel is used, it is either start date,end
						// date or just date or just time
						singlePanel = panel.items.items[2].items.items[0];
						if (singlePanel.items.length == 2) {
							var date = singlePanel.items.items[0].getValue();
							var time = singlePanel.items.items[1].getValue();
							filter.value = {
								"date" : date,
								"time" : time,
							}
						} else if (singlePanel.items.length == 1) {
							var dateOrTime=singlePanel.items.items[0].getValue();
							if (singlePanel.items.items[0].xtype == "datefield") {
								filter.value = {
									"date" : dateOrTime,
								}
							} else if (singlePanel.items.items[0].xtype == "datefield") {
								filter.value = {
									"time" : dateOrTime,
								}
							}
						}
					}
				} else {
					filter.value = panel.items.items[2].value;
				}
				filters.push(filter);
			}
			return filters;

		}

		// Method Definition to clear the filter
		var clearFilters = function() {
			gridObject.store.clearFilter();
			Ext.getCmp("clearFilterBtn").setDisabled(true);
		}
		// Method Definition to apply the advance filter
		// 1. Clears all existing filters
		// 2. Fetched new data
		// 3. Applied filters based on fetched new values
		var applyAdvanceFilter = function(isSave) {
			clearFilters();
			filterValues = fetchFilterData();
			var remoteFilter = new Ext.util.Filter({
				// Following paramter will be sent to API
				property : 'advanceFilter',
				value : filterValues,
			});
			
			//Products can call an API here to update the JSON to back-end.
			console.log('The filter created in JSON format: ' +  JSON.stringify(filterValues));
			
			gridObject.store.filter(remoteFilter);
			Ext.getCmp("clearFilterBtn").setDisabled(false);
			var filter_name = Ext.getCmp("customFilterName").getValue();
	        var applied_filter = Ext.get('applied_filters').empty();
	        Ext.get('filter_label').show();
			var appliedFilterLabel = Ext.create('Ext.container.Container',{
		        componentCls: 'idera-ext-advfilter-applied-filter',
		        renderTo: Ext.get('applied_filters'),
		        autoEl: {
		            tag: 'div'
		        },
		        items:[{
		        	   xtype: 'component',
		        	   componentCls: 'idera-ext-advfilter-label',
		        	   autoEl: {
		   	               tag: 'div',
		   	               html: (filter_name != "" && filter_name != undefined && isSave) ? filter_name : "Unsaved Filter" 
		        	   }
		           }, {
		        	   xtype: 'component',
		        	   componentCls: 'idera-ext-advfilter-close',
		        	   autoEl: {
		   	               tag: 'div'
		        	   },
		        	   listeners: {
		        		   render: function(c) {
		        			   c.getEl().on({
		        					click: function() {
		        						Ext.getCmp('clearFilterBtn').click();
		        						Ext.get('applied_filters').empty();
		        						Ext.get('filter_label').hide();
		        						$("#filter_label").css("display","none");
		        						Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));	        						
		        					}
		        				});
		        		   }
		        	   }
		           }],
		        listeners: {
		            click: function() {
		                // handle click here
		            	return;
		            }
		        }
			});

			var newFilterName = Ext.getCmp("customFilterName").getValue();
			
			if (isSave && !isEdit) {
				//GOWRESWARAN Insert into a map with newFilterName as key and filterValues as value which can be used when clicked again.
				userSavedFilters[newFilterName]=filterValues;
				gridObject.if_applied_filter_saved = true;
				gridObject.applied_filter = undefined;
				var data = {
					"name" : newFilterName,
					"filter" : filterValues
				}			
				Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("noSavedFilter"));
				Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
				Ext.getCmp("advanceFilterMenu").add(
						createSavedFilterMenuItem(newFilterName, newFilterName));
			}
			else if(isSave && isEdit){
				//GOWRESWARAN Update into a map with newFilterName as key and filterValues as value which can be used when clicked again.
				userSavedFilters[newFilterName]=filterValues;
				gridObject.if_applied_filter_saved = true;
				gridObject.applied_filter = undefined;
				if(isEditingUnSavedFilter){
					Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
					Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("noSavedFilter"));
					Ext.getCmp("advanceFilterMenu").add(
							createSavedFilterMenuItem(newFilterName, newFilterName));
				}
			}
			else if(!isSave){
				gridObject.if_applied_filter_saved = false;
				gridObject.applied_filter = {
					"name" : "",
					"filter": filterValues
				};
				Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("noSavedFilter"));
				Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
				Ext.getCmp("advanceFilterMenu").add(
						createSavedFilterMenuItem("Unsaved Filter", "unSavedFilter"));
				var advancedButton = Ext.get('advanced-filter-button');
				advancedButton.focus();
			}
			
			if(isSave){
		        
				Ext.Ajax.request({
		    		url : productRestUrlHTTPS+'/AddAlertsAdvanceFilter?filterName='+newFilterName+'&filterConfig='+JSON.stringify(filterValues),
		    		method:'POST',
		    		headers: {"Authorization": header},
		    		success : function(response, opts) {
		    			console.log(response.responseText);
		    		},

		    		failure : function(response, opts) {
		    			console.log('server-side failure with status code '
		    					+ response.status);
		    		}
		    	});
			}

		}

		var deleteSavedFilter = function(id) {
			//GOWRESWARAN Remove from map with newFilterName as key
			delete userSavedFilters[id];
			Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp(id));
			// Within the container we have one component for filner name and separator
			// So if the number of the components in the container is 2, it does not have
			// any filter appended yet.
			if (Ext.getCmp("advanceFilterMenu").items.length == 2) {
				Ext.getCmp("advanceFilterMenu").add({
					text : 'No Saved Filters',
					disabled : true,
					id : 'noSavedFilter'
				});
			}
			if(Ext.get('applied_filters').el.dom.innerText.trim()==id){
				Ext.getCmp('clearFilterBtn').click();
			}
			if(id=="unSavedFilter"){
				Ext.getCmp('clearFilterBtn').click();
			}else{
				Ext.Ajax.request({
		    		url : productRestUrlHTTPS+'/DeleteAlertsAdvanceFilter?filterName='+id,
		    		method:'POST',
		    		headers: {"Authorization": header},
		    		success : function(response, opts) {
		    			console.log(response.responseText);
		    		},

		    		failure : function(response, opts) {
		    			console.log('server-side failure with status code '
		    					+ response.status);
		    		}
		    	});
			}
			
			
		}

		var applyFilter = function(id) {
			if(id!="unSavedFilter"){
				Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
			}
			// Fetch the Filter JSON configuration and apply the filter
			console.log(id);
			var applied_filter = Ext.get('applied_filters').empty();
	        Ext.get('filter_label').show();
			
			Ext.create('Ext.container.Container',{
		        componentCls: 'idera-ext-advfilter-applied-filter',
		        renderTo: Ext.get('applied_filters'),
		        autoEl: {
		            tag: 'div'
		        },
		        items:[{
		        	   xtype: 'component',
		        	   componentCls: 'idera-ext-advfilter-label',
		        	   autoEl: {
		   	               tag: 'div',
		   	               html: id
		        	   }
		           }, {
		        	   xtype: 'component',
		        	   componentCls: 'idera-ext-advfilter-close',
		        	   autoEl: {
		   	               tag: 'div'
		        	   },
		        	   listeners: {
		        		   render: function(c) {
		        			   c.getEl().on({
		        					click: function() {
		        						Ext.getCmp('clearFilterBtn').click();
		        						Ext.get('applied_filters').empty();
		        						Ext.get('filter_label').hide();
		        						$("#filter_label").css("display","none");
		        						Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
		        					}
		        				});
		        		   }
		        	   }
		           }],
		        listeners: {
		            click: function() {
		                // handle click here
		            	return;
		            }
		        }
			});
			
			//GOWRESWARAN Fet obj from Map and reuse to apply to filter.
			obj=userSavedFilters[id];
			var remoteFilter = new Ext.util.Filter({
				// Following paramte!r will be sent to API
				property : 'advanceFilter',
				value : obj,
			});
			gridObject.store.filter(remoteFilter);
			Ext.getCmp("clearFilterBtn").setDisabled(false);
		}

		var createSavedFilterMenuItem = function(name, id) {
			var tooltip;
			return {
				text : name,
				id:id,
	            listeners:{
	                click:{
	                    fn:function(){
	                    	applyFilter(id);
	                        setTimeout(function(){
	                            tooltip.close();
	                        }, 200);
	                    }
	                },
	                afterrender: function(){
	                    setTimeout(function(){
	                    	tooltipTools=[];
	                    	tooltipWidth=56;
	                    	if(id=='unSavedFilter'){
	                    		tooltipWidth=76;
	                    		tooltipTools.push({
	                                iconCls: 'x-fa fa-save',
	                                handler: function(event, toolEl, panelHeader) {
	                                    showAdvancedFilterDialog(gridObject,true,"",true,productRestUrlHTTPS,header,dmProducts);
	                                    tooltip.close();
	                                }
	                            });
	                    		tooltipTools.push({
	                                iconCls: 'x-fa fa-pencil',
	                                handler: function(event, toolEl, panelHeader) {
	                                    showAdvancedFilterDialog(gridObject,true,"",true,productRestUrlHTTPS,header,dmProducts);
	                                    tooltip.close();
	                                }
	                            });
	                    		tooltipTools.push({ 
	                                iconCls: 'x-fa fa-trash',
	                                handler: function(event, toolEl, panelHeader) {
	                                    deleteSavedFilter(id);
	                                    gridObject.if_applied_filter_saved = true;
	        							gridObject.applied_filter = undefined;
	                                    tooltip.close();
	                                    Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
	                                }
	                            });
	                    	}else{
	                    		tooltipTools.push({
	                                iconCls: 'x-fa fa-pencil',
	                                handler: function(event, toolEl, panelHeader) {
	                                    showAdvancedFilterDialog(gridObject,true,name,false,productRestUrlHTTPS,header,dmProducts);
	                                    tooltip.close();
	                                }
	                            });
	                    		tooltipTools.push({ 
	                                iconCls: 'x-fa fa-trash',
	                                handler: function(event, toolEl, panelHeader) {
	                                    deleteSavedFilter(id);
	                                    tooltip.close();
	                                }
	                            });
	                    	}
	                        tooltip = Ext.create('Ext.tip.ToolTip', {
	                            target: id, // id of the target for which to show the tooltip
	                            cls: 'idera-ext-advanced-grid-tooltip',
	                            anchor: 'left',
	                            width: tooltipWidth,
	                            height: 36,
	                            shadow: false,
	                            // autoHide should be false.
	                            // If set to true the tooltip hides itself and the buttons inside can't be clicked
	                            autoHide: true,
	                            hideDelay : 5000,
	                            // This adds the close icon for the tooltip when set to true
	                            closable : false,
	                            bodyPadding: 0,
	                            tools: tooltipTools,
	                        })
	                    }, 300);
	                }
	            }
			}
		}

		var createAdvancedDialog = function() {
			var afw = Ext
					.create(
							'Ext.window.Window',
							{
								width : 720,
								height : 530,
								modal:true,//To make the window with overlay to mask everything behind it when displayed
								constrain : true,
								componentCls : 'idera-ext-window',
								cls : 'idera-ext-advfilter-window',
								title : 'Advance Filtering',
								id: 'advfilter-window',
								titleAlign : 'left',
								listeners: {
									focusleave: {
										fn: function() {
											var advancedButton = Ext.get('advanced-filter-button');
											advancedButton.focus();	
										}
									},
									close: function() {
										var advancedButton = Ext.get('advanced-filter-button');
										advancedButton.focus();
									},
	                                afterRender: function () {
	                                    setTimeout(function () {
	                                        var element = Ext.get('customFilterName');
	                                        if(element) {
	                                            element.component.focus();
	                                        }
	                                    }, 300);
	                                }
								},
								items : [
										{
											xtype : 'container',
											height : 52,
											componentCls : 'idera-ext-description-box',
											items : [ {
												html : [ 'Create custom filters that allow you segment instances using single or multiple conditions.' ]
											} ]
										},
										{
											xtype : 'container',
											componentCls : 'idera-ext-content-box',
											id : 'contentBox',
											items : topItems,
										},
										{
											xtype : 'button',
											text : 'Add Filter',
											margin: "16px 0px 16px 16px",
											componentCls : 'idera-ext-advfilter-addfilter-button',
											listeners : {
												click : {
													fn : function() {
														this.ownerCt.items.items[1]
																.add(createPanel(
																		"", "", "",
																		""));

													}
												}
											}
										},
										{
											xtype : 'button',
											text : 'Clear Filters',
											id:'clearAllFiltersBtn',
											disabled:true,
											componentCls : 'idera-ext-advfilter-applyfilter-button',
											listeners : {
												click : {
													fn : function() {
														var count = Ext.get('contentBox').component.items.items.length;
														for(var i = 3; i < count; i++) {
															Ext.get('contentBox').component.remove(Ext.get('contentBox').component.items.items[3],true);
														}
														Ext.getCmp("saveFilterBtn").setDisabled(true);
										                Ext.getCmp("clearAllFiltersBtn").setDisabled(true);
										                Ext.getCmp("applyFilterBtn").setDisabled(true);
													}
												}
											}
										} ],
								bbar : [ '->', {
									type : 'button',
									scale : 'medium',
									text : 'Cancel',
									handler : function() {
										var win = Ext.WindowManager.getActive();
										if (win) {
											win.close();
										}
									}
								}, {
									type : 'button',
									scale : 'medium',
									text : 'Apply Filters without Saving ',
									id:'applyFilterBtn',
									disabled:true,
									handler : function() {
										applyAdvanceFilter(false);
										var win = Ext.WindowManager.getActive();
										if (win) {
											win.close();
										}
									}
								}, {
									type : 'button',
									scale : 'medium',
									text : 'Save and Filter',
									disabled:true,
									id:'saveFilterBtn',
									handler : function() {
										var filterName = Ext.getCmp("customFilterName").getValue();
										if(undefined == filterName || "" == filterName) {
											Ext.get('customFilterName').addCls('idera-textbox-required');
											Ext.getCmp("customFilterName").markInvalid("This field is required");
	                                        return;
										}
										/* On save we have to set the if_saved_filter_flag to false */
										gridObject.if_applied_filter_saved = true;
										gridObject.applied_filter = undefined;
										applyAdvanceFilter(true);
										var win = Ext.WindowManager.getActive();
										if (win) {
											win.close();
										}
									}
								} ]
							});
			afw.show();
			//afw.focus();
			
			// Create the tooltip for help anchor 
			var tip1 = Ext.create('Ext.tip.ToolTip', {
				target: 'help-tooltip',
				html: 'A field is what the grid will be filter by for example instance.<br/><br/>An operator compares'     +
					  'the value of a field on its left with one or more values (or functions) on its right. Word'         + 
					  'Operators can be equal, not equal, contains, does not contain a word(s). Wild Card symbol * '       +
					  'can be added to contain statements as a way to match select characters to multiple expressions'     +
					  ' for example Windows*. Number Operators can be greater than, less than, equal and greater than, '   +
					  'equal and less than, equal or not equal to a number value. Date/Time Operators are before, after '  +
					  'and between a set time period.<br/><br/>'                                                           +
					  'A conditional statement can be applied to filters to determine if additional filter(s) should be '  +
					  'added, in place of or excluded from the orginal filter.  Conditional statements are AND, OR, NOT.',
				dismissDelay: 0,
				anchor:'left',
				width: 446
			});
		}
			
		if (isEdit) {
			if(gridObject.applied_filter != undefined && isEditingUnSavedFilter) {
				filterName = gridObject.applied_filter.name;
				initiateTopItems();
				filters = gridObject.applied_filter.filter
				for (i = 0; i < filters.length; i++) {
					filter = filters[i]
					topItems.push(createPanel(filter.property, filter.op,
							filter.value, filter.logicOperator));
				}
				createAdvancedDialog();
				//Enabling all buttons as Edit functionality
				Ext.getCmp("saveFilterBtn").setDisabled(false);
				Ext.getCmp("clearAllFiltersBtn").setDisabled(false);
				Ext.getCmp("applyFilterBtn").setDisabled(false);
				return;
			}
			
			//GOWRESWARAN
			//Fetch this from local map which is updated during save and populate again. (Use savedFilterName which is param passed in method showAdvancedFilterDialog)
			filterName = savedFilterName;
			initiateTopItems();
			filters = userSavedFilters[savedFilterName];
			for (i = 0; i < filters.length; i++) {
				filter = filters[i]
				topItems.push(createPanel(filter.property, filter.op,
						filter.value, filter.logicOperator));
			}
			createAdvancedDialog();
			//Enabling all buttons as Edit functionality
			Ext.getCmp("saveFilterBtn").setDisabled(false);
			Ext.getCmp("clearAllFiltersBtn").setDisabled(false);
			Ext.getCmp("applyFilterBtn").setDisabled(false);
			
			
		} else {
			initiateTopItems();
			topItems.push(createPanel("","","",""));
			createAdvancedDialog();
		}
}
	
    Ext.state.Manager.setProvider(Ext.create('Ext.state.LocalStorageProvider'));
    Ext.tip.QuickTipManager.init();
    Ext.define('SQLDMAlerts', {
        extend: 'Ext.data.Model',
        fields: [{
            "name": "Severity",
            "type": "string",
            "mapping": "name"
        }, {
            "name": "time",
            "type": "string",
            "mapping": "UTCOccurrenceDateTime"
        }, {
            "name": "summary",
            "type": "string",
            "mapping": "Heading"
        }, {
            "name": "Instance",
            "type": "string",
            "mapping": "ServerName"
        }, {
            "name": "SQLdmRepository",
            "type": "string",
            "mapping": "DatabaseName"
        }, {
            "name": "Category",
            "type": "string",
            "mapping": "Metric.MetricCategory"
        }, {
            "name": "details",
            "type": "string",
            "mapping": ""
        },{
            "name": "MetricName",
            "type": "string",
            "mapping": "Metric.Name"
        }]
    });

    Ext.on('loadAlertsList', function (restUrl, header, productRestUrl,dmProducts,isLocalProduct) {
        // CSS Logic to make grid fit in page
        var windowHeight = $('body').height();
        $('.cwf-overview-wrapper').parent().height(windowHeight - 36);
        $('.cwf-overview-wrapper').find('.z-window-embedded').height('100%');
        $('.cwf-overview-wrapper').find('.z-window-embedded > div').height('100%');
      
        if (Ext.getCmp('sqldm-alerts-list') !== undefined) {
            Ext.getCmp('sqldm-alerts-list').destroy();
        }
        var pattern = /http:\/\/(.*):(\d+)\/(.*)/i;
        if (pattern.test(restUrl)) {
            var match = pattern.exec(restUrl);
            restUrl = "https://" + match[1] + ":5171/" + match[3];
            if(isLocalProduct=="true"){
            	restUrl = "https://" + document.location.hostname + ":5171/" + match[3];
            }
        }
        
        if (pattern.test(productRestUrl)) {
            var match = pattern.exec(productRestUrl);
            productRestUrlHTTPS = "https://" + match[1] + ":5171/" + match[3];
            if(isLocalProduct=="true"){
            	productRestUrlHTTPS = "https://" + document.location.hostname + ":5171/" + match[3];
            }
        }
        
		
        var rowEditing1 = Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToMoveEditor: 1,
            autoCancel: false
        });

        var psize = Ext.state.Manager.get('alertsGrid.pageSize');
    	if(psize==undefined){
    		psize=25;
    	}
    	
        var myStore1 = Ext.create('Ext.data.Store', {
            storeId: 'sqldm-alerts-store',
            remoteSort: true,
            remoteFilter: true,
            model: 'SQLDMAlerts',
            autoLoad: 'true',
            proxy: {
                type: 'ajax',
                url: restUrl,
                headers: {"Authorization": header},
                reader: {
                    type: 'json',
                    root: 'Alerts',
                    totalProperty: 'totalAlerts'
                },
                pageParam:'gPage',
                limitParam:'gLimit',
                enablePaging: true,
                timeout:180000,
            },
            autoLoad: {start: 0, limit: psize},
            pageSize: psize,
            listeners:{
    			beforeload: function( store, operation, eOpts ){
    				Ext.state.Manager.set('alertsGrid.pageSize',store.pageSize);
    			} 
    		}
        });

        var bbar = Ext.create('Ext.PagingToolbarWithNumber', {
            store: Ext.data.StoreManager.lookup('sqldm-alerts-store'),
            displayInfo: false,
            displayMsg: 'Displaying {0} - {1} of {2} records',
            emptyMsg: "No topics to display",
            disablePageSizeInput: false
        });

        var sqldm_ext_alerts_grid = Ext
            .create(
                'Ext.grid.Panel',
                {
                    header: false,
                    store: Ext.data.StoreManager
                        .lookup('sqldm-alerts-store'),
                    dockedItems: [{
                        xtype: 'toolbar',
                        dock: 'top',
                        padding: "4px 8px 4px 8px",
                        border: 0,
                        items: [
                            {
                                iconCls: 'idera-grid-download',
                                menu: {
                                    xtype: 'menu',
                                    items: [
                                        {
                                            text: 'Excel',
                                            listeners: {
                                                click: {
                                                    fn: function () {
                                                        sqldm_ext_alerts_grid
                                                            .saveDocumentAs({
                                                                type: 'xlsx',
                                                                title: 'My export',
                                                                fileName: 'myExport.xlsx'
                                                            });
                                                    }
                                                }
                                            }
                                        },
                                        {
                                            text: 'CSV',
                                            listeners: {
                                                click: {
                                                    fn: function () {
                                                        sqldm_ext_alerts_grid
                                                            .saveDocumentAs({
                                                                type: 'csv',
                                                                title: 'My export',
                                                                fileName: 'myExport.csv'
                                                            });
                                                    }
                                                }
                                            }
                                        },
                                        {
                                            text: 'HTML',
                                            listeners: {
                                                click: {
                                                    fn: function () {
                                                        sqldm_ext_alerts_grid
                                                            .saveDocumentAs({
                                                                type: 'html',
                                                                title: 'My export',
                                                                fileName: 'myExport.html'
                                                            });
                                                    }
                                                }
                                            }
                                        }]
                                }
                            },
                            {
            					text : 'Advanced Filter',
            					glyph: "xf0b0@FontAwesome",
            					/* The id for the advanced filter is needed to focus back on the grid when window is closed */
            					/* All ids have to be changed when implemented for products to ensure that the focus goes back */
            					/* to the correct grid */
            					id: 'advanced-filter-button',
            					menu : {
            						xtype : 'menu',
            						width: 174,
            						id:'advanceFilterMenu',
            						componentCls: 'idera-advanced-filter-menu',
            						items : [
            								{
            									text : 'Create New Filter',
            									xtype: 'button',
            									margin: 4,
            									componentCls: 'idera-advfilter-menu-button',
            									listeners:{
            										click:{
            											fn:function(){
            												Ext.getCmp('clearFilterBtn').click();
            												Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
            												if(Ext.get('applied_filters').el.dom.innerText.trim()=="Unsaved Filter"||Ext.get('applied_filters').el.dom.innerText.trim()=="unSavedFilter"){
            													Ext.getCmp('clearFilterBtn').click();
            													sqldm_ext_alerts_grid.if_applied_filter_saved = true;
            													sqldm_ext_alerts_grid.applied_filter = undefined;
            												}
            												showAdvancedFilterDialog(sqldm_ext_alerts_grid,false,undefined,false,productRestUrlHTTPS,header,dmProducts);
            											}
            										}
            									}
            								}, {
            									xtype : 'tbseparator',
            									componentCls: 'idera-ext-advanced-filter-menu-seperator'
            										
            								}]
            					}
            				},
            				{
            					xtype : 'button',
            					text : 'Clear Filters',
            					id:'clearFilterBtn',
            					disabled:true,
            					listeners:{
            						click:{
            							fn:function(){
            								Ext.get('filter_label').hide();
            								Ext.get('applied_filters').empty();
            								$("#filter_label").css("display","none");
            								myStore1.clearFilter();
            								Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
            								this.setDisabled(true);
            							}
            						}
            					}
            				}
                        ]
                    }],
                    id: 'sqldm-alerts-list',
                    collapsible: true,
                    stateful: true,
                    stateId: 'sqldmAlertsListGrid',
                    collapsed: false,
                    columns: [
                        {
                            "text": "!",
                            "width": 20,
                            "sortable": true,
                            "hideable": false,
                            "stateId": "alerts.status",
                            renderer: function (value, meta, record) {
                                if (record.data.Severity == "Critical") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (record.data.Severity == "Warning") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (record.data.Severity == "Ok") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return "";
                            }
                        },
                        {
                            "text": "Time",
                            "width": 60,
                            "sortable": true,
                            "hideable": false,
                            "dataIndex": "time",
                            "stateId": "alerts.time",
                            renderer: function (value, meta, record) {
                                var regex = /\/Date\((.*)\)\//i;
                                var returnValue = "NA";
                                if (regex.test(value)) {
                                    var regex_match = value.match(regex);
                                    var date = new Date(parseInt(regex_match[1]));
                                    var month = date.getMonth() + 1;
                                    var day = date.getDate();
                                    var year = date.getFullYear();
                                    var hours = date.getHours();
                                    var minutes = date.getMinutes();
                                    if (minutes < 10) {
                                        minutes = "0" + minutes;
                                    }
                                    var ampm = "AM";
                                    if (hours > 12) {
                                        hours -= 12;
                                        ampm = "PM";
                                    }
                                    returnValue = month + "/" + day + "/" + year + " " + hours + ":" + minutes + " " + ampm;
                                }
                                return returnValue;
                            }
                        },
                        {
                            "text": "Summary",
                            "width": 200,
                            "sortable": true,
                            "hideable": false,
                            "dataIndex": "summary",
                            "stateId": "alerts.summary"
                        }, {
                            "text": "Instance",
                            "width": 150,
                            "sortable": true,
                            "hideable": false,
                            "dataIndex": "Instance",
                            "stateId": "alerts.instanceName",
                            renderer: function (value, meta, record) {
                                var regex = /.*:\/\/.*\/.*\/(.*)\/.*/i;
                                var insName = "";
                                if(regex.test(window.location.href)) {
                                    var match = window.location.href.match(regex);
                                    insName = match[1];
                                }
                                var sqlServerId = record.get("SQLServerId");
                                var url = "/sqldm/"+insName+"/singleInstance/"+sqlServerId;
                                meta.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                return "<a href="+url+">" + value + "</a>";
                            }
                        }, {
                            "text": "Database",
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "SQLdm Repository",
                            "stateId": "alerts.databaseName",
                            renderer: function (value, meta, record) {
                                if (value == null || value.length() == 0) {
                                    return "N/A";
                                }
                                return value;
                            }
                        }, {
                            "text": "Category",
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Category",
                            "stateId": "alerts.category"
                        }, {
                            "text": "Show Details",
                            "width": 90,
                            renderer: function (value, meta, record) {
                                return "<a href='#'>Show Details</a>"
                            },
                            listeners : {
                                click: function () {
                                    var alert = JSON.stringify(arguments[5].data);
                                    var alertsList = [];
                                    for (var i = 0; i < Ext.getStore('sqldm-alerts-store').data.items.length; i++) {
                                        alertsList.push(Ext.getStore('sqldm-alerts-store').data.items[i].data);
                                    }
                                    zAu.send(new zk.Event(zk.Widget.$('$alertsShowDetailsListenerDiv'), "onCustomEvent", [alert, JSON.stringify(alertsList)], {toServer: true}));
                                }
                            }
                        }
                    ],
                    forceFit: true,
                    height: isNaN(windowHeight) ? 500 : windowHeight - 206,
                    renderTo: Ext.get("sqldm-alerts-list-grid"),
                    listeners : {
            			focusleave: {
            				fn: function() {
            					if(Ext.get('advfilter-window') != null) {
            					    return;
            					}
            					else {
            						 /* if the saved filter is false and there are no applied filters, then prompt has to be shown for save */ 
            						if(!sqldm_ext_alerts_grid.if_applied_filter_saved && Ext.get('applied_filters').dom.childNodes.length > 0) {
            							Ext.MessageBox
            							.show({
            								title : 'Question',
            								cls : "idera-ext-questionbox",
            								width : 600,
            								message : "You need to save the changes to the applied advanced filter or it will be lost. Do you want to save your modifications now?",
            								buttonAlign : 'right',
            								buttons : Ext.Msg.YESNO,
            								iconCls : 'idera-message-box-icon-question',
            								fn : function(
            										btn) {
            									if (btn == "no") {
            										 /* If the user does not intend to save the filter, the if_applied_filter will be set to true */ 
            										 /* to prevent further prompts */ 
            										sqldm_ext_alerts_grid.if_applied_filter_saved = true;
            										sqldm_ext_alerts_grid.applied_filter = undefined;
                                                    Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
            										var advancedButton = Ext.get('advanced-filter-button');
            										Ext.getCmp('clearFilterBtn').click();
            										advancedButton.focus();
            									} else if (btn == "yes") {
            										 /* If the user selects yes, advanced filter window needs to be opened with current filter */ 
            										showAdvancedFilterDialog(sqldm_ext_alerts_grid, true,undefined,true,productRestUrlHTTPS,header,dmProducts);
            									}
            								}
            							});
            						}
            					}
            				}
            			}
            		},
                    plugins: [{
                        "ptype": "gridfilters"
                    }, rowEditing1, {
                        "ptype": "gridexporter"
                    }],
                    features: [{
                        ftype: 'grouping'
                    }],
                    bbar: bbar,
                });

        Ext.on("groupByTabNavigation", function (tabName) {
        	Ext.getCmp('clearFilterBtn').click();
            switch (tabName) {
                case "Severity":
                	myStore1.sort("Severity");
                    myStore1.group("Severity");
                    Ext.getCmp('advanced-filter-button').setVisible(false);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
                    break;
                case "Instance":
                	myStore1.sort("Instance");
                    myStore1.group("Instance");
                    Ext.getCmp('advanced-filter-button').setVisible(false);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
                    break;
                case "Metric":
                	myStore1.sort("Metric");
                    myStore1.group("MetricName");
                    Ext.getCmp('advanced-filter-button').setVisible(false);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
                    break;
                case "Category":
                	myStore1.sort("Category");
                    myStore1.group("Category");
                    Ext.getCmp('advanced-filter-button').setVisible(false);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
                    break;
                case "SQLdmRepo":
                	myStore1.sort("SQLdmRepository");
                    myStore1.group("SQLdmRepository");                    
                    Ext.getCmp('advanced-filter-button').setVisible(false);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
                    break;
				case "AdvancedFilter":
                    myStore1.group("");
                    Ext.getCmp('advanced-filter-button').setVisible(true);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
                    break;
                default:
                	myStore1.sort("time");
                    myStore1.group("");
                    myStore1.sort("time");
                    Ext.getCmp('advanced-filter-button').setVisible(false);
                    Ext.getCmp("clearFilterBtn").setDisabled(true);
            }
        });
        
        var tabs = Ext.create('Ext.tab.Panel', {
            renderTo: 'alerts_grid_in_tab_panel',
            componentCls: 'idera-ext-tabpanel',
            id: 'alerts_tab_grid',
            width: "100%",
            activeTab: 0,     // first tab initially active
            bodyCls: 'alerts_tab_grid-body',
            items: [
                {
                    title: 'Active',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "Active");
                            }
                        }
                    }
                },
                {
                    title: 'Severity',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "Severity");
                            }
                        }
                    }
                },
                {
                    title: 'Instance',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "Instance");
                            }
                        }
                    }
                },
                {
                    title: 'Metric',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "Metric");
                            }
                        }
                    }
                },
                {
                    title: 'Category',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "Category");
                            }
                        }
                    }
                },
                {
                    title: 'SQLdmRepository',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "SQLdmRepo");
                            }
                        }
                    }
                },
                {
                    title: 'Advanced Filter',
                    tabConfig: {
                        listeners: {
                            click: function() {
                                Ext.fireEvent("groupByTabNavigation", "AdvancedFilter");
                            }
                        }
                    }
                }
            ],
            listeners: {
                afterrender: function () {
                    // This is needed to fix SQLCORE-5357 where the button bar spacing is changing on tab change
                    setTimeout(function() {
                        Ext.getCmp("alerts_tab_grid").setActiveTab(1);
                        Ext.getCmp("alerts_tab_grid").setActiveTab(0);
                        Ext.fireEvent("groupByTabNavigation", "Active");
                    },300);
                }
            }
        });
        
        /* When the grid is instantiated there is no applied filter on the grid */
    	/* So setting the if_applied_filter_saved to true to prevent prompt for */
    	/* save on auto-focus */
    	sqldm_ext_alerts_grid.if_applied_filter_saved = true;
    	sqldm_ext_alerts_grid.applied_filter = undefined;
        
    	Ext.Ajax.request({
    		url : productRestUrlHTTPS+'/GetAlertsAdvanceFilters',
    		headers: {"Authorization": header},
    		success : function(response, opts) {
    			var obj = JSON.parse(response.responseText);
    			
    			if(obj.length==0){
    				Ext.getCmp("advanceFilterMenu").add({text:'No Saved Filters',disabled:true,id:'noSavedFilter'});
    			}
    			for (i = 0; i < obj.length; i++) {
    				savedFilterItem = createSavedFilterMenuItem(obj[i].filterName,obj[i].filterName);
    				Ext.getCmp("advanceFilterMenu").add(savedFilterItem);
    				userSavedFilters[obj[i].filterName]=obj[i].filterConfig;
    			}
    		},

    		failure : function(response, opts) {
    			console.log('server-side failure with status code '
    					+ response.status);
    		}
    	});
    	
    	var deleteSavedFilter = function(id){
    		//Add API to delete the Filter from backedn
    		Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp(id));
    		if(Ext.getCmp("advanceFilterMenu").items.length==2){
    			Ext.getCmp("advanceFilterMenu").add({text:'No Saved Filters',disabled:true,id:'noSavedFilter'});
    		}
    		if(Ext.get('applied_filters').el.dom.innerText.trim()==id){
    			Ext.getCmp('clearFilterBtn').click();
    		}
    		
    		Ext.Ajax.request({
        		url : productRestUrlHTTPS+'/DeleteAlertsAdvanceFilter?filterName='+id,
        		method:'POST',
        		headers: {"Authorization": header},
        		success : function(response, opts) {
        			console.log(response.responseText);
        		},

        		failure : function(response, opts) {
        			console.log('server-side failure with status code '
        					+ response.status);
        		}
        	});
    		
    	}
    	
    	var applyFilter = function(id){
    		if(id!="unSavedFilter"){
    			Ext.getCmp("advanceFilterMenu").remove(Ext.getCmp("unSavedFilter"));
    		}
    		//Fetch the Filter JSON configuration and apply the filter
    		console.log(id);
            var applied_filter = Ext.get('applied_filters').empty();
            Ext.get('filter_label').show();
    		
    		Ext.create('Ext.container.Container',{
    	        componentCls: 'idera-ext-advfilter-applied-filter',
    	        renderTo: Ext.get('applied_filters'),
    	        autoEl: {
    	            tag: 'div'
    	        },
    	        items:[{
    	        	   xtype: 'component',
    	        	   componentCls: 'idera-ext-advfilter-label',
    	        	   autoEl: {
    	   	               tag: 'div',
    	   	               html: id
    	        	   }
    	           }, {
    	        	   xtype: 'component',
    	        	   componentCls: 'idera-ext-advfilter-close',
    	        	   autoEl: {
    	   	               tag: 'div'
    	        	   },
    	        	   listeners: {
    	        		   render: function(c) {
    	        			   c.getEl().on({
    	        					click: function() {
    	        						Ext.getCmp('clearFilterBtn').click();
    	        						Ext.get('applied_filters').empty();
    	        						Ext.get('filter_label').hide();
    	        						$("#filter_label").css("display","none");
    	        					}
    	        				});
    	        		   }
    	        	   }
    	           }],
    	        listeners: {
    	            click: function() {
    	                // handle click here
    	            	return;
    	            }
    	        }
    		});
    		
    		//GOWRESWARAN Fet obj from Map and reuse to apply to filter.
    		obj=userSavedFilters[id];
    		var remoteFilter = new Ext.util.Filter({
    			// Following paramte!r will be sent to API
    			property : 'advanceFilter',
    			value : obj,
    		});
    		sqldm_ext_alerts_grid.store.filter(remoteFilter);
    		Ext.getCmp("clearFilterBtn").setDisabled(false);
    	}

        var createSavedFilterMenuItem = function(name,id){
            var tooltip;
            return {
                text : name,
                id:id,
                listeners:{
                    click:{
                        fn:function(){
                            applyFilter(id);
                            setTimeout(function(){
                                tooltip.close();
    						}, 200);
                        }
                    },
                    afterrender: function(){
                        setTimeout(function(){
                            tooltip = Ext.create('Ext.tip.ToolTip', {
                                target: id, // id of the target for which to show the tooltip
                                cls: 'idera-ext-advanced-grid-tooltip',
                                anchor: 'left',
                                width: 56,
                                height: 36,
                                shadow: false,
                                // autoHide should be false.
                                // If set to true the tooltip hides itself and the buttons inside can't be clicked
                                autoHide: true,
                                hideDelay: 5000,
                                // This adds the close icon for the tooltip when set to true
                                closable : false,
                                bodyPadding: 0,
                                tools: [{ // edit icon to edit saved filter
                                    iconCls: 'x-fa fa-pencil',
                                    handler: function(event, toolEl, panelHeader) {
                                        showAdvancedFilterDialog(sqldm_ext_alerts_grid,true,name,false,productRestUrlHTTPS,header,dmProducts);
                                        tooltip.close();
                                    }
                                },
                                    { // Delete icon to save delete filter
                                        iconCls: 'x-fa fa-trash',
                                        handler: function(event, toolEl, panelHeader) {
                                            deleteSavedFilter(id);
                                            tooltip.close();
                                        }
                                    }]
                            })
                        }, 300);
                    }
                }
            }
        }
        
    	
    });
	
});