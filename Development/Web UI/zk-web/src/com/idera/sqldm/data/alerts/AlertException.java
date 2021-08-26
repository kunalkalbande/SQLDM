package com.idera.sqldm.data.alerts;

import java.io.Serializable;

import com.idera.common.rest.RestException;

public class AlertException extends RestException {

	static final long serialVersionUID = 1L;

	public AlertException() {
		super();
	}

	public AlertException(String msgKey) {
		super(msgKey);
	}

	public AlertException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}

	public AlertException(Throwable throwable) {
		super(throwable);
	}

	public AlertException(Throwable throwable, String msgKey) {
		super(throwable, msgKey);
	}

}
