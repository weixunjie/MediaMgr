USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[RemoteControlDeviceStatus]    Script Date: 09/17/2014 15:44:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RemoteControlScheduleTask](

	[TaskId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientIdentify] [nvarchar](50) NULL,
	[DeviceType] [nvarchar](50) NULL,
	[NewDeviceStatus] [nvarchar](50) NULL,
	[ACTempature] [nvarchar](50) NULL,
	[ACMode] [nvarchar](50) NULL,
	[Weeks] [nvarchar](250) NULL,
	[TaskTime] [nvarchar](50) NULL,
	[LASTRUNDATE] datetime null
) ON [PRIMARY]

GO


