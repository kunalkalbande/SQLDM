zk.$package('com.idera.sqldm.d3zk.chart');
com.idera.sqldm.d3zk.chart.HeatmapChart = zk.$extends(com.idera.sqldm.d3zk.chart.RectangularChart, {
	_hasRendered: false,
	_orient: "vertical",
	_animateBars: false,
	_animationDelay: 50,
	_animationLength: 500,
	_data: {},
	_view: null,
	_hide: false,
	
	//backbone.js code starts here
	_hoverBox:Backbone.View.extend({
	    tagName: "div",
	    className: "hoverBoxContainerOuter",
	    events: {
	        "click .close": "hide",
	        "mousemove div": "move",
	        "mousemove img": "move"
	    },
	    initialize: function() {
	        this.container = this.options.container || document.body;
	    },
	    move: function() {
	        if (this.options.hideOnMouseover) {
	            this.hide();
	        }
	        return false;
	    },
	    render: function() {
	        var a = this;
	        $(this.el).empty();
	        $(this.el).append('<img class="arrowIcon" height="10" width="17">');
	        if (!this.options.hideClose) {
	            $(this.el).append('<div class="close"><img src="~./sqldm/com/idera/sqldm/images/close-small.png"></div>')
	        }
	        $(this.el).append('<div class="hoverBoxContainer"></div>');
	        $(this.el).append('<img style="display:none" class="arrowIconDown" src="~./sqldm/com/idera/sqldm/images/hoverbox-down-arrow-white.png" height="10" width="17">');
	        $(this.el).find(".hoverBoxContainer").addClass("white");
	        $(this.el).find(".hoverBoxContainer").css("z-index", 10);
	        $(this.el).find(".close").css("z-index", 11);
	        this.setContent(this.options.content);
	        if (this.options.height) {
	            $(this.el).find(".hoverBoxContainer").css("height", this.options.height);
	        }
	        if (this.options.width) {
	            $(this.el).find(".hoverBoxContainer").css("width", this.options.width);
	        }
	        return this;
	    },
	    setContent: function(a) {
	        this.options.content = a;
	        $(this.el).find(".hoverBoxContainer").html(a);
	    },
	    setArrowState: function(a) {
	        if (a === "ok" || a === "critical" || a === "maintenancemode" || a === "warning") {
	            $(this.el).find("img.arrowIcon").attr("src", "~./sqldm/com/idera/sqldm/images/hoverbox-up-arrow-white-"+a+".png");
	        } else {
	            $(this.el).find("img.arrowIcon").attr("src", "~./sqldm/com/idera/sqldm/images/hoverbox-up-arrow-white.png");
	        }
	    },
	    show: function(d) {
	        var a = this;
	        var c = function() {
	            var f = $(a.render().el);
	            $(a.container).append(f);
	            a.reposition(d);
	        };
	        if (this.options.delay) {
	            clearTimeout(this.delayTimeout);
	            this.delayTimeout = setTimeout(c, this.options.delay)
	        } else {
	            c.call();
	        }
	    },
	    hide: function() {
	        clearTimeout(this.delayTimeout);
	        if (this.options.hide) {
	            this.options.hide();
	        }
	        $(this.el).detach();
	    },
	    reposition: function(g) {
	    	var arrowIconDown ="none",pos="left";
	        if (!g) {
	            var f = $(this.options.relativeTo || document.body);
	            g = f.offset();
	            g.top += f.outerHeight() + 10
	        }
	        var outerWidth = $(this.el).outerWidth() || 232;
	        var outerHeight = $(this.el).height() || 516;
	        var d = outerWidth; //$(this.el).outerWidth();
	        var h = this.options.offset || 0;
	        var a = $(document.body).width();
	        g.left += h;
	        if (g.left + d > a) {
	            if (f) {
	                g.left = g.left + f.outerWidth() - d;
	            } else {
	                g.left = g.left + g.width - d;
	            }
	            pos = "right";
	            this.$(".arrowIcon").css("right", "15px");
	            this.$(".arrowIconDown").css("right", "15px");
	        } else {
	            this.$(".arrowIcon").css("left", "15px");
	            this.$(".arrowIconDown").css("left", "15px");
	        } //if (g.top + $(this.el).height() > $(window).height()) {
	        if (g.top + outerHeight > $(window).height()) {
	            var c = null;
	            if (f) {
	                c = f.offset().top - $(this.el).height() - this.$(".arrowIcon").height();
	            } else {
	                //c = g.top - $(this.el).height() - g.height - this.$(".arrowIcon").height();
	            	c = g.top - outerHeight - g.height - this.$(".arrowIcon").height();
	            } if (c >= 0) {
	                g.top = c;
	                arrowIconDown = "block";
	                this.$(".arrowIcon").css("display", "none");
	                this.$(".arrowIconDown").css("display", "block");
	            }
	        }
	        $(this.el).css({
	            left: g.left,
	            top: g.top
	        })
	        
	        return {"left": g.left, "top": g.top,"arrowIconDown": arrowIconDown,"pos": pos};
	    }
	}),
	
	_heatmapWidget:Backbone.View.extend({
		className: 'heatmapContainer',
		
		events: {
			'mouseover .box': 'showTooltip',
			'mouseout svg': 'hideTooltip',
			'click .box': 'showDetails'
		},
		
		showTooltip: function(c){
			if (this.frozen) {
	            return;
	        }
	        var a = d3.select(c.target.parentNode).datum();
	        if (a) {
	            this.labelHoverBox.show(this.offsetForBox(c.target));
	            this.labelHoverBox.setContent(this.options.formatTooltip(a, this.labelHoverBox));
	        } else {
	            this.hideTooltip();
	        }
		},
		
		hideTooltip: function(){
			if (this.frozen) {
	            return;
	        }
	        this.labelHoverBox.hide();
		},
		
		sendDataToserver: function(d){
			zAu.send(new zk.Event(zk.Widget.$('$heatmapChartDiv'),'onServerBoxClick',d));
		},
		
		showDetails: function(c){
			if (this.frozen) {
	            this.hideDetails();
	            this.sendDataToserver();
	            return;
	        }
	        this.hideTooltip();
	        this.setFrozen(true);
	        d3.select(d3.select(c.target)[0][0].parentNode).attr("class", "box nohilight");
	        var a = d3.select(c.target).data();
	        if (a.length > 0) {
	        	var pos = this.detailsHoverBox.reposition(this.offsetForBox(c.target));
	        	var d = JSON.stringify({"id":a[0].id, "left":pos.left, "top":pos.top,"pos":pos.pos,"arrowIconDown":pos.arrowIconDown,"idx":a[0].idx,"name":a[0].label,"state":a[0].state});
	        	this.sendDataToserver(d);
	        } else {
	            this.hideDetails();
	        }
		},
		
		hideDetails: function(a){
			this.detailsHoverBox.hide();
		},
		
		initialize: function(){
			_.bindAll(this, 'update', 'resize');
			//$(this.el).css("background-image",'url("bg-linen-dark.jpg")');
			$(window).bind('resize', this.resize);
		},
		
		render: function(){
			var a = this;
	        this.labelHoverBox = new a.options.jsThis._hoverBox({
	            white: true,
	            hideOnMouseover: true,
	            hideClose: true
	        });
	        this.detailsHoverBox = new a.options.jsThis._hoverBox({
	            white: true,
	            hide: function() {
	                a.setFrozen(false);
	            }
	        });
			
			$(this.el).html('<img style="margin:20px 30px" alt="Smiley face" width=32 height=32><svg width=0 height=0>');
			return this;
		},
		
		resize: function(){
			if (this.resizeTimeout) {
				clearTimeout(this.resizeTimeout);
			}
			this.resizeTimeout = setTimeout(this.update, 500);
		},
		
		update: function() {
	        if (this.frozen) {
	            return;
	        }
	        var u = this;
	        var q = 0;
	        var a = 30;
	        var t = 25;
	        var f = 1;
	        if (this.data.length < 300) {
	            f = 1.5
	        }
	        if (this.data.length < 200) {
	            f = 2
	        }
	        if (this.data.length < 150) {
	            f = 2.5
	        }
	        if (this.data.length < 100) {
	            f = 3
	        }
	        var o = Math.max(1, f / 1.25);
	        var p = Math.min(0.6, f * 0.35);
	        this.width = Math.round(a * f);
	        this.height = Math.round(t * f);
	        $(this.el).css("padding", 4 * f);
	        var n = this.columns;
	        var l = this.rows;
	        this.columns = Math.max(5, Math.floor($(this.el).width() / (this.width + q)));
	        this.rows = Math.max(3, Math.ceil(this.data.length / this.columns));
	        if (this.previousDataLength != this.data.length || n != this.columns || l != this.rows) {
	            this.generateCoordinateMap(this.columns, this.rows)
	        }
	        var g = function(w) {
	            return (u.coords[w].x) * u.width + (u.coords[w].x) * q
	        };
	        var v = function(w) {
	            return (u.coords[w].y) * u.height + (u.coords[w].y) * q
	        };
	        this.$("img").hide();
	        var d = new this.options.jsThis.rainbow(this.options.jsThis);
	        d.setSpectrum(["bc2708", 0], ["bc2708", 70], ["fcc416", 85], ["449c06", 100]);
	        var k = d3.select(this.$("svg")[0]);
	        k.attr("height", Math.max(0, this.rows * this.height + this.rows * q - q)).attr("width", Math.max(0, this.columns * this.width + this.columns * q - q));
	        var j = k.selectAll("g.box").data(this.data, function(w) {
	            return w.productId + '_' + w.id;
	        });
	        var s = function(z, w) {
	            if (z.idx > 1) {
	                return "#94979f";
	            }
	            if(z.state === "maintenancemode") {
	            	return "#94979f";
	            }
	            return "#" + d.colorAt(z.idx * 100);
	        };
	        var c = j.enter().append("g").attr("class", "box");
	        c.append("rect");
	        c.append("line").attr("class", "line1");
	        c.append("line").attr("class", "line2");
	        c.append("line").attr("class", "line3");
	        c.append("line").attr("class", "line4");
	        var h = function(w) {
	            w.attr("fill", s).attr("transform", function(A, z) {
	                return "translate(" + g(z) + "," + v(z) + ")"
	            });
	            w.select("rect").attr("width", function(A, z) {
	                return u.width
	            }).attr("height", function(A, z) {
	                return u.height
	            });
	            w.select(".line1").attr("stroke-width", o).attr("stroke", "#ffffff").attr("opacity", p).attr("x1", 0).attr("y1", o / 2).attr("x2", u.width - o).attr("y2", o / 2);
	            w.select(".line2").attr("stroke-width", o).attr("stroke", "#ffffff").attr("opacity", p).attr("x1", o / 2).attr("y1", o).attr("x2", o / 2).attr("y2", u.height - o);
	            w.select(".line3").attr("stroke-width", o).attr("stroke", "#000000").attr("opacity", 0.75).attr("x1", u.width - (o / 2)).attr("y1", 0).attr("x2", u.width - (o / 2)).attr("y2", u.height);
	            w.select(".line4").attr("stroke-width", o).attr("stroke", "#000000").attr("opacity", 0.75).attr("x1", 0).attr("y1", u.height - (o / 2)).attr("x2", u.width - (o / 2)).attr("y2", u.height - (o / 2))
	        };
	        h(c);
	        h(j.transition().duration(500));
	        j.exit().transition().duration(500).style("opacity", 0).remove();
	    },
	    
		generateCoordinateMap: function(c, g) {
	        this.coords = [];
	        var f = 0;
	        for (var a = 0; a < this.data.length; a++) {
	            var d = _.last(this.coords);
	            if (f == 0) {
	                this.coords.push({
	                    x: 0,
	                    y: 0
	                });
	                f++
	            } else {
	                if (d.x == 0 || d.y == g - 1) {
	                    if (f < c) {
	                        this.coords.push({
	                            x: f,
	                            y: 0
	                        })
	                    } else {
	                        this.coords.push({
	                            x: c - 1,
	                            y: f - c + 1
	                        })
	                    }
	                    f++
	                } else {
	                    this.coords.push({
	                        x: d.x - 1,
	                        y: d.y + 1
	                    })
	                }
	            }
	        }
	    },

		setData: function(a){
			this.previousDataLength = this.data ? this.data.length : 0;
	        this.data = a.sort(function(d, c) {
	        	if(d.state != c.state) {
	        		if(d.state === "maintenancemode") {
		        		return 1;
		        	}
	        		if(c.state === "maintenancemode") {
		        		return -1;
		        	}
	        	}
	            if (d.idx < c.idx) {
	                return -1
	            }
	            if (d.idx > c.idx) {
	                return 1
	            }
	            if (d.label < c.label) {
	                return -1
	            }
	            if (d.label > c.label) {
	                return 1
	            }
	            return 0
	        });
	        this.update();
		},
		
		offsetForBox: function (a) {
			var c = $(a).offset();
	        c.width = this.width;
	        c.height = this.height;
	        c.top += c.height + 7;
	        c.left -= 7;
	        return c;
		},
		
		setFrozen: function (a) {
			this.frozen = a;
	        if (a) {
	            d3.selectAll(".box").attr("class", "box frozen")
	        } else {
	            d3.selectAll(".box").attr("class", "box")
	        }
		}
		}),

	_heatmapView:Backbone.View.extend({
		
		className: 'heatmapPanel',
		
		template: _.template(
	      '<div class="bodyContent"></div>'
	    ),
		
		
		initialize: function(){
			var self = this;
			_.bindAll(this,'update', 'formatTooltip');
			
			this.heatmap = new self.options.jsThis._heatmapWidget({
				jsThis: self.options.jsThis,
				formatTooltip: this.formatTooltip
			});
			$(self.options.divID).append($(this.el));
			$(this.el).html(this.template());
		},
		
		
		render: function(){
			this.$('.bodyContent').html(this.heatmap.render().el);
			return this;
		},
		
		formatTooltip: function(data, hoverbox) {
	    var div = $('<div>'+
	                '  <div class="serverHeatMapHeader">'+
	                '    <div class="label"></div>'+
	                '    <div class="healthLabel"></div>'+
	                '  </div>'+
	                '  <div class="serverHeatMapSummary">'+
	                '    <div class="lastUpdatedLabel">(click for details)</div>'+
	                '  </div>'+
	                '</div>');

	    this.setHeaderState(div, data.state, hoverbox);

	    div.find('.label').text(data.label).attr('title', data.label);
	    // a sort value greater than 1 means it hasn't been seen in a while
	    if (data.idx > 1) {
	      div.find('.healthLabel').html('Health Index: 0%');
	      div.find('.lastUpdatedLabel').html('This server has not been seen<br>for more than 5 minutes');
	    } else {
	    	var healthIdx = data.idx*100;
	    	div.find('.healthLabel').text('Health Index: '+(healthIdx.toFixed(2))+'%');
	    }
	    return div;
	  },
	  
		setHeaderState: function(div, state, hoverbox) {
			div.find('.serverHeatMapHeader').removeClass('state_unknown', 'state_ok', 'state_warning', 'state_critical','state_maintenancemode');
			div.find('.serverHeatMapHeader').addClass("state_"+state);
			hoverbox.setArrowState(state);
	  },
		update: function(a){
			this.heatmap.setData(a);
	  }

	}),
	
	
	rainbow: function(jsThis){
		var a = null;
		var d = 0;
		var c = 100;
		var g = [['ff0000',10],['ffff00',25],['00ff00',50],['0000ff',75]];
		f(g);
		function f(j) {
			if (j.length < 2) {
				throw new Error('Rainbow must have two or more colours.')
			} else {
				a = [];
				for (var k = 0; k < j.length - 1; k++) {
						var h = new jsThis.colourGradient();
						h.setGradient(j[k][0], j[k + 1][0]);
						h.setNumberRange(j[k][1], j[k + 1][1]);
						a[k] = h;
				}
			g = j;
			}
		}
		this.setColors = this.setColours;
		this.setSpectrum = function () {
								f(arguments);
								return this;
							};
		this.setSpectrumByArray = function (h) {
								f(h);
								return this;
							};
		this.colourAt = function (j) {
							if (isNaN(j)) {
								throw new TypeError(j + ' is not a number');
							} else {
								if (a.length === 1) {
									return a[0].colourAt(j);
								} else {
									for (var h = 0; h < a.length; h++) {
											if (j >= a[h].minNumber && j <= a[h].maxNumber) {
													return a[h].colourAt(j);
											}
									}
									return a[0].colourAt(0);
								}
							}
						};
		this.colorAt = this.colourAt;
		this.setNumberRange = function (j, h) {
									if (h > j) {
											d = j;
											c = h;
											f(g);
									} else {
										throw new RangeError('maxNumber (' + h + ') is not greater than minNumber (' + j + ')');
									}
								return this;
								};
	},
	
	colourGradient: function(){
		var h = 'ff0000';
		var c = '0000ff';
		var f = 0;
		var d = 100;
		this.setGradient = function (k, l) {
				h = a(k);
				c = a(l);
			};
		this.setNumberRange = function (l, k) {
									if (k > l) {
										this.minNumber = l;
										this.maxNumber = k;
										f = l;
										d = k;
									} else {
										throw new RangeError('maxNumber (' + k + ') is not greater than minNumber (' + l + ')');
									}
					};
			this.colourAt = function (k) {
						return j(k, h.substring(0, 2), c.substring(0, 2)) + j(k, h.substring(2, 4), c.substring(2, 4)) + j(k, h.substring(4, 6), c.substring(4, 6));
				};
		function j(p, l, t) {
				var q = p;
				if (q < f) {
					q = f;
				}
				if (q > d) {
					q = d;
				}
				var s = d - f;
				var n = parseInt(l, 16);
				var k = parseInt(t, 16);
				var u = (k - n) / s;
				var o = Math.round(u * (q - f) + n);
				return formatHex(o.toString(16));
			}
		formatHex = function (k) {
				if (k.length === 1) {
						return '0' + k;
				} else {
					return k;
				}
			};
		function g(k) {
				var l = /^#?[0-9a-fA-F]{6}$/i;
				return l.test(k);
			}
		function a(k) {
				if (g(k)) {
					return k.substring(k.length - 6, k.length);
				} else {
					var n = [['red','ff0000'],['lime','00ff00'],['blue','0000ff'],['yellow','ffff00'],['orange','ff8000'],['aqua','00ffff'],['fuchsia','ff00ff'],['white','ffffff'],
							['black','000000'],['gray','808080'],['grey','808080'],['silver','c0c0c0'],['maroon','800000'],['olive','808000'],['green','008000'],['teal','008080'],
							['navy','000080'],['purple','800080']];
					for (var l = 0; l < n.length; l++) {
							if (k.toLowerCase() === n[l][0]) {
								return n[l][1];
							}
					}
					throw new Error(k + ' is not a valid colour.');
				}
			}
	},

	//backbone.js code ends here	
	getHasRendered: function() { 
		return this._hasRendered;
	},
	
	setHasRendered: function(value) { 
		this._hasRendered = value;
	},
	
	getData: function(){
		return this._data;
	},
	
	setData: function(value){
		this._data = value;
		if(this._hasRendered) this.rerenderChart();
	},
	
	getOrient: function() {
		return this._orient;
	},
	
	setOrient: function(value) {
		if(value != this._orient){
			this._orient = value;
			if(this._hasRendered) this.rerenderChart();
		}
	},
	
	getAnimateBars: function() {
		return this._animateBars;
	},
	
	setAnimateBars: function(value) {
		if(value != this._animateBars){
			this._animateBars = value;
			if(this._hasRendered) this.rerenderChart();
		}
	},
	
	getAnimationDelay: function() {
		return this._animationDelay;
	},
	
	setAnimationDelay: function(value) {
		if(value != this._animationDelay){
			this._animationDelay = value;
			if(this._hasRendered) this.rerenderChart();
		}
	},
	
	getAnimationLength: function() {
		return this._animationLength;
	},
	
	setAnimationLength: function(value) {
		if(value != this._animationLength){
			this._animationLength = value;
			if(this._hasRendered) this.rerenderChart();
		}
	},
	
	setHide: function(){
		if(this._view)
			this._view.heatmap.hideDetails();
	},
	
	
	rerenderChart: function() {
		var self = this;
		var div = document.getElementById(this.uuid);
		
		if(div == null) return;
		var uuid = "#" + this.uuid;
		
		var hoverBox = d3.select(".hoverBoxContainerOuter");
		
		if(hoverBox && hoverBox.select(".hoverBoxContainer")){
			hoverBox.remove();
		}
		
		var svg = d3.select(uuid).select(".heatmapPanel");
		if( svg ) svg.remove();
		
		var view = new this._heatmapView({divID:uuid,jsThis:this});
		this._view = view;
		
		view.render();
		view.update(this._data);
		
		this._hasRendered = true;
	}
});