package com.idera.sqldm.ui.components;

import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;

public class VisualTag extends HtmlMacroComponent {

	public enum TagStyle {
		ADD, REMOVE, NEUTRAL
	}

	private static final long serialVersionUID = 1L;

	@Wire
	private Div tagDiv;

	public void setOnClickEventListener(EventListener<Event> eventListener) {

		this.addEventListener("onClick", eventListener);

		tagDiv.setSclass(eventListener != null ? "clickable-tag" : "");
	}

	@Wire
	private Label tagLabel;

	public String getTagLabel() {
		return tagLabel.getValue();
	}

	public void setTagLabel(String tagLabel) {
		this.tagLabel.setValue(tagLabel);
	}

	@Wire
	private Image tagImage;

	private TagStyle tagStyle = TagStyle.NEUTRAL;

	public TagStyle getTagStyle() {
		return tagStyle;
	}

	public void setTagStyle(TagStyle tagStyle) {
		this.tagStyle = tagStyle;

		updateTag();
	}

	@Wire
	private Div tagImageDiv;

	public VisualTag(String tagLabel) {

		initializeComponent();

		setTagLabel(tagLabel);

		updateTag();
	}

	public VisualTag() {

		initializeComponent();

		updateTag();
	}

	private void initializeComponent() {
		setMacroURI("~./com/idera/sqldm/ui/components/visualTag.zul");

		compose();
	}

	private void updateTag() {

		switch (tagStyle) {

		case ADD:
			tagLabel.setSclass("padding-all-4 H5 add-tag-label");
			tagImageDiv.setSclass("tag-corner-image add-tag-corner-image");
			tagImage.setSrc("/images/add-circle.png");
			break;

		case REMOVE:
			tagLabel.setSclass("padding-all-4 H5 remove-tag-label");
			tagImageDiv.setSclass("tag-corner-image remove-tag-corner-image");
			tagImage.setSrc("/images/remove-circle.png");
			break;

		case NEUTRAL:
			tagLabel.setSclass("padding-all-4 H5 add-tag-label");
			tagImageDiv.setSclass("tag-corner-image add-tag-corner-image");
			tagImage.setSrc("");
			break;
		}

	}

}
