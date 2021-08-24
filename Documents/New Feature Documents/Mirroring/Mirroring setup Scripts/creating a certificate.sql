xp_msver
DBCC TRACEON(1400)

select * from blobtabinsert 
insert into blobtab values(1,'aa')

create certificate

CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'bsearle'

CREATE CERTIFICATE TestCertificate
   ENCRYPTION BY PASSWORD = 'bsearle'
   WITH SUBJECT = 'This is a test certificate', 
   START_DATE = '1/1/2006',
   EXPIRY_DATE = '12/31/2008';

select cert_id('TestCertificate')

create table test(col1 varbinary(8000))
insert into test values(EncryptbyCert(256, 'Hello'))
select Decryptbycert(256, col1) from test
--0x9EAB589C39753D7A08698E930DC7B7944FA4DAC24BDAC385C8F6C73C2A2477DEF9BB9F00853115ADF96E91BB2AEEE5FE8923BC2CD4C3C7FBA77D8D2A1E901303F8C95FA08EF2D163FBEA7D0DF7ECA1A820AEE7A2B2F7593EE1ABCB933E94C8C40CF42F4E52FB5BDF579F2F598E21F91CD30540CB1AE076692D6A8705D7AF708F
select DecryptByCert(256, 0x9EAB589C39753D7A08698E930DC7B7944FA4DAC24BDAC385C8F6C73C2A2477DEF9BB9F00853115ADF96E91BB2AEEE5FE8923BC2CD4C3C7FBA77D8D2A1E901303F8C95FA08EF2D163FBEA7D0DF7ECA1A820AEE7A2B2F7593EE1ABCB933E94C8C40CF42F4E52FB5BDF579F2F598E21F91CD30540CB1AE076692D6A8705D7AF708F)
drop certificate TestCertificate
drop master key

-- Sample T-SQL Script to demonstrate Certificate Encryption

-- Use the AdventureWorks database
USE AdventureWorks;

-- Create a Database Master Key
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'p@ssw0rd';

-- Create a Temp Table
CREATE TABLE Person.#Temp
(ContactID   INT PRIMARY KEY,
 FirstName   NVARCHAR(200),
 MiddleName  NVARCHAR(200),
 LastName    NVARCHAR(200),
 eFirstName  VARBINARY(200),
 eMiddleName VARBINARY(200),
 eLastName   VARBINARY(200));

-- Create a Test Certificate, encrypted by the DMK
CREATE CERTIFICATE TestCertificate
   WITH SUBJECT = 'sample Test Certificate',
   EXPIRY_DATE = '10/31/2009';

-- EncryptByCert demonstration encrypts 100 names from the Person.Contact table
INSERT
INTO Person.#Temp (ContactID, eFirstName, eMiddleName, eLastName)
SELECT ContactID, EncryptByCert(Cert_ID('TestCertificate'), FirstName),
	EncryptByCert(Cert_ID('TestCertificate'), MiddleName), 
	EncryptByCert(Cert_ID('TestCertificate'), LastName)
FROM Person.Contact
WHERE ContactID <= 100;

SELECT *
FROM Person.#Temp;

-- DecryptByCert demonstration decrypts the previously encrypted data
UPDATE Person.#Temp
SET FirstName = DecryptByCert(Cert_ID('TestCertificate'), eFirstName),
	MiddleName = DecryptByCert(Cert_ID('TestCertificate'), eMiddleName),
	LastName = DecryptByCert(Cert_ID('TestCertificate'), eLastName);

-- View the results
SELECT *
FROM Person.#Temp;

-- Clean up work:  drop temp table, test certificate and master key
DROP TABLE Person.#Temp;
DROP CERTIFICATE TestCertificate;
DROP MASTER KEY;