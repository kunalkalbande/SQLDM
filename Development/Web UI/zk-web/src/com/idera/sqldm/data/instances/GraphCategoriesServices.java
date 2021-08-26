package com.idera.sqldm.data.instances;

import java.util.ArrayList;
import java.util.List;

import com.idera.sqldm.data.instances.GraphCategories.GraphCategoriesEnum;

public class GraphCategoriesServices {
	
	public  List<GraphCategories> getDefaultList() {
		
		List<GraphCategories> list = new ArrayList<>();
		for(GraphCategoriesEnum ace: GraphCategoriesEnum.values()) {
			list.add(ace.getDefaultPosition()-1, new GraphCategories(ace));				
		}
		return list;
	}
	
	public List<GraphCategories> getGraphCategoryOrderedList(List<GraphCategories> gcList){
		List<GraphCategories> list = new ArrayList<>();
		for (GraphCategories graphCategories : gcList) {
			list.add(graphCategories.getPosition(), graphCategories);
		}
		
		return list;
	}
}
