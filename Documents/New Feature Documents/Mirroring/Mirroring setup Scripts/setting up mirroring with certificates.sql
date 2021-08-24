waitfor delay '00:10:00'

drop user HOST_W_login
drop user HOST_W_User
drop certificate HOST_W_CERT

drop endpoint endpoint_mirroring
drop user bsearled_user
drop login bsearled_login
drop certificate bsearled_cert 
drop master key

drop endpoint endpoint_mirroring
drop user bsearlel_user
drop login bsearlel_login
drop certificate HOST_W_CERT 
drop master key
drop endpoint mirroring

drop endpoint endpoint_mirroring
drop user bsearlel_user
drop login bsearlel_login
drop certificate bsearlel_cert 
drop master key
drop endpoint mirroring

-- HOST A - bsearlel
create master key encryption by password = 'abc123!!';
GO

create certificate bsearlel_cert with subject = 'bsearlel certificate', start_date = '2007/11/01', expiry_date = '2020/11/01';
GO

drop endpoint endpoint_mirroring
Create endpoint endpoint_mirroring state = started
as tcp(listener_port = 5022, listener_ip = all)
for database_mirroring (authentication = certificate bsearlel_cert, encryption = disabled, role = all);
GO

Backup certificate bsearlel_cert to file = 'c:\bsearlel_cert.cer';
GO

-- HOST B
create master key encryption by password = 'abc123!!';
GO

create certificate bsearled_cert with subject = 'bsearled certificate', start_date = '2007/11/01', expiry_date = '2020/11/01';
GO
--drop endpoint mirroring
drop endpoint endpoint_mirroring
Create endpoint endpoint_mirroring state = started
as tcp(listener_port = 5022, listener_ip = all)
for database_mirroring (authentication = certificate bsearled_cert, encryption = disabled, role = all);
GO

Backup certificate bsearled_cert to file = 'c:\bsearled_cert.cer';
GO

-- HOST W
create master key encryption by password = 'abc123!!';
GO

create certificate HOST_W_cert with subject = 'HOST_W certificate', start_date = '2007/11/01', expiry_date = '2020/11/01';
GO

drop endpoint endpoint_mirroring

Create endpoint endpoint_mirroring state = started
as tcp(listener_port = 5022, listener_ip = all)
for database_mirroring (authentication = certificate HOST_W_cert, encryption = disabled, role = witness);
GO

Backup certificate HOST_W_cert to file = 'c:\HOST_W_cert.cer';
GO

-- HOST A again
create login bsearled_login with PASSWORD = 'abc123!!';
GO

create user bsearled_user from login bsearled_login;
GO

Create certificate bsearled_cert
Authorization bsearled_user
From file = 'c:\bsearled_cert.cer';
GO

Grant CONNECT ON Endpoint::endpoint_mirroring to [bsearled_login];
GO
------
create login HOST_W_login with PASSWORD = 'abc123!!';
GO

create user HOST_W_user from login HOST_W_login;
GO

Create certificate HOST_W_cert
Authorization HOST_W_user
From file = 'c:\HOST_W_cert.cer';
GO

Grant CONNECT ON Endpoint::endpoint_mirroring to [HOST_W_login];
GO

-- HOST B again
create login bsearlel_login with PASSWORD = 'abc123!!';
GO

create user bsearlel_user from login bsearlel_login;
GO

Create certificate bsearlel_cert
Authorization bsearlel_user
From file = 'c:\bsearlel_cert.cer';
GO

Grant CONNECT ON Endpoint::Endpoint_mirroring to [bsearlel_login];
GO

-------
create login HOST_W_login with PASSWORD = 'abc123!!';
GO

create user HOST_W_user from login HOST_W_login;
GO

Create certificate HOST_W_cert
Authorization HOST_W_user
From file = 'c:\HOST_W_cert.cer';
GO

Grant CONNECT ON Endpoint::Endpoint_mirroring to [HOST_W_login];
GO

-- HOST W again
create login bsearlel_login with PASSWORD = 'abc123!!';
GO

create user bsearlel_user from login bsearlel_login;
GO

Create certificate bsearlel_cert
Authorization bsearlel_user
From file = 'c:\bsearlel_cert.cer';
GO

Grant CONNECT ON Endpoint::Endpoint_mirroring to [bsearlel_login];
GO

-------
create login bsearled_login with PASSWORD = 'abc123!!';
GO

create user bsearled_user from login bsearled_login;
GO

Create certificate bsearled_cert
Authorization bsearled_user
From file = 'c:\bsearled_cert.cer';
GO

Grant CONNECT ON Endpoint::endpoint_mirroring to [bsearled_login];
GO

---Host A
ALTER ENDPOINT [endpoint_mirroring] STATE = STARTED;

-- HOST B again
alter database sample set partner = 'TCP://bsearlel.redhouse.hq:5022';
GO

-- HOST A again
alter database sample set partner = 'TCP://bsearled.redhouse.hq:5022';
GO

alter database sample set witness = 'TCP://contractd1.redhouse.hq:5022';
GO 