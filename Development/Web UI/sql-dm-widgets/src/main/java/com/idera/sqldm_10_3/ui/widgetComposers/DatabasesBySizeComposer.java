package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.LargestDatabaseBySizeInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.LargestDatabaseBySizeInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class DatabasesBySizeComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<LargestDatabaseBySizeInstance> listModel;
	private Logger logger = Logger.getLogger(DatabasesBySizeComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<LargestDatabaseBySizeInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((LargestDatabaseBySizeInstance)instance).getFileSize();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				LargestDatabaseBySizeInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<LargestDatabaseBySizeInstance>(
				(Collection<? extends LargestDatabaseBySizeInstance>) obj);

		Collections.sort(listModel, new Comparator<LargestDatabaseBySizeInstance>() {
			@Override
			public int compare(LargestDatabaseBySizeInstance o1, LargestDatabaseBySizeInstance o2) {
				Double v1 = o1.getFileSize();
				Double v2 = o2.getFileSize();
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
