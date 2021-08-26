package com.idera.sqldm.ui.components;

import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

public class SideBarStatusArea extends HtmlMacroComponent {

	public enum State {
		Neutral, OK, Critical, Warning
	}

	private static final long serialVersionUID = 1L;

	private static final String LEFT_STATUS_ELEMENT_SCLASS_BASE = "shadow left-status-element";
	private static final String CENTER_STATUS_ELEMENT_SCLASS_BASE = "shadow center-status-element";
	private static final String RIGHT_STATUS_ELEMENT_SCLASS_BASE = "shadow right-status-element";

	private static final String LEFT_STATUS_ELEMENT_SCLASS_DEFAULT = LEFT_STATUS_ELEMENT_SCLASS_BASE
			+ " darker-gray-background";
	private static final String CENTER_STATUS_ELEMENT_SCLASS_DEFAULT = CENTER_STATUS_ELEMENT_SCLASS_BASE
			+ " dark-gray-background";
	private static final String RIGHT_STATUS_ELEMENT_SCLASS_DEFAULT = RIGHT_STATUS_ELEMENT_SCLASS_BASE
			+ " darker-gray-background";

	@Wire
	private Div statusAreaHeaderDiv;

	@Wire
	private Div leftStatusElementDiv;

	@Wire
	private Label leftTitleLabel;

	public String getLeftTitle() {
		return leftTitleLabel.getValue();
	}

	public void setLeftTitle(String leftTitleLabel) {
		this.leftTitleLabel.setValue(leftTitleLabel);
	}

	@Wire
	private Label leftValueLabel;

	public String getLeftValue() {
		return leftValueLabel.getValue();
	}

	public void setLeftValue(String leftValueLabel) {
		this.leftValueLabel.setValue(leftValueLabel);
	}

	private SideBarStatusArea.State leftState = SideBarStatusArea.State.Neutral;

	public SideBarStatusArea.State getLeftState() {
		return leftState;
	}

	public void setLeftState(SideBarStatusArea.State leftState) {
		this.leftState = leftState;

		updateLeftStatusElement();
	}

	@Wire
	private Div centerStatusElementDiv;

	@Wire
	private Label centerTitleLabel;

	public String getCenterTitle() {
		return centerTitleLabel.getValue();
	}

	public void setCenterTitle(String centerTitleLabel) {
		this.centerTitleLabel.setValue(centerTitleLabel);
	}

	@Wire
	private Label centerValueLabel;

	public String getCenterValue() {
		return centerValueLabel.getValue();
	}

	public void setCenterValue(String centerValueLabel) {
		this.centerValueLabel.setValue(centerValueLabel);
	}

	private SideBarStatusArea.State centerState = SideBarStatusArea.State.Neutral;

	public SideBarStatusArea.State getCenterState() {
		return centerState;
	}

	public void setCenterState(SideBarStatusArea.State centerState) {
		this.centerState = centerState;

		updateCenterStatusElement();
	}

	@Wire
	private Div rightStatusElementDiv;

	@Wire
	private Label rightTitleLabel;

	public String getRightTitle() {
		return rightTitleLabel.getValue();
	}

	public void setRightTitle(String rightTitleLabel) {
		this.rightTitleLabel.setValue(rightTitleLabel);
	}

	@Wire
	private Label rightValueLabel;

	public String getRightValue() {
		return rightValueLabel.getValue();
	}

	public void setRightValue(String rightValueLabel) {
		this.rightValueLabel.setValue(rightValueLabel);
	}

	private SideBarStatusArea.State rightState = SideBarStatusArea.State.Neutral;

	public SideBarStatusArea.State getRightState() {
		return rightState;
	}

	public void setRightState(SideBarStatusArea.State rightState) {
		this.rightState = rightState;

		updateRightStatusElement();
	}

	public SideBarStatusArea() {
		setMacroURI("~./com/idera/sqldm/ui/components/sideBarStatusArea.zul");

		compose();

		updateLeftStatusElement();
		updateRightStatusElement();
		updateCenterStatusElement();
	}

	private void updateLeftStatusElement() {

		updateStatusElements(leftState, leftStatusElementDiv,
				LEFT_STATUS_ELEMENT_SCLASS_DEFAULT,
				LEFT_STATUS_ELEMENT_SCLASS_BASE, true);
	}

	private void updateCenterStatusElement() {

		updateStatusElements(centerState, centerStatusElementDiv,
				CENTER_STATUS_ELEMENT_SCLASS_DEFAULT,
				CENTER_STATUS_ELEMENT_SCLASS_BASE, false);
	}

	private void updateRightStatusElement() {

		updateStatusElements(rightState, rightStatusElementDiv,
				RIGHT_STATUS_ELEMENT_SCLASS_DEFAULT,
				RIGHT_STATUS_ELEMENT_SCLASS_BASE, false);
	}

	private void updateStatusElements(SideBarStatusArea.State state,
			Div statusElementDiv, String defaultSclass, String baseSclass,
			boolean updateHeader) {

		String statusElementSclass;
		String headerSclass;

		switch (state) {

		case OK:
			statusElementSclass = baseSclass + " pale-green-background";
			headerSclass = updateHeader ? "pale-green-background" : null;
			break;
		case Critical:
			statusElementSclass = baseSclass + " red-background";
			headerSclass = updateHeader ? "red-background" : null;
			break;
		case Warning:
			statusElementSclass = baseSclass + " yellow-background";
			headerSclass = updateHeader ? "yellow-background" : null;
			break;
		case Neutral:
		default:
			statusElementSclass = defaultSclass;
			headerSclass = updateHeader ? "darker-gray-background" : null;
			break;
		}

		statusElementDiv.setSclass(statusElementSclass);

		if (updateHeader) {
			statusAreaHeaderDiv.setSclass(headerSclass);
		}
	}
}
