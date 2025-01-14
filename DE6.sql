USE [DE6]
GO
/****** Object:  Table [dbo].[LichSu]    Script Date: 7/29/2024 2:26:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichSu](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NgayDauGia] [datetime] NULL,
	[GiaTri] [decimal](18, 0) NULL,
	[NgayBatDau] [datetime] NULL,
	[NgayKetThuc] [datetime] NULL,
	[TenDonViSD] [nvarchar](255) NULL,
	[IDTTCB] [int] NULL,
	[IsDelete] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThongTinCB]    Script Date: 7/29/2024 2:26:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThongTinCB](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](255) NULL,
	[Loai] [nvarchar](255) NULL,
	[DiaChi] [nvarchar](255) NULL,
	[MoTa] [nvarchar](255) NULL,
	[GiaTri] [decimal](18, 0) NULL,
	[TenDonViCQ] [nvarchar](255) NULL,
	[IsDelete] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Token]    Script Date: 7/29/2024 2:26:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Token](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Users_ID] [int] NULL,
	[access_token] [nvarchar](255) NULL,
	[refresh_token] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/29/2024 2:26:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[Pass] [nvarchar](255) NULL,
	[Role] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[LichSu] ON 

INSERT [dbo].[LichSu] ([ID], [NgayDauGia], [GiaTri], [NgayBatDau], [NgayKetThuc], [TenDonViSD], [IDTTCB], [IsDelete]) VALUES (1, CAST(N'2024-06-11T11:15:48.863' AS DateTime), CAST(1000000 AS Decimal(18, 0)), CAST(N'2024-05-11T11:15:48.863' AS DateTime), CAST(N'2024-07-22T11:15:48.863' AS DateTime), N'SCT', 1, 0)
INSERT [dbo].[LichSu] ([ID], [NgayDauGia], [GiaTri], [NgayBatDau], [NgayKetThuc], [TenDonViSD], [IDTTCB], [IsDelete]) VALUES (2, CAST(N'2024-06-11T11:15:48.863' AS DateTime), CAST(88888 AS Decimal(18, 0)), CAST(N'2024-06-11T11:15:48.863' AS DateTime), CAST(N'2024-06-11T11:15:48.863' AS DateTime), N'SCT', 1, 0)
SET IDENTITY_INSERT [dbo].[LichSu] OFF
GO
SET IDENTITY_INSERT [dbo].[ThongTinCB] ON 

INSERT [dbo].[ThongTinCB] ([ID], [Ten], [Loai], [DiaChi], [MoTa], [GiaTri], [TenDonViCQ], [IsDelete]) VALUES (1, N'bến xe tàu', N'Bến xe', N'cần thơ', N'hyhy', CAST(1000 AS Decimal(18, 0)), N'GTVT', 0)
SET IDENTITY_INSERT [dbo].[ThongTinCB] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([ID], [UserName], [Pass], [Role]) VALUES (1, N'admin', N'f52EmOY2EqOlO+TvezMgDgWOo+sI249P1hzRKVcu1gE=', 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[LichSu]  WITH CHECK ADD FOREIGN KEY([IDTTCB])
REFERENCES [dbo].[ThongTinCB] ([ID])
GO
ALTER TABLE [dbo].[Token]  WITH CHECK ADD FOREIGN KEY([Users_ID])
REFERENCES [dbo].[Users] ([ID])
GO
