package com.idera.sqldm.ui.prescreptiveanalysis;

import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Toolbarbutton;
import org.zkoss.zul.Window;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listitem;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zkplus.databind.AnnotateDataBinder;

import com.idera.sqldm.data.prescreptiveanalysis.PrescreptiveAnalysisRecord;
import com.idera.sqldm.data.prescreptiveanalysis.Property;
import com.idera.sqldm.data.prescreptiveanalysis.Recommendation;
import com.idera.sqldm.data.prescreptiveanalysis.RecommendationLink;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class PrescreptiveAnalysisDetailsComposer extends SelectorComposer<Window> {
	
	private static final Logger log = Logger.getLogger(PrescreptiveAnalysisDetailsComposer.class);
	
    @Wire
    private Window analysisDetailsWindow;

	@Wire("#paDetailsListbox")
	private Listbox paDetailsListbox;

	@Wire("#propertiesListbox")
	private Listbox propertiesListbox;

	@Wire("#linksListbox")
	private Listbox linksListbox;

	@Wire
    private Div recDetails;
	@Wire
    private Label lblFindingText;
	@Wire
    private Label lblExplanationText;
	@Wire
    private Label lblRecommendationText;

	private ListModel<Recommendation> recommendations;

	public ListModel<Recommendation> getRecommendations() {
        return recommendations;
    }

	private ListModel<Property> properties;

	public ListModel<Property> getProperties() {
        return properties;
    }

	private ListModel<RecommendationLink> recommendationLinks;

	public ListModel<RecommendationLink> getRecommendationLinks() {
        return recommendationLinks;
    }
	
	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Window comp) throws Exception {
		super.doAfterCompose(comp);
		// comp.setVariable("controller", this, false);

		Integer analysisID 
				= (Integer) Executions.getCurrent().getArg().get("analysisID");
		Integer instanceId 
				= (Integer) Executions.getCurrent().getArg().get("instanceId");

		List<Recommendation> response = SQLDMRestClient.getInstance()
				.getPrescreptiveAnalysisDetailsForInstance(
					Integer.toString(instanceId),
					Integer.toString(analysisID)
				);
		
		recommendations = new ListModelList<Recommendation>(response);
		// ((ListModelList<Recommendation>) recommendations).setMultiple(true);

		if(paDetailsListbox != null){
			paDetailsListbox.setModel(recommendations);
		}
    }

	@Listen("onSelect = #paDetailsListbox")
	public void showDetails() {
		Listitem selectedItem = paDetailsListbox.getSelectedItem();
		
		if(selectedItem != null){
			recDetails.setVisible(true);
			Recommendation recommendation = (Recommendation) selectedItem.getValue();
			
			lblFindingText.setValue(recommendation.getFindingText());
			lblExplanationText.setValue(recommendation.getImpactExplanationText());
			lblRecommendationText.setValue(recommendation.getRecommendationText());

			properties = new ListModelList<Property>(recommendation.getProperties());

			if(propertiesListbox != null){
				propertiesListbox.setModel(properties);
			}

			recommendationLinks = new ListModelList<RecommendationLink>(recommendation.getRecommendationLinks());

			if(linksListbox != null){
				linksListbox.setModel(recommendationLinks);
			}
		}
	}
}
