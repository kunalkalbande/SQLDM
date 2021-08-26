package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IOWidgetInstance;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IOWidgetInstance;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByIOComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<IOWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(InstancesByIOComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<IOWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((IOWidgetInstance)instance).getPhysicalIO();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_DISKVIEW;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				IOWidgetInstance.class.getName());
	}
	
	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<IOWidgetInstance>(
				(Collection<? extends IOWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<IOWidgetInstance>() {
			@Override
			public int compare(IOWidgetInstance o1, IOWidgetInstance o2) {
				Double v1 = Double.parseDouble(o1.getPhysicalIO());
				Double v2 = Double.parseDouble(o2.getPhysicalIO());
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
