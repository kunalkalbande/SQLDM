package com.idera.sqldm.data;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonInclude.Include;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.InstanceStatus;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.common.rest.EncryptedStringDeserializer;
import com.idera.common.rest.EncryptedStringSerializer;
import com.idera.core.tags.Tag;
import com.idera.sqldm.data.InstanceCredentials.AccountType;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SQLDMInstance {

	@JsonProperty("ID")
	@JsonInclude(Include.NON_DEFAULT)
	private int id;

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	@JsonProperty("SQLServerName")
	@JsonInclude(Include.NON_EMPTY)
	private String sqlServerName;

	public String getSqlServerName() {
		return sqlServerName;
	}

	public void setSqlServerName(String sqlServerName) {
		this.sqlServerName = sqlServerName;
	}

	@JsonProperty("IntegratedWindowsImpersonation")
	@JsonInclude(Include.NON_NULL)
	private Boolean integratedWindowsImpersonation = false;

	public Boolean getIntegratedWindowsImpersonation() {
		
		return integratedWindowsImpersonation;
	}

	public void setIntegratedWindowsImpersonation(
			Boolean integratedWindowsImpersonation) {
		this.integratedWindowsImpersonation = integratedWindowsImpersonation;
	}

	@JsonProperty("IntegratedSecurity")
	@JsonInclude(Include.NON_NULL)
	private Boolean integratedSecurity = false;

	public Boolean getIntegratedSecurity() {
		
		return integratedSecurity;
	}

	public void setIntegratedSecurity(Boolean integratedSecurity) {
		this.integratedSecurity = integratedSecurity;
	}

	@JsonProperty("SQLUsername")
	@JsonInclude(Include.NON_NULL)
	private String sqlUsername;

	public String getSqlUsername() {
		return sqlUsername;
	}

	public void setSqlUsername(String sqlUsername) {
		this.sqlUsername = sqlUsername;
	}

	@JsonProperty("SQLPassword")
	@JsonDeserialize(using = EncryptedStringDeserializer.class)
	@JsonSerialize(using = EncryptedStringSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private String sqlPassword;

	public String getSqlPassword() {
		return sqlPassword;
	}

	public void setSqlPassword(String sqlPassword) {
		this.sqlPassword = sqlPassword;
	}

	@JsonProperty("WMIUsername")
	@JsonInclude(Include.NON_NULL)
	private String wmiUsername;

	public String getWmiUsername() {
		return wmiUsername;
	}

	public void setWmiUsername(String wmiUsername) {
		this.wmiUsername = wmiUsername;
	}

	@JsonProperty("WMIPassword")
	@JsonDeserialize(using = EncryptedStringDeserializer.class)
	@JsonSerialize(using = EncryptedStringSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private String wmiPassword;

	public String getWmiPassword() {
		return wmiPassword;
	}

	public void setWmiPassword(String wmiPassword) {
		this.wmiPassword = wmiPassword;
	}

	@JsonProperty("RegisteredDate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private Date registeredDate;

	public Date getRegisteredDate() {
		return registeredDate;
	}

	public void setRegisteredDate(Date registeredDate) {
		this.registeredDate = registeredDate;
	}

	@JsonProperty("LastUpdate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private Date lastUpdate;

	public Date getLastUpdate() {
		return lastUpdate;
	}

	public void setLastUpdate(Date lastUpdate) {
		this.lastUpdate = lastUpdate;
	}

	@JsonProperty("FirstDiscovered")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private Date firstDiscovered;

	public Date getFirstDiscovered() {
		return firstDiscovered;
	}

	public void setFirstDiscovered(Date firstDiscovered) {
		this.firstDiscovered = firstDiscovered;
	}

	@JsonProperty("LastDiscovered")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private Date lastDiscovered;

	public Date getLastDiscovered() {
		return lastDiscovered;
	}

	public void setLastDiscovered(Date lastDiscovered) {
		this.lastDiscovered = lastDiscovered;
	}

	@JsonProperty("UseOleForWmi")
	@JsonInclude(Include.NON_DEFAULT)
	private boolean useOleForWmi;

	public boolean getUseOleForWmi() {
		return useOleForWmi;
	}

	@JsonProperty("SqlVersion")
	@JsonInclude(Include.NON_EMPTY)
	private String sqlVersion;

	public String getSqlVersion() {
		return sqlVersion;
	}

	public void setSqlVersion(String sqlVersion) {
		this.sqlVersion = sqlVersion;
	}

	@JsonProperty("SqlEdition")
	@JsonInclude(Include.NON_EMPTY)
	private String sqlEdition;

	public String getSqlEdition() {
		return sqlEdition;
	}

	public void setSqlEdition(String sqlEdition) {
		this.sqlEdition = sqlEdition;
	}

	@JsonProperty("ActiveHostID")
	@JsonInclude(Include.NON_DEFAULT)
	private int activeHostId;

	public int getActiveHostId() {
		return activeHostId;
	}

	public void setActiveHostId(int activeHostId) {
		this.activeHostId = activeHostId;
	}

	@JsonProperty("Enabled")
	private boolean enabled;

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	@JsonProperty("Owner")
	@JsonInclude(Include.NON_NULL)
	private String owner;

	public String getOwner() {
		return owner;
	}

	public void setOwner(String owner) {
		this.owner = owner;
	}

	@JsonProperty("Location")
	@JsonInclude(Include.NON_NULL)
	private String location;

	public String getLocation() {
		return location;
	}

	public void setLocation(String location) {
		this.location = location;
	}

	@JsonProperty("Comments")
	@JsonInclude(Include.NON_NULL)
	private String comments;

	public String getComments() {
		return comments;
	}

	public void setComments(String comments) {
		this.comments = comments;
	}

	@JsonProperty("Port")
	private int port;

	public int getPort() {
		return port;
	}

	@JsonProperty("Availability")
	@JsonInclude(Include.NON_DEFAULT)
	private int availability;

	public int getAvailability() {
		return availability;
	}

	public void setAvailability(int availability) {
		this.availability = availability;
	}

	@JsonProperty("AvailabilityCode")
	@JsonInclude(Include.NON_DEFAULT)
	private int availabilityCode;

	public int getAvailabilityCode() {
		return availabilityCode;
	}

	public void setAvailabilityCode(int availabilityCode) {
		this.availabilityCode = availabilityCode;
	}

	@JsonProperty("AvailabilityDate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonInclude(Include.NON_NULL)
	private Date availabilityDate;

	public Date getAvailabilityDate() {
		return availabilityDate;
	}

	public void setAvailabilityDate(Date availabilityDate) {
		this.availabilityDate = availabilityDate;
	}

	@JsonProperty("ResponseTime")
	@JsonInclude(Include.NON_DEFAULT)
	private long responseTime;

	public long getResponseTime() {
		return responseTime;
	}

	public void setResponseTime(long responseTime) {
		this.responseTime = responseTime;
	}

	@JsonProperty("Tags")
	@JsonInclude(Include.NON_NULL)
	private List<Tag> tags = new ArrayList<Tag>();

	public List<Tag> getTags() {
		return tags;
	}

	public void setTags(List<Tag> tags) {
		this.tags = tags;
	}

	@JsonProperty("DisplaySqlVersion")
	@JsonInclude(Include.NON_NULL)
	private String dispSqlVersion;

	public String getDispSqlVersion() {
		return dispSqlVersion;
	}

	@JsonProperty("SkipValidation")
	private boolean skipValidation;

	public boolean getSkipValidation() {
		return skipValidation;
	}

	public void setSkipValidation(boolean skipValidation) {
		this.skipValidation = skipValidation;
	}
	
	@JsonProperty("InstanceState")
	private int instanceState;
	
	public int getInstanceState()
	{
		return instanceState;
	}
	
	@JsonIgnore
	public boolean isMonitored() {
		return getRegisteredDate() != null;
	}

	@JsonIgnore
	public boolean isInstanceUp() {
		return (isMonitored() && isEnabled() && getAvailability() == 1);
	}

	@JsonIgnore
	public InstanceStatus getStatus() {
		
		return InstanceStatus.getInstanceStatus(isMonitored(), isEnabled(), getInstanceState());		
	}

	@JsonIgnore
	public InstanceCredentials getInstanceCredentials() {
		InstanceCredentials instanceCredentials = new InstanceCredentials(
				getSqlAccountType(), sqlUsername, sqlPassword,
				getWmiAccountType(), wmiUsername, wmiPassword);

		return instanceCredentials;
	}

	public void setInstanceCredentials(InstanceCredentials instanceCredentials) {

		if (AccountType.WINDOWS_USER_ACCOUNT.equals(instanceCredentials
				.getSqlAccountType())) {

			setIntegratedWindowsImpersonation(true);
			setIntegratedSecurity(false);

			setSqlUsername(instanceCredentials.getSqlUsername());
			setSqlPassword(instanceCredentials.getSqlPassword());

		} else if (AccountType.SQL_SERVER_USER_ACCOUNT
				.equals(instanceCredentials.getSqlAccountType())) {

			setIntegratedWindowsImpersonation(false);
			setIntegratedSecurity(false);

			setSqlUsername(instanceCredentials.getSqlUsername());
			setSqlPassword(instanceCredentials.getSqlPassword());

		} else {

			setIntegratedWindowsImpersonation(false);
			setIntegratedSecurity(true);

			setSqlUsername("");
			setSqlPassword("");
		}

		if (AccountType.WINDOWS_USER_ACCOUNT.equals(instanceCredentials
				.getWmiAccountType())) {

			setWmiUsername(instanceCredentials.getWmiUsername());
			setWmiPassword(instanceCredentials.getWmiPassword());

		} else {

			setWmiUsername("");
			setWmiPassword("");
		}
	}

	private AccountType getSqlAccountType() {

		if ((getIntegratedWindowsImpersonation() != null) && (getIntegratedWindowsImpersonation() == true)) {

			return AccountType.WINDOWS_USER_ACCOUNT;

		} else if ((getIntegratedSecurity() != null) && (getIntegratedSecurity() == true)) {

			return AccountType.USE_SQL_ELEMENTS_SERVICE_ACCOUNT;

		} else {

			return AccountType.SQL_SERVER_USER_ACCOUNT;
		}
	}

	private AccountType getWmiAccountType() {

		if ((getWmiUsername() != null) && (!getWmiUsername().isEmpty())
				&& (getWmiPassword() != null) && (!getWmiPassword().isEmpty())) {

			return AccountType.WINDOWS_USER_ACCOUNT;

		} else {

			return AccountType.USE_SQL_ELEMENTS_SERVICE_ACCOUNT;
		}
	}
	
	@JsonIgnore
	public String getDisplayVersionSortValue()
	{
		if ((getDispSqlVersion() != null) && (!getDispSqlVersion().isEmpty()))
		{
			return getDispSqlVersion();
		}
		else
		{
			return "ZZZ";
		}
	}

}
