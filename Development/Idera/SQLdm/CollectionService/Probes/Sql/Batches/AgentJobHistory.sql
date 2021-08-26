select
	sjh.job_id,
	sjh.instance_id,
	sjh.step_name,
	dateadd(mi,datediff(mi,getdate(),getutcdate()),
	convert(datetime,convert(varchar(20), run_date) 
						+ ' ' + substring(right(replicate(0,6)+convert(varchar(6),run_time),6),1,2)
						+':'+substring(right(replicate(0,6)+convert(varchar(6),run_time),6),3,2)+':'+substring(right(replicate(0,6)
						+convert(varchar(6),run_time),6),5,2))) as runDate,
	sjh.run_status,
	sjh.run_duration,
	sjh.step_id,
	sjh.retries_attempted,
	message,
	sj.name
from
  msdb..sysjobhistory sjh (nolock),
  msdb..sysjobs sj (nolock)
where
  sjh.job_id = sj.job_id
  {0}
order by
  sjh.job_id,
  sjh.instance_id

