package com.idera.sqldm_10_3.server.web;

import java.util.List;

public class ColorGradient {
	int minRange;
	int maxRange;
	String minColorRange;
	String maxColorRange;
	List<ColorScale> colorRange;

	public String getColorAt(Double index) {
		String color = "";
		try {
			color =  computeColor(index);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return color;
	}

	public void setSpectrum(List<ColorScale> colorRange2) {
		colorRange = colorRange2;
	}

	
	private String computeColor(Double indx) throws Exception {
		
			if(colorRange == null)
				throw new Exception("Color spectrum not set!");
			
		
		for(int i=0; i<colorRange.size()-1; i++){
			int min = colorRange.get(i).getRange();
			int max =colorRange.get(i+1).getRange();
			if(indx >=min && indx < max){
				minColorRange = colorRange.get(i).getColor();
				maxColorRange = colorRange.get(i+1).getColor();
				minRange = min;
				maxRange = max; break;
			}
		}
		
		return func(indx, minColorRange.substring(0, 2), maxColorRange.substring(0, 2)) 
				+ func(indx, minColorRange.substring(2, 4), maxColorRange.substring(2, 4))
				+ func(indx, minColorRange.substring(4, 6), maxColorRange.substring(4, 6));
	}
	
	private String func(Double indx, String substring, String substring2) {
		Double num = indx;
		int min = minRange;
		int max = maxRange;
		if(num < min) num = Double.valueOf(min);
		if(num > max) num = Double.valueOf(max);

		double s =max - min;

		int n = Integer.parseInt(substring, 16);
		int k = Integer.parseInt(substring2, 16);

		double u = (k-n)/s;
		long o = Math.round(u*(num-min) + n);
		String Color = (Long.toString(o, 16));
		if(Color.length() == 1)
			return "0" + Color;
		else 
			return Color;
	}
}
