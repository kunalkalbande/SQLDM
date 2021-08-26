package com.idera.sqldm.helpers;

import java.util.List;

import com.idera.common.InstanceStatus;
import com.idera.common.Utility;
import com.idera.i18n.I18NStrings;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.data.instances.CategoriesEntry;

public class InstanceHelper {
	
	private static final String BOOLEAN_TRUE_SORT_VALUE = "AAAAA";
	private static final String BOOLEAN_FALSE_SORT_VALUE = "BBBBB";
	private static final String STRING_BOTTOM_SORT_VALUE = "ZZZZZ";
	private static final Long LONG_BOTTOM_SORT_VALUE = Long.MAX_VALUE;
	private static final Long LONG_ALMOST_BOTTOM_SORT_VALUE = Long.MAX_VALUE - 100;
	private static final Integer INTEGER_BOTTOM_SORT_VALUE = Integer.MAX_VALUE;
	private static final Integer INTEGER_ALMOST_BOTTOM_SORT_VALUE = Integer.MAX_VALUE - 100;

	public static String getResponseTimeString(InstanceStatus instanceStatus,
			Long responseTime) {

		String responseTimeString = Utility.getMessage(I18NStrings.N_A);

		if ((instanceStatus != null) && (responseTime != null)) {

			switch (instanceStatus) {
			case CONNECTED:
			case SLOW:
				if (responseTime == 0) {
					responseTimeString = Utility
							.getMessage(I18NStrings.LESS_THAN_ONE_MILI_SECONDS);
				} else {
					responseTimeString = Utility.getMessage(
							I18NStrings.MILI_SECONDS, responseTime);
				}
				break;
			default:
				responseTimeString = Utility.getMessage(I18NStrings.N_A);
				break;
			}
		}

		return responseTimeString;
	}
	
	public static String getFormattedInstancePropertyString(
			String instanceProperty) {

		return ((instanceProperty != null) && (!instanceProperty.trim()
				.isEmpty())) ? instanceProperty : Utility
				.getMessage(I18NStrings.N_A);
	}

	public static String getFormattedInstancePropertyString(
			Integer instanceProperty) {

		return ((instanceProperty != null) && (!instanceProperty.equals(0))) ? String
				.format("%,d", instanceProperty) : Utility
				.getMessage(I18NStrings.N_A);
	}

	public static String getFormattedInstancePropertyString(
			Boolean instanceProperty) {

		if (instanceProperty != null) {

			return instanceProperty ? Utility.getMessage(I18NStrings.YES)
					: Utility.getMessage(I18NStrings.NO);

		} else {
			return Utility.getMessage(I18NStrings.N_A);
		}
	}
	
	public static String getSortValue(String sortValue) {

		return ((sortValue != null) && (!sortValue.isEmpty())) ? sortValue
				: STRING_BOTTOM_SORT_VALUE;
	}

	public static String getSortValue(Boolean sortValue,
			InstanceStatus instanceStatus) {

		if ((sortValue != null) && (instanceStatus != null)
				&& (instanceStatus != InstanceStatus.DISABLED)) {

			return sortValue ? BOOLEAN_TRUE_SORT_VALUE
					: BOOLEAN_FALSE_SORT_VALUE;
		} else {
			return STRING_BOTTOM_SORT_VALUE;
		}
	}

	public static String getSortValue(String sortValue,
			InstanceStatus instanceStatus) {

		if ((sortValue != null) && (instanceStatus != null)
				&& (instanceStatus != InstanceStatus.DISABLED)) {
			return sortValue;
		} else {
			return STRING_BOTTOM_SORT_VALUE;
		}
	}

	public static Long getSortValue(Long sortValue,
			InstanceStatus instanceStatus) {

		if ((sortValue != null) && (instanceStatus != null)) {

			switch (instanceStatus) {
			case DISABLED:
				return LONG_BOTTOM_SORT_VALUE;
			case CONNECTED:
			case SLOW:
				return sortValue;
			default:
				return LONG_ALMOST_BOTTOM_SORT_VALUE;
			}
		} else {
			return LONG_ALMOST_BOTTOM_SORT_VALUE;
		}
	}

	public static Integer getSortValue(Integer sortValue,
			InstanceStatus instanceStatus) {

		if ((sortValue != null) && (instanceStatus != null)) {

			switch (instanceStatus) {
			case DISABLED:
				return INTEGER_BOTTOM_SORT_VALUE;
			case CONNECTED:
			case SLOW:
				return sortValue;
			default:
				return INTEGER_ALMOST_BOTTOM_SORT_VALUE;
			}
		} else {
			return INTEGER_ALMOST_BOTTOM_SORT_VALUE;
		}
	}
	
	
	public static String getCategoryIcons(List<CategoriesEntry> list, String catName){
		if(list != null ){
			for(CategoriesEntry entry : list){
				if(catName.equalsIgnoreCase(entry.getName())){
					int severity = entry.getMaxSeverity();;
					if(severity == SeverityCodeToStringEnum.CRITICAL.getId()) {
					   return entry.getName()+SeverityCodeToStringEnum.CRITICAL.getCatIcon();
					} else if(severity == SeverityCodeToStringEnum.WARNING.getId()){
					   return entry.getName()+SeverityCodeToStringEnum.WARNING.getCatIcon();
					} else if(severity == SeverityCodeToStringEnum.INFORMATIONAL.getId()){
					   return entry.getName()+SeverityCodeToStringEnum.INFORMATIONAL.getCatIcon();
					} else {
					   return entry.getName()+SeverityCodeToStringEnum.OK.getCatIcon();
					}
				}
			}
		}
		return catName+SeverityCodeToStringEnum.OK.getCatIcon();
	}

	public static Integer getCategoryMaxSeverity(List<CategoriesEntry> list, String catName){
		if(list != null ){
			for(CategoriesEntry entry : list){
				if(catName.equalsIgnoreCase(entry.getName())){
					return entry.getMaxSeverity();
				}
			}
		}
		return 1;
	}

}
