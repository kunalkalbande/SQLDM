package com.idera.sqldm_10_3.data;

import util.LocalizedException;

import java.io.Serializable;

public class InstanceException extends LocalizedException {

	static final long serialVersionUID = 1L;

	public InstanceException() {
		super();
	}

	public InstanceException(String msgKey) {
		super(msgKey);
	}

	public InstanceException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}

	public InstanceException(Throwable throwable) {
		super(throwable);
	}

	public InstanceException(Throwable throwable, String msgKey) {
		super(throwable, msgKey);
	}

}
