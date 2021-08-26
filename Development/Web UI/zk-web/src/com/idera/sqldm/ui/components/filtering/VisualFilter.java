package com.idera.sqldm.ui.components.filtering;

import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.data.Filter;
import com.idera.sqldm.server.web.ELFunctions;

public class VisualFilter extends HtmlMacroComponent {

	private static final String FILTER_VALUE_ATTRIBUTE_KEY = "visual_filter_value_attribute";
	private static final long serialVersionUID = 1L;

	@Wire
	private Div filterDiv;

	public void setOnClickEventListener(EventListener<Event> eventListener) {

		this.addEventListener("onClick", eventListener);

		filterDiv.setSclass(eventListener != null ? "hand-on-mouseover "
				+ filterDiv.getSclass() : "");
	}

	@Wire
	private Image filterTypeImage;

	@Wire
	private Label filterValueLabel;

	private Filter filter;

	public Filter getFilter() {
		return filter;
	}

	public void setFilter(Filter filter) {
		updateFilter(filter);
	}

	public VisualFilter(Filter filter) {

		setMacroURI("~./com/idera/sqldm/ui/components/visualFilter.zul");

		compose();

		updateFilter(filter);
	}

	private void updateFilter(Filter filter) {

		this.filter = filter;

		String filterValue = "".equals(filter.getValue().trim()) ? ELFunctions
				.getLabel(I18NStrings.NOT_SPECIFIED) : filter.getValue();

		filterValueLabel.setValue(filterValue);
		filterValueLabel.setTooltip(filterValue);

		filterTypeImage.setSrc(ELFunctions.getImageURL(filter.getType()
				.getIconUrl(), "small"));

		this.setAttribute(FILTER_VALUE_ATTRIBUTE_KEY, filter);
	}

	public static VisualFilter getVisualFilterFromEvent(Event event) {

		VisualFilter visualFilter = null;

		if ((event != null) && (event.getTarget() != null)) {

			visualFilter = (VisualFilter) event.getTarget();

			Object filter = visualFilter
					.getAttribute(FILTER_VALUE_ATTRIBUTE_KEY);

			if ((filter != null) && (filter instanceof Filter)) {
				visualFilter.setFilter((Filter) filter);
			}
		}

		return visualFilter;
	}
}
