zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.RectangularChart = zk.$extends(com.idera.sqldm.d3zk.chart.AbstractChart, {

	_plotMarginTop: "0",
	_plotMarginRight: "0",
	_plotMarginBottom: "0",
	_plotMarginLeft: "0",
	
	_xAxisTickFormat: null,
	_yAxisTickFormat: null,

	_yAxisInnerTickSize: 0,
	_yAxisOuterTickSize: 0,
	_yAxisTickPadding: 0,
	_yAxisTitle: null,
	_yAxisLabelOrient: "left",
	_yAxisCustomMaxDomainValue: false,
	_yAxisCustomMinDomainValue: false,
	_yAxisMinDomainValue: 0,
	_yAxisMaxDomainValue:100,
	_xAxisCustomMaxDomainValue: false,
	_xAxisCustomMinDomainValue: false,
	_xAxisMinDomainValue: null,
	_xAxisMaxDomainValue:null,
	
	_hasRendered: false,
	
	getYAxisCustomMaxDomainValue: function() { 
		return this._yAxisCustomMaxDomainValue;
	},
	//x axis domain configuration changes
	getXAxisCustomMaxDomainValue: function() { 
		return this._xAxisCustomMaxDomainValue;
	},

	setYAxisCustomMaxDomainValue: function(value) { 
		if( value != this._yAxisCustomMaxDomainValue ) { 
			this._yAxisCustomMaxDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	//x axis domain configuration changes
	setXAxisCustomMaxDomainValue: function(value) { 
		if( value != this._xAxisCustomMaxDomainValue ) { 
			this._xAxisCustomMaxDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	
	getYAxisCustomMinDomainValue: function() { 
		return this._yAxisCustomMinDomainValue;
	},

	//x axis domain configuration changes
	getXAxisCustomMinDomainValue: function() { 
		return this._xAxisCustomMinDomainValue;
	},
	
	setYAxisCustomMinDomainValue: function(value) { 
		if( value != this._yAxisCustomMinDomainValue ) { 
			this._yAxisCustomMinDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	//x axis domain configuration changes
	setXAxisCustomMinDomainValue: function(value) { 
		if( value != this._xAxisCustomMinDomainValue ) { 
			this._xAxisCustomMinDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getYAxisMinDomainValue: function() { 
		return this._yAxisMinDomainValue;
	},

	//x axis domain configuration changes
	getXAxisMinDomainValue: function() { 
		return this._xAxisMinDomainValue;
	},

	setYAxisMinDomainValue: function(value) { 
		if( value != this._yAxisMinDomainValue ) { 
			this._yAxisMinDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	//x axis domain configuration changes
	setXAxisMinDomainValue: function(value) { 
		if( value != this._xAxisMinDomainValue ) { 
			this._xAxisMinDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
			
	getYAxisMaxDomainValue: function() { 
		return this._yAxisMaxDomainValue;
	},

	//x axis domain configuration changes
	getXAxisMaxDomainValue: function() { 
		return this._xAxisMaxDomainValue;
	},

	setYAxisMaxDomainValue: function(value) { 
		if( value != this._yAxisMaxDomainValue ) { 
			this._yAxisMaxDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	//x axis domain configuration changes
	setXAxisMaxDomainValue: function(value) { 
		if( value != this._xAxisMaxDomainValue ) { 
			this._xAxisMaxDomainValue = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getPlotMarginTop: function() { 
		return this._plotMarginTop;
	},

	setPlotMarginTop: function(value) { 
		if( value != this._plotMarginTop ) { 
			this._plotMarginTop = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getPlotMarginRight: function() { 
		return this._plotMarginRight;
	},

	setPlotMarginRight: function(value) {
		if( value != this._plotMarginRight ) {
			this._plotMarginRight = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getPlotMarginBottom: function() { 
		return this._plotMarginBottom;
	},

	setPlotMarginBottom: function(value) {
		if( value != this._plotMarginBottom ) { 
			this._plotMarginBottom = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getPlotMarginLeft: function() { 
		return this._plotMarginLeft;
	},

	setPlotMarginLeft: function(value) { 
		if( value != this._plotMarginLeft ) { 
			this._plotMarginLeft = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getXAxisTickFormat: function() { 
		return this._xAxisTickFormat;
	},
	
	setXAxisTickFormat: function(value) {
		if( value != this._xAxisTickFormat ) { 
			this._xAxisTickFormat = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getYAxisTickFormat: function() { 
		return this._yAxisTickFormat;
	},
	
	setYAxisTickFormat: function(value) {
		if( value != this._yAxisTickFormat ) { 
			this._yAxisTickFormat = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getXAxisTickFormat: function() { 
		return this._xAxisTickFormat;
	},
	
	setXAxisTickFormat: function(value) {
		if( value != this._xAxisTickFormat ) { 
			this._xAxisTickFormat = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getYAxisTickFormat: function() { 
		return this._yAxisTickFormat;
	},
	
	setYAxisTickFormat: function(value) {
		if( value != this._yAxisTickFormat ) { 
			this._yAxisTickFormat = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},	
	
	getYAxisInnerTickSize: function() { 
		return this._yAxisInnerTickSize;
	},
	
	setYAxisInnerTickSize: function(value) {
		if( value != this._yAxisInnerTickSize ) { 
			this._yAxisInnerTickSize = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getYAxisOuterTickSize: function() { 
		return this._yAxisOuterTickSize;
	},
	
	setYAxisOuterTickSize: function(value) {
		if( value != this._yAxisOuterTickSize ) { 
			this._yAxisOuterTickSize = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getYAxisTickPadding: function() { 
		return this._yAxisTickPadding;
	},
	
	setYAxisTickPadding: function(value) {
		if( value != this._yAxisTickPadding ) { 
			this._yAxisTickPadding = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getYAxisTitle: function() { 
		return this._yAxisTitle;
	},
	
	setYAxisTitle: function(value) { 
		if( value != this._yAxisTitle ) { 
			this._yAxisTitle = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getYAxisLabelOrient: function() { 
		return this._yAxisLabelOrient;
	},
	
	setYAxisLabelOrient: function(value) {
		if( value != this._yAxisLabelOrient ) { 
			this._yAxisLabelOrient = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},	
	
	computeMargins: function() { 
	
		var marginLeft = 0, marginRight = 0, marginTop = 0, marginBottom = 0;
		var div = document.getElementById(this.uuid);
		
		if( div != null ) {
		
			var clientWidth = parseInt(div.clientWidth);
			var clientHeight = parseInt(div.clientHeight);
			
			if( this._plotMarginLeft.indexOf("%") != -1 ) { 
				marginLeft = parseInt(clientWidth * (parseInt(this._plotMarginLeft) / 100));
			}
			else { 
				marginLeft = parseInt(this._plotMarginLeft);
			}
			
			if( this._plotMarginRight.indexOf("%") != -1 ) { 
				marginRight = parseInt(clientWidth * (parseInt(this._plotMarginRight) / 100));
			}
			else { 
				marginRight = parseInt(this._plotMarginRight);
			}
			
			if( this._plotMarginTop.indexOf("%") != -1 ) { 
				marginTop = parseInt(clientHeight * (parseInt(this._plotMarginTop) / 100));
			}
			else { 
				marginTop = parseInt(this._plotMarginTop);
			}
			
			if( this._plotMarginBottom.indexOf("%") != -1 ) { 
				marginBottom = parseInt(clientWidth * (parseInt(this._plotMarginBottom) / 100));
			}
			else { 
				marginBottom = parseInt(this._plotMarginBottom);
			}		
		
		}
		
		return { top: marginTop, right: marginRight, bottom: marginBottom, left: marginLeft };
	}	
	

});