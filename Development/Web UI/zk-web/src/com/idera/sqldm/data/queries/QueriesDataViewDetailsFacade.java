package com.idera.sqldm.data.queries;


public class QueriesDataViewDetailsFacade {
private double waitDuration ;
private int totalQueries;
private  int duration;
private int reads;
private int writes;
private int totalIO;
private int waitTime;
private int blockingTime;
public double getWaitDuration() {
	return waitDuration;
}
public int getTotalQueries() {
	return totalQueries;
}
public int getDuration() {
	return duration;
}
public int getReads() {
	return reads;
}
public int getWrites() {
	return writes;
}
public int getTotalIO() {
	return totalIO;
}
public int getWaitTime() {
	return waitTime;
}
public int getBlockingTime() {
	return blockingTime;
}

}
