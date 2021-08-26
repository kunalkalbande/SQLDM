
/****** Object:  StoredProcedure [dbo].[p_GetRuleNameAvailable]    Script Date: 13-08-2015 18:41:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
if (object_id('p_GetRuleNameAvailable') is not null)
begin
drop procedure [p_GetRuleNameAvailable]
end
go

--EXEC p_GetRuleNameAvailable 'SQL diagnostic manager Service Status testing',0
CREATE PROCEDURE [dbo].[p_GetRuleNameAvailable](
	@RuleName nvarchar(255) =NULL,
	@Available bit output
)
AS
begin
	declare @err int
	declare @Usages int
	declare @NRuleName varchar(255)
	
	select @NRuleName=Convert(xml,SerializedObject).value('(/NotificationRule/@Description)[1]', 'varchar(500)') 
	from NotificationRules
	where LOWER(RTRIM(LTRIM(Convert(xml,SerializedObject).value('(/NotificationRule/@Description)[1]', 'varchar(500)'))))= LOWER(RTRIM(LTRIM(@RuleName)))

	
	if (@NRuleName is NOT NULL)
		SET @Available= 0
	else 
		SET @Available= 1
						
	select @err = @@error
	return @err
END	















GO


