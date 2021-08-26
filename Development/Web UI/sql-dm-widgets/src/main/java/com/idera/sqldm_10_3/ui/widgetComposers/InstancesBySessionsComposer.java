package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TotalSessionWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TotalSessionWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesBySessionsComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<TotalSessionWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(InstancesBySessionsComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<TotalSessionWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((TotalSessionWidgetInstance)instance).getNumberOfSessions();
		return utilization;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				TotalSessionWidgetInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<TotalSessionWidgetInstance>(
				(Collection<? extends TotalSessionWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<TotalSessionWidgetInstance>() {
			@Override
			public int compare(TotalSessionWidgetInstance o1, TotalSessionWidgetInstance o2) {
				Integer v1 = o1.getNumberOfSessions();
				Integer v2 = o2.getNumberOfSessions();
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
