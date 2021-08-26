zk.$package('com.idera.sqldm.server.web.component.zul.grid');
com.idera.server.web.component.zul.grid.ExtListheader = zk.$extends(
		zul.sel.Listheader, {

			getZclass : function() {
				var zcls = this._zclass;
				return zcls != null ? zcls : 'z-listheader';
			},

			redraw : function(out) {
				var uuid = this.uuid, zcls = this.getZclass(), label = this
						.domContent_();

				out.push('<th', this.domAttrs_(), '><div id="', uuid,
						'-cave" class="', zcls, '-cnt"', this
								.domTextStyleAttr_(), '>', '<span>',
						((!this.firstChild && label == "") ? "&nbsp;" : label),// ZK-805
																				// MenuPopup
																				// without
																				// columns
																				// issue
						'</span>', '<div class="', zcls, '-sort-img"></div>');

				if (this.parent._menupopup && this.parent._menupopup != 'none')
					out.push('<a id="', uuid,
							'-btn"  href="javascript:;" class="', zcls,
							'-btn"></a>');

				for (var w = this.firstChild; w; w = w.nextSibling)
					w.redraw(out);

				out.push('</div></th>');
			}

		});