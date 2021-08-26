--------------------------------------------------------------------------------
--  Batch: Table Details 2005
--  Tables: sysobjects,
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - schema_name or user_name
--------------------------------------------------------------------------------
use [{0}]

declare @table_id int
set @table_id = {1}

select
	ObjectId = o.id,
	DBName = db_name(),
	TableName = o.name,
	Owner = {2}(o.uid)
from
	sysobjects o (nolock)
where
	o.id =  @table_id

select
  {2}(o.uid),
  object_name(o.id),
  '',
  case
    when o.xtype = 'TR'
      then convert(varchar(255),
        case
          when isnull(objectproperty(o.id,'ExecIsInsteadOfTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'instead of trigger (disabled)'
                else 'instead of trigger'
              end
          when isnull(objectproperty(o.id,'ExecIsLastUpdateTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'last update trigger (disabled)'
                else 'last update trigger'
              end

          when isnull(objectproperty(o.id,'ExecIsLastInsertTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'last insert trigger (disabled)'
                else 'last insert trigger'
              end
          when isnull(objectproperty(o.id,'ExecIsLastDeleteTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'last delete trigger (disabled)'
                else 'last delete trigger'
              end
          when isnull(objectproperty(o.id,'ExecIsFirstUpdateTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'first update trigger (disabled)'
                else 'first update trigger'
              end
          when isnull(objectproperty(o.id,'ExecIsFirstInsertTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'first insert trigger (disabled)'
                else 'first insert trigger'
              end
          when isnull(objectproperty(o.id,'ExecIsFirstDeleteTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'first delete trigger (disabled)'
                else 'first delete trigger'
              end
          when  isnull(objectproperty(o.id,'ExecIsAfterTrigger'),'0') = 1
            then
              case
                when isnull(objectproperty(o.id,'ExecIsTriggerDisabled'),'0') = 1
                  then 'after trigger (disabled)'
                else 'after trigger'
              end
          else 'trigger'
        end)
    when o.xtype = 'C'
      then 'check constraint'
    when o.xtype = 'D'
      then 'default'
    when o.xtype = 'PK'
      then 'primary key'
    else 'view'
  end,
  ''
from
  sysobjects o
where
  o.parent_obj = @table_id
  and o.xtype not in ('F','C')
union
  select distinct
	{2}(o.uid),
    object_name(d.id),
    '',
    case
      when o.xtype = 'C'
        then 'check constraint'
      when o.xtype = 'TF'
        then 'table function'
      when o.xtype = 'IF'
        then 'inline table function'
      when o.xtype = 'FN'
        then 'function'
      when o.xtype = 'V'
        then 'view'
      when o.xtype = 'P'
        then convert(varchar(255),
          case
            when  isnull(objectproperty(o.id,'ExecIsStartup'),'0') = 0
              then 'stored proc'
            else 'startup procedure'
          end)
    end,
    case
      when d.resultobj = 1 and d.selall = 1
        then 'update/select *'
      when d.resultobj = 1 and d.selall = 0
        then 'update'
      when d.resultobj = 0 and d.selall = 1
        then 'select *'
      else 'read'
    end
  from
    sysdepends d ,
    sysobjects o
  where
    d.id = o.id
    and d.depid <> d.id
    and d.depid = @table_id
	and o.xtype in ('C','TF','IF','FN','V','P')
union
  select
    {2}(o.uid),
    object_name (r.fkeyid),
    object_name(constid),
    'table - foreign key',
    'integrity'
  from
    sysreferences r ,
    sysobjects o
  where
    r.rkeyid = o.id
    and r.rkeyid = @table_id

select
  object_name(o.id),
  case
    when o.xtype = 'U'
      then 'table'
    when o.xtype = 'F'
      then 'foreign key'
  end,
  'integrity'
from
  sysobjects o
where
  o.parent_obj = @table_id
  and o.xtype = 'F'


select
	i.indid,
	i.name,
	i.used,
	i.rows,
	i.dpages,
	stats_date(i.id, i.indid),
	'rows modified'=
   (
     select
       rowmodctr
     from
       sysindexes
     where
       indid in (0,1)
       and id = @table_id
   ),
 'unique' =
   case
     when status&2 = 2
       then cast(1 as bit)
     else cast(0 as bit)
   end,
 'clustered' =
   case
     when indid = 1
       then cast(1 as bit)
     else cast(0 as bit)
   end,
 'fill factor' = cast(OrigFillFactor as smallint),
 'index levels' = indexproperty(@table_id, name, 'indexdepth'),
 filegroup_name(groupid)
 from
   sysindexes i
 where
   id = @table_id
   and i.status&32 <> 32
