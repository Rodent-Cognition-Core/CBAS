USE [Cogbytes]
GO
/****** Object:  Table [dbo].[RepPI]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RepPI](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PIID] [int] NOT NULL,
	[RepID] [int] NOT NULL,
 CONSTRAINT [PK__RepPI__3214EC271FB504A3] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RepAuthor]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RepAuthor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AuthorOrder] [int] NULL,
	[AuthorID] [int] NOT NULL,
	[RepID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetStrain]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetStrain](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StrainID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetGeno]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetGeno](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GenoID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetAge]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetAge](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AgeID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetSex]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetSex](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SexID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetTask]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetTask](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TaskID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetSpecies]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetSpecies](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SpeciesID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRepository]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRepository](
	[RepID] [int] IDENTITY(1,1) NOT NULL,
	[RepoLinkGuid] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Date] [date] NOT NULL,
	[DOI] [nvarchar](max) NULL,
	[Keywords] [nvarchar](max) NULL,
	[PrivacyStatus] [bit] NOT NULL,
	[Description] [text] NULL,
	[AdditionalNotes] [text] NULL,
	[Link] [nvarchar](max) NULL,
	[Username] [nvarchar](100) NOT NULL,
	[DateRepositoryCreated] [date] NULL,
	[DataCiteURL] [nvarchar](200) NULL,
 CONSTRAINT [PK__UserRepo__2F6FA9ABEB91AACF] PRIMARY KEY CLUSTERED 
(
	[RepID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Upload]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Upload](
	[UploadID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DateUpload] [date] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[AdditionalNotes] [nvarchar](max) NULL,
	[IsIntervention] [bit] NULL,
	[InterventionDescription] [nvarchar](max) NULL,
	[ImageIds] [nvarchar](100) NULL,
	[ImageDescription] [nvarchar](max) NULL,
	[Housing] [nvarchar](100) NULL,
	[LightCycle] [nvarchar](100) NULL,
	[TaskBattery] [nvarchar](max) NOT NULL,
	[NumSubjects] [int] NULL,
	[RepID] [int] NOT NULL,
	[FileTypeID] [int] NOT NULL,
 CONSTRAINT [PK__Upload__6D16C86D61E77ADF] PRIMARY KEY CLUSTERED 
(
	[UploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[SearchCog]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SearchCog]
AS
SELECT        dbo.Upload.UploadID, dbo.Upload.Name, dbo.Upload.DateUpload, dbo.Upload.Description, dbo.Upload.AdditionalNotes, dbo.Upload.IsIntervention, dbo.Upload.InterventionDescription, dbo.Upload.ImageIds, 
                         dbo.Upload.ImageDescription, dbo.Upload.Housing, dbo.Upload.LightCycle, dbo.Upload.TaskBattery, dbo.Upload.RepID, dbo.Upload.FileTypeID, dbo.UserRepository.Title, dbo.UserRepository.Date, 
                         dbo.UserRepository.Keywords, dbo.UserRepository.DOI, dbo.RepAuthor.AuthorID, dbo.RepPI.PIID, dbo.DatasetTask.TaskID, dbo.DatasetSpecies.SpeciesID, dbo.DatasetSex.SexID, dbo.DatasetStrain.StrainID, 
                         dbo.DatasetGeno.GenoID, dbo.DatasetAge.AgeID, dbo.Upload.NumSubjects
FROM            dbo.Upload INNER JOIN
                         dbo.UserRepository ON dbo.Upload.RepID = dbo.UserRepository.RepID INNER JOIN
                         dbo.RepAuthor ON dbo.UserRepository.RepID = dbo.RepAuthor.RepID LEFT OUTER JOIN
                         dbo.RepPI ON dbo.UserRepository.RepID = dbo.RepPI.RepID LEFT OUTER JOIN
                         dbo.DatasetTask ON dbo.Upload.UploadID = dbo.DatasetTask.UploadID LEFT OUTER JOIN
                         dbo.DatasetSpecies ON dbo.Upload.UploadID = dbo.DatasetSpecies.UploadID LEFT OUTER JOIN
                         dbo.DatasetSex ON dbo.Upload.UploadID = dbo.DatasetSex.UploadID LEFT OUTER JOIN
                         dbo.DatasetStrain ON dbo.Upload.UploadID = dbo.DatasetStrain.UploadID LEFT OUTER JOIN
                         dbo.DatasetGeno ON dbo.Upload.UploadID = dbo.DatasetGeno.GenoID LEFT OUTER JOIN
                         dbo.DatasetAge ON dbo.Upload.UploadID = dbo.DatasetAge.UploadID
GO
/****** Object:  Table [dbo].[Age]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Age](
	[AgeID] [int] IDENTITY(1,1) NOT NULL,
	[AgeInMonth] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AgeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Author]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Author](
	[AuthorID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Affiliation] [nvarchar](max) NULL,
	[Username] [nvarchar](100) NULL,
 CONSTRAINT [PK__Author__70DAFC141FF78043] PRIMARY KEY CLUSTERED 
(
	[AuthorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileType]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileType](
	[FileTypeID] [int] IDENTITY(1,1) NOT NULL,
	[FileType] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FileTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Genotype]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Genotype](
	[GenoID] [int] IDENTITY(1,1) NOT NULL,
	[Genotype] [nvarchar](100) NOT NULL,
	[Link] [nvarchar](max) NULL,
	[Description] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[GenoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PI]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PI](
	[PIID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](100) NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Affiliation] [nvarchar](max) NULL,
	[Email] [nvarchar](100) NULL,
 CONSTRAINT [PK__PI__5F86BE60D12C588B] PRIMARY KEY CLUSTERED 
(
	[PIID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sex]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sex](
	[SexID] [int] IDENTITY(1,1) NOT NULL,
	[Sex] [char](1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SexID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Species]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Species](
	[SpeciesID] [int] IDENTITY(1,1) NOT NULL,
	[Species] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SpeciesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Strain]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Strain](
	[StrainID] [int] IDENTITY(1,1) NOT NULL,
	[Strain] [nvarchar](100) NOT NULL,
	[DiseaseModel] [nvarchar](100) NULL,
	[Link] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[StrainID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Task]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[TaskID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[OriginalName] [nvarchar](100) NULL,
	[ShortName] [nvarchar](100) NULL,
	[TaskDescription] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[SearchCog2]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SearchCog2]
AS
SELECT        UserRepository.RepID, RepoLinkGuid, Upload.UploadID, Title, Date, DOI, Keywords, PrivacyStatus, UserRepository.Description, UserRepository.AdditionalNotes, Link, Username, DateRepositoryCreated, 
                         STUFF
                             ((SELECT        ', ' + CONCAT(Author.FirstName, '-', Author.LastName)
                                 FROM            RepAuthor INNER JOIN
                                                          Author ON Author.AuthorID = RepAuthor.AuthorID
                                 WHERE        RepAuthor.RepID = UserRepository.RepID
                                 ORDER BY AuthorOrder FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Author, STUFF
                             ((SELECT        ', ' + PI.FullName
                                 FROM            RepPI INNER JOIN
                                                          PI ON PI.PIID = RepPI.PIID
                                 WHERE        RepPI.RepID = UserRepository.RepID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS PI, Upload.Name AS UploadName, Upload.DateUpload, 
                         Upload.Description AS UploadDescription, Upload.AdditionalNotes AS UploadAdditionalNotes, /*UploadFile.PermanentFilePath,*/ Upload.IsIntervention, Upload.InterventionDescription, Upload.ImageIds, 
                         Upload.ImageDescription, Upload.Housing, Upload.LightCycle, Upload.TaskBattery, Upload.NumSubjects, FileType.FileType, STUFF
                             ((SELECT DISTINCT ', ' + Task.ShortName
                                 FROM            DatasetTask INNER JOIN
                                                          Task ON Task.TaskID = DatasetTask.TaskID
                                 WHERE        DatasetTask.UploadID = Upload.UploadID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Task, STUFF
                             ((SELECT DISTINCT ', ' + Species.Species
                                 FROM            DatasetSpecies INNER JOIN
                                                          Species ON Species.SpeciesID = DatasetSpecies.SpeciesID
                                 WHERE        DatasetSpecies.UploadID = Upload.UploadID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Species, STUFF
                             ((SELECT DISTINCT ', ' + sex.sex
                                 FROM            DatasetSex INNER JOIN
                                                          Sex ON sex.sexID = DatasetSex.sexID
                                 WHERE        DatasetSex.UploadID = Upload.UploadID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Sex, STUFF
                             ((SELECT DISTINCT ', ' + Strain.Strain
                                 FROM            DatasetStrain INNER JOIN
                                                          Strain ON Strain.StrainID = DatasetStrain.StrainID
                                 WHERE        DatasetStrain.UploadID = Upload.UploadID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Strain, STUFF
                             ((SELECT DISTINCT ', ' + Genotype.Genotype
                                 FROM            DatasetGeno INNER JOIN
                                                          Genotype ON Genotype.GenoID = DatasetGeno.GenoID
                                 WHERE        DatasetGeno.UploadID = Upload.UploadID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Genotype, STUFF
                             ((SELECT DISTINCT ', ' + Age.AgeInMonth
                                 FROM            DatasetAge INNER JOIN
                                                          Age ON Age.AgeID = DatasetAge.AgeID
                                 WHERE        DatasetAge.UploadID = Upload.UploadID FOR XML PATH(''), type ).value('.', 'nvarchar(max)'), 1, 2, '') AS Age
FROM            UserRepository INNER JOIN
                         Upload ON Upload.RepID = UserRepository.RepID INNER JOIN
                         FileType ON FileType.FileTypeID = Upload.FileTypeID
GO
/****** Object:  Table [dbo].[Image]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ImagePath] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UploadFile]    Script Date: 4/23/2025 11:42:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UploadFile](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserFileName] [nvarchar](max) NOT NULL,
	[SystemFileName] [nvarchar](max) NOT NULL,
	[DateUploaded] [date] NOT NULL,
	[DateFileCreated] [date] NOT NULL,
	[FileSize] [int] NOT NULL,
	[PermanentFilePath] [nvarchar](max) NOT NULL,
	[UploadID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserRepository] ADD  CONSTRAINT [DF__UserRepos__RepoL__70DDC3D8]  DEFAULT (newid()) FOR [RepoLinkGuid]
GO
ALTER TABLE [dbo].[DatasetAge]  WITH CHECK ADD FOREIGN KEY([AgeID])
REFERENCES [dbo].[Age] ([AgeID])
GO
ALTER TABLE [dbo].[DatasetAge]  WITH CHECK ADD  CONSTRAINT [FK__DatasetAg__Uploa__5FB337D6] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[DatasetAge] CHECK CONSTRAINT [FK__DatasetAg__Uploa__5FB337D6]
GO
ALTER TABLE [dbo].[DatasetGeno]  WITH CHECK ADD FOREIGN KEY([GenoID])
REFERENCES [dbo].[Genotype] ([GenoID])
GO
ALTER TABLE [dbo].[DatasetGeno]  WITH CHECK ADD  CONSTRAINT [FK__DatasetGe__Uploa__5BE2A6F2] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[DatasetGeno] CHECK CONSTRAINT [FK__DatasetGe__Uploa__5BE2A6F2]
GO
ALTER TABLE [dbo].[DatasetSex]  WITH CHECK ADD FOREIGN KEY([SexID])
REFERENCES [dbo].[Sex] ([SexID])
GO
ALTER TABLE [dbo].[DatasetSex]  WITH CHECK ADD  CONSTRAINT [FK__DatasetSe__Uploa__6383C8BA] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[DatasetSex] CHECK CONSTRAINT [FK__DatasetSe__Uploa__6383C8BA]
GO
ALTER TABLE [dbo].[DatasetSpecies]  WITH CHECK ADD FOREIGN KEY([SpeciesID])
REFERENCES [dbo].[Species] ([SpeciesID])
GO
ALTER TABLE [dbo].[DatasetSpecies]  WITH CHECK ADD  CONSTRAINT [FK__DatasetSp__Uploa__6B24EA82] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[DatasetSpecies] CHECK CONSTRAINT [FK__DatasetSp__Uploa__6B24EA82]
GO
ALTER TABLE [dbo].[DatasetStrain]  WITH CHECK ADD FOREIGN KEY([StrainID])
REFERENCES [dbo].[Strain] ([StrainID])
GO
ALTER TABLE [dbo].[DatasetStrain]  WITH CHECK ADD  CONSTRAINT [FK__DatasetSt__Uploa__5812160E] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[DatasetStrain] CHECK CONSTRAINT [FK__DatasetSt__Uploa__5812160E]
GO
ALTER TABLE [dbo].[DatasetTask]  WITH CHECK ADD FOREIGN KEY([TaskID])
REFERENCES [dbo].[Task] ([TaskID])
GO
ALTER TABLE [dbo].[DatasetTask]  WITH CHECK ADD  CONSTRAINT [FK__DatasetTa__Uploa__6754599E] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[DatasetTask] CHECK CONSTRAINT [FK__DatasetTa__Uploa__6754599E]
GO
ALTER TABLE [dbo].[RepAuthor]  WITH CHECK ADD  CONSTRAINT [FK__RepAuthor__Autho__4F7CD00D] FOREIGN KEY([AuthorID])
REFERENCES [dbo].[Author] ([AuthorID])
GO
ALTER TABLE [dbo].[RepAuthor] CHECK CONSTRAINT [FK__RepAuthor__Autho__4F7CD00D]
GO
ALTER TABLE [dbo].[RepAuthor]  WITH CHECK ADD  CONSTRAINT [FK__RepAuthor__RepID__6B24EA82] FOREIGN KEY([RepID])
REFERENCES [dbo].[UserRepository] ([RepID])
GO
ALTER TABLE [dbo].[RepAuthor] CHECK CONSTRAINT [FK__RepAuthor__RepID__6B24EA82]
GO
ALTER TABLE [dbo].[RepPI]  WITH CHECK ADD  CONSTRAINT [FK__RepPI__PIID__2180FB33] FOREIGN KEY([PIID])
REFERENCES [dbo].[PI] ([PIID])
GO
ALTER TABLE [dbo].[RepPI] CHECK CONSTRAINT [FK__RepPI__PIID__2180FB33]
GO
ALTER TABLE [dbo].[RepPI]  WITH CHECK ADD  CONSTRAINT [FK__RepPI__RepID__22751F6C] FOREIGN KEY([RepID])
REFERENCES [dbo].[UserRepository] ([RepID])
GO
ALTER TABLE [dbo].[RepPI] CHECK CONSTRAINT [FK__RepPI__RepID__22751F6C]
GO
ALTER TABLE [dbo].[Upload]  WITH CHECK ADD  CONSTRAINT [FK__Upload__FileType__5441852A] FOREIGN KEY([FileTypeID])
REFERENCES [dbo].[FileType] ([FileTypeID])
GO
ALTER TABLE [dbo].[Upload] CHECK CONSTRAINT [FK__Upload__FileType__5441852A]
GO
ALTER TABLE [dbo].[Upload]  WITH CHECK ADD  CONSTRAINT [FK__Upload__RepID__534D60F1] FOREIGN KEY([RepID])
REFERENCES [dbo].[UserRepository] ([RepID])
GO
ALTER TABLE [dbo].[Upload] CHECK CONSTRAINT [FK__Upload__RepID__534D60F1]
GO
ALTER TABLE [dbo].[UploadFile]  WITH CHECK ADD  CONSTRAINT [FK__UploadFil__Uploa__3C34F16F] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[UploadFile] CHECK CONSTRAINT [FK__UploadFil__Uploa__3C34F16F]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[27] 4[12] 2[43] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Upload"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 298
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "UserRepository"
            Begin Extent = 
               Top = 7
               Left = 346
               Bottom = 170
               Right = 596
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RepAuthor"
            Begin Extent = 
               Top = 7
               Left = 644
               Bottom = 170
               Right = 838
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RepPI"
            Begin Extent = 
               Top = 7
               Left = 886
               Bottom = 148
               Right = 1080
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DatasetTask"
            Begin Extent = 
               Top = 7
               Left = 1128
               Bottom = 148
               Right = 1322
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DatasetSpecies"
            Begin Extent = 
               Top = 154
               Left = 886
               Bottom = 295
               Right = 1080
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DatasetSex"
            Begin Extent = 
               Top = 154
               Left = 1128
               Bottom = 295
               Right = 1322
            End
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SearchCog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DatasetStrain"
            Begin Extent = 
               Top = 175
               Left = 48
               Bottom = 316
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DatasetGeno"
            Begin Extent = 
               Top = 175
               Left = 290
               Bottom = 316
               Right = 484
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DatasetAge"
            Begin Extent = 
               Top = 175
               Left = 532
               Bottom = 316
               Right = 726
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SearchCog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SearchCog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SearchCog2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SearchCog2'
GO
