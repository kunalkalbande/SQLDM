if (object_id('p_GetExcludedWaitTypes') is not null)
begin
drop procedure p_GetExcludedWaitTypes
end
go

create procedure p_GetExcludedWaitTypes
as
begin
select wt.WaitType,mk.MapKey as XE_key 
	from
		WaitCategories wc
		left join WaitTypes wt on wc.CategoryID = wt.CategoryID
		left join XEMapKeys mk on  mk.WaitType = wt.WaitType and mk.SQLVersion = 11
		where wc.Category = 'Excluded'

end
