if (object_id('p_GetMirroringPreferredConfig') is not null)
begin
drop procedure [p_GetMirroringPreferredConfig]
end
go


CREATE procedure [dbo].[p_GetMirroringPreferredConfig] as

SELECT [MirroringGuid]
      ,[MirrorInstanceID]
      ,[PrincipalInstanceID]
      ,[NormalConfiguration]
	  ,[DatabaseName]
      ,[WitnessAddress]
  FROM [MirroringPreferredConfig] (nolock)




