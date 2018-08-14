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



USE FileServiceDb;

print 'Creating stored procedures'
GO

CREATE OR ALTER PROCEDURE GetFileWithOwner @FileId nvarchar(900)
AS
  SELECT
    f.[Id],
    f.[FileName],
    f.[Description],
    f.[Size],
    f.[MimeType],
    f.[DateCreated],
    f.[DateModified],
    u.[Id],
    u.[Username]
  FROM [Files] f
    LEFT JOIN [Users] u ON f.[OwnerId] = u.[Id]
  WHERE f.[Id] = @FileId
GO

CREATE OR ALTER PROCEDURE GetFileWithoutOwner @FileId nvarchar(900)
AS
  SELECT
    f.[Id],
    f.[FileName],
    f.[Description],
    f.[Size],
    f.[MimeType],
    f.[DateCreated],
    f.[DateModified]
  FROM [Files] f
  WHERE f.[Id] = @FileId
GO

CREATE OR ALTER PROCEDURE GetAvailableFilesByUser @UserId nvarchar(900)
AS
  SELECT
    f.[Id],
    f.[FileName],
    f.[Description],
    f.[Size],
    f.[MimeType],
    f.[DateCreated],
    f.[DateModified],
    u.[Id],
    u.[Username]
  FROM [Files] f
    INNER JOIN [Users] u ON f.[OwnerId] = u.[Id]
    LEFT JOIN [FileShare] s ON s.[FileId] = f.[Id]
  WHERE f.[OwnerId] = @UserId OR s.[UserId] = @UserId
GO

CREATE OR ALTER PROCEDURE GetUsersBySharedFile @FileId nvarchar(900)
AS
  SELECT
    u.[Id],
    u.[Username]
  FROM [Files] f
    INNER JOIN [FileShare] s ON s.[FileId] = f.[Id]
    INNER JOIN [Users] u ON s.[UserId] = u.[Id]
  WHERE f.[Id] = @FileId
GO

print 'Stored procedures created'
print 'Granting permissions for procedures'
GO

GRANT EXECUTE ON GetFileWithOwner TO dotnetUser;
GRANT EXECUTE ON GetFileWithoutOwner TO dotnetUser;
GRANT EXECUTE ON GetAvailableFilesByUser TO dotnetUser;
GRANT EXECUTE ON GetUsersBySharedFile TO dotnetUser;
GO

print 'Permissions granted'
GO