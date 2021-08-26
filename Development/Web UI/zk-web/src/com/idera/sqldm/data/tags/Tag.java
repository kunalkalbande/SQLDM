package com.idera.sqldm.data.tags;


import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Tag {
	@JsonProperty("Id") 
	private int Id;

	public int getId() {
		return Id;
	}
	public int getMaxSeverity() {
		return MaxSeverity;
	}
	@JsonProperty("Name") 
	private String Name;
	@JsonProperty("MaxSeverity") 
	private int MaxSeverity;
	@JsonProperty("count") 
	int count;
	@JsonProperty("Instances")
	List<Integer> instances;
	

	public enum AlertSeverity {
		OK(1),
		INFORMATIONAL(2),
		WARNING(4),
		CRITICAL(8);
		private int id;
		AlertSeverity(int id){
			this.id = id;
		}
		
		public int getId(){
			return id;
		}
	}

	
	public String getName() {
		return Name;
	}
	public String getState() {
		
		switch(MaxSeverity) {
			case 1: return "Ok";
			case 2: return "Informational";
			case 4: return "Warning";
			case 8: return "Critical";
			default: return "Ok";
			
		}
	}
	public int getCount() {
		if(instances == null) {
			return 0;
		}
		//System.out.println("Count Is: " + this.instances.size());
		return this.instances.size();
		
	}
	/*public Tag(int id, String name, int state, 
			int count) {
		super();
		this.Id = id;
		this.Name = name;
		this.MaxSeverity = state;
		this.count = count;
	}*/
	public void setId(int id) {
		Id = id;
	}
	public void setName(String name) {
		Name = name;
	}
	public void setMaxSeverity(int maxSeverity) {
		MaxSeverity = maxSeverity;
	}
	public void setCount(int count) {
		this.count = count;
	}
	
}
