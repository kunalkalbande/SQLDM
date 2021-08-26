zk.load("zul.zul");

zk.addBeforeInit(function() {
	
	zkMLhr = {};
	Object.extend(zkMLhr, zulHdr);
	
	//Resize the column. 
	zkMLhr.resize = function (col1, icol, wd1, keys) {
		var box = getZKAttr(col1.parentNode, "rid");
		if (box) {
			var meta = zkau.getMeta(box);
			if (meta)
				meta.resizeCol(
					$parentByType(col1, "Lhrs"), icol, col1, wd1, keys);
		}
	};
	
	zk.copy(zkMLhr, {
	
		_zkLhrinit: zkMLhr.init,
		
		init: function (cmp) {
			
			zkMLhr._zkLhrinit(cmp);
			
			var pp = getZKAttr(cmp.parentNode, "mpop");
				zk.listen(cmp, "mouseover", zkMLhr.onHdOver);
				zk.listen(cmp, "mouseout", zkMLhr.onHdOut);
				
			if (pp != "zk_n_a") {
				var btn = $e(cmp, "btn");
				if (btn) {
					zk.listen(btn, "click", zkMLhr.onMenuClick);
		
					// zid might not be ready yet.
					zk.addInitLater(function () {
						var mpop = $e(pp);
						if (!mpop)
							mpop = zkau.getByZid(cmp.parentNode, pp);
						if (mpop) {
							if (getZKAttr(mpop, "autocreate") == "true" &&
								getZKAttr(cmp.parentNode, "columnshide") != "true" &&
								getZKAttr(cmp, "asc") != "true" &&
								getZKAttr(cmp, "dsc") != "true")
									zk.remove($e(cmp.id + "!btn"));
							zk.on(mpop, "close", zkMLhr.onMenuClose);
							zk.on(mpop, "onOuter", zkMLhr.onMenuOuter);
						}
					});
				}
			}
			
		},
				
		onMenuOuter: function (mpop) {
			zk.on(mpop, "close", zkMLhr.onMenuClose);
		},
		
		onMenuClick: function (evt) {
			if (!evt) evt = window.event;
			var cmp = $parentByType(Event.element(evt), "MLhr"),
				pp = zkau.getByZid(cmp.parentNode, getZKAttr(cmp.parentNode, "mpop")),
				btn = $e(cmp, "btn");
			
			if (!pp) return;
			zk.addClass(cmp, getZKAttr(cmp, "zcls") + "-visi");
		
			if (getZKAttr(pp, "autocreate") == "true") {
				var group = getZKAttr(cmp.parentNode, "columnsgroup") == "true",
					asc = getZKAttr(cmp, "asc") == "true",
					dsc = getZKAttr(cmp, "dsc") == "true",
					ul = $e(pp, "cave");
				if (ul) {
					var li = zk.firstChild(ul, "LI");
					if (group) {
						li.style.display = asc || dsc ? "" : "none";
						li = zk.nextSibling(li, "LI");
					}
					if (li) li.style.display = asc ? "" : "none";
					li = zk.nextSibling(li, "LI");
					if (li) li.style.display = dsc ? "" : "none";
		
					//separator
					li = zk.nextSibling(li, "LI");
					if (li) li.style.display = (asc||dsc) ? "" : "none";
		
				}
			}
		
			pp.style.position = "absolute";
			zk.setVParent(pp);
			zk.position(pp, btn, "after_start");
			var xy = zk.revisedOffset(cmp), t = $int(pp.style.top);
			if (xy[1] < t) pp.style.top = t - 4 + "px";
		
			zkMpop2.context(pp, cmp);
			setZKAttr(pp, "menuId", cmp.id);
			Event.stop(evt); // avoid onSort event.
		},
		
		onMenuClose: function (pp) {
			var cmp = $e(getZKAttr(pp, "menuId")),
				zcls = getZKAttr(cmp, "zcls");
			zk.rmClass(cmp, zcls + "-visi");
			zk.rmClass(cmp, zcls + "-over");
			return false; // stop event propagation
		},
		
		onHdOver: function (evt) {
			if (!evt) evt = window.event;
			var cmp = $parentByType(Event.element(evt), "MLhr"),
				btn = $e(cmp, "btn");
			if( getZKAttr(cmp, "showmenu") ) {
				zk.addClass(cmp, getZKAttr(cmp, "zcls") + "-over");
				if (btn) btn.style.height = cmp.offsetHeight - 1 + "px";
			}
		},
		
		onHdOut: function (evt) {
			if (!evt) evt = window.event;
			var cmp = $parentByType(Event.element(evt), "MLhr"),
				zcls = getZKAttr(cmp, "zcls");
			if (!zk.hasClass(cmp, zcls + "-visi") &&
				(!zk.ie || !zk.isAncestor(cmp, evt.relatedTarget || evt.toElement)))
				zk.rmClass(cmp, zcls + "-over");
		}
		
	});
	
	
});