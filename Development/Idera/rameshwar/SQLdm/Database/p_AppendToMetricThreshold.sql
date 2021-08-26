if (object_id('p_AppendToMetricThreshold') is not null)
begin
drop procedure p_AppendToMetricThreshold
end
go


CREATE PROCEDURE p_AppendToMetricThreshold
	@XmlText nvarchar(max),
	@Value nvarchar(256),
	@ValueType nvarchar(256),
	@Result nvarchar(4000) output
AS
BEGIN
declare @xmlDoc int,
		@OP nvarchar(256),
		@Enabled nvarchar(256),
		@valueArray nvarchar(4000),
		@serviceState nvarchar(256),
		@stateFound bit

		SET @OP = 'EQ'
		SET @Enabled = 'true'
		SET @valueArray = ''
		SET @stateFound = 0

		if (@XmlText is not null)
		begin
			exec sp_xml_preparedocument @xmlDoc output, @XmlText
	
			select @OP = [Op], @Enabled = [Enabled] from openxml(@xmlDoc, '/Threshold', 3) with (Op nvarchar(256), Enabled nvarchar(256))

			declare read_threshold_entry cursor
			for
				select servicestate from openxml(@xmlDoc, '//anyType', 2) with (servicestate nvarchar(256) 'text()')

			open read_threshold_entry
			fetch read_threshold_entry into @serviceState
			while @@fetch_status = 0
			begin
				if (@serviceState = @Value)
					set @stateFound = 1
				set @valueArray = @valueArray + '<anyType xsi:type="' + @ValueType + '">'
				set @valueArray = @valueArray + @serviceState
				set @valueArray = @valueArray + '</anyType>'

				fetch read_threshold_entry into @serviceState
			end
	
			Close read_threshold_entry
			deallocate read_threshold_entry
	
			exec sp_xml_removedocument @xmlDoc
		end

		-- We don't want to add a value that already exists
		if @stateFound <> 1
		begin
			set @valueArray = @valueArray + '<anyType xsi:type="' + @ValueType + '">'
			set @valueArray = @valueArray + @Value
			set @valueArray = @valueArray + '</anyType>'
		end

		set @Result = '<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XmlSchema" Op="'
		set @Result = @Result + @OP
		set @Result = @Result + '" Enabled="'
		set @Result = @Result + @Enabled
		set @Result = @Result + '"><Value xsi:type="ArrayOfAnyType">'
		set @Result = @Result + @valueArray
		set @Result = @Result + '</Value></Threshold>'
END