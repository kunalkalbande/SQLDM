package com.idera.sqldm.ui.error;

import org.apache.commons.lang.exception.ExceptionUtils;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;
import org.zkoss.zul.Window;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.widgets.ModalDialogTemplate;

public class ErrorComposer extends ModalDialogTemplate {

	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(ErrorComposer.class);

	private static final long serialVersionUID = 1L;

	private static final String MINIMUM_HEIGHT = "300px";

	private static final String MAXIMUM_HEIGHT = "700px";

	@Wire
	private Label stackTraceLabel;

	@Wire
	private Button detailsButton;

	@Wire
	private Div detailsSection;

	@Wire
	protected Window errorDialogWindow;

	public void doAfterCompose(Window component) throws Exception {

		super.doAfterCompose(component);

		Exception exception = (Exception) Executions.getCurrent().getAttribute(
				"javax.servlet.error.exception");

		if (exception != null) {

			logger.error(exception,
					I18NStrings.EXCEPTION_CAUGHT_BY_DEFAULT_ERROR_PAGE);
			stackTraceLabel.setValue(ExceptionUtils.getStackTrace(exception));
		}

		errorDialogWindow.setHeight("300px");
	}

	@Listen("onClick = button#detailsButton")
	public void showOrHideDetails() {

		detailsSection.setVisible(!detailsSection.isVisible());

		detailsButton
				.setLabel(ELFunctions.getLabel(detailsSection.isVisible() ? I18NStrings.HIDE_DETAILS
						: I18NStrings.SHOW_DETAILS));

		if (detailsSection.isVisible()) {
			errorDialogWindow.setHeight(MAXIMUM_HEIGHT);
		} else {
			errorDialogWindow.setHeight(MINIMUM_HEIGHT);
		}

	}

}
