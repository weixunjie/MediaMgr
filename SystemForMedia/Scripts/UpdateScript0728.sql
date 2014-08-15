
USE [MediaMgrSystem]
GO




alter  table  dbo.ParamConfig add  IntervalTimeFromStopToPlay nvarchar(50)

GO

alter  table  dbo.ParamConfig add  MaxClientsCountForVideo nvarchar(50)
GO


alter  table  dbo.ParamConfig add  MaxClientsCountForAudio nvarchar(50)
GO


alter  table  dbo.ParamConfig add  MaxClientsCountForRemoteControl nvarchar(50)
GO

alter  table  dbo.ParamConfig drop column  MaxClientsCount 



