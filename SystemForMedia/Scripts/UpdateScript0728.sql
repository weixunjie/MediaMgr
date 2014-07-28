
USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[SingalConnectedClients]    Script Date: 07/28/2014 15:02:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SingalConnectedClients](
	[ConnectionId] [nvarchar](50) NULL,
	[ConnectionIdentify] [nvarchar](150) NULL,
	[ConnectionType] [nvarchar](50) NULL
) ON [PRIMARY]

GO



alter  table  dbo.DeviceInfo add  isUsedForRemoteControl int