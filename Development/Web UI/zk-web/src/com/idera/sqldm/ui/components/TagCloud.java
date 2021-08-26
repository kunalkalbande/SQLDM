package com.idera.sqldm.ui.components;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.server.web.ELFunctions;

public class TagCloud extends HtmlMacroComponent {

	private static final long serialVersionUID = 1L;

	private static final int MAX_FONT_SIZE_PT = 18;
	private static final int MIN_FONT_SIZE_PT = 9;

	private static final int MAX_PADDING_PX = 20;
	private static final int MIN_PADDING_PX = 5;

	private Map<String, Integer> tags = new HashMap<String, Integer>();

	public Map<String, Integer> getTags() {
		return tags;
	}

	public void setTags(Map<String, Integer> tags) {
		this.tags = tags;

		updateTagCloud();
	}

	@Wire
	protected Div tagCloudDiv;

	@Wire
	protected Label errorLabel;

	public TagCloud() {
		setMacroURI("~./com/idera/sqldm/ui/components/tagCloud.zul");

		compose();

		updateTagCloud();
	}
	
	public void setTagOnClickEventListener(EventListener<Event> eventListener) {		
		for (Component tagComponent : tagCloudDiv.getChildren()) {
			
			if (tagComponent == null || !(tagComponent instanceof Label)) continue;

			Label tag = (Label) tagComponent;

			if (eventListener != null) {
				tag.setSclass("link-no-size");
				tag.addEventListener("onClick", eventListener);
			}
		}
	}

	private void updateTagCloud() {

		try {
			if (!tags.isEmpty()) {

				// create a list of tag weights from the tag Map
				List<Integer> tagWeights = new ArrayList<Integer>();

				for (Map.Entry<String, Integer> entry : tags.entrySet()) {

					tagWeights.add(entry.getValue());
				}

				// find the largest tag weight.
				float maxTagWeight = Collections.max(tagWeights);
				float minTagWeight = Collections.min(tagWeights);

				tagCloudDiv.getChildren().clear();
				for (Map.Entry<String, Integer> entry : tags.entrySet()) {

					float fontSize = MIN_FONT_SIZE_PT;
					float padding = MIN_PADDING_PX;

					// calculate the proportional font size
					if (entry.getValue() > 0) {

						float normalizedWeight = (entry.getValue() - minTagWeight)
								/ (maxTagWeight - minTagWeight);
						
						fontSize = (normalizedWeight * MAX_FONT_SIZE_PT) + 9;
						padding = normalizedWeight * MAX_PADDING_PX;

						if (fontSize < MIN_FONT_SIZE_PT) {
							fontSize = MIN_FONT_SIZE_PT;
						}

						if (padding < MIN_PADDING_PX) {
							padding = MIN_PADDING_PX;
						}
					}

					// create a label for the tag and set its font size
					Label tagLabel = new Label(entry.getKey());
					tagLabel.setStyle("font-size: " + fontSize + "pt;"
							+ "padding-left: " + padding + "px;"
							+ "padding-right: " + padding + "px;"
							+ "padding-bottom: " + "10px;");
					
					tagCloudDiv.appendChild(tagLabel);
				}

				tagCloudDiv.setVisible(true);
				errorLabel.setVisible(false);

			} else {

				handleError(I18NStrings.NO_TAGS_FOUND);
			}
		} catch (Exception x) {

			handleError(I18NStrings.ERROR_OCCURRED_LOADING_TAGS);
		}
	}

	private void handleError(String i18NMessageKey) {

		errorLabel.setValue(ELFunctions.getMessage(i18NMessageKey));

		tagCloudDiv.setVisible(false);
		errorLabel.setVisible(true);
	}
}
