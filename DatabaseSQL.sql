USE [master]
GO
/****** Object:  Database [Product_Category]    Script Date: 11/14/2024 5:54:43 PM ******/
CREATE DATABASE [Product_Category]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Product_Category', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Product_Category.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Product_Category_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Product_Category_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Product_Category] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Product_Category].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Product_Category] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Product_Category] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Product_Category] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Product_Category] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Product_Category] SET ARITHABORT OFF 
GO
ALTER DATABASE [Product_Category] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Product_Category] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Product_Category] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Product_Category] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Product_Category] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Product_Category] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Product_Category] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Product_Category] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Product_Category] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Product_Category] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Product_Category] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Product_Category] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Product_Category] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Product_Category] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Product_Category] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Product_Category] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Product_Category] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Product_Category] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Product_Category] SET  MULTI_USER 
GO
ALTER DATABASE [Product_Category] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Product_Category] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Product_Category] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Product_Category] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Product_Category] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Product_Category] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Product_Category] SET QUERY_STORE = OFF
GO
USE [Product_Category]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsDeleted] [bit] NULL,
	[ParentId] [int] NULL,
 CONSTRAINT [PK__Categori__19093A0B121F43B0] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[ProductId] [int] NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OrderDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[ReceverName] [nvarchar](50) NULL,
	[Address] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](12) NULL,
	[Note] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderVoucher]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderVoucher](
	[OrderVoucherID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NULL,
	[VoucherID] [int] NULL,
	[DiscountApplied] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderVoucherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](100) NOT NULL,
	[CategoryId] [int] NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Stock] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[ImgName] [nvarchar](50) NULL,
 CONSTRAINT [PK__Products__B40CC6CD751209AB] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserGroups]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGroups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](256) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[IsActive] [bit] NULL,
	[GroupId] [int] NULL,
	[RoleId] [int] NULL,
	[LastUpdated] [datetime] NULL,
 CONSTRAINT [PK__Users__1788CC4C9FA369D5] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voucher]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voucher](
	[VoucherID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[DiscountValue] [decimal](10, 2) NULL,
	[DiscountType] [nvarchar](20) NULL,
	[ExpiryDate] [datetime] NULL,
	[Status] [nvarchar](20) NULL,
	[MaxUsage] [int] NULL,
	[MaxPerUsage] [int] NULL,
	[UsedCount] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[VoucherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoucherAssignment]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherAssignment](
	[AssignmentID] [int] IDENTITY(1,1) NOT NULL,
	[VoucherID] [int] NULL,
	[CampaignID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoucherCampaign]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherCampaign](
	[CampaignID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Description] [nvarchar](max) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[TargetAudience] [nvarchar](255) NULL,
	[Status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoucherRecipient]    Script Date: 11/14/2024 5:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherRecipient](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VoucherId] [int] NULL,
	[UserId] [int] NULL,
	[GroupId] [int] NULL,
	[IsUsed] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241105035404_UpdateCategories', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241105043955_UpdateProducts', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (1, N'Laptop', NULL, 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (2, N'Iphone', N'Iphone', 0, 4)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (3, N'Lenovo', N'Lenovo', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (4, N'Mobile phone', N'Mobile phone', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (5, N'Dresses', N'Dresses', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (6, N'T-shirts', N'T-shirts', 0, 9)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (7, N'Jeans', NULL, 0, 9)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (8, N'Bags', N'Bags', 0, 9)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (9, N'Fashion', N'Fashion', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (10, N'MacBook', N'MacBook', 0, 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (11, N'Table', NULL, 0, 12)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (12, N'Furniture', NULL, 0, NULL)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItems] ON 

INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (1, NULL, 5, 3, CAST(3000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (6, 6, 7, 1, CAST(250.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (7, 6, 8, 3, CAST(1200.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (8, 18, 5, 1, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (11, 21, 5, 1, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (12, 22, 5, 1, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (13, 23, 5, 1, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (14, 26, 8, 1, CAST(1200.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (15, 27, 8, 1, CAST(1200.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (16, 28, 5, 1, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (17, 29, 5, 1, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (18, 30, 7, 1, CAST(250.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (6, 1, CAST(N'2024-11-08T11:51:55.373' AS DateTime), N'Cancelled', NULL, N'Nguyen Thanh Loi', N'Vo van ngan', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (18, 14, CAST(N'2024-11-12T16:56:41.927' AS DateTime), N'Pending', CAST(0.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (21, 14, CAST(N'2024-11-12T17:05:38.480' AS DateTime), N'Confirmed', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (22, 1, CAST(N'2024-11-12T17:06:50.520' AS DateTime), N'Pending', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (23, 1, CAST(N'2024-11-12T17:12:31.250' AS DateTime), N'Pending', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (24, 1, CAST(N'2024-11-12T17:12:38.597' AS DateTime), N'Pending', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (25, 1, CAST(N'2024-11-12T17:12:42.830' AS DateTime), N'Pending', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (26, 1, CAST(N'2024-11-12T17:13:35.100' AS DateTime), N'Pending', CAST(1200.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (27, 14, CAST(N'2024-11-12T17:14:08.660' AS DateTime), N'Pending', CAST(1200.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (28, 1, CAST(N'2024-11-12T17:24:54.143' AS DateTime), N'Pending', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (29, 14, CAST(N'2024-11-12T17:26:14.173' AS DateTime), N'Pending', CAST(1000.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (30, 14, CAST(N'2024-11-12T17:27:16.383' AS DateTime), N'Pending', CAST(250.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount], [ReceverName], [Address], [PhoneNumber], [Note]) VALUES (31, 14, CAST(N'2024-11-12T17:27:26.447' AS DateTime), N'Pending', CAST(250.00 AS Decimal(18, 2)), N'Nguyen Thanh Loi', N'loi', N'0939603573', N'')
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderVoucher] ON 

INSERT [dbo].[OrderVoucher] ([OrderVoucherID], [OrderID], [VoucherID], [DiscountApplied]) VALUES (1, 1, 2, CAST(10.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[OrderVoucher] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (5, N'Nitro 5', 1, CAST(1000.00 AS Decimal(18, 2)), 3, CAST(N'2024-11-05T11:43:12.077' AS DateTime), 0, N'product-1.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (6, N'Iphone 15', 2, CAST(500.00 AS Decimal(18, 2)), 0, CAST(N'2024-11-05T11:43:28.967' AS DateTime), 0, N'product-2.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (7, N'Iphone13', 2, CAST(250.00 AS Decimal(18, 2)), 2, CAST(N'2024-11-05T11:57:02.433' AS DateTime), 0, N'product-3.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (8, N'MSI2', 1, CAST(1200.00 AS Decimal(18, 2)), 4, NULL, 0, N'product-4.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (9, N'MSI3', 1, CAST(1000.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-05T15:36:43.637' AS DateTime), 0, N'product-5.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (10, N'MSI4', 1, CAST(1000.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-07T10:52:19.440' AS DateTime), 0, N'product-6.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (11, N'Red Women Shirts ', 6, CAST(50.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-13T17:36:21.817' AS DateTime), 0, N'download (4).jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (12, N'Black Women Shirts ', 6, CAST(60.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-13T17:38:24.417' AS DateTime), 0, N'download (3).jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (13, N'Red Women Shirts ', 6, CAST(50.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-13T17:36:21.000' AS DateTime), 1, N'download (4).jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (16, N'Blue Men Shirts ', 6, CAST(30.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-14T10:29:46.277' AS DateTime), 0, N'download (2).jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (17, N'Black Men Shirts ', 6, CAST(30.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-14T10:30:42.233' AS DateTime), 0, N'OIP.jpg')
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RoleId], [Name]) VALUES (1, N'Admin')
INSERT [dbo].[Roles] ([RoleId], [Name]) VALUES (2, N'Customer')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[UserGroups] ON 

INSERT [dbo].[UserGroups] ([GroupId], [GroupName], [Description]) VALUES (1, N'Member', N'Member')
INSERT [dbo].[UserGroups] ([GroupId], [GroupName], [Description]) VALUES (2, N'VIP', N'VIP')
SET IDENTITY_INSERT [dbo].[UserGroups] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive], [GroupId], [RoleId], [LastUpdated]) VALUES (1, N'Loi', N'AQAAAAIAAYagAAAAEGXc1CbtUNVYzxgNGFgooG3Mgk0M7QN7yZ9zl0MtxeTN5Z0+RDLBKQmCWzTGWBeWYA==', N'loi@gmail.com', CAST(N'2024-11-05T15:10:26.260' AS DateTime), 1, 1, 2, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive], [GroupId], [RoleId], [LastUpdated]) VALUES (2, N'Thanh', N'AQAAAAIAAYagAAAAEGzdNxBQKHBWKNSEMZzPhGhxwxMtJacR1ERSBm9rCH3CcLbPbrPwmLd5DTN+Y8E4ng==', N'thanh@gmail.com', CAST(N'2024-11-05T16:19:32.753' AS DateTime), 1, 1, 2, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive], [GroupId], [RoleId], [LastUpdated]) VALUES (6, N'ns@gmail.com', N'AQAAAAIAAYagAAAAEEjoCioH5M+Kf64i1F4vehjYX+vpHnDN8OoaSNgLncThS+mSKq/TTke22xHX2qENbQ==', N'ns@gmail.com', CAST(N'2024-11-07T14:45:04.693' AS DateTime), 1, 1, 2, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive], [GroupId], [RoleId], [LastUpdated]) VALUES (7, N'Thien Thanh', N'AQAAAAIAAYagAAAAEDU0h9tJ0aw+hVqaaFW1x0eEb2reRmGoZM0KZfZJyRrH7z4/mIBerYQtBEJ5FtAsEw==', N'thienthanh@gmail.com', CAST(N'2024-11-08T09:03:15.850' AS DateTime), 1, 1, 2, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive], [GroupId], [RoleId], [LastUpdated]) VALUES (14, N'Lê Thái C', N'AQAAAAIAAYagAAAAEJrPsh3nA3rTmE3+X1XCuiffsIPr1NHuI9mi1Wt3YAexXys97iMgGmeqddi6sUyCBg==', N'loiloicong121@gmail.com', CAST(N'2024-11-11T17:49:19.030' AS DateTime), 1, 1, 2, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive], [GroupId], [RoleId], [LastUpdated]) VALUES (15, N'Test User', N'AQAAAAIAAYagAAAAEG7sADWJaPcBjMRIsUlQQK0meifeWsES+Yulz9mBSTFVpb1XOCSSYaAff75f8FSPYA==', N'test@gmail.com', CAST(N'2024-11-14T16:56:30.147' AS DateTime), 0, 1, 2, CAST(N'2024-11-14T17:34:53.440' AS DateTime))
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[Voucher] ON 

INSERT [dbo].[Voucher] ([VoucherID], [Code], [DiscountValue], [DiscountType], [ExpiryDate], [Status], [MaxUsage], [MaxPerUsage], [UsedCount]) VALUES (1, N'HELLO', CAST(10.00 AS Decimal(10, 2)), N'Percent', CAST(N'2024-11-08T09:58:31.060' AS DateTime), N'Inactive', 10, 1, 0)
INSERT [dbo].[Voucher] ([VoucherID], [Code], [DiscountValue], [DiscountType], [ExpiryDate], [Status], [MaxUsage], [MaxPerUsage], [UsedCount]) VALUES (2, N'HELLO2', CAST(10.00 AS Decimal(10, 2)), N'Percent', CAST(N'2024-12-08T17:19:55.610' AS DateTime), N'active', 10, 1, 0)
INSERT [dbo].[Voucher] ([VoucherID], [Code], [DiscountValue], [DiscountType], [ExpiryDate], [Status], [MaxUsage], [MaxPerUsage], [UsedCount]) VALUES (3, N'HELLO1', CAST(10.00 AS Decimal(10, 2)), N'Percent', CAST(N'2024-12-08T10:24:16.147' AS DateTime), N'active', 10, 1, 0)
SET IDENTITY_INSERT [dbo].[Voucher] OFF
GO
SET IDENTITY_INSERT [dbo].[VoucherCampaign] ON 

INSERT [dbo].[VoucherCampaign] ([CampaignID], [Name], [Description], [StartDate], [EndDate], [TargetAudience], [Status]) VALUES (1, N'Giam gia tet nguyen dan', N'Giam gia tet nguyen dan', CAST(N'2024-11-08T10:00:32.083' AS DateTime), CAST(N'2024-11-08T10:00:32.083' AS DateTime), N'All', N'Inactive')
SET IDENTITY_INSERT [dbo].[VoucherCampaign] OFF
GO
/****** Object:  Index [IX_Products_CategoryId]    Script Date: 11/14/2024 5:54:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Roles__737584F6D87809CB]    Script Date: 11/14/2024 5:54:43 PM ******/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__UserGrou__6EFCD434A03BE200]    Script Date: 11/14/2024 5:54:43 PM ******/
ALTER TABLE [dbo].[UserGroups] ADD UNIQUE NONCLUSTERED 
(
	[GroupName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4A0C2453D]    Script Date: 11/14/2024 5:54:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__536C85E4A0C2453D] ON [dbo].[Users]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D10534DEB8913B]    Script Date: 11/14/2024 5:54:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__A9D10534DEB8913B] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Voucher__A25C5AA7CE2B9C42]    Script Date: 11/14/2024 5:54:43 PM ******/
ALTER TABLE [dbo].[Voucher] ADD UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Stock]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT ('active') FOR [Status]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT ((0)) FOR [UsedCount]
GO
ALTER TABLE [dbo].[VoucherCampaign] ADD  DEFAULT ('active') FOR [Status]
GO
ALTER TABLE [dbo].[VoucherRecipient] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_ParentId] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_ParentId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[OrderVoucher]  WITH CHECK ADD FOREIGN KEY([VoucherID])
REFERENCES [dbo].[Voucher] ([VoucherID])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK__Products__Catego__2B3F6F97] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK__Products__Catego__2B3F6F97]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_User_Role]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[UserGroups] ([GroupId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserGroups]
GO
ALTER TABLE [dbo].[VoucherAssignment]  WITH CHECK ADD FOREIGN KEY([CampaignID])
REFERENCES [dbo].[VoucherCampaign] ([CampaignID])
GO
ALTER TABLE [dbo].[VoucherAssignment]  WITH CHECK ADD FOREIGN KEY([VoucherID])
REFERENCES [dbo].[Voucher] ([VoucherID])
GO
ALTER TABLE [dbo].[VoucherRecipient]  WITH CHECK ADD FOREIGN KEY([GroupId])
REFERENCES [dbo].[UserGroups] ([GroupId])
GO
ALTER TABLE [dbo].[VoucherRecipient]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[VoucherRecipient]  WITH CHECK ADD FOREIGN KEY([VoucherId])
REFERENCES [dbo].[Voucher] ([VoucherID])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
USE [master]
GO
ALTER DATABASE [Product_Category] SET  READ_WRITE 
GO
