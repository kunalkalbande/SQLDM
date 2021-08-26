package com.idera.sqldm.data;

import java.io.Serializable;

import util.LocalizedException;

public class SQLDMInstanceException extends LocalizedException{

static final long serialVersionUID = 1L;
	
	public SQLDMInstanceException() {
		super();
	}
	
	public SQLDMInstanceException(String msg) {
		super(msg);
	}
	
	public SQLDMInstanceException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}
	
	public SQLDMInstanceException(Throwable t, String msgKey) {
		super(t, msgKey);
	}
	
	public SQLDMInstanceException(Throwable t, String msgKey, Serializable... varargs) {
		super(t, msgKey, varargs);
	}	
	
	public SQLDMInstanceException(Throwable t) {
		super(t);
	}	
	
}
