jq2 = jQuery.noConflict();
jq2(function ($) {
	
	var tooltipRenderer = function(value, metaData, record, rowIdx, colIdx,
			store) {
		value = Ext.String.htmlEncode(value);
		metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
		return value;
	}
	
    Ext.state.Manager.setProvider(Ext.create('Ext.state.LocalStorageProvider'));
    Ext.tip.QuickTipManager.init();
    Ext.define('User', {
        extend: 'Ext.data.Model',
        fields: [{
            "name": "Overview.InstanceName",
            "type": "string",
            "mapping": "Overview.InstanceName",
        }, {
            "name": "ServerStatus.HealthIndex",
            "type": "string",
            "mapping": "ServerStatus.HealthIndex",
        }, {
            "name": "Overview.ProductVersion",
            "type": "string",
            "mapping": "Overview.ProductVersion",
        }, {
            "name": "Overview.sqlServiceStatus",
            "type": "string",
            "mapping": "Overview.sqlServiceStatus",
        }, {
            "name": "Overview.agentServiceStatus",
            "type": "string",
            "mapping": "Overview.agentServiceStatus",
        }, {
            "name": "Overview.dtcServiceStatus",
            "type": "string",
            "mapping": "Overview.dtcServiceStatus",
        }, {
            "name": "Overview.osMetricsStatistics.AvailableBytes",
            "type": "string",
            "mapping": "Overview.osMetricsStatistics.AvailableBytes",
        }, {
            "name": "Overview.systemProcesses.blockedProcesses",
            "type": "string",
            "mapping": "Overview.systemProcesses.blockedProcesses",
        }, {
            "name": "Overview.statistics.cpuPercentage",
            "type": "string",
            "mapping": "Overview.statistics.cpuPercentage",
        }, {
            "name": "Overview.osMetricsStatistics.AvgDiskQueueLength",
            "type": "string",
            "mapping": "Overview.osMetricsStatistics.AvgDiskQueueLength",
        },{
            "name" : "Overview.Tags",
            "type" : "string",
            "mapping" : "Overview.Tags"
        },{
            "name" :"ServerStatus.MaxSeverity",
            "type" :"string",
            "mapping":"ServerStatus.MaxSeverity"
        },{
            "name" :"Services",
            "type" :"string",
            "mapping":"AlertCategoryWiseMaxSeverity.Services"
        },{
            "name" :"Sessions",
            "type" :"string",
            "mapping":"AlertCategoryWiseMaxSeverity.Sessions"
        },{
            "name" :"Operational",
            "type" :"string",
            "mapping":"AlertCategoryWiseMaxSeverity.Operational"
        },{
            "name" :"Virtualization",
            "type" :"string",
            "mapping":"AlertCategoryWiseMaxSeverity.Virtualization"
        },{
            "name" :"Product.InstanceName",
            "type" :"string",
            "mapping":"Product.InstanceName"
        },{
           "name"  :"Overview.maintenanceModeEnabled",
		   "type"  :"boolean",
	       "mapping":"Overview.maintenanceModeEnabled"
	    }
	]
    });

    Ext
        .on('loadInstancesList', function (restUrl, header, isLocalProduct) {
        	if(Ext.getCmp('user3')!==undefined){
        		Ext.getCmp('user3').destroy();
        	}
			var pattern = /http:\/\/(.*):(\d+)\/(.*)/i;
			var match = pattern.exec(restUrl);
			restUrl = "https://" + match[1] + ":5171/" + match[3];
			if(isLocalProduct=="true"){
				restUrl = "https://" + document.location.hostname + ":5171/" + match[3];
			}
            var rowEditing1 = Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToMoveEditor: 1,
                autoCancel: false
            });
            

            var psize = Ext.state.Manager.get('instancesGrid.pageSize');
        	if(psize==undefined){
        		psize=25;
        	}
            var myStore1 = Ext.create('Ext.data.Store', {
                storeId: 'user3',
                remoteSort: false,
                remoteFilter: false,
                model: 'User',
                autoLoad: 'true',
                proxy: {
                    type: 'ajax',
                    url: restUrl,
                    headers: {"Authorization": header},
                    reader: {
                        type: 'json',
                        root: 'ServerSummaryContainerList',
                        totalProperty:'totalResults'
                    },
                    enablePaging: true,
                    timeout:180000,
                },
                autoLoad: {start: 0, limit: psize},
                pageSize: psize,
                listeners:{
        			beforeload: function( store, operation, eOpts ){
        				Ext.state.Manager.set('instancesGrid.pageSize',store.pageSize);
        			} 
        		}
            });

            var bbar = Ext.create('Ext.PagingToolbarWithNumber', {
                store: Ext.data.StoreManager.lookup('user3'),
                displayInfo: false,
                displayMsg: 'Displaying {0} - {1} of {2} records',
                emptyMsg: "No topics to display",
                disablePageSizeInput: false
            });

            var windowHeight = $('body').height();
            var gridHeight = isNaN(windowHeight) ? 550 : windowHeight - 137;
            // This is needed because in zkoss code container height is set to 800px

            var grid_user3 = Ext
                .create(
                    'Ext.grid.Panel',
                    {
                        header: false,
                        store: Ext.data.StoreManager
                            .lookup('user3'),
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            padding: "4px 8px 4px 8px",
                            border: 0,
                            items: [{
                                iconCls: 'idera-grid-download',
                                menu: {
                                    xtype: 'menu',
                                    items: [
                                        {
                                            text: 'Excel',
                                            listeners: {
                                                click: {
                                                    fn: function () {
                                                        grid_user3
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
                                                        grid_user3
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
                                                        grid_user3
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
                                    xtype : 'combobox',
                                    cls : 'idera-ext-grid-search',
                                    height:24,
                                    minChars:0,
                                    //Expand method is overridden which will be called when a text is typed.
                                    expand : function() {
                                        if(this.getRawValue()!=""){
                                            myStore1.clearFilter();
                                            //Used to change the Trigger from Search Icon to Clear icon
                                            this.addCls("idera-ext-grid-search-clear");
                                            var searchValue=this.getRawValue();
                                            var filters = [ new Ext.util.Filter({
                                                filterFn : function(item) {
                                                    return item.get('Overview.InstanceName').toLowerCase()
                                                        .indexOf(searchValue.toLowerCase()) > -1;
                                                }
                                            }) ];
                                            myStore1.filter(filters);
                                        }
                                    },
                                    //Collapse method is overridden which will be called when a text is cleared.
                                    collapse:function(){
                                        //Used to change the Trigger from Clear Icon to Search icon
                                        this.removeCls("idera-ext-grid-search-clear");
                                        myStore1.clearFilter();
                                    },
                                    //When Trigger is When text is there, clear the text and collapse is called to clear the filter
                                    onTriggerClick:function(){
                                        if(this.getRawValue()!=""){
                                            this.reset();
                                            this.collapse();
                                        }
                                    }
                                }]
                        }],
                        id: 'user3',
                        title: 'Ext Grid Config without Grouping',
                        collapsible: true,
                        stateful: true,
                        stateId: 'instancesListGrid',
                        collapsed: false,
                        columns: [{
                        	"text": "SWA",
                            "width": 50,
                            "sortable": false,
                            "hideable": false,
                            "tooltip" : 'SWA',
                            "draggable": false,
                            "locked":true,
						
                        },{
                            "text": "Instance Name",
                            "width": 250,
                            "sortable": true,
                            "hideable": false,
                            "dataIndex": "Overview.InstanceName",
                            "stateId": "Overview.InstanceName",
                            "tooltip" : 'Instance Name',
                            "draggable": false,
                            "locked":true,
							sorter : function(a,b){
								v1=a.data.Overview.maintenanceModeEnabled;
								v2=b.data.Overview.maintenanceModeEnabled;
								if(v1 == v2){
									return 0;
								}
								else if(v1){
									return 1;
								}
								else if(v2){
									return -1;
								}
							},
                            renderer: function (value, meta, record) {
                            	var insName = record.get("Product").InstanceName;
                            	var sqlServerId = record.get("Overview").SQLServerId;
                            	var url = "/sqldm/"+insName+"/singleInstance/"+sqlServerId;
                            	meta.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                            	return "<a href="+url+">" + value + "</a>";
                            }

                        }, {
                            "text": "Health",
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "stateId": "Health",
                            "tooltip" : 'Health',
                            sorter : function(a, b) {
                            	v1=a.data.ServerStatus.HealthIndex;
                            	v2=b.data.ServerStatus.HealthIndex;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("ServerStatus").HealthIndex;
                                valFixed = parseFloat(val).toFixed(2);
                                meta.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(valFixed) + '"';
								if(record.get("Overview").maintenanceModeEnabled){
									meta.innerCls = "fa fa-heartbeat fa-maintenance";	
								}
								else{
									if(val>=85){
										meta.innerCls = "fa fa-heartbeat fa-ok";
									}else if(val>=70){
										meta.innerCls = "fa fa-heartbeat fa-warning-yellow";
									}else{
										meta.innerCls = "fa fa-heartbeat fa-critical";
									}
								}
								
                                // return " " + valFixed + "%";
                                return "<span style='font-family: Source Sans Pro,sans-serif; font-size: 14px; padding-left: 4px'>" + valFixed + "%" +"</span>"
                            }


                        }, {
                            "text": "Alert",
                            "tooltip" : 'Alert',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "stateId": "ServerStatus.MaxSeverity",
                            sorter : function(a, b) {
                            	v1=a.data.ServerStatus.MaxSeverity;
                            	v2=b.data.ServerStatus.MaxSeverity;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("ServerStatus").MaxSeverity;
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }
                        }, {
                            "text": "CPU",
                            "tooltip" : 'CPU',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "",
                            "stateId": "CPU",
                            sorter : function(a, b) {
                            	v1=a.data.AlertCategoryWiseMaxSeverity.Cpu;
                            	v2=b.data.AlertCategoryWiseMaxSeverity.Cpu;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("AlertCategoryWiseMaxSeverity").Cpu;
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }

                        }, {
                            "text": "Memory",
                            "tooltip" : 'Memory',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "",
                            "stateId": "Memory",
                            sorter : function(a, b) {
                            	v1=a.data.AlertCategoryWiseMaxSeverity.Memory;
                            	v2=b.data.AlertCategoryWiseMaxSeverity.Memory;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("AlertCategoryWiseMaxSeverity").Memory;
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }

                        }, {
                            "text": "I/O",
                            "tooltip" : 'I/O',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "",
                            "stateId": "I/O",
                            sorter : function(a, b) {
                            	v1=a.data.AlertCategoryWiseMaxSeverity.IO;
                            	v2=b.data.AlertCategoryWiseMaxSeverity.IO;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("AlertCategoryWiseMaxSeverity").IO;
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }

                        }, {
                            "text": "Databases",
                            "tooltip" : 'Databases',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "",
                            "stateId": "Databases",
                            sorter : function(a, b) {
                            	v1=a.data.AlertCategoryWiseMaxSeverity.Databases;
                            	v2=b.data.AlertCategoryWiseMaxSeverity.Databases;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("AlertCategoryWiseMaxSeverity").Databases;
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }

                        }, {
                            "text": "Logs",
                            "tooltip" : 'Logs',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "",
                            "stateId": "Logs",
                            sorter : function(a, b) {
                            	v1=a.data.AlertCategoryWiseMaxSeverity.Logs;
                            	v2=b.data.AlertCategoryWiseMaxSeverity.Logs;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("AlertCategoryWiseMaxSeverity").Logs;
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }

                        }, {
                            "text": "Queries",
                            "tooltip" : 'Queries',
                            "width": 50,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "",
                            "stateId": "Queries",
                            sorter : function(a, b) {
                            	v1=a.data.AlertCategoryWiseMaxSeverity.Queries;
                            	v2=b.data.AlertCategoryWiseMaxSeverity.Queries;
                                return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
                            },
                            renderer: function (value, meta, record) {
                                val = record.get("AlertCategoryWiseMaxSeverity").Queries
                                if (val == "8") {
                                    meta.innerCls = "fa fa-window-close fa-critical";
                                } else if (val == "4") {
                                    meta.innerCls = "fa fa-warning fa-warning-yellow";
                                } else if (val == "2") {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                } else {
                                    meta.innerCls = "fa fa-check-circle fa-ok";
                                }
                                return value;
                            }

                        }, {
                            "text": "Version",
                            "tooltip" : 'Version',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.ProductVersion",
                            "stateId": "Overview.ProductVersion",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "Status",
                            "tooltip" : 'Status',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.sqlServiceStatus",
                            "stateId": "Overview.sqlServiceStatus",
                            "renderer" : function(value, meta, record) {
				
				if(record.get("Overview").maintenanceModeEnabled){
					val = "Maint. Mode";
				}
				else{
					val = record.get("Overview").sqlServiceStatus;
				}

				return val;
				}

                        }, {
                            "text": "Agent Status",
                            "tooltip" : 'Agent Status',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.agentServiceStatus",
                            "stateId": "Overview.agentServiceStatus",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "DTC Status",
                            "tooltip" : 'DTC Status',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.dtcServiceStatus",
                            "stateId": "Overview.dtcServiceStatus",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "Available Memory",
                            "tooltip" : 'Available Memory',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.osMetricsStatistics.AvailableBytes",
                            "stateId": "Overview.osMetricsStatistics.AvailableBytes",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "Blocked Session",
                            "tooltip" : 'Blocked Session',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.systemProcesses.blockedProcesses",
                            "stateId": "Overview.systemProcesses.blockedProcesses",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "CPU Activity",
                            "tooltip" : 'CPU Activity',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.statistics.cpuPercentage",
                            "stateId": "Overview.statistics.cpuPercentage",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "Disk Queue Length",
                            "tooltip" : 'Disk Queue Length',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Overview.osMetricsStatistics.AvgDiskQueueLength",
                            "stateId": "Overview.osMetricsStatistics.AvgDiskQueueLength",
                            "renderer" : tooltipRenderer,

                        }, {
                            "text": "Services",
                            "tooltip" : 'Services',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Services",
                            "stateId": "Services",
                            "renderer" : tooltipRenderer,
                            "hidden" : true

                        }, {
                            "text": "Sessions",
                            "tooltip" : 'Sessions',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Sessions",
                            "stateId": "Sessions",
                            "renderer" : tooltipRenderer,
                            "hidden" : true

                        }, {
                            "text": "Virtualization",
                            "tooltip" : 'Virtualization',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Virtualization",
                            "stateId": "Virtualization",
                            "renderer" : tooltipRenderer,
                            "hidden" : true

                        }, {
                            "text": "Operational",
                            "tooltip" : 'Operational',
                            "width": 100,
                            "sortable": true,
                            "hideable": true,
                            "dataIndex": "Operational",
                            "stateId": "Operational",
                            "renderer" : tooltipRenderer,
                            "hidden" : true

                        }],
                        forceFit : true,
                        height: gridHeight,
                        renderTo: Ext.get("grid_row_editing_url"),
                        plugins: [{
                            "ptype": "gridfilters"
                        }, rowEditing1, {
                            "ptype": "gridexporter"
                        }],
                        features : [ {
                            ftype : 'grouping'
                        } ],
                        bbar: bbar,
                    });
            myStore1.clearFilter();
            //Used to change the Trigger from Search Icon to Clear icon
            Ext.on("SeverityFilterApplied", function (severity) {
            	myStore1.clearFilter();
            	if(severity!=-1){
	                var filters = [ new Ext.util.Filter({
	                    filterFn : function(item) {
	                        return item.get('ServerStatus.MaxSeverity').toLowerCase()
	                            .indexOf(severity) > -1;
	                    }
	                }) ];
	                myStore1.filter(filters);
            	}
            });

            Ext.on("groupByDropdownChange", function (dropDownSelection) {
                switch (dropDownSelection) {
                    case "Severity":
                        myStore1.group("ServerStatus.MaxSeverity");
                        break;
                    case "Tags":
                        myStore1.group("Overview.Tags");
                        break;
                    case "SQLdmRepo":
                        myStore1.group("Product.InstanceName");
                        break;
                    default:
                        myStore1.group("");
                }
            })

        });
});
