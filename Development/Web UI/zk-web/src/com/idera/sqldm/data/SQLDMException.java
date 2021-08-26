package com.idera.sqldm.data;

import java.io.Serializable;

import util.LocalizedException;

public class SQLDMException extends LocalizedException {

	static final long serialVersionUID = 1L;

	public SQLDMException() {
		super();
	}

	public SQLDMException(String msgKey) {
		super(msgKey);
	}

	public SQLDMException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}

	public SQLDMException(Throwable throwable) {
		super(throwable);
	}

	public SQLDMException(Throwable throwable, String msgKey) {
		super(throwable, msgKey);
	}

}
