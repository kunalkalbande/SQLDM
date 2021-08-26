package com.idera.sqldm.data;

import java.util.Arrays;
import java.util.List;

import org.apache.commons.lang.builder.EqualsBuilder;

import com.idera.i18n.I18NStrings;

public class InstanceCredentials {

	public static enum AccountType {
		USE_SQL_ELEMENTS_SERVICE_ACCOUNT(
				I18NStrings.SQL_ELEMENTS_SERVICE_ACCOUNT), WINDOWS_USER_ACCOUNT(
				I18NStrings.WINDOWS_USER_ACCOUNT), SQL_SERVER_USER_ACCOUNT(
				I18NStrings.SQL_SERVER_LOGIN_ACCOUNT);

		private String i18nKey;

		AccountType(String i18nKey) {
			this.i18nKey = i18nKey;
		}

		public String getI18nKey() {
			return i18nKey;
		}

		public static List<AccountType> getSqlAccountTypes() {
			return Arrays.asList(AccountType.values());
		}

		public static List<AccountType> getWmiAccountTypes() {
			return Arrays.asList(USE_SQL_ELEMENTS_SERVICE_ACCOUNT,
					WINDOWS_USER_ACCOUNT);
		}
	}

	private AccountType sqlAccountType;

	public AccountType getSqlAccountType() {
		return sqlAccountType;
	}

	public void setSqlAccountType(AccountType sqlAccountType) {
		this.sqlAccountType = sqlAccountType;
	}

	private AccountType wmiAccountType;

	public AccountType getWmiAccountType() {
		return wmiAccountType;
	}

	public void setWmiAccountType(AccountType wmiAccountType) {
		this.wmiAccountType = wmiAccountType;
	}

	private String sqlUsername;

	public String getSqlUsername() {
		return sqlUsername;
	}

	public void setSqlUsername(String sqlUsername) {
		this.sqlUsername = sqlUsername;
	}

	private String sqlPassword;

	public String getSqlPassword() {
		return sqlPassword;
	}

	public void setSqlPassword(String sqlPassword) {
		this.sqlPassword = sqlPassword;
	}

	private String wmiUsername;

	public String getWmiUsername() {
		return wmiUsername;
	}

	public void setWmiUsername(String wmiUsername) {
		this.wmiUsername = wmiUsername;
	}

	private String wmiPassword;

	public String getWmiPassword() {
		return wmiPassword;
	}

	public void setWmiPassword(String wmiPassword) {
		this.wmiPassword = wmiPassword;
	}

	public InstanceCredentials() {

		setSqlAccountType(AccountType.USE_SQL_ELEMENTS_SERVICE_ACCOUNT);
		setWmiAccountType(AccountType.USE_SQL_ELEMENTS_SERVICE_ACCOUNT);
	}

	public InstanceCredentials(AccountType sqlAccountType, String sqlUsername,
			String sqlPassword, AccountType wmiAccountType, String wmiUsername,
			String wmiPassword) {

		setSqlAccountType(sqlAccountType);
		setSqlUsername(sqlUsername != null ? sqlUsername : "");
		setSqlPassword(sqlPassword != null ? sqlPassword : "");
		setWmiAccountType(wmiAccountType);
		setWmiUsername(wmiUsername != null ? wmiUsername : "");
		setWmiPassword(wmiPassword != null ? wmiPassword : "");
	}

	@Override
	public boolean equals(Object other) {

		if ((other == null) || !(other instanceof InstanceCredentials)) {
			return false;
		}

		if (other == this) {
			return true;
		}

		InstanceCredentials otherCredentials = (InstanceCredentials) other;

		return new EqualsBuilder()
				.append(this.getSqlAccountType(),
						otherCredentials.getSqlAccountType())
				.append(this.getSqlUsername(),
						otherCredentials.getSqlUsername())
				.append(this.getSqlPassword(),
						otherCredentials.getSqlPassword())
				.append(this.getWmiAccountType(),
						otherCredentials.getWmiAccountType())
				.append(this.getWmiUsername(),
						otherCredentials.getWmiUsername())
				.append(this.getWmiPassword(),
						otherCredentials.getWmiPassword()).isEquals();
	}

}
