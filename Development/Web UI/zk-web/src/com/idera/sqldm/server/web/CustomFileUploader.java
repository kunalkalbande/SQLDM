package com.idera.sqldm.server.web;

import org.apache.commons.fileupload.FileUploadBase.SizeLimitExceededException;
import org.zkoss.zk.au.http.AuUploader;
import util.NonLocalizedByteSizeFormatter;
import com.idera.i18n.I18NStrings;

public class CustomFileUploader extends AuUploader {
	protected String handleError(Throwable ex) {
		if(ex instanceof SizeLimitExceededException) {
			SizeLimitExceededException e = (SizeLimitExceededException) ex;
			return ELFunctions.getLabelWithParams(
					I18NStrings.MAX_CSV_FILE_UPLOAD_SIZE, 
					NonLocalizedByteSizeFormatter.getFriendlyString(e.getPermittedSize()), 
					NonLocalizedByteSizeFormatter.getFriendlyString(e.getActualSize()));
		}
		return super.handleError(ex);
	} 
}