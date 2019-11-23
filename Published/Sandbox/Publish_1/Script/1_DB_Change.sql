USE [EmployeeLogin_SandBox]
GO
/****** Object:  Table [dbo].[AppCompanyLeaveManagement]    Script Date: 23-11-2019 13:27:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppCompanyLeaveManagement](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[UserID] [int] NOT NULL,
	[CompanyID] [int] NOT NULL,
	[TotalYearlyLeaves] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_AppCompanyLeaveManagement] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppLeaveApprovedStatus]    Script Date: 23-11-2019 13:27:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppLeaveApprovedStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_AppLeaveApprovedStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppLeaveType]    Script Date: 23-11-2019 13:27:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppLeaveType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_AppLeaveType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppMasterLeaveType]    Script Date: 23-11-2019 13:27:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppMasterLeaveType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_AppMasterLeaveType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserLeave]    Script Date: 23-11-2019 13:27:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserLeave](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[UserID] [int] NOT NULL,
	[AppMasterLeaveTypeID] [int] NOT NULL,
	[AppLeaveTypeId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[AppLeaveApprovedStatusId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CompanyID] [int] NOT NULL,
	[ApprovedBy] [int] NULL,
	[TotalDays] [decimal](18, 2) NOT NULL,
	[IsPaidLeave] [bit] NOT NULL,
 CONSTRAINT [PK_AppUserLeave] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppVersionHistory]    Script Date: 23-11-2019 13:27:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppVersionHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[VersionName] [varchar](500) NOT NULL,
	[VersionID] [int] NOT NULL,
	[Message] [varchar](5000) NOT NULL,
	[IsMustUpdate] [bit] NOT NULL,
	[DeviceType] [int] NOT NULL,
 CONSTRAINT [PK_AppVersionHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AppUserLeave] ADD  CONSTRAINT [DF_AppUserLeave_AppMasterLeaveTypeID]  DEFAULT ((1)) FOR [AppMasterLeaveTypeID]
GO
ALTER TABLE [dbo].[AppUserLeave] ADD  CONSTRAINT [DF_AppUserLeave_TotalDays]  DEFAULT ((0)) FOR [TotalDays]
GO
ALTER TABLE [dbo].[AppUserLeave] ADD  CONSTRAINT [DF_AppUserLeave_IsPaidLeave]  DEFAULT ((1)) FOR [IsPaidLeave]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_AppLeaveApprovedStatus] FOREIGN KEY([AppLeaveApprovedStatusId])
REFERENCES [dbo].[AppLeaveApprovedStatus] ([ID])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_AppLeaveApprovedStatus]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_AppLeaveType] FOREIGN KEY([AppLeaveTypeId])
REFERENCES [dbo].[AppLeaveType] ([ID])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_AppLeaveType]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_AppMasterLeaveType] FOREIGN KEY([AppMasterLeaveTypeID])
REFERENCES [dbo].[AppMasterLeaveType] ([ID])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_AppMasterLeaveType]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_AppUserLeave] FOREIGN KEY([ID])
REFERENCES [dbo].[AppUserLeave] ([ID])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_AppUserLeave]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_Company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_Company]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_Users]
GO
ALTER TABLE [dbo].[AppUserLeave]  WITH CHECK ADD  CONSTRAINT [FK_AppUserLeave_Users1] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AppUserLeave] CHECK CONSTRAINT [FK_AppUserLeave_Users1]
GO
