USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[LogInfo]    Script Date: 11/19/2014 15:32:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LogInfoForSingalRConnection](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[LogName] [nvarchar](150) NULL,
	[LogDesp] [nvarchar](3000) NULL,
	[LogDate] [datetime] NULL
) ON [PRIMARY]

GO


