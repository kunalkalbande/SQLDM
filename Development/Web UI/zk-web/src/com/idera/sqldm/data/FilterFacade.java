package com.idera.sqldm.data;

import java.io.IOException;
import java.util.List;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.rest.JSONHelper;

public class FilterFacade {

	public static String serializeFilterList(List<Filter> filters)
			throws IOException {

		return JSONHelper.serializeToJson(filters);
	}

	public static List<Filter> deserializeFilterList(String serializedFilterList)
			throws IOException {

		return JSONHelper.deserializeFromJson(serializedFilterList,
				new TypeReference<List<Filter>>() {
				});
	}
}
