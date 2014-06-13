USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[DeviceInfo]    Script Date: 06/13/2014 16:08:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DeviceInfo](
	[DeviceId] [bigint] IDENTITY(1,1) NOT NULL,
	[DeviceName] [nvarchar](250) NULL,
	[GroupId] [nvarchar](10) NULL,
	[DeviceIPAddress] [nvarchar](50) NULL
) ON [PRIMARY]

GO

