USE [master]
GO
/****** Object:  Database [Product_Category]    Script Date: 11/6/2024 6:00:41 PM ******/
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/6/2024 6:00:41 PM ******/
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
/****** Object:  Table [dbo].[Cart]    Script Date: 11/6/2024 6:00:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cart](
	[CartId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Total] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[CartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartItems]    Script Date: 11/6/2024 6:00:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CartId] [int] NULL,
	[ProductId] [int] NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 11/6/2024 6:00:41 PM ******/
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
/****** Object:  Table [dbo].[OrderItems]    Script Date: 11/6/2024 6:00:41 PM ******/
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
/****** Object:  Table [dbo].[Orders]    Script Date: 11/6/2024 6:00:41 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/6/2024 6:00:41 PM ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 11/6/2024 6:00:41 PM ******/
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
 CONSTRAINT [PK__Users__1788CC4C9FA369D5] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241105035404_UpdateCategories', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241105043955_UpdateProducts', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Cart] ON 

INSERT [dbo].[Cart] ([CartId], [UserId], [CreatedDate], [ModifiedDate], [Total]) VALUES (1, 1, CAST(N'2024-11-05T21:54:06.553' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Cart] OFF
GO
SET IDENTITY_INSERT [dbo].[CartItems] ON 

INSERT [dbo].[CartItems] ([CartItemId], [CartId], [ProductId], [Quantity], [Price]) VALUES (8, 1, 7, 1, CAST(250.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[CartItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (1, N'Laptop', N'Laptop', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (2, N'Iphone', N'Iphone', 0, 4)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (3, N'Lenovo', N'Lenovo', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (4, N'Mobile phone', N'Mobile phone', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (5, N'Dresses', N'Dresses', 0, NULL)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (6, N'T-shirts', N'T-shirts', 0, 9)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (7, N'Jeans', N'Jeans', 0, 9)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (8, N'Bags', N'Bags', 0, 9)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [IsDeleted], [ParentId]) VALUES (9, N'Fashion', N'Fashion', 0, NULL)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItems] ON 

INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (1, NULL, 5, 3, CAST(3000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (2, 2, 5, 3, CAST(1000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [Quantity], [Price]) VALUES (3, 2, 7, 1, CAST(250.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount]) VALUES (1, 1, CAST(N'2024-11-06T11:34:20.120' AS DateTime), N'Completed', NULL)
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount]) VALUES (2, 1, CAST(N'2024-11-06T11:49:15.887' AS DateTime), N'Pending', CAST(3250.00 AS Decimal(18, 2)))
INSERT [dbo].[Orders] ([OrderId], [UserId], [OrderDate], [Status], [TotalAmount]) VALUES (3, 1, CAST(N'2024-11-06T11:55:05.660' AS DateTime), N'Pending', CAST(250.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (5, N'Nitro 5', 1, CAST(1000.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-05T11:43:12.077' AS DateTime), NULL, N'product-1.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (6, N'Iphone 15', 2, CAST(500.00 AS Decimal(18, 2)), 0, CAST(N'2024-11-05T11:43:28.967' AS DateTime), NULL, N'product-2.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (7, N'Iphone13', 2, CAST(250.00 AS Decimal(18, 2)), 5, CAST(N'2024-11-05T11:57:02.433' AS DateTime), 1, N'product-3.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (8, N'MSI2', 1, CAST(1200.00 AS Decimal(18, 2)), 9, NULL, 1, N'product-4.jpg')
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryId], [Price], [Stock], [CreatedAt], [IsDeleted], [ImgName]) VALUES (9, N'MSI3', 1, CAST(1000.00 AS Decimal(18, 2)), 10, CAST(N'2024-11-05T15:36:43.637' AS DateTime), 0, N'product-5.jpg')
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive]) VALUES (1, N'Loi', N'AQAAAAIAAYagAAAAEE/DQ4JZMT2LIFxWLSi8lCBNfwtCmzi/toIwzdeDxi+/cugt5QHgBsqPm8zkPbSqCg==', N'loi@gmail.com', CAST(N'2024-11-05T15:10:26.260' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive]) VALUES (2, N'Thanh', N'AQAAAAIAAYagAAAAEGzdNxBQKHBWKNSEMZzPhGhxwxMtJacR1ERSBm9rCH3CcLbPbrPwmLd5DTN+Y8E4ng==', N'thanh@gmail.com', CAST(N'2024-11-05T16:19:32.753' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [CreatedAt], [IsActive]) VALUES (5, N'LoiS', N'AQAAAAIAAYagAAAAEEuOjJve9Bh646LvY8/L9ddiNZIX82Rolxttyc238cqJffEKKa4VvU+Q4KhzqigwFg==', N'loiloicong121@gmail.com', CAST(N'2024-11-06T15:37:30.297' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [IX_Products_CategoryId]    Script Date: 11/6/2024 6:00:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4A0C2453D]    Script Date: 11/6/2024 6:00:41 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__536C85E4A0C2453D] ON [dbo].[Users]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D10534DEB8913B]    Script Date: 11/6/2024 6:00:41 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__A9D10534DEB8913B] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cart] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Cart] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
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
ALTER TABLE [dbo].[Cart]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD FOREIGN KEY([CartId])
REFERENCES [dbo].[Cart] ([CartId])
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
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
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK__Products__Catego__2B3F6F97] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK__Products__Catego__2B3F6F97]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
USE [master]
GO
ALTER DATABASE [Product_Category] SET  READ_WRITE 
GO
