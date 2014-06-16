USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[ScheduleInfo]    Script Date: 6/16/2014 10:37:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScheduleInfo](
	[ScheduleId] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleName] [nvarchar](50) NULL,
	[PprgramId] [nvarchar](50) NULL,
	[ScheduleTime] [nvarchar](50) NULL
) ON [PRIMARY]

GO

