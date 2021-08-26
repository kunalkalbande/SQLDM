package com.idera.sqldm.data.tags;

import java.io.Serializable;

import util.LocalizedException;

public class TagException extends LocalizedException {

	static final long serialVersionUID = 1L;

	public TagException() {
		super();
	}

	public TagException(String msgKey) {
		super(msgKey);
	}

	public TagException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}

	public TagException(Throwable throwable) {
		super(throwable);
	}

	public TagException(Throwable throwable, String msgKey) {
		super(throwable, msgKey);
	}

}
