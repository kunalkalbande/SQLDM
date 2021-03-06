/****** Object:  StoredProcedure [dbo].[p_AddAuditableEvent]    Script Date: 02/07/2013 10:43:21 ******/
if (object_id('p_AddAuditableEvent') is not null)
begin
drop procedure p_AddAuditableEvent
end
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[p_AddAuditableEvent](
	@ActionID int,
	@Date datetime,
	@Workstation NVARCHAR(256),
	@WorkstationUser NVARCHAR(256),
	@SQLUser NVARCHAR(256),	
	@Name NVARCHAR(256),
	@MetaData NVARCHAR(4000),
	@Header NVARCHAR(500)
	)
AS
BEGIN
declare @e int

IF not EXISTS (SELECT * FROM AuditableActions WHERE ActionID=@ActionID)
BEGIN
  set  @e= @@error
END
else
begin
INSERT INTO [dbo].[AuditableEvents]
           ([ActionID]
           ,[DateTime]
           ,[Workstation]
           ,[WorkstationUser]
           ,[SQLUser]
           ,[Name]
           ,[MetaData]
           ,[Header])
     VALUES
           (@ActionID
            ,@Date
            ,@Workstation
            ,@WorkstationUser
            ,@SQLUser
            ,@Name
            ,@MetaData
            ,@Header)
                        
            
           SET @e = @@error                      
END
return @e
end