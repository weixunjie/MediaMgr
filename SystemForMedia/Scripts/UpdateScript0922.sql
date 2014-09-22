USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[EncoderInfo]    Script Date: 09/22/2014 15:03:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VideoEncoderInfo](
	[EncoderId] [bigint] IDENTITY(1,1) NOT NULL,
	[EncoderName] [nvarchar](50) NULL,
	[BaudRate] [nvarchar](50) NULL,
	[UdpAddress] [nvarchar](50) NULL	
) ON [PRIMARY]

GO


