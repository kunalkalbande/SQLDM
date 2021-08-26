zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.AbstractChart = zk.$extends(zul.wgt.Div, {

	_colorPalette: "category10",
	_currentClientHeight: -1,
	_currentClientWidth: -1,

	_hasRendered: false,
	
	getColorPalette: function() { 
		return this._colorPalette;
	},
	
	setColorPalette: function(value) { 
		if( value != this._colorPalette ) { 
			this._colorPalette = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	bind_ : function(evt) {
		
		this.$supers('bind_', arguments);
		
		zWatch.listen({
			onSize: this
		});
		
		this.rerenderChart();
	},
		
	onSize: function() { 

		this.$supers('onSize', arguments);
		
		var div = document.getElementById(this.uuid);
		if( div == null ) return;
		
		if( this._currentClientHeight != div.clientHeight || this._currentClientWidth != div.clientWidth ) { 
			this.rerenderChart();
			this._currentClientHeight = div.clientHeight;
			this._currentClientWidth = div.clientWidth;
		}
		
	},
	
	_getColorPalette: function() { 
		switch(this._colorPalette) { 
			case "category10":
				return d3.scale.category10();
			case "category20":
				return d3.scale.category20();
			case "category20b":
				return d3.scale.category20b();
			case "category20c":
				return d3.scale.category20c();
			default:
				
				var regexp = /colorbrewer\.([A-Za-z0-9]+)\[([0-9]+)\]/;
				var groups = regexp.exec(this._colorPalette);
				
				if( groups != null && groups.length == 3 && colorbrewer[groups[1]] != null && colorbrewer[groups[1]][groups[2]] != null ) {
					return d3.scale.ordinal().range(colorbrewer[groups[1]][groups[2]]);
				}

				return null;
		}
	},
	
	_createColorPalette: function() {
		this._color = this._getColorPalette();
		this._color.domain(d3.map(this._model).keys());
	}

});