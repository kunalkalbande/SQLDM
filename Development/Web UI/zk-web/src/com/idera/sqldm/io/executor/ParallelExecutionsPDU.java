package com.idera.sqldm.io.executor;

import java.util.concurrent.Callable;

import org.springframework.security.context.SecurityContext;
import org.springframework.security.context.SecurityContextHolder;


public abstract class ParallelExecutionsPDU  {
	
	private PDUBean pduBean;
	public ParallelExecutionsPDU() {
	}
	
	public PDUBean getPduBean() {
		return pduBean;
	}

	public void setPduBean(PDUBean pduBean) {
		this.pduBean = pduBean;
	}

	public abstract PDUBean task();
	public abstract void taskComplete(PDUBean returnObject);

	public class PDUBean {
		private Object response;
		private ParallelExecutionsPDU pePDU;
		
		public PDUBean(Object response, ParallelExecutionsPDU pePDU) {
			super();
			this.response = response;
			this.pePDU = pePDU;
		}
		
		public Object getResponse() {
			return response;
		}
		
		public ParallelExecutionsPDU getPePDU() {
			return pePDU;
		}
	}
	
		/**
		 * @author Accolite
		 *
		 * PDUBeanCallable class implementing callable interface creates threads for executing parallel API calls
		 */
	    public static class PDUBeanCallable implements Callable<PDUBean>{
			private SecurityContext context;
			private ParallelExecutionsPDU pePDU;
			
			/**
			 * @param pe
			 */
			public PDUBeanCallable(ParallelExecutionsPDU pe) {
				this.context = SecurityContextHolder.getContext();
				this.pePDU=pe;
			}
			
			/**
			 * @return SecurityContext
			 */
			public SecurityContext getSecurityContext() {
		        return this.context;
		    }
	
		    /**
		     * @param context
		     */
		    public void setSecurityContext(SecurityContext context) {
		        this.context = context;
		    }
	
			/* 
			 *Setting Security Context to the main thread 
			 */
			@Override
			public PDUBean call() throws Exception {
				SecurityContextHolder.setContext(context);
				return pePDU.task();
			}
		   
		}
}