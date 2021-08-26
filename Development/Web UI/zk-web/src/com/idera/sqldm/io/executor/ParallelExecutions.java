package com.idera.sqldm.io.executor;

import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Set;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

import org.apache.log4j.Logger;
import org.springframework.security.context.SecurityContext;
import org.springframework.security.context.SecurityContextHolder;

import com.idera.common.UIWorkerThread;
import com.idera.sqldm.io.executor.ParallelExecutionsPDU.PDUBean;
import com.idera.sqldm.io.executor.ParallelExecutionsPDU.PDUBeanCallable;

public class ParallelExecutions {

	private Set<Callable<PDUBean>> callables;
	
	public ParallelExecutions () {
		callables = new HashSet<Callable<PDUBean>>();
	}
	
	/*
	 * Dt : 26/08/2016
	 * Commented the call to PDUBean and added a call to PDUBeanCallable which adds security context for each new Thread
	 */
	public void addToCallable(final ParallelExecutionsPDU pePDU) {
		
		/*callables.add(new Callable<PDUBean>() {
			public PDUBean call() throws Exception {
				return pePDU.task();
			}
		}*/
		
		callables.add(new PDUBeanCallable(pePDU));
	}
	
	
	/*public void invokeAll() throws Exception {
		List<Future<PDUBean>> futures = ParallelExecutorService.getExecutorService().getExecutor().invokeAll(callables);
		for(Future<PDUBean> future : futures) {
			PDUBean pduBean = future.get();
			pduBean.getPePDU().taskComplete(pduBean);
		}
	}*/

	private static class DeferExecutor extends UIWorkerThread {
		private final Logger log = Logger.getLogger(DeferExecutor.class);
		ParallelExecutions pe;

		public DeferExecutor(ParallelExecutions pe) {
			this.pe = pe;
		}

		@Override
		public void run() {
			try {
				pe.processThreads();
			} catch (Exception e) {
				log.error(e.getMessage(), e);
			}
		}
	}
	public void invokeAll() throws Exception {
		UIWorkerThread t = new DeferExecutor(this);
		SecurityContext sc = SecurityContextHolder.getContext();
		com.idera.sqldm.server.web.session.SessionUtil.setSessionVariable("sc", sc);
		t.setSecurityContext(SecurityContextHolder.getContext());
		t.start();
	}
	public void processThreads() throws Exception {
		List<Future<PDUBean>> futures = new LinkedList<Future<PDUBean>>();
		for(Callable<PDUBean>callable : callables) {
			futures.add(ParallelExecutorService.getExecutorService().getExecutor().submit(callable));
		}
		for(Future<PDUBean> future : futures) {
			PDUBean pduBean = future.get();
			if (pduBean != null && pduBean.getPePDU() != null) {
				pduBean.getPePDU().taskComplete(pduBean);				
			}
		}		
	}
	private static class ParallelExecutorService {
		private ExecutorService executorService = Executors.newCachedThreadPool();
		private static ParallelExecutorService pes = new ParallelExecutorService(); 
		private ParallelExecutorService() {
		}
		
		public static ParallelExecutorService getExecutorService() {
			return pes;
		}
		
		public ExecutorService getExecutor() {
			return executorService;
		}
		
		@Override
		protected void finalize() throws Throwable {
			super.finalize();
			getExecutor().shutdown();
		}		
	}
}