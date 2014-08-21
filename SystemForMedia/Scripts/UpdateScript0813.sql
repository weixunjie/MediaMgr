
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
go
update ParamConfig set IntervalTimeFromStopToPlay='2500'



alter  table  dbo.ParamConfig add  MaxClientsCountForRemoteControl nvarchar(50)
GO

update ParamConfig set MaxClientsCountForVideo='XoYmQBH84Vw='

update ParamConfig set MaxClientsCountForAudio='XoYmQBH84Vw='
update ParamConfig set MaxClientsCountForRemoteControl='XoYmQBH84Vw='



alter  table  dbo.EncoderInfo add  BaudRate  nvarchar(50)
GO

alter  table  dbo.EncoderInfo add  ClientIdentify  nvarchar(50)
GO

alter  table  dbo.EncoderInfo add  Priority  nvarchar(50)
GO