package com.idera.sqldm.ui.alerts;

import java.util.Iterator;
import java.util.List;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;
import org.zkoss.zul.Combobox;

public class CustomComboBoxConvertor implements Converter {
 
    @SuppressWarnings({ "unchecked" })
  public Object coerceToUi(Object val, Component comp, BindContext ctx) {
     Combobox box;
       box = (Combobox) comp;
      if (box.getSelectedItem() == null) {
           String instanceName = null;
           if (val != null) {
              List<CustomComboBoxModel> locationList = (List<CustomComboBoxModel>) box
                      .getModel();
                for (Iterator<CustomComboBoxModel> i = locationList.iterator(); i
                      .hasNext();) {
                	CustomComboBoxModel tmp = i.next();
                  if (tmp.getId() == (Integer) val) {
                	  instanceName = tmp.getValue();
                   }
               }
           }
           return instanceName;
      } else
          return box.getSelectedItem().getLabel();
    }
 
   public Object coerceToBean(Object val, Component comp, BindContext ctx) {
 
       Combobox box;
       box = (Combobox) comp;
 
      if (box.getSelectedItem() == null)
          return val;
      else{
          return box.getSelectedItem().getValue();
     }
 
    }
}