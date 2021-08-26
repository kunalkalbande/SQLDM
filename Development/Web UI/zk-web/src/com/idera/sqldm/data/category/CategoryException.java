package com.idera.sqldm.data.category;

import java.io.Serializable;

import util.LocalizedException;

public class CategoryException extends LocalizedException {

	static final long serialVersionUID = 1L;

	public CategoryException() {
		super();
	}

	public CategoryException(String msgKey) {
		super(msgKey);
	}

	public CategoryException(String msgKey, Serializable... varargs) {
		super(msgKey, varargs);
	}

	public CategoryException(Throwable throwable) {
		super(throwable);
	}

	public CategoryException(Throwable throwable, String msgKey) {
		super(throwable, msgKey);
	}
}