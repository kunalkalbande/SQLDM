package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.MostActiveConnection;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.MostActiveConnection;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByActiveConnectionsComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<MostActiveConnection> listModel;
	private Logger logger = Logger.getLogger(InstancesByActiveConnectionsComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<MostActiveConnection>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((MostActiveConnection)instance).getConnections();
		return utilization;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				MostActiveConnection.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<MostActiveConnection>(
				(Collection<? extends MostActiveConnection>) obj);

		Collections.sort(listModel, new Comparator<MostActiveConnection>() {
			@Override
			public int compare(MostActiveConnection o1, MostActiveConnection o2) {
				Integer v1 = o1.getConnections();
				Integer v2 = o2.getConnections();
				return v2.compareTo(v1);
			}
		});		
	}

	@Override
	public Product getSelectedInstanceProduct(int selectedIndex) {
		return listModel.get(selectedIndex).getProduct();
	}

	@Override
	public int getInstanceId(int selectedIndex) {
		return listModel.get(selectedIndex).getInstanceId();
	}

	@Override
	public Logger getLogger() {
		return logger;
	}

}
