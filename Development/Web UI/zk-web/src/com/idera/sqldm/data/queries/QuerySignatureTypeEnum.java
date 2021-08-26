package com.idera.sqldm.data.queries;

public enum QuerySignatureTypeEnum {

	STORED_PROCEDURE("Stored Procedure", 0), SINGLE_STATEMENT("SQL Statement", 1), BATCH(
			"SQL Batch", 2);

	private String signatureType;
	private int index;

	private QuerySignatureTypeEnum(String signatureType, int index) {
		this.signatureType = signatureType;
		this.index = index;
	}

	public String getSignatureType() {
		return signatureType;
	}

	public void setSignatureType(String signatureType) {
		this.signatureType = signatureType;
	}

	public int getIndex() {
		return index;
	}

	public void setIndex(int index) {
		this.index = index;
	}

}
