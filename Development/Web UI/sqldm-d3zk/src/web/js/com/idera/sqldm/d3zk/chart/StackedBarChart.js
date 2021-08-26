zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.StackedBarChart = zk.$extends(com.idera.sqldm.d3zk.chart.RectangularChart, {

	_orient: "vertical",
	
	_innerPadding: 0,
	_outerPadding: 0,
	_series: { },
	
	_hasRendered: false,
	
	_animateBars: true,
	_animationDelay: 50,
	_animationLength: 500,
	
	_currentClientWidth: 0,
	_currentClientHeight: 0,
	
	_showValueLabels: true,
	_valueFormat: "",
	_xAxisTitle: null,
	_xAxisTicksCount: 0,
	_mouseOverText: null,
	_legendLinks:null,
	_axisLinks:null,
	_barLinks: null,
	_yAxisTickPadding: 0,
	_yAxis:null,
	_y:null,
	_drillable:false,
	
	getDrillable:function(){
		return this._drillable;
	},

	setDrillable:function(value){
    	if(value != this.drillable){
    		this._drillable = value;
    		if(this._hasRendered) this.rerenderChart();
    	}
    },

	getXAxisTickPadding: function() {
    		return this._xAxisTickPadding;
    },

    setXAxisTickPadding: function(value) {
        if( value != this._xAxisTickPadding ) {
            this._xAxisTickPadding = value;
            if( this._hasRendered ) this.rerenderChart();
        }
    },
	
	getAxisLinks: function() { 
		return this._axisLinks;
	},
	
	setAxisLinks: function(value) { 
		this._axisLinks = value;
		if( this._hasRendered ) this.rerenderChart();
	},
	
	
	getLegendLinks: function() { 
		return this._legendLinks;
	},
	
	setLegendLinks: function(value) { 
		this._legendLinks = value;
		if( this._hasRendered ) this.rerenderChart();
	},
	
	
	getBarLinks: function() { 
		return this._barLinks;
	},
	
	setBarLinks: function(value) { 
		this._barLinks = value;
		if( this._hasRendered ) this.rerenderChart();
	},
	
	
	getMouseOverText: function() { 
		return this._mouseOverText;
	},
	
	setMouseOverText: function(value) { 
		if( value != this._mouseOverText ) { 
			this._mouseOverText = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getXAxisTitle: function() { 
		return this._xAxisTitle;
	},
	
	setXAxisTitle: function(value) { 
		if( value != this._xAxisTitle ) { 
			this._xAxisTitle = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getXAxisTicksCount: function() { 
		return this._xAxisTicksCount;
	},
	
	setXAxisTicksCount: function(value) { 
		if( value != this._xAxisTicksCount ) { 
			this._xAxisTicksCount = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getOrient: function() { 
		return this._orient;
	},
	
	setOrient: function(value) { 
		if( value != this._orient ) { 
			this._orient = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getHasRendered: function() { 
		return this._hasRendered;
	},
	
	setHasRendered: function(value) { 
		this._hasRendered = value;
	},
	
	getInnerPadding: function() { 
		return this._innerPadding;
	},

	setInnerPadding: function(value) { 
		if( value != this._innerPadding ) { 
			this._innerPadding = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},
	
	getOuterPadding: function() { 
		return this._outerPadding;
	},
	
	setOuterPadding: function(value) {
		if( value != this._outerPadding ) { 
			this._outerPadding = value;
			if( this._hasRendered ) this.rerenderChart();
		}
	},

	getSeries: function() { 
		return this._series;
	},
	
	setSeries: function(value) { 
		this._series = value;
		if( this._hasRendered ) this.rerenderChart();
	},
	

	
	getAnimateBars: function() { 
		return this._animateBars;
	},
	
	setAnimateBars: function(value) { 
		if( value != this._animateBars ) {
			this._animateBars = value;
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
	
	getShowValueLabels: function() {
		return this._showValueLabels;
	},
	
	setShowValueLabels: function(value) {
		if( value != this._showValueLabels ) { 
			this._showValueLabels = value;
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

	getZclass: function(){
		var zcls = this._zclass;
		return zcls ? zcls : "z-d3-barchart";
	},

	_createYAxis:function(){
	    this._yAxis = d3.svg.axis().scale(this._y).orient("left");

        if( this._yAxisTickFormat != null ) {
            this._yAxis.tickFormat(d3.format(this._yAxisTickFormat));
        }

        this._yAxis.innerTickSize(this._yAxisInnerTickSize);
        this._yAxis.outerTickSize(this._yAxisOuterTickSize);
        this._yAxis.tickPadding(this._yAxisTickPadding);

	},
	
	rerenderChart: function() {
		
		var local = this;
		
		var div = document.getElementById(this.uuid);
		if( div == null ) return;
		
		var svg = d3.select("#" + this.uuid).select("svg");
		if( svg ) svg.remove();
		
		var clientWidth = parseInt(div.clientWidth);
		var clientHeight = parseInt(div.clientHeight);
		
		var margin = this.computeMargins();		
		
		var width = parseInt(clientWidth - margin.left - margin.right);
		var height = parseInt(clientHeight - margin.bottom - margin.top);

		var svg = d3.select("#" + this.uuid).append("svg")
			.attr("width", clientWidth)
			.attr("height", clientHeight)
			.append("g")
			.attr("transform", "translate(" + margin.left + "," + margin.top + ")")
			.attr("width", clientWidth)
			.attr("height", clientHeight);

		var x,y; 
		
		if( this._orient == "horizontal" ) {
			x = d3.scale.linear().range([0, width]);
			this._y = d3.scale.ordinal().rangeRoundBands([0, height], this._innerPadding, this._outerPadding);
		}
		else { 
			x = d3.scale.ordinal().rangeRoundBands([0, width], this._innerPadding, this._outerPadding);
			this._y = d3.scale.linear().range([height, 0]);
		}

		var xAxis = d3.svg.axis().scale(x).orient("bottom");
		
		if( this._xAxisTickFormat != null ) { 
			xAxis.tickFormat(d3.format(this._xAxisTickFormat));
		}
        xAxis.tickPadding(this._xAxisTickPadding);

		var color = this._getColorPalette();

        var data = d3.map(this._series).get("data");

        color.domain(d3.keys(data[0]).filter(function(key) { return key !== "series" && key != "categories" && key!="total"; }));

        data.forEach(function(d) {
                        var y0 = 0;
                        d.categories = color.domain().map(function(name) { return {name: name, y0: y0, y1: y0 += +d[name], value:d[name]}; });
                        d.total = d.categories[d.categories.length - 1].y1;
                    });
        //data.sort(function(a, b) { return b.total - a.total; });
        
        var state=null;
        var xAxisOverlap = false;
        var startingBarLength = 0;
        var easingFunction = "quad";
        if( this._orient == "vertical" ) {
            x.domain(data.map(function(d) { return d.series; }));

              var xAxisDetail = svg.append("g")
                  .attr("class", "x axis")
                  .attr("transform", "translate(0," + height + ")")
                  .call(xAxis);

              //Check if x-axis labels overlap
              //Calculate length of x axis label tick
                var totalTickLength=0,labelHeight=0,labelWidth=0;

                xAxisDetail.selectAll(".tick text").each(function(d, i) {
                    var bbox = d3.select(this).node().getBBox();
                    totalTickLength = totalTickLength+bbox.width;
                    if(labelHeight < bbox.height){
                      labelHeight=bbox.height;
                    }

                    if(labelWidth < bbox.width){
                      labelWidth=bbox.width;
                    }
                });
                
                var numOfTicks = xAxisDetail.selectAll("text").size();
                var xValue = Math.ceil(numOfTicks / this._xAxisTicksCount);
                if(this._xAxisTicksCount != 0 && this._xAxisTicksCount < numOfTicks) {
	                xAxisDetail.selectAll("text")
	                .style("visibility", function(d, i) {
	                	if(i % xValue != 0)
	                		return "hidden";
	                	return "visible";
	                });
                }

                if(totalTickLength >0 && totalTickLength > width){
                  xAxisOverlap=true;
                  //rotate by some degree
                  xAxisDetail.selectAll("text")
                    .style("text-anchor", "end")
                    .attr("dx", "-.8em")
                    .attr("dy", ".15em")
                    .attr("transform", function(d) {
                        return "rotate(-65)";
                        });
                    //Check if labels will overlap

                    if(labelWidth > margin.bottom){
                        this._plotMarginBottom = parseInt(labelWidth + 10) + "px";
                        this.rerenderChart();
                    }
                }

              this._y.domain([0, d3.max(data, function(d) { return d.total; })]);
              this._createYAxis();

              y=this._y;

              svg.append("g")
                  .attr("class", "y axis")
                  .call(this._yAxis)
                .append("text")
                  .attr("transform", "rotate(-90)")
                  .attr("y", 0-margin.left)
                  .attr("x", 0-(height/2))
                  .attr("dy", ".71em")
                  .style("text-anchor", "middle")
                  .text(this._yAxisTitle);

              state = svg.selectAll(".series")
                  .data(data)
                .enter().append("g")
                  .attr("class", "g")
                  .attr("transform", function(d) { return "translate(" + x(d.series) + ",0)"; });

              var rect;
              if( this._animateBars ) {
                  rect=state.selectAll("rect")
                                    .data(function(d) { return d.categories; }).enter().append("rect")
                                    .attr("class", "bar")
                                    .attr("width", x.rangeBand())
                                    .attr("y", height)
                                    .attr("height", startingBarLength)
                                    .style("fill", function(d) { return color(d.name); });

                  rect.transition()
                        .ease(easingFunction)
                        .duration(this._animationLength)
                        .delay(this._animationDelay)
                        .attr("height", function(d) { return y(d.y0) - y(d.y1); })
                        .attr("y", function(d) { return y(d.y1); });

              }else{
                  rect = state.selectAll("rect")
                      .data(function(d) { return d.categories; })
                      .enter().append("rect") 
                      .attr("width", x.rangeBand())
                      .attr("y", function(d) { return y(d.y1); })
                      .attr("height", function(d) { return y(d.y0) - y(d.y1); })
                      .style("fill", function(d) { return color(d.name); });
              }
              
              this._createClickEventOnAxis(xAxisDetail,this._axisLinks);
        }else{

             this._y.domain(data.map(function(d) { return d.series; }));
             y=this._y;
             this._createYAxis();
             x.domain([0, d3.max(data, function(d) { return d.total; })]);

              svg.append("g")
                  .attr("class", "x axis")
                  .attr("transform", "translate(0," + height + ")")
                  .call(xAxis);

              var yAxisDetail=svg.append("g")
                  .attr("class", "y axis")
                  .call(this._yAxis);
              
              
              var yAxisTooltipdiv = d3.select("body")
  	  				.append("div")  // declare the tooltip div 
  	  				.style("position", "absolute")
  	  				.style("text-align", "center")
  	  				.style("padding", "2px")
  	  				.style("background", "white")
  	  				.style("border-radius", "2px")
  	  				.style("pointer-events", "none")
  	  				.style("z-index", "10")
  	  				.style("font", "10px Montserrat")
  	  				.style("opacity", 0);
              
              yAxisDetail.selectAll(".tick text").on("mouseover", function(d,i,j){
            	  yAxisTooltipdiv.transition()        
                  .duration(200)      
                  .style("opacity", .9).style("visibility", "visible");      
            	  yAxisTooltipdiv.html(d + "<br/>" )  
                  .style("left", (d3.event.pageX) + "px")     
                  .style("top", (d3.event.pageY - 28) + "px"); 
              }).on("mouseout",function(){
            	  yAxisTooltipdiv.transition()        
                 .duration(500)      
                 .style("opacity", 0).style("visibility", "hidden");							
  				});
              
              yAxisDetail.selectAll(".tick text").each(function(d, i) {
                  var textLength = this.getComputedTextLength();
				  var proposedLabel = d;
				  var proposedLabelArray = proposedLabel.split('');
				  var leftPadding = 12;
				  var allowedLength = margin.left - 2 * leftPadding;
				  
					while(textLength > allowedLength && proposedLabelArray.length){
						proposedLabelArray.pop(); proposedLabelArray.pop(); proposedLabelArray.pop();
						if (proposedLabelArray.length===0) {
							proposedLabel = "";
						} else { 
							proposedLabel = proposedLabelArray.join('') + "...";
						}
						
						d3.select(this).text(proposedLabel);
						textLength = this.getComputedTextLength();
					}
              });
              
              if(this._yAxisTitle){
                svg.append("text")
                  .attr("transform", "rotate(-90)")
                  .attr("y", 0-margin.left)
                  .attr("x", 0-(height/2))
                  .attr("dy", ".71em")
                  .style("text-anchor", "middle")
                  .text(this._yAxisTitle);
                  }

              state = svg.selectAll(".series")
                  .data(data)
                .enter().append("g")
                  .attr("class", "g")
                  .attr("transform", function(d) { return "translate(0," + y(d.series) + ")"; });

              var rect;
              if( this._animateBars ) {
                    rect=state.selectAll("rect")
                                      .data(function(d) { return d.categories; }).enter().append("rect")
                                      .attr("class", "bar")
                                      .attr("height", this._y.rangeBand())
                                      .attr("x", height)
                                      .attr("width", startingBarLength)
                                      .style("fill", function(d) { return color(d.name); });

                    rect.transition()
                          .ease(easingFunction)
                          .duration(this._animationLength)
                          .delay(this._animationDelay)
                          .attr("width", function(d) { return x(d.y1) - x(d.y0); })
                          .attr("x", function(d) { return x(d.y0); });

                }else{
                    rect = state.selectAll("rect")
                      .data(function(d) { return d.categories; })
                       .enter().append("rect")
                      .attr("height", this._y.rangeBand())
                      .attr("x", function(d) { return x(d.y0); })
                      .attr("width", function(d) { return x(d.y1) - x(d.y0); })
                      .style("fill", function(d) { return color(d.name); });
                }

              this._createClickEventOnAxis(yAxisDetail,this._axisLinks);
              
        }

        if(this._xAxisTitle){
        	var xPos=width / 2;
        	if(xAxisOverlap){
        		xPos=width;
        	}
        	svg.append("text")      // text label for the x axis
            .attr("x", xPos )
            .attr("y", height + margin.bottom)
            .style("text-anchor", "middle")
            .text(this._xAxisTitle);
        }
        
        if(this._mouseOverText && this._mouseOverText != ""){
      	  
      	  var div = d3.select("body")
    	  		.append("div")  // declare the tooltip div 
    	  		.style("position", "absolute")
    	  		.style("text-align", "center")
    	  		.style("width", "60px")
    	  		.style("padding", "2px")
    	  		.style("background", "white")
    	  		.style("border", "1px")
    	  		.style("border-style", "solid")
    	  		.style("border-color", "black")
    	  		.style("border-radius", "8px")
    	  		.style("pointer-events", "none")
    	  		.style("z-index", "10")
    	  		.style("font", "10px Montserrat")
    	  		.style("opacity", 0); 
      	  
            //Add mouse over
            state.selectAll("rect")
    			.on("mouseover", function(d,i,j){
    				
    				div.transition().duration(200)      
    					.style("opacity", .9).style("visibility", "visible");      
    				div.html(local._mouseOverText.replace("%s",(d.value)) + "<br/>" )  
    					.style("left", (d3.event.pageX) + "px")     
    					.style("top", (d3.event.pageY - 28) + "px");
    				
    				if(local._barLinks != null) {
    					d3.select(this).style("cursor","pointer");
    					d3.select(this).style("cursor","hand");
    				}
              })     
    			.on("mouseout",function(){
    				 div.transition().duration(500)      
    				 	.style("opacity", 0).style("visibility", "hidden");
     				if(local._barLinks != null) {
     	   				 d3.select(this).style("cursor","default");
    				}

    			});
        }

        
          var legend = svg.selectAll(".legend")
              .data(color.domain().slice().reverse())
            .enter().append("g")
              .attr("class", "legend")
              .attr("transform", function(d, i) { return "translate(0," + ((i * 20)-margin.top) + ")"; });

          var xPosition  = width + margin.right;
          var legendTextSpace=24;
          var allowedLegendWidth = margin.right - legendTextSpace;
          legend.append("rect")
              .attr("x", xPosition -18)
              .attr("width", 18)
              .attr("height", 18)
              .style("fill", color);
          
          legend.append("text")
              .attr("x", xPosition - legendTextSpace)
              .attr("y", 9)
              .attr("dy", ".35em")
              .style("text-anchor", "end")
              .text(function(d) { return d; })
              .filter(function(d) {
            	  try {
            		  return allowedLegendWidth < this.getComputedTextLength();
            	  } catch(e) {
            		  //Handling exception thrown in IE
            		  return allowedLegendWidth < this.getBBox();
            	  }
              })
              .each(function(d) { // ridiculous routine where we test to see if label is short enough to fit
                  var proposedLabel = d;
                  var proposedLabelArray = proposedLabel.split('');
                  var computeLength=this.getComputedTextLength();
                  while (computeLength > allowedLegendWidth && proposedLabelArray.length) {
                      // pull out 3 chars at a time to speed things up (one at a time is too slow)
                      proposedLabelArray.pop(); proposedLabelArray.pop(); proposedLabelArray.pop();
                      if (proposedLabelArray.length===0) {
                          proposedLabel = "";
                      } else {
                          proposedLabel = proposedLabelArray.join('') + "..."; // manually truncate with ellipsis
                      }
                      d3.select(this).text(proposedLabel);
                      computeLength = this.getComputedTextLength();
                  }
              });
          
          if(this._legendLinks != null){
        	  legend.selectAll("text").each(function(d, i) {
        		 if(local._legendLinks[d] != null){
        			 d3.select(this).style("text-decoration","underline");
        			 d3.select(this).style("fill","blue");
        			 d3.select(this).on("mouseover", function(d,i,j){
        				 d3.select(this).style("cursor","pointer");
        				 d3.select(this).style("cursor","hand");
        			 });
        			 d3.select(this).on("mouseout", function(d,i,j){
        				 d3.select(this).style("cursor","default");
        			 });
        			 d3.select(this).on("click", function(d){
                      	  var link = local._legendLinks[d];
                      	  eval(link);
                     });
        		 } 
        	  });
          }
          
          if(this._barLinks != null) {
        	  state.selectAll("rect")
/*        	  .on("mouseover", function(d,i,j){
  				 d3.select(this).style("cursor","pointer");
  				 d3.select(this).style("cursor","hand");
        	  })     
        	  .on("mouseout",function(){
   				 d3.select(this).style("cursor","default");
        	  })*/
        	  .on("dblclick", function() {
  				eval(local._barLinks);
  			});
          }
          if(this._drillable){
      		/*var div = this._div;
      		var x = this._x;
      		var y = this._y;
      		var chartHeight = this._chartHeight;
      		var chartWidth = this._chartWidth;*/
      		var x1,x2;
      		/*var clientHeight = this.__clientHeight;
      		var clientWidth = this._clientWidth;
      		var xAxis = this._xAxis;
      		var yAxis = this._yAxis;*/
      		var origin,destination;
      		//svg.on("mousedown",zoomselected);
      		var brush = d3.svg.brush()
      					    .x(x)
      					    .on("brush", brushmove)
      					    .on("brushend", brushend);
      		
      		var brushSVG = svg.append("g")
      			.attr("id","drillSelectedArea")
      			.attr("class", "brush")
      			.style("stroke","#fff")
      			.style("fill-opacity",".125")
      			.style("shape-rendering","crispEdges")
      			.call(brush)
      			.selectAll('rect')
      			.attr('height', height);
      			
      		
      		svg.on('mousedown', function(){
      		      var e= this;
      			  origin =  d3.mouse(e);
      			  brush_elm = svg.select(".brush").node();
      			  new_click_event = new Event('mousedown');
      			  new_click_event.pageX = d3.event.pageX;
      			  new_click_event.clientX = d3.event.clientX;
      			  new_click_event.pageY = d3.event.pageY;
      			  new_click_event.clientY = d3.event.clientY;
      			  brush_elm.dispatchEvent(new_click_event);
      			});
      		function brushmove() {
      			  var extent = brush.extent();
      			  x1=extent[0];
      			  console.log("x1 : "+x1);
      			  x2=extent[1];
      			  console.log("x2 : "+x2);
      		}
      		function brushend() {
      				
      				//d3.select(".brush").call(brush.clear());
      			    var windowWidget = zk.Widget.$('$historyPanel');
      			    var dataLength = data.length;
      			    var totalDataPoints = width/dataLength;
      			    x1=x1-15;
      			    x2=x2-15;
      			    var x1Flag = true;
      			    var x2Flag = true;
      			    if(x1<0){
      			    	startDate = data[0].series;
      			    	x1Flag = false;
      			    }
      			    if(x2<0){
      			    	endDate = data[0].series;
      			    	x2Flag = true;
      			    }
      			    if(x1Flag){
      			    	startDate = data[Math.floor(x1/totalDataPoints)].series;
      			    }
      				if(x2Flag){
      					endDate = data[Math.floor(x2/totalDataPoints)].series;
			    	}1
      			    var startAndEnd = startDate + "&" + endDate;
      			    //alert(startAndEnd);
      			    zAu.send(new zk.Event(windowWidget,'onDrillChange', startAndEnd));
      			    brushSVG.remove();
      		}
          
      	
    		}
          
          this._hasRendered = true;
          
	},
	
	_createClickEventOnAxis: function(axis,links){
		//Make axis label clickable
        if(this._axisLinks != null){
        	axis.selectAll(".tick text").each(function(d, i) {
      		 if(links[d] != null){
      			 d3.select(this).style("text-decoration","underline");
      			 d3.select(this).style("fill","blue");
      			 d3.select(this).on("click", function(d){
                 	  var link = links[d];
                 	  eval(link);
                   });
      			 d3.select(this).on("mouseover", function(d,i,j){
      				 d3.select(this).style("cursor","pointer");
      				 d3.select(this).style("cursor","hand");
      			 });
      			 d3.select(this).on("mouseout", function(d,i,j){
      				 d3.select(this).style("cursor","default");
      			 });
      		 } 
      	  });
        }
	},
_getXAxisValues:function(){
		
		var svg = this._svg;
		var div = this._div;
		var x = this._x;
		var y = this._y;
		var chartHeight = this._chartHeight;
		var chartWidth = this._chartWidth;
		var x1,x2;
		var clientHeight = this.__clientHeight;
		var clientWidth = this._clientWidth;
		var xAxis = this._xAxis;
		var yAxis = this._yAxis;
		var origin,destination;
		//svg.on("mousedown",zoomselected);
		var brush = d3.svg.brush()
					    .x(x)
					    .on("brush", brushmove)
					    .on("brushend", brushend);
		
		var brushSVG = svg.append("g")
			.attr("id","drillSelectedArea")
			.attr("class", "brush")
			.style("stroke","#fff")
			.style("fill-opacity",".125")
			.style("shape-rendering","crispEdges")
			.call(brush)
			.selectAll('rect')
			.attr('height', chartHeight);
			
		
		svg.on('mousedown', function(){
		      var e= this;
			  origin =  d3.mouse(e);
			  brush_elm = svg.select(".brush").node();
			  new_click_event = new Event('mousedown');
			  new_click_event.pageX = d3.event.pageX;
			  new_click_event.clientX = d3.event.clientX;
			  new_click_event.pageY = d3.event.pageY;
			  new_click_event.clientY = d3.event.clientY;
			  brush_elm.dispatchEvent(new_click_event);
			});
		function brushmove() {
			  var extent = brush.extent();
			  x1=extent[0];
			  console.log("x1 : "+x1);
			  x2=extent[1];
			  console.log("x2 : "+x2);
		}
		function brushend() {
				
				//d3.select(".brush").call(brush.clear());
			    var windowWidget = zk.Widget.$('$historyPanel');
			    var startAndEnd = x1 + "&" + x2 + "&" + this._xAxisTicksCount;
			    //alert(startAndEnd);
			    zAu.send(new zk.Event(windowWidget,'onDrillChange', startAndEnd));
			    brushSVG.remove();
		}
	
	},	
		
});