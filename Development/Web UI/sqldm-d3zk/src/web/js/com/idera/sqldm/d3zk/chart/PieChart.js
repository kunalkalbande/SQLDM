zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.PieChart = zk.$extends(com.idera.sqldm.d3zk.chart.AbstractChart, {
	
	_hasRendered: false,
	_showSliceLabels: false,
	_sortDirection: "asc",
	
	_model: {},
	
	_animateSlices: true,
	_animationDelay: 50,
	_animationLength: 500,
	
	_innerRadiusPercentage: 0,
	_outerRadiusPercentage: 85,
	
	_valueFormat: "",
	_labelThreshold: 0.05,
	_mouseoverEnabled: true,
	
	_colorCodes : null,

	getModel: function() { 
		return this._model;
	},
	
	setModel: function(value) { 
		this._model = value;
		if( this._hasRendered ) this.rerenderChart();
	},
	
	getShowSliceLabels: function() { 
		return this._showSliceLabels;
	},
	
	setShowSliceLabels: function(value) { 
		if( this._showSliceLabels != value ) { 
			this._showSliceLabels = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getAnimateSlices: function() { 
		return this._animateSlices;
	},
	
	setAnimateSlices: function(value) { 
		if( value != this._animateSlices ) {
			this._animateSlices = value;
		}
	},
	
	getAnimationDelay: function() {
		return this._animationDelay;
	},
	
	setAnimationDelay: function(value) {
		if( value != this._animationDelay ) {
			this._animationDelay = value;
		}
	},
	
	getAnimationLength: function() {
		return this._animationLength;
	},
	
	setAnimationLength: function(value) {
		if( value != this._animationLength ) {
			this._animationLength = value;
		}
	},	
	
	getInnerRadiusPercentage: function() { 
		return this._innerRadiusPercentage;
	},
	
	setInnerRadiusPercentage: function(value) {
		if( value != this._innerRadiusPercentage ) {
			this._innerRadiusPercentage = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getOuterRadiusPercentage: function() { 
		return this._outerRadiusPercentage;
	},
	
	setOuterRadiusPercentage: function(value) {
		if( value != this._outerRadiusPercentage ) {
			this._outerRadiusPercentage = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getValueFormat: function() { 
		return this._valueFormat;
	},
	
	setValueFormat: function(value) { 
		if( value != this._valueFormat ) { 
			this._valueFormat = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getSortDirection: function() { 
		return this._sortDirection;
	},
	
	setSortDirection: function(value) {
		if( value != this._sortDirection ) {
			this._sortDirection = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getLabelThreshold: function() { 
		return this._labelThreshold;
	},
	
	setLabelThreshold: function(value) { 
		if( value != this._labelThreshold ) { 
			this._labelThreshold = value;
			if( this._hasRendered ) this.rerenderChart();			
		}
	},
	
	getMouseoverEnabled: function() { 
		return this._mouseoverEnabled;
	},
	
	setMouseoverEnabled: function(value) { 
		if( value != this._mouseoverEnabled ) { 
			this._mouseoverEnabled = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getColorCodes : function() {
		return this._colorCodes;
	},

	setColorCodes : function(value) {
		this._colorCodes = value;
	},

	getZclass: function(){
		var zcls = this._zclass;
		return zcls ? zcls : "z-d3-piechart";
	},
	
	rerenderChart: function() {
		
		var div = document.getElementById(this.uuid);
		if( div == null ) return;
		
		var uuid = this.uuid;
		var svg = d3.select("#" + this.uuid).select("svg");
		if( svg ) svg.remove();
		
		var clientWidth = parseInt(div.clientWidth);
		var clientHeight = parseInt(div.clientHeight);
		
		var width = clientWidth;
		var height = clientHeight;

		var min = Math.min(width, height);
		var outerRadius = (min / 2) * (this._outerRadiusPercentage / 100);
		var innerRadius = outerRadius * (this._innerRadiusPercentage / 100);
		
		var map = d3.map(this._model);
		var entries = map.entries();
		
		var arc = d3.svg.arc()
			.startAngle(function(d){ return d.startAngle; })
			.endAngle(function(d){ return d.endAngle; })		
			.outerRadius(outerRadius)
			.innerRadius(innerRadius);

		var sortAsc = function(a,b) { return a.value < b.value ? -1 : a.value > b.value ? 1 : 0; };
		var sortDsc = function(a,b) { return b.value < a.value ? -1 : b.value > a.value ? 1 : 0; };

		var array_keys = new Array();
		var array_values = new Array();
		var color;
		
		if(this._colorCodes) {
		
			for (var key in this._colorCodes) {
			    array_keys.push(key);
			    array_values.push(this._colorCodes[key]);
			}
			
			color = d3.scale.ordinal()
			 .domain(array_keys)
		 .range(array_values);
		
		} else {
			color = this._getColorPalette();
		}
		
		var pie = d3.layout.pie()
			.sort( this._sortDirection == "asc" ? sortAsc : sortDsc )
			.value(function(d){ return d.value; });

		var svg = d3.select("#" + this.uuid).append("svg")
			.attr("width", clientWidth)
			.attr("height", clientHeight)
			.append("g")
			.attr("transform", "translate(" + (width / 2) + "," + (height / 2) + ")")
			.attr("width", clientWidth)
			.attr("height", clientHeight);

		var g = svg.append("g").attr("class", "arc");

		var paths = g.selectAll("path").data(pie(entries));		
		
		if( this._animateSlices ) { 

			var pieTween = function(d, i) { 
				var i = d3.interpolate({startAngle: 0, endAngle: 0}, {startAngle: d.startAngle, endAngle: d.endAngle});
				return function(t) {
					var b = i(t);
					return arc(b);
				};
			};
			
			paths.enter().append("svg:path")
				.attr("d", arc)
				.attr("fill", function(d, i) { return color(d.data.key); })
				.transition()
				.duration(this._animationLength)
				.delay(this._animationDelay)
				.attrTween("d", pieTween);
			
		}
		else { 
			paths.enter().append("svg:path")
				.attr("d", arc)
				.attr("fill", function(d, i) { return color(d.data.key); });
		}
				
		if( this._showSliceLabels ) { 
			
			var textOffset = 24;
			var labelThreshold = this._labelThreshold; 
			
			var textTween = function(d, i) { 
				
				var a = 0;
				var b = (d.startAngle + d.endAngle - Math.PI)/2;

				var fn = d3.interpolateNumber(a, b);
				return function(t) {
					var val = fn(t);
					return "translate(" + Math.cos(val) * (outerRadius+textOffset) + "," + Math.sin(val) * (outerRadius+textOffset) + ")";
				};				
				
			};
			
			var lines = g.selectAll("line").data(pie(entries));
			
			lines.enter().append("svg:line")
				.attr("x1", 0)
				.attr("x2", 0)
				.attr("y1", -outerRadius-3)
				.attr("y2", -outerRadius-15)
				.attr("class", function(d) { 
					var clazz = "label-tick"; 
					var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
					if( percent < labelThreshold ) clazz += " hidden";
					return clazz;
				})
				.attr("index", function(d,i) { return i; })
				.attr("transform", function(d) {
					return "rotate(" + (d.startAngle+d.endAngle)/2 * (180/Math.PI) + ")";
				});

			lines.transition()
				.duration(this._animationLength)
				.delay(this._animationDelay)
				.attr("transform", function(d) {
					return "rotate(" + (d.startAngle+d.endAngle)/2 * (180/Math.PI) + ")";
				});

			lines.exit().remove();

			var valueFormat = d3.format(this._valueFormat);
			
			var valueLabels = g.selectAll("text.value").data(pie(entries));

			valueLabels.enter().append("svg:text")
				.attr("class", function(d) { 
					var clazz = "value"; 
					var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
					if( percent < labelThreshold ) clazz += " hidden";
					return clazz;
				})
				.attr("transform", function(d) {
					return "translate(" + Math.cos(((d.startAngle+d.endAngle - Math.PI)/2)) * (outerRadius+textOffset) + "," + Math.sin((d.startAngle+d.endAngle - Math.PI)/2) * (outerRadius+textOffset) + ")";
				})
				.attr("dy", function(d){
					var isPastHalf = (d.startAngle+d.endAngle)/2 > Math.PI/2 && (d.startAngle+d.endAngle)/2 < Math.PI*1.5;
					return isPastHalf ? 5 : -7;
				})
				.attr("text-anchor", function(d){
					return (d.startAngle+d.endAngle)/2 < Math.PI ? "beginning" : "end";
				})
				.attr("index", function(d,i) { return i; })
				.text(function(d){
					return valueFormat(d.value);
				});

			valueLabels.transition()
				.duration(this._animationLength)
				.delay(this._animationDelay)
				.attrTween("transform", textTween);

			valueLabels.exit().remove();
			
			var nameLabels = g.selectAll("text.units").data(pie(entries));

			nameLabels.enter().append("svg:text")
				.attr("class", function(d) { 
					var clazz = "units"; 
					var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
					if( percent < labelThreshold ) clazz += " hidden";
					return clazz;
				})
				.attr("transform", function(d) {
					return "translate(" + Math.cos(((d.startAngle+d.endAngle - Math.PI)/2)) * (outerRadius+textOffset) + "," + Math.sin((d.startAngle+d.endAngle - Math.PI)/2) * (outerRadius+textOffset) + ")";
				})
				.attr("dy", function(d){
					var isPastHalf = (d.startAngle+d.endAngle)/2 > Math.PI/2 && (d.startAngle+d.endAngle)/2 < Math.PI*1.5;
					return isPastHalf ? 17 : 5;
				})
				.attr("text-anchor", function(d){
					return (d.startAngle+d.endAngle)/2 < Math.PI ? "beginning" : "end";
				})
				.attr("index", function(d,i) { return i; })
				.text(function(d){
					return d.data.key;
				});

			g.selectAll("text.units").each(function(d, i) {

				var select = d3.select(this);
				if( select.attr("class").indexOf("hidden") != -1 ) return;
				
				var node = select.node();
				if( node == null ) return;
				
				var bbox = node.getBBox();
				var xShift = Math.abs(Math.cos(((d.startAngle+d.endAngle - Math.PI)/2)) * (outerRadius+textOffset));
				
				while( (bbox.width + xShift) > width/2 ) {
					
					var text = d3.select(this).node(); 
					text.textContent = text.textContent.substring(0, text.textContent.length-1) + "...";
					if( text.textContent.length <= 4 ) break;
					
					bbox = d3.select(this).node().getBBox();
					if( (bbox.width + xShift) > width/2 ) text.textContent = text.textContent.substring(0, text.textContent.length-3);
				}
				
			});			
			
			nameLabels.transition()
				.duration(this._animationLength)
				.delay(this._animationDelay)
				.attrTween("transform", textTween);

			nameLabels.exit().remove();			
		
			if( this._showSliceLabels && this._mouseoverEnabled ) { 
	
				paths.on('mouseover', function(d,i) {
					
					d3.select("#" + uuid).selectAll("text.units").each(function(d) {
						
						var label = d3.select(this);
						
						if( label.attr("index") != i ) return;

						label.attr("class", "units highlighted");
						
						var node = label.node();
						if( node == null ) return;
						
						var bbox = node.getBBox();
						var xShift = Math.abs(Math.cos(((d.startAngle+d.endAngle - Math.PI)/2)) * (outerRadius+textOffset));
						
						while( (bbox.width + xShift) > width/2 ) {
							
							var text = d3.select(this).node(); 
							text.textContent = text.textContent.substring(0, text.textContent.length-1) + "...";
							if( text.textContent.length <= 4 ) break;
							
							bbox = d3.select(this).node().getBBox();
							if( (bbox.width + xShift) > width/2 ) text.textContent = text.textContent.substring(0, text.textContent.length-3);
						}						
						
					});

					d3.select("#" + uuid).selectAll("line.label-tick").each(function(d) {
						var label = d3.select(this);
						if( label.attr("index") == i ) label.attr("class", "label-tick highlighted");
					});
					
					d3.select("#" + uuid).selectAll("text.value").each(function(d) {
						var label = d3.select(this);
						if( label.attr("index") == i ) label.attr("class", "value highlighted");
					});
					
				});
				
				paths.on('mouseout', function(d,i) {
					
					d3.select("#" + uuid).selectAll("text.units").each(function(d) {

						var clazz = "units";
						var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
						if( percent < labelThreshold ) clazz += " hidden";
						
						var label = d3.select(this);
						if( label.attr("index") == i ) label.attr("class", clazz);

					});
					
					d3.select("#" + uuid).selectAll("line.label-tick").each(function(d) {
						
						var clazz = "label-tick";
						var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
						if( percent < labelThreshold ) clazz += " hidden";
						
						var label = d3.select(this);
						if( label.attr("index") == i ) label.attr("class", clazz);

					});
					
					d3.select("#" + uuid).selectAll("text.value").each(function(d) {
						
						var clazz = "value";
						var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
						if( percent < labelThreshold ) clazz += " hidden";
						
						var label = d3.select(this);
						if( label.attr("index") == i ) label.attr("class", clazz);

					});  			

				});
	
			}
			
		}
		
	}
});