USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[GroupInfo]    Script Date: 06/13/2014 16:09:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GroupInfo](
	[GroupID] [bigint] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](250) NOT NULL,
	[ChannelId] [nvarchar](50) NULL
) ON [PRIMARY]

GO

