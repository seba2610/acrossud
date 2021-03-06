
ALTER DATABASE [acrossud_v1] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [acrossud_v1].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [acrossud_v1] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [acrossud_v1] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [acrossud_v1] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [acrossud_v1] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [acrossud_v1] SET ARITHABORT OFF 
GO
ALTER DATABASE [acrossud_v1] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [acrossud_v1] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [acrossud_v1] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [acrossud_v1] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [acrossud_v1] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [acrossud_v1] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [acrossud_v1] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [acrossud_v1] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [acrossud_v1] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [acrossud_v1] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [acrossud_v1] SET  DISABLE_BROKER 
GO
ALTER DATABASE [acrossud_v1] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [acrossud_v1] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [acrossud_v1] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [acrossud_v1] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [acrossud_v1] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [acrossud_v1] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [acrossud_v1] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [acrossud_v1] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [acrossud_v1] SET  MULTI_USER 
GO
ALTER DATABASE [acrossud_v1] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [acrossud_v1] SET DB_CHAINING OFF 
GO
ALTER DATABASE [acrossud_v1] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [acrossud_v1] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [acrossud_v1]
GO
/****** Object:  StoredProcedure [dbo].[AddEntityProperty]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddEntityProperty]
    @EntityId int,
	@PropertyId int,
	@PropertyValue nvarchar(MAX)
AS   
    INSERT INTO EntityProperty(EntityId,PropertyId,Value) VALUES(@EntityId, @PropertyId, @PropertyValue)
	SELECT @@IDENTITY;

GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdateEntityProperty]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddOrUpdateEntityProperty]
    @EntityId int,
	@PropertyId int,
	@Value nvarchar(MAX)
AS   
	declare @ep_id int = -1

	set @ep_id = (SELECT TOP(1) Id FROM EntityProperty WHERE EntityId = @EntityId AND PropertyId = @PropertyId);

	if (@ep_id <> -1)
		BEGIN
			UPDATE EntityProperty
			SET Value =  @Value
			WHERE Id = @ep_id;

			SELECT @ep_id;
		END
	ELSE
		BEGIN
			INSERT INTO EntityProperty(EntityId,PropertyId,Value) VALUES(@EntityId, @PropertyId, @Value)
			SELECT @@IDENTITY;
		END


GO
/****** Object:  StoredProcedure [dbo].[CreateEntity]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateEntity]
	@Name nvarchar(500),   
    @Description nvarchar(MAX)
AS   
    INSERT INTO Entity(Name,Description) VALUES(@Name, @Description)
	SELECT @@IDENTITY;

GO
/****** Object:  StoredProcedure [dbo].[DeleteEntity]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteEntity]
	@EntityId int
AS   
    SET NOCOUNT ON; 

	BEGIN TRAN
		DELETE FROM EntityProperty WHERE EntityId = @EntityId
		DELETE FROM Entity WHERE Id = @EntityId
		SELECT @@ROWCOUNT
	COMMIT TRAN

GO
/****** Object:  StoredProcedure [dbo].[DeleteEntityPropertyByName]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteEntityPropertyByName]
	@EntityId int,
	@PropertyName nvarchar(50)
AS   
    SET NOCOUNT ON; 

	BEGIN TRAN

		declare @property_id int = (SELECT TOP(1) Id FROM Property WHERE Name = @PropertyName);
		DELETE FROM EntityProperty WHERE EntityId = @EntityId AND PropertyId = @property_id;
		SELECT @@ROWCOUNT
	COMMIT TRAN

GO
/****** Object:  StoredProcedure [dbo].[GetEntities]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEntities]
AS   
    SET NOCOUNT ON;  
    SELECT *
    FROM Entity

GO
/****** Object:  StoredProcedure [dbo].[GetEntitiesFiltered]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEntitiesFiltered]
@PropertyName nvarchar(50),
@PropertyOperator nvarchar(30),
@Value nvarchar(max)
AS   

declare  @property_type nvarchar(50) = (SELECT Top(1) [Property].[Type] From Property WHERE Name = @PropertyName);

IF (@property_type = 'Bool')
BEGIN
	IF (@PropertyOperator = 'Equal')
		BEGIN
			SELECT * 
			FROM Entity as E, EntityProperty as Ep, Property as P
			WHERE Ep.PropertyId = P.Id AND P.Name = @PropertyName AND Ep.EntityId = E.Id AND Ep.Value = @Value
		END
END
GO
/****** Object:  StoredProcedure [dbo].[GetEntity]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEntity]
	@EntityId int
AS   
    SET NOCOUNT ON;  
    SELECT *
    FROM Entity
	WHERE Id = @EntityId

GO
/****** Object:  StoredProcedure [dbo].[GetEntityProperties]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEntityProperties]
	@EntityId int
AS   
    SET NOCOUNT ON;  
    SELECT EP.Value as PropertyValue, P.Name as PropertyName, P.Description as PropertyDescription, P.Type as PropertyType, P.[Order] as PropertyOrder
    FROM Entity as E, EntityProperty as EP, Property as P
	WHERE EP.EntityId = @EntityId AND EP.PropertyId = P.Id
	ORDER BY P.[Order] ASC

GO
/****** Object:  StoredProcedure [dbo].[GetProperties]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetProperties]
AS   
    SET NOCOUNT ON;  
    SELECT *
    FROM Property
	ORDER BY [Order] asc

GO
/****** Object:  StoredProcedure [dbo].[GetPropertyByName]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPropertyByName]
@Name nvarchar(50)
AS   
    SET NOCOUNT ON;  
    SELECT *
    FROM Property
	WHERE Name = @Name

GO
/****** Object:  StoredProcedure [dbo].[GetValueOfPropertiesByEntityId]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetValueOfPropertiesByEntityId]
	@EntityId int
AS   
    SET NOCOUNT ON;  
    SELECT P.Id, P.Name, P.Type, P.Description, P.[Order], EP.Value as Value
	FROM Property as P, EntityProperty as EP
    WHERE EP.EntityId = @EntityId AND EP.PropertyId = P.Id
	ORDER BY P.[Order] ASC

GO
/****** Object:  StoredProcedure [dbo].[GetValueOfPropertyByName]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetValueOfPropertyByName]
	@EntityId int,
	@PropertyName nvarchar(50)
AS   
    SET NOCOUNT ON;  
    SELECT  Top(1) P.Id, P.Name, P.Type, P.Description, P.[Order], EP.Value as Value
	FROM Property as P, EntityProperty as EP
    WHERE P.Name = @PropertyName AND EP.EntityId = @EntityId AND EP.PropertyId = P.Id

GO
/****** Object:  StoredProcedure [dbo].[UpdateEntity]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateEntity]
	@EntityId int,
	@Name nvarchar(500),
	@Description nvarchar(max)
AS   
    SET NOCOUNT ON;  
    UPDATE Entity
	SET Name =  @Name, Description = @Description
	WHERE Id = @EntityId
	SELECT @EntityId

GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Entity]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Entity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_entity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EntityProperty]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntityProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[PropertyId] [int] NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_EntityProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Property]    Script Date: 07/11/2016 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Property](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [RoleNameIndex]    Script Date: 07/11/2016 16:42:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 07/11/2016 16:42:53 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 07/11/2016 16:42:53 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_RoleId]    Script Date: 07/11/2016 16:42:53 ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 07/11/2016 16:42:53 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UserNameIndex]    Script Date: 07/11/2016 16:42:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[EntityProperty]  WITH CHECK ADD  CONSTRAINT [Entity-EntityProperty] FOREIGN KEY([EntityId])
REFERENCES [dbo].[Entity] ([Id])
GO
ALTER TABLE [dbo].[EntityProperty] CHECK CONSTRAINT [Entity-EntityProperty]
GO
ALTER TABLE [dbo].[EntityProperty]  WITH CHECK ADD  CONSTRAINT [Property-EntityProperty] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Property] ([Id])
GO
ALTER TABLE [dbo].[EntityProperty] CHECK CONSTRAINT [Property-EntityProperty]
GO
USE [master]
GO
ALTER DATABASE [acrossud_v1] SET  READ_WRITE 
GO
