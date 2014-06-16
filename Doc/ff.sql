USE [master]
GO
/****** Object:  Database [MediaMgrSystem]    Script Date: 6/15/2014 8:53:49 PM ******/
CREATE DATABASE [MediaMgrSystem]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MediaMgrSystem', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\MediaMgrSystem.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MediaMgrSystem_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\MediaMgrSystem_log.ldf' , SIZE = 3456KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [MediaMgrSystem] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MediaMgrSystem].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MediaMgrSystem] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET ARITHABORT OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [MediaMgrSystem] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MediaMgrSystem] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MediaMgrSystem] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MediaMgrSystem] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MediaMgrSystem] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MediaMgrSystem] SET  MULTI_USER 
GO
ALTER DATABASE [MediaMgrSystem] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MediaMgrSystem] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MediaMgrSystem] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MediaMgrSystem] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [MediaMgrSystem]
GO
/****** Object:  Table [dbo].[ChannelInfo]    Script Date: 6/15/2014 8:53:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChannelInfo](
	[ChannelId] [bigint] IDENTITY(1,1) NOT NULL,
	[ChannelName] [nchar](10) NULL,
	[UdpBroadcastAddressForPlay] [nvarchar](150) NULL,
	[UdpBroadcastAddressForSchedule] [nvarchar](150) NULL,
	[UdpBroadcastAddressForEncoder] [nvarchar](150) NULL,
	[ScheduelId] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DeviceInfo]    Script Date: 6/15/2014 8:53:49 PM ******/
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
/****** Object:  Table [dbo].[FileInfo]    Script Date: 6/15/2014 8:53:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileInfo](
	[FileName] [nvarchar](250) NULL,
	[BitRate] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GroupInfo]    Script Date: 6/15/2014 8:53:49 PM ******/
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
/****** Object:  Table [dbo].[ProgramInfo]    Script Date: 6/15/2014 8:53:49 PM ******/
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
/****** Object:  Table [dbo].[ScheduleInfo]    Script Date: 6/15/2014 8:53:49 PM ******/
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
USE [master]
GO
ALTER DATABASE [MediaMgrSystem] SET  READ_WRITE 
GO
