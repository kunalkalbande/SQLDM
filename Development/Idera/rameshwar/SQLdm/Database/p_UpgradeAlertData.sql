if (object_id('p_UpgradeAlertData') is not null)
begin
drop procedure p_UpgradeAlertData
end
go

create procedure p_UpgradeAlertData
as
begin


	declare @TimeOut datetime,
			@LeftToDo bigint,
			@BeginDate datetime,
			@EndDate datetime
				

	set @TimeOut = DATEADD(ss, 180, GETDATE())
			
	if (select count([Internal_Value]) from [dbo].[RepositoryInfo] where [Name] = 'UpgradedTo7.0') <> 0
	begin

		declare @VersionUpgradeDate datetime
		
		select @VersionUpgradeDate = Character_Value from [dbo].[RepositoryInfo] where [Name] = 'UpgradedTo7.0'

		select @BeginDate = Character_Value from RepositoryInfo where Name = 'AlertsLastDateUpgraded'
		if (@BeginDate is null)
		begin
			set @BeginDate = @VersionUpgradeDate
		end
		
		Select @LeftToDo = Internal_Value from RepositoryInfo where Name = 'AlertsLeftToDo'

		while (GETDATE() < @TimeOut) and (@LeftToDo > 0)
		begin	
			if (object_id('AlertsTempData') is null)
			begin
				create table AlertsTempData (
					AlertID int,
					UTCOccurrenceDateTime datetime,
					Severity tinyint)
			end
			
			truncate table AlertsTempData
			
			insert into AlertsTempData 
				select 
					AlertID, 
					UTCOccurrenceDateTime, 
					Severity*2 
				from 
					[dbo].Alerts 
				where 
					UTCOccurrenceDateTime < @VersionUpgradeDate and
					Severity in (2,4)

			set @EndDate = DATEADD(dd, -1, @BeginDate)

			update 
				Alerts 
					set Alerts.Severity = new.Severity
				from 
					AlertsTempData new
				where 
					new.UTCOccurrenceDateTime <= @BeginDate and
					new.UTCOccurrenceDateTime >= @EndDate and
					Alerts.AlertID = new.AlertID
		
			delete from AlertsTempData where UTCOccurrenceDateTime <= @BeginDate and UTCOccurrenceDateTime >= @EndDate
		
			Select @LeftToDo = Count(AlertID) from AlertsTempData
			
			update RepositoryInfo set Character_Value = @EndDate where Name = 'AlertsLastDateUpgraded'
			update RepositoryInfo set Internal_Value = @LeftToDo where Name = 'AlertsLeftToDo'
			
			Set @BeginDate = @EndDate

		end
		
		if (GETDATE() > @TimeOut)
		begin
			Select 'Time Expired'
			return
		end
		
		if @LeftToDo = 0
		BEGIN
			drop table AlertsTempData
		END
		
		-- SeverActivity
		select @BeginDate = Character_Value from RepositoryInfo where Name = 'SALastDateUpgraded'
		if (@BeginDate is null)
		begin
			set @BeginDate = @VersionUpgradeDate
		end

		Select @LeftToDo = Internal_Value from RepositoryInfo where Name = 'SALeftToDo'
		
		while (GETDATE() < @TimeOut) and (@LeftToDo > 0)
		begin
		
			if (object_id('SATempData') is null)
			begin
				create table SATempData (
					SQLServerID int,
					UTCCollectionDateTime datetime,
					StateOverview xml)
			end
			
			truncate table SATempData
			
			set @EndDate = DATEADD(dd, -1, @BeginDate)
			
			insert into SATempData
				select
						SQLServerID,
						UTCCollectionDateTime,
						CAST(StateOverview as XML)
					from
						ServerActivity
					Where
						UTCCollectionDateTime <= @BeginDate and
						UTCCollectionDateTime >= @EndDate
			
			while exists (select * from SATempData where StateOverview.exist('/Servers/Server//State/@Severity[.="4"]') = 1)
				update SATempData 
					  set StateOverview.modify('replace value of (/Servers/Server//State/@Severity[.="4"])[1] with "8"')

			while exists (select * from SATempData where StateOverview.exist('/Servers/Server//State/@Severity[.="2"]') = 1)
				update SATempData 
					set StateOverview.modify('replace value of (/Servers/Server//State/@Severity[.="2"])[1] with "4"')

			update [dbo].[ServerActivity] 
					set [ServerActivity].[StateOverview] = convert(nvarchar(max), b.StateOverview) 
				from 
					SATempData b 
				where 
					ServerActivity.SQLServerID = b.SQLServerID 
					and ServerActivity.UTCCollectionDateTime = b.UTCCollectionDateTime
					
			Select @LeftToDo = count(*) from ServerActivity where UTCCollectionDateTime < @EndDate
					
			update RepositoryInfo set Character_Value = @EndDate where Name = 'SALastDateUpgraded'
			update RepositoryInfo set Internal_Value = @LeftToDo where Name = 'SALeftToDo'
			
			Set @BeginDate = @EndDate

		end
		
		if @LeftToDo = 0
		BEGIN
			drop table SATempData
		END
		
		if (GETDATE() > @TimeOut)
		begin
			Select 'Time Expired'
			return
		end
		
		-- Tasks
		select @BeginDate = Character_Value from RepositoryInfo where Name = 'TasksLastDateUpgraded'
		if (@BeginDate is null)
		begin
			set @BeginDate = @VersionUpgradeDate
		end

		Select @LeftToDo = Internal_Value from RepositoryInfo where Name = 'TasksLeftToDo'

		while (GETDATE() < @TimeOut) and (@LeftToDo > 0)
		begin
			set @EndDate = DATEADD(dd, -1, @BeginDate)
			
			update Tasks set Severity = Severity*2 where CreatedOn <= @BeginDate and CreatedOn >= @EndDate and Severity in (2,4)
		
			select @LeftToDo = Count(TaskID) from Tasks where CreatedOn < @EndDate and Severity in (2,4)
			
			update RepositoryInfo set Character_Value = @EndDate where Name = 'TasksLastDateUpgraded'
			update RepositoryInfo set Internal_Value = @LeftToDo where Name = 'TasksLeftToDo'
			
			set @BeginDate = @EndDate
		end
		
		if (GETDATE() > @TimeOut)
		begin
			Select 'Time Expired'
			return
		end
		
	end
end