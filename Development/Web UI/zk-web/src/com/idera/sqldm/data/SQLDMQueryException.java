package com.idera.sqldm.data;

import java.io.Serializable;

import util.LocalizedException;

public class SQLDMQueryException extends LocalizedException {

	static final long serialVersionUID = 1L;

	public SQLDMQueryException() {
		super();
	}

	public SQLDMQueryException(String msgKey) {
		super(msgKey);
	}

	public SQLDMQueryException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}

	public SQLDMQueryException(Throwable throwable) {
		super(throwable);
	}

	public SQLDMQueryException(Throwable throwable, String msgKey) {
		super(throwable, msgKey);
	}

}
