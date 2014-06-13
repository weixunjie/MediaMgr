USE [MediaMgrSystem]
GO

/****** Object:  Table [dbo].[ChannelInfo]    Script Date: 06/13/2014 16:08:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ChannelInfo](
	[ChannelId] [bigint] IDENTITY(1,1) NOT NULL,
	[ChannelName] [nchar](10) NULL,
	[UdpBroadcastAddressForPlay] [nvarchar](150) NULL,
	[UdpBroadcastAddressForSchedule] [nvarchar](150) NULL,
	[UdpBroadcastAddressForEncoder] [nvarchar](150) NULL
) ON [PRIMARY]

GO

