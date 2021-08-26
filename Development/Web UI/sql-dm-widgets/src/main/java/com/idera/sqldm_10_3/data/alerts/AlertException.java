package com.idera.sqldm_10_3.data.alerts;

import com.idera.common.rest.RestException;

import java.io.Serializable;

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
