USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[ProgramInfo]    Script Date: 06/13/2014 16:09:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProgramInfo](
	[ProgramId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProgramName] [nvarchar](250) NULL,
	[MappingFiles] [nvarchar](950) NULL
) ON [PRIMARY]

GO

