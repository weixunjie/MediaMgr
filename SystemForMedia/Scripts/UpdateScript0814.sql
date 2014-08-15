USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[RunningEncoder]    Script Date: 08/14/2014 17:54:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RunningEncoder](
	[ClientIdentify] [nvarchar](50) NULL,
	[Priority] [nvarchar](50) NULL,
	[GroupIds] [nvarchar](50) NULL
) ON [PRIMARY]

GO


