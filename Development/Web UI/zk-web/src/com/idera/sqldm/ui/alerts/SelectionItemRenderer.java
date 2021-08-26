package com.idera.sqldm.ui.alerts;

import org.zkoss.zul.Comboitem;
import org.zkoss.zul.ComboitemRenderer;

public class SelectionItemRenderer implements ComboitemRenderer{
	@SuppressWarnings({ "unchecked", "static-access" })
    @Override
    public void render(Comboitem combItem, Object obj, int index)
            throws Exception {
 
        Enum<?> enumObj = (Enum) obj;
        combItem.setLabel(enumObj.name());
        combItem.setValue(enumObj.valueOf(enumObj.getClass(), enumObj.name()));
    }
}
