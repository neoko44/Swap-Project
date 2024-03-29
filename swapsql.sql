USE [master]
GO
/****** Object:  Database [SwapProject]    Script Date: 21.04.2023 11:40:44 ******/
CREATE DATABASE [SwapProject]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SwapProject', FILENAME = N'C:\Users\alperen.kocabalkan\SwapProject.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SwapProject_log', FILENAME = N'C:\Users\alperen.kocabalkan\SwapProject_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [SwapProject] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SwapProject].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SwapProject] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SwapProject] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SwapProject] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SwapProject] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SwapProject] SET ARITHABORT OFF 
GO
ALTER DATABASE [SwapProject] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SwapProject] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SwapProject] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SwapProject] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SwapProject] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SwapProject] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SwapProject] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SwapProject] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SwapProject] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SwapProject] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SwapProject] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SwapProject] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SwapProject] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SwapProject] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SwapProject] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SwapProject] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SwapProject] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SwapProject] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SwapProject] SET  MULTI_USER 
GO
ALTER DATABASE [SwapProject] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SwapProject] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SwapProject] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SwapProject] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SwapProject] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SwapProject] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [SwapProject] SET QUERY_STORE = OFF
GO
USE [SwapProject]
GO
/****** Object:  Table [dbo].[BuyOrders]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuyOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CryptoId] [int] NULL,
	[Parity] [varchar](10) NULL,
	[ParityPrice] [decimal](18, 8) NULL,
	[Price] [decimal](18, 8) NULL,
	[Amount] [decimal](18, 8) NULL,
	[Total] [decimal](18, 8) NULL,
	[Type] [varchar](10) NULL,
	[CommissionFee] [decimal](18, 8) NULL,
	[Collected] [decimal](18, 8) NULL,
	[Status] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_BuyOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyWallets]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyWallets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CryptoId] [varchar](50) NULL,
	[Total] [decimal](18, 8) NULL,
 CONSTRAINT [PK_Commisions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cryptos]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cryptos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](10) NULL,
	[Status] [bit] NULL,
	[Commission] [numeric](18, 8) NULL,
 CONSTRAINT [PK_Cryptos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operations]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_Operations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SellOrders]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SellOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CryptoId] [int] NULL,
	[Parity] [varchar](10) NULL,
	[ParityPrice] [decimal](18, 8) NULL,
	[Price] [decimal](18, 8) NULL,
	[Amount] [decimal](18, 8) NULL,
	[Total] [decimal](18, 8) NULL,
	[Type] [varchar](10) NULL,
	[CommissionFee] [decimal](18, 8) NULL,
	[Collected] [decimal](18, 8) NULL,
	[Status] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_SellOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserOperations]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOperations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[OperationId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_UserOperations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Phone] [varchar](50) NULL,
	[Address] [varchar](500) NULL,
	[PasswordSalt] [varbinary](500) NULL,
	[PasswordHash] [varbinary](500) NULL,
	[Status] [bit] NULL,
	[RoleId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallets]    Script Date: 21.04.2023 11:40:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Type] [varchar](10) NULL,
	[Balance] [decimal](18, 8) NULL,
	[Status] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [SwapProject] SET  READ_WRITE 
GO
