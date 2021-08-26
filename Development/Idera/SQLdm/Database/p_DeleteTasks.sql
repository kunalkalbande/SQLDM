if (object_id('p_DeleteTasks') is not null)
begin
drop procedure p_DeleteTasks
end
go

create procedure [dbo].[p_DeleteTasks]
	@XmlDocument nvarchar(max) = null,
	@TaskID int = null
as
begin

declare @TasksToDelete table(TaskID int) 
declare @xmlDoc int
declare @e int

if @XmlDocument is not null 
begin
	-- Prepare XML document if there is one
	exec sp_xml_preparedocument @xmlDoc output, @XmlDocument

	insert into @TasksToDelete
	select
		TaskID 
	from openxml(@xmlDoc, '//Task', 1)
		with (TaskID int)

	exec sp_xml_removedocument @xmlDoc
end

if @TaskID is not null
	insert into @TasksToDelete values(@TaskID)

if (@TaskID is null and @XmlDocument is null)
	delete from [Tasks]
else
	delete from [Tasks] where [TaskID] in (select [TaskID] from @TasksToDelete)

select @e = @@error

return @e

end
 
