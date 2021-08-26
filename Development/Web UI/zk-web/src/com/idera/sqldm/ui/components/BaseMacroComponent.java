package com.idera.sqldm.ui.components;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zkplus.databind.AnnotateDataBinder;

public abstract class BaseMacroComponent extends HtmlMacroComponent {

	private static final long serialVersionUID = 1L;

	protected abstract String getZulUrl();

	protected AnnotateDataBinder dataBinder;

	public AnnotateDataBinder getAnnotateDataBinder() {
		return dataBinder;
	}

	public void setAnnotateDataBinder(AnnotateDataBinder annotateDataBinder) {
		dataBinder = annotateDataBinder;

		loadComponent();
	}

	public BaseMacroComponent() {

		setMacroURI(getZulUrl());

		compose();
	}

	protected void loadComponent() {
		if (dataBinder != null) {
			dataBinder.loadComponent(this);
		}
	}

	protected void loadComponent(Component component) {
		if (dataBinder != null) {
			dataBinder.loadComponent(component);
		}
	}

}
