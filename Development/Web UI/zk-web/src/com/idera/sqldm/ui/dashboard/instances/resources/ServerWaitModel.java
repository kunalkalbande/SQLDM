package com.idera.sqldm.ui.dashboard.instances.resources;

import com.idera.sqldm.utils.Utility;

public class ServerWaitModel implements Comparable<ServerWaitModel>{
	
	private String category;
	private String waitType;
	private Double waitingTasks;
	private Double waitingTime;
	private String description;
	private String help;
	private double totalWait;
	
	public ServerWaitModel(){
		
	}
	
	public ServerWaitModel(String category, String waitType){
		this.category = category;
		this.waitType = waitType;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public String getWaitType() {
		return waitType;
	}

	public void setWaitType(String waitType) {
		this.waitType = waitType;
	}
	
	public Double getWaitingTasks() {
		return Utility.round(waitingTasks, 2);
	}

	public void setWaitingTasks(Double waitingTasks) {
		this.waitingTasks = waitingTasks;
	}

	public Double getWaitingTime() {
		return Utility.round(waitingTime, 2);
	}

	public void setWaitingTime(Double waitingTime) {
		this.waitingTime = waitingTime;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public String getHelp() {
		return help;
	}

	public void setHelp(String help) {
		this.help = help;
	}

	public double getTotalWait() {
		return totalWait;
	}

	public void setTotalWait(double totalWait) {
		this.totalWait = totalWait;
	}

	@Override
	public boolean equals(Object obj) {
		if (obj == this) return true;
		if (obj == null || (obj.getClass() != this.getClass())) return false;
		 
		ServerWaitModel other = (ServerWaitModel) obj;
		 //StringUtils.
		return (category == other.category 
                || (category != null && category.equals(other.getCategory())))
           && (waitType == other.waitType 
                   || (waitType != null && waitType.equals(other.getWaitType())));
   }
	
	@Override
	public int hashCode() {
		final int prime = 31;
        int result = 1;
        result = prime * result
                + ((category == null) ? 0 : category.hashCode());
        result = prime * result
                + ((waitType == null) ? 0 : waitType.hashCode());
        return result;
	}

	@Override
	public int compareTo(ServerWaitModel o) {
		if(this.category.equals(o.category)){
			return this.waitType.compareTo(o.waitType);
		}
		return this.category.compareTo(o.category);
	}
	

}
