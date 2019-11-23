USE [EmployeeLogin_SandBox]
GO
SET IDENTITY_INSERT [dbo].[AppCompanyLeaveManagement] ON 
GO
INSERT [dbo].[AppCompanyLeaveManagement] ([ID], [DateCreated], [DateModified], [UserID], [CompanyID], [TotalYearlyLeaves]) VALUES (1, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), 146, 13, CAST(12.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[AppCompanyLeaveManagement] OFF
GO
SET IDENTITY_INSERT [dbo].[AppLeaveApprovedStatus] ON 
GO
INSERT [dbo].[AppLeaveApprovedStatus] ([ID], [dateCreated], [DateModified], [Name], [Description]) VALUES (1, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Pending', NULL)
GO
INSERT [dbo].[AppLeaveApprovedStatus] ([ID], [dateCreated], [DateModified], [Name], [Description]) VALUES (2, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Sanctioned', NULL)
GO
INSERT [dbo].[AppLeaveApprovedStatus] ([ID], [dateCreated], [DateModified], [Name], [Description]) VALUES (3, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Canceled', NULL)
GO
INSERT [dbo].[AppLeaveApprovedStatus] ([ID], [dateCreated], [DateModified], [Name], [Description]) VALUES (4, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Reverted', NULL)
GO
INSERT [dbo].[AppLeaveApprovedStatus] ([ID], [dateCreated], [DateModified], [Name], [Description]) VALUES (5, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Rejected', NULL)
GO
SET IDENTITY_INSERT [dbo].[AppLeaveApprovedStatus] OFF
GO
SET IDENTITY_INSERT [dbo].[AppLeaveType] ON 
GO
INSERT [dbo].[AppLeaveType] ([ID], [DateCreated], [DateModified], [Name], [Description]) VALUES (1, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Full Leave', NULL)
GO
INSERT [dbo].[AppLeaveType] ([ID], [DateCreated], [DateModified], [Name], [Description]) VALUES (2, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'First Half Leave', NULL)
GO
INSERT [dbo].[AppLeaveType] ([ID], [DateCreated], [DateModified], [Name], [Description]) VALUES (3, CAST(N'2019-10-10T00:00:00.000' AS DateTime), CAST(N'2019-10-10T00:00:00.000' AS DateTime), N'Second Half Leave', NULL)
GO
SET IDENTITY_INSERT [dbo].[AppLeaveType] OFF
GO
SET IDENTITY_INSERT [dbo].[AppMasterLeaveType] ON 
GO
INSERT [dbo].[AppMasterLeaveType] ([ID], [DateCreated], [Name], [Description]) VALUES (1, CAST(N'2019-10-09T00:00:00.000' AS DateTime), N'Casual Leave', NULL)
GO
INSERT [dbo].[AppMasterLeaveType] ([ID], [DateCreated], [Name], [Description]) VALUES (2, CAST(N'2019-10-09T00:00:00.000' AS DateTime), N'Duty Leave', NULL)
GO
INSERT [dbo].[AppMasterLeaveType] ([ID], [DateCreated], [Name], [Description]) VALUES (3, CAST(N'2019-10-09T00:00:00.000' AS DateTime), N'Sick Leave', NULL)
GO
INSERT [dbo].[AppMasterLeaveType] ([ID], [DateCreated], [Name], [Description]) VALUES (4, CAST(N'2019-10-09T00:00:00.000' AS DateTime), N'Leave Without Pay', NULL)
GO
SET IDENTITY_INSERT [dbo].[AppMasterLeaveType] OFF
GO
