USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[RemoteControlDeviceStatus]    Script Date: 08/01/2014 16:35:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RemoteControlDeviceStatus](
	[ClientIdentify] [nvarchar](50) NULL,
	[DeviceType] [nvarchar](50) NULL,
	[DeviceStatus] [nvarchar](50) NULL,
	[ACTempature] [nvarchar](50) NULL,
	[ACMode] [nvarchar](50) NULL
) ON [PRIMARY]

GO


