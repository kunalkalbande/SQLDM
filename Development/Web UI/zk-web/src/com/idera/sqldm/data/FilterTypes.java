package com.idera.sqldm.data;

import com.idera.i18n.I18NStrings;

public enum FilterTypes {
	INSTANCES(I18NStrings.INSTANCES, "instance-icon"), 
	TAGS(I18NStrings.TAGS, "tag-icon"), 
	LOCATIONS(I18NStrings.LOCATIONS, "location-icon"), 
	OWNERS(I18NStrings.OWNERS, "user-icon"), 
	VERSIONS(I18NStrings.SERVER_VERSIONS, "version-icon");

	String i18nKey;

	private String iconUrl;

	FilterTypes(String i18nKey, String iconUrl) {
		this.i18nKey = i18nKey;
		this.iconUrl = iconUrl;
	}

	public String getI18nKey() {
		return i18nKey;
	}

	public String getIconUrl() {
		return iconUrl;
	}

	public static FilterTypes fromString(String value) {
		for (FilterTypes tmp : FilterTypes.values()) {
			if (tmp.toString().equals(value))
				return tmp;
		}
		return null;
	}
}