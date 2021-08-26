package com.idera.sqldm.ui.components;

import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;

public class SideBarItem extends HtmlMacroComponent {

	private static final long serialVersionUID = 1L;

	@Wire
	private Label countLabel;

	@Wire
	private Image iconImage;

	@Wire
	private Label titleLabel;

	public String getCount() {
		return countLabel.getValue();
	}

	public void setCount(String count) {
		countLabel.setValue(count);
	}

	public String getIconUrl() {
		return iconImage.getSrc();
	}

	public void setIconUrl(String iconUrl) {
		iconImage.setSrc(iconUrl);
	}

	public String getTitle() {
		return titleLabel.getValue();
	}

	public void setTitle(String title) {
		titleLabel.setValue(title);
	}

	public SideBarItem() {
		setMacroURI("~./com/idera/sqldm/ui/components/sideBarItem.zul");

		compose();
	}

}
