package com.idera.sqldm.data;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

public enum SeverityCodeToStringEnum {
	NULL(-3, "null", "null", "", -1, "UnknownStatus", "Server", ""),
	LOADING(-2, "Loading", "Loading", "Loading", -1, "", "server-16x16",""),
	ALL(-1, "ALL", "ALL", "", 1, "", "", ""),
	CRITICAL(8, "CRITICAL", "CRITICAL", "Critical", 2, "Critical", "ServerCritical-16x16", "#ED1C24"),
	OK(1, "OK", "OK", "", 3, "OK", "ServerOK-16x16", "#22B14C"),
	WARNING(4, "WARNING", "WARNING", "Warning", 4, "Warning", "ServerWarning-16x16", "#FFC90E"),
	INFORMATIONAL(2, "MAINT. MODE", "MAINT. MODE", "Info", 4, "MaintenanceMode", "ServerMaint. Mode-16x16", "#0054A6");

	private int id;
	private String label;
	private String uiLabel;
	private String catIcon;
	private int order;
	private String styleName;
	private String serverCat16PixIcon;
	private String colorCode;
	
	private static Map<String, String> colorCodesMap;
	private static List<SeverityCodeToStringEnum> LeftNavigationStatusList;
	private static Map<Integer, SeverityCodeToStringEnum> intCodeToObjMap;

	SeverityCodeToStringEnum(int id, String label, String uiLabel, String catIcon, 
			int order, String styleName, String serverCat16PixIcon, String colorCode) {
		this.id = id;
		this.label = label;
		this.uiLabel = uiLabel;
		this.catIcon = catIcon;
		this.order = order;
		this.styleName = styleName;
		this.serverCat16PixIcon = serverCat16PixIcon;
		this.colorCode = colorCode;
	}

	private static synchronized Map<Integer, SeverityCodeToStringEnum> getMap() {
		if (intCodeToObjMap == null) {
			intCodeToObjMap = new HashMap<>();
			for (SeverityCodeToStringEnum sctse : SeverityCodeToStringEnum.values()) {
				intCodeToObjMap.put(sctse.getId(), sctse);
			}
		}
		
		return intCodeToObjMap;
	}

	public static synchronized SeverityCodeToStringEnum getSeverityEnumForId(int id) {
		return getMap().get(id);
	}

	public synchronized static List<SeverityCodeToStringEnum> getLeftNavigationStatusList() {
		if (LeftNavigationStatusList == null) {
			int count = 0;
			Map<Integer, SeverityCodeToStringEnum> map = new HashMap<Integer, SeverityCodeToStringEnum>();
			for (SeverityCodeToStringEnum sctse : SeverityCodeToStringEnum.values()) {
				if (sctse.getOrder() > 0) {
					map.put(count, sctse);
					count++;
				}
			}
			
			List<SeverityCodeToStringEnum> list = new LinkedList<SeverityCodeToStringEnum>();
			for (int i=0; i<count; i++) {
				if (map.get(i) == null) {
					throw new RuntimeException("SeverityCodeToStringEnum.class is not having right UI position values.");
				}
				
				list.add(map.get(i));
			}
			LeftNavigationStatusList = list;
		}
		return LeftNavigationStatusList;
	}

	public static Map<String, String> getColorCodesMap() {
		if(colorCodesMap == null) {
			colorCodesMap = new HashMap<>();
			for(SeverityCodeToStringEnum sctse: SeverityCodeToStringEnum.values()) {
				colorCodesMap.put(sctse.getLabel(), sctse.getColorCode());
			}
		}
		return colorCodesMap;
	}

	public static SeverityCodeToStringEnum getEnumFromLabel(String label) {
		for (SeverityCodeToStringEnum sctse: getMap().values()) {
			if (label.equalsIgnoreCase(sctse.getLabel())) {
				return sctse;
			}
		}
		return null;
	}
	
	public int getId() {
		return id;
	}

	public String getLabel() {
		return label;
	}

	public String getUiLabel() {
		return uiLabel;
	}

	public String getCatIcon() {
		return catIcon;
	}

	public void setCatIcon(String catIcon) {
		this.catIcon = catIcon;
	}

	public int getOrder() {
		return order;
	}

	public String getStyleName() {
		return styleName;
	}

	public String getServerCat16PixIcon() {
		return serverCat16PixIcon;
	}

	public String getColorCode() {
		return colorCode;
	}
}
