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



USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[ScheduleTaskInfo]    Script Date: 06/17/2014 18:46:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScheduleTaskInfo](
	[ScheduleId] [nvarchar](50) NULL,
	[ScheduleTaskId] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleTaskStartTime] [nvarchar](50) NULL,
	[ScheduleTaskEndTime] [nvarchar](50) NULL,
	[ScheduleTaskProgarmid] [nvarchar](50) NULL,
	[ScheduleTaskPriority] [nvarchar](50) NULL,
	[ScheduleTaskWeeks] [nvarchar](1500) NULL,
	[ScheduleTaskspecialDays] [nvarchar](1500) NULL
) ON [PRIMARY]

GO


