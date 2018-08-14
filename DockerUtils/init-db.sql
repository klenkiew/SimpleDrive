IF NOT EXISTS
(SELECT name
 FROM master.sys.server_principals
 WHERE name = 'dotnetUser')
  BEGIN
    CREATE LOGIN dotnetUser WITH PASSWORD = 'Password1';
  END
GO

IF db_id('FileServiceDb') is null
  BEGIN
    print 'Creating database FileServiceDb'
    CREATE DATABASE FileServiceDb;
  END
GO

USE FileServiceDb;
IF USER_ID('dotnetUser') IS NULL
  CREATE USER dotnetUser FOR LOGIN dotnetUser;
GO

ALTER ROLE [db_ddladmin] ADD MEMBER [dotnetUser]
ALTER ROLE [db_datawriter] ADD MEMBER [dotnetUser]
ALTER ROLE [db_datareader] ADD MEMBER [dotnetUser]
GO

IF db_id('UsersDb') is null
  BEGIN
    print 'Creating database UsersDb'
    CREATE DATABASE UsersDb;
  END
GO

USE UsersDb;
IF USER_ID('dotnetUser') IS NULL
  CREATE USER dotnetUser FROM LOGIN dotnetUser;
GO

ALTER ROLE [db_ddladmin] ADD MEMBER [dotnetUser]
ALTER ROLE [db_datawriter] ADD MEMBER [dotnetUser]
ALTER ROLE [db_datareader] ADD MEMBER [dotnetUser]
GO