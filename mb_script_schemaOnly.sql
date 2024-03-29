USE [master]
GO
/****** Object:  Database [Mousebytes]    Script Date: 2023-10-15 1:08:41 PM ******/
CREATE DATABASE [Mousebytes]
 CONTAINMENT = NONE
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Mousebytes].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Mousebytes] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Mousebytes] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Mousebytes] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Mousebytes] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Mousebytes] SET ARITHABORT OFF 
GO
ALTER DATABASE [Mousebytes] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Mousebytes] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Mousebytes] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Mousebytes] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Mousebytes] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Mousebytes] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Mousebytes] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Mousebytes] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Mousebytes] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Mousebytes] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Mousebytes] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Mousebytes] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Mousebytes] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Mousebytes] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Mousebytes] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Mousebytes] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Mousebytes] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Mousebytes] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Mousebytes] SET  MULTI_USER 
GO
ALTER DATABASE [Mousebytes] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Mousebytes] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Mousebytes] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Mousebytes] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Mousebytes] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Mousebytes] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Mousebytes', N'ON'
GO
ALTER DATABASE [Mousebytes] SET QUERY_STORE = OFF
GO
USE [Mousebytes]
GO
/****** Object:  UserDefinedFunction [dbo].[CSVToTable]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[CSVToTable] (@InStr VARCHAR(MAX))
RETURNS @TempTab TABLE
   (id int not null)
AS
BEGIN
    ;-- Ensure input ends with comma
	SET @InStr = REPLACE(@InStr + ',', ',,', ',')
	DECLARE @SP INT
DECLARE @VALUE VARCHAR(1000)
WHILE PATINDEX('%,%', @INSTR ) <> 0 
BEGIN
   SELECT  @SP = PATINDEX('%,%',@INSTR)
   SELECT  @VALUE = LEFT(@INSTR , @SP - 1)
   SELECT  @INSTR = STUFF(@INSTR, 1, @SP, '')
   INSERT INTO @TempTab(id) VALUES (@VALUE)
END
	RETURN
END
GO
/****** Object:  Table [dbo].[Site]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site](
	[SiteID] [int] IDENTITY(1,1) NOT NULL,
	[Institution] [nvarchar](max) NULL,
	[Country] [nvarchar](50) NULL,
 CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED 
(
	[SiteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PI]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PI](
	[PID] [int] IDENTITY(1,1) NOT NULL,
	[PName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PI] PRIMARY KEY CLUSTERED 
(
	[PID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PISite]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PISite](
	[PSID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NOT NULL,
	[SiteID] [int] NOT NULL,
 CONSTRAINT [PK_PISite] PRIMARY KEY CLUSTERED 
(
	[PSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[View_PI_Site]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_PI_Site]
AS
SELECT        dbo.PISite.PID, dbo.PI.PName, dbo.Site.Institution, dbo.Site.Country
FROM            dbo.PISite INNER JOIN
                         dbo.PI ON dbo.PI.PID = dbo.PISite.PID INNER JOIN
                         dbo.Site ON dbo.Site.SiteID = dbo.PISite.SiteID
GO
/****** Object:  Table [dbo].[Strain]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Strain](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Strain] [nvarchar](50) NOT NULL,
	[Disease_Model] [nvarchar](50) NULL,
	[Link] [nvarchar](100) NULL,
 CONSTRAINT [PK_Strain] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rbt_data_cached]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rbt_data_cached](
	[SessionID] [int] NOT NULL,
	[AVG_Average - Correct Choice Latency at 0.2s SD] [float] NULL,
	[AVG_Average - Correct Choice Latency at 0.5s SD] [float] NULL,
	[AVG_Average - Correct Choice Latency at 1s SD] [float] NULL,
	[AVG_Average - Correct Choice Latency at 2s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 0.2s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 0.5s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 1s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 2s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 0.2s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 0.5s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 1s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 2s SD] [float] NULL,
	[AVG_Blank Touch Latency] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 1] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 2] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 3] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 4] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 5] [float] NULL,
	[AVG_Correct Centre touch latency] [float] NULL,
	[AVG_Correct Choice Latency] [float] NULL,
	[AVG_Correct latency grid 1] [float] NULL,
	[AVG_Correct latency grid 2] [float] NULL,
	[AVG_Correct latency grid 3] [float] NULL,
	[AVG_Correct latency grid 4] [float] NULL,
	[AVG_Correct latency grid 5] [float] NULL,
	[AVG_Correct Reward Collection] [float] NULL,
	[AVG_Correct touch latency] [float] NULL,
	[AVG_count at 0.2s] [float] NULL,
	[AVG_count at 0.5s] [float] NULL,
	[AVG_count at 1s] [float] NULL,
	[AVG_count at 2s] [float] NULL,
	[AVG_Discrimination Sensitivity at 0.2s ] [float] NULL,
	[AVG_Discrimination Sensitivity at 0.5s ] [float] NULL,
	[AVG_Discrimination Sensitivity at 1s ] [float] NULL,
	[AVG_Discrimination Sensitivity at 2s ] [float] NULL,
	[AVG_Distractor 0.5s Delay Correct Rejection] [float] NULL,
	[AVG_Distractor 0.5s Delay False Alarm Latency] [float] NULL,
	[AVG_Distractor 0.5s Delay False Alarm Rate] [float] NULL,
	[AVG_Distractor 0.5s Delay Hit] [float] NULL,
	[AVG_Distractor 0.5s Delay Hit Latency] [float] NULL,
	[AVG_Distractor 0.5s Delay Hit Rate] [float] NULL,
	[AVG_Distractor 0.5s Delay Miss] [float] NULL,
	[AVG_Distractor 0.5s Delay Mistake] [float] NULL,
	[AVG_Distractor 0.5s Delay Response Bias] [float] NULL,
	[AVG_Distractor 0.5s Delay Reward Latency] [float] NULL,
	[AVG_Distractor 0.5s Delay Sensitivity (d)] [float] NULL,
	[AVG_Distractor 1s Delay Correct Rejection] [float] NULL,
	[AVG_Distractor 1s Delay False Alarm Latency] [float] NULL,
	[AVG_Distractor 1s Delay False Alarm Rate] [float] NULL,
	[AVG_Distractor 1s Delay Hit] [float] NULL,
	[AVG_Distractor 1s Delay Hit Latency] [float] NULL,
	[AVG_Distractor 1s Delay Hit Rate] [float] NULL,
	[AVG_Distractor 1s Delay Miss] [float] NULL,
	[AVG_Distractor 1s Delay Mistake] [float] NULL,
	[AVG_Distractor 1s Delay Response Bias] [float] NULL,
	[AVG_Distractor 1s Delay Reward Latency] [float] NULL,
	[AVG_Distractor 1s Delay Sensitivity (d)] [float] NULL,
	[AVG_Distrator Presentation Correct Rejection] [float] NULL,
	[AVG_Distrator Presentation False Alarm Latency] [float] NULL,
	[AVG_Distrator Presentation False Alarm Rate] [float] NULL,
	[AVG_Distrator Presentation Hit] [float] NULL,
	[AVG_Distrator Presentation Hit Latency] [float] NULL,
	[AVG_Distrator Presentation Hit Rate] [float] NULL,
	[AVG_Distrator Presentation Miss] [float] NULL,
	[AVG_Distrator Presentation Mistake] [float] NULL,
	[AVG_Distrator Presentation Response Bias] [float] NULL,
	[AVG_Distrator Presentation Reward Latency] [float] NULL,
	[AVG_Distrator Presentation Sensitivity (d)] [float] NULL,
	[AVG_End Summary - % Correct] [float] NULL,
	[AVG_End Summary - After Reward Pause] [float] NULL,
	[AVG_End Summary - All CS- touches] [float] NULL,
	[AVG_End Summary - All CS+ touches] [float] NULL,
	[AVG_End Summary - Blank Touches] [float] NULL,
	[AVG_END SUMMARY - Breakpoint] [float] NULL,
	[AVG_End Summary - Centre ITI Touches] [float] NULL,
	[AVG_End Summary - Condition] [float] NULL,
	[AVG_End Summary - Correct Image] [float] NULL,
	[AVG_End Summary - Correct Rejections] [float] NULL,
	[AVG_End Summary - Correction Trial Correct Rejections] [float] NULL,
	[AVG_End Summary - Correction Trial Mistakes] [float] NULL,
	[AVG_End Summary - CorrectRejection at 0.2s SD] [float] NULL,
	[AVG_End Summary - CorrectRejection at 0.5s SD] [float] NULL,
	[AVG_End Summary - CorrectRejection at 1s SD] [float] NULL,
	[AVG_End Summary - CorrectRejection at 2s SD] [float] NULL,
	[AVG_End Summary - Corrects] [float] NULL,
	[AVG_End Summary - CS - Beam Breaking] [float] NULL,
	[AVG_End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[AVG_End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[AVG_End Summary - CS + Beam Breaking] [float] NULL,
	[AVG_End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[AVG_End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[AVG_End Summary - Hits] [float] NULL,
	[AVG_End Summary - Hits at 0.2s SD] [float] NULL,
	[AVG_End Summary - Hits at 0.5s SD] [float] NULL,
	[AVG_End Summary - Hits at 1s SD] [float] NULL,
	[AVG_End Summary - Hits at 2s SD] [float] NULL,
	[AVG_End Summary - Incorrect Touches] [float] NULL,
	[AVG_End Summary - Left ITI Touches] [float] NULL,
	[AVG_End Summary - Miss at 0.2s SD] [float] NULL,
	[AVG_End Summary - Miss at 0.5s SD] [float] NULL,
	[AVG_End Summary - Miss at 1s SD] [float] NULL,
	[AVG_End Summary - Miss at 2s SD] [float] NULL,
	[AVG_End Summary - Misses] [float] NULL,
	[AVG_End Summary - Mistake at 0.2s SD] [float] NULL,
	[AVG_End Summary - Mistake at 0.5s SD] [float] NULL,
	[AVG_End Summary - Mistake at 1s SD] [float] NULL,
	[AVG_End Summary - Mistake at 2s SD] [float] NULL,
	[AVG_End Summary - Mistakes] [float] NULL,
	[AVG_End Summary - No Correction Trials] [float] NULL,
	[AVG_End Summary - No of non correction trials] [float] NULL,
	[AVG_End Summary - No. images] [float] NULL,
	[AVG_END SUMMARY - Number of target touches] [float] NULL,
	[AVG_End Summary - Omissions - Omission] [float] NULL,
	[AVG_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [float] NULL,
	[AVG_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [float] NULL,
	[AVG_END SUMMARY - Revised total response time] [float] NULL,
	[AVG_END SUMMARY - REWARD COLLECTION LATENCY] [float] NULL,
	[AVG_End Summary - Right ITI Touches] [float] NULL,
	[AVG_END SUMMARY - Schedule Length] [float] NULL,
	[AVG_End Summary - Stimulus Duration] [float] NULL,
	[AVG_End Summary - Total Trials] [float] NULL,
	[AVG_End Summary - Touches to lit CS-] [float] NULL,
	[AVG_End Summary - Touches to lit CS+] [float] NULL,
	[AVG_End Summary - Tray Beam Breaking] [float] NULL,
	[AVG_End Summary - Tray CS - Beam Breaking] [float] NULL,
	[AVG_End Summary - Tray CS + Beam Breaking] [float] NULL,
	[AVG_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[AVG_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[AVG_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[AVG_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[AVG_End Summary - Trials Completed] [float] NULL,
	[AVG_False Alarm Rate at 0.2s] [float] NULL,
	[AVG_False Alarm Rate at 0.5s] [float] NULL,
	[AVG_False Alarm Rate at 1s] [float] NULL,
	[AVG_False Alarm Rate at 2s] [float] NULL,
	[AVG_Hit Rate at 0.2s] [float] NULL,
	[AVG_Hit Rate at 0.5s] [float] NULL,
	[AVG_Hit Rate at 1s] [float] NULL,
	[AVG_Hit Rate at 2s] [float] NULL,
	[AVG_Incorrect Choice Latency] [float] NULL,
	[AVG_Incorrect Touch Latency] [float] NULL,
	[AVG_No Distractor Correct Rejection] [float] NULL,
	[AVG_No Distractor False Alarm Latency] [float] NULL,
	[AVG_No Distractor False Alarm Rate] [float] NULL,
	[AVG_No Distractor Hit] [float] NULL,
	[AVG_No Distractor Hit Latency] [float] NULL,
	[AVG_No Distractor Hit Rate] [float] NULL,
	[AVG_No Distractor Miss] [float] NULL,
	[AVG_No Distractor Mistake] [float] NULL,
	[AVG_No Distractor Response Bias] [float] NULL,
	[AVG_No Distractor Reward Latency] [float] NULL,
	[AVG_No Distractor Sensitivity (d)] [float] NULL,
	[AVG_Normalized - End Summary - All CS- touches] [float] NULL,
	[AVG_Normalized - End Summary - All CS+ touches] [float] NULL,
	[AVG_Normalized - End Summary - CS - Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS + Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Touches to lit CS-] [float] NULL,
	[AVG_Normalized - End Summary - Touches to lit CS+] [float] NULL,
	[AVG_Normalized - End Summary - Tray Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Tray CS - Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Tray CS + Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[AVG_Omission to Tray Latency] [float] NULL,
	[AVG_Omissions - Total] [float] NULL,
	[AVG_Perseverative Correct - Total] [float] NULL,
	[AVG_Perseverative Incorrect - Total] [float] NULL,
	[AVG_Premature Responses - Total] [float] NULL,
	[AVG_Response Bias at 0.2s] [float] NULL,
	[AVG_Response Bias at 0.5s] [float] NULL,
	[AVG_Response Bias at 1s] [float] NULL,
	[AVG_Response Bias at 2s] [float] NULL,
	[AVG_Reward Collection Latency] [float] NULL,
	[AVG_Reward IR Breaks - Reward Beam Cnt] [float] NULL,
	[AVG_Reward Retrieval Latency] [float] NULL,
	[AVG_Screen IR Breaks - Screen IR Cnt] [float] NULL,
	[AVG_Threshold - Accuracy %] [float] NULL,
	[AVG_Threshold - Condition] [float] NULL,
	[AVG_Threshold - Omission %] [float] NULL,
	[AVG_Threshold - Trials] [float] NULL,
	[AVG_Tray entered - Cnt] [float] NULL,
	[AVG_Trial Analysis - Accuracy%] [float] NULL,
	[AVG_Trial Analysis - Condition] [float] NULL,
	[AVG_Trial Analysis - Correct] [float] NULL,
	[AVG_Trial Analysis - Correct Resp Latency with No Distract] [float] NULL,
	[AVG_Trial Analysis - Correct Response Latency] [float] NULL,
	[AVG_Trial Analysis - CS- Approach Latency] [float] NULL,
	[AVG_Trial Analysis - CS- Touch Latency] [float] NULL,
	[AVG_Trial Analysis - CS+ Approach Latency] [float] NULL,
	[AVG_Trial Analysis - CS+ Touch Latency] [float] NULL,
	[AVG_Trial Analysis - Incorrect] [float] NULL,
	[AVG_Trial Analysis - Incorrect Response Latency] [float] NULL,
	[AVG_Trial Analysis - Omission] [float] NULL,
	[AVG_Trial Analysis - Omission%] [float] NULL,
	[AVG_Trial Analysis - Omissions - Omission] [float] NULL,
	[AVG_Trial Analysis - Premature] [float] NULL,
	[AVG_Trial Analysis - Reward Collection Latency] [float] NULL,
	[AVG_Trial Analysis - Reward Given - Condition] [float] NULL,
	[AVG_Trial Analysis - Stimulus Duration] [float] NULL,
	[AVG_Trial Analysis - Time] [float] NULL,
	[AVG_Trial Analysis - Time To Distraction] [float] NULL,
	[AVG_trial by trial anal - Correct Rejections] [float] NULL,
	[AVG_trial by trial anal - Correction Trial Correct Rejections] [float] NULL,
	[AVG_trial by trial anal - Correction Trial Mistakes] [float] NULL,
	[AVG_trial by trial anal - Current image] [float] NULL,
	[AVG_trial by trial anal - Distractor Time] [float] NULL,
	[AVG_trial by trial anal - Hits] [float] NULL,
	[AVG_trial by trial anal - ITI] [float] NULL,
	[AVG_trial by trial anal - Misses] [float] NULL,
	[AVG_trial by trial anal - Mistakes] [float] NULL,
	[AVG_trial by trial anal - No of non correction trials] [float] NULL,
	[AVG_trial by trial anal - Stimulus Duration] [float] NULL,
	[AVG_Trial by trial analysis - Image response choice latency] [float] NULL,
	[AVG_Trial by trial analysis - Left chosen] [float] NULL,
	[AVG_Trial by trial analysis - One trial] [float] NULL,
	[AVG_Trial by trial analysis - Optimal side chosen] [float] NULL,
	[AVG_Trial by trial analysis - Optimal side chosen with milkshake given] [float] NULL,
	[AVG_Trial by trial analysis - Optimal side chosen without milkshake given] [float] NULL,
	[AVG_Trial by trial analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[AVG_Trial by trial analysis - Right chosen] [float] NULL,
	[AVG_Trial by trial analysis - Spurious feedback given] [float] NULL,
	[AVG_Trial by trial analysis - Spurious no milkshake] [float] NULL,
	[AVG_Trial by trial analysis - Spurious with milkshake] [float] NULL,
	[AVG_Trial by trial analysis - Trial type] [float] NULL,
	[AVG_True Blank Touches at Grid 1] [float] NULL,
	[AVG_True Blank Touches at Grid 2] [float] NULL,
	[AVG_True Blank Touches at Grid 3] [float] NULL,
	[AVG_True Blank Touches at Grid 4] [float] NULL,
	[AVG_True Blank Touches at Grid 5] [float] NULL,
	[AVG_Whole session analysis - Condition] [float] NULL,
	[AVG_Whole session analysis - Image response choice latency] [float] NULL,
	[AVG_Whole session analysis - Number of Reversals] [float] NULL,
	[AVG_Whole session analysis - Optimal side chosen] [float] NULL,
	[AVG_Whole session analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[AVG_Whole session analysis - Trials completed] [float] NULL,
	[SessionId2] [int] NOT NULL,
	[STDEV_Average - Correct Choice Latency at 0.2s SD] [float] NULL,
	[STDEV_Average - Correct Choice Latency at 0.5s SD] [float] NULL,
	[STDEV_Average - Correct Choice Latency at 1s SD] [float] NULL,
	[STDEV_Average - Correct Choice Latency at 2s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 0.2s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 0.5s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 1s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 2s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 0.2s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 0.5s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 1s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 2s SD] [float] NULL,
	[STDEV_Blank Touch Latency] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 1] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 2] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 3] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 4] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 5] [float] NULL,
	[STDEV_Correct Centre touch latency] [float] NULL,
	[STDEV_Correct Choice Latency] [float] NULL,
	[STDEV_Correct latency grid 1] [float] NULL,
	[STDEV_Correct latency grid 2] [float] NULL,
	[STDEV_Correct latency grid 3] [float] NULL,
	[STDEV_Correct latency grid 4] [float] NULL,
	[STDEV_Correct latency grid 5] [float] NULL,
	[STDEV_Correct Reward Collection] [float] NULL,
	[STDEV_Correct touch latency] [float] NULL,
	[STDEV_count at 0.2s] [float] NULL,
	[STDEV_count at 0.5s] [float] NULL,
	[STDEV_count at 1s] [float] NULL,
	[STDEV_count at 2s] [float] NULL,
	[STDEV_Discrimination Sensitivity at 0.2s ] [float] NULL,
	[STDEV_Discrimination Sensitivity at 0.5s ] [float] NULL,
	[STDEV_Discrimination Sensitivity at 1s ] [float] NULL,
	[STDEV_Discrimination Sensitivity at 2s ] [float] NULL,
	[STDEV_Distractor 0.5s Delay Correct Rejection] [float] NULL,
	[STDEV_Distractor 0.5s Delay False Alarm Latency] [float] NULL,
	[STDEV_Distractor 0.5s Delay False Alarm Rate] [float] NULL,
	[STDEV_Distractor 0.5s Delay Hit] [float] NULL,
	[STDEV_Distractor 0.5s Delay Hit Latency] [float] NULL,
	[STDEV_Distractor 0.5s Delay Hit Rate] [float] NULL,
	[STDEV_Distractor 0.5s Delay Miss] [float] NULL,
	[STDEV_Distractor 0.5s Delay Mistake] [float] NULL,
	[STDEV_Distractor 0.5s Delay Response Bias] [float] NULL,
	[STDEV_Distractor 0.5s Delay Reward Latency] [float] NULL,
	[STDEV_Distractor 0.5s Delay Sensitivity (d)] [float] NULL,
	[STDEV_Distractor 1s Delay Correct Rejection] [float] NULL,
	[STDEV_Distractor 1s Delay False Alarm Latency] [float] NULL,
	[STDEV_Distractor 1s Delay False Alarm Rate] [float] NULL,
	[STDEV_Distractor 1s Delay Hit] [float] NULL,
	[STDEV_Distractor 1s Delay Hit Latency] [float] NULL,
	[STDEV_Distractor 1s Delay Hit Rate] [float] NULL,
	[STDEV_Distractor 1s Delay Miss] [float] NULL,
	[STDEV_Distractor 1s Delay Mistake] [float] NULL,
	[STDEV_Distractor 1s Delay Response Bias] [float] NULL,
	[STDEV_Distractor 1s Delay Reward Latency] [float] NULL,
	[STDEV_Distractor 1s Delay Sensitivity (d)] [float] NULL,
	[STDEV_Distrator Presentation Correct Rejection] [float] NULL,
	[STDEV_Distrator Presentation False Alarm Latency] [float] NULL,
	[STDEV_Distrator Presentation False Alarm Rate] [float] NULL,
	[STDEV_Distrator Presentation Hit] [float] NULL,
	[STDEV_Distrator Presentation Hit Latency] [float] NULL,
	[STDEV_Distrator Presentation Hit Rate] [float] NULL,
	[STDEV_Distrator Presentation Miss] [float] NULL,
	[STDEV_Distrator Presentation Mistake] [float] NULL,
	[STDEV_Distrator Presentation Response Bias] [float] NULL,
	[STDEV_Distrator Presentation Reward Latency] [float] NULL,
	[STDEV_Distrator Presentation Sensitivity (d)] [float] NULL,
	[STDEV_End Summary - % Correct] [float] NULL,
	[STDEV_End Summary - After Reward Pause] [float] NULL,
	[STDEV_End Summary - All CS- touches] [float] NULL,
	[STDEV_End Summary - All CS+ touches] [float] NULL,
	[STDEV_End Summary - Blank Touches] [float] NULL,
	[STDEV_END SUMMARY - Breakpoint] [float] NULL,
	[STDEV_End Summary - Centre ITI Touches] [float] NULL,
	[STDEV_End Summary - Condition] [float] NULL,
	[STDEV_End Summary - Correct Image] [float] NULL,
	[STDEV_End Summary - Correct Rejections] [float] NULL,
	[STDEV_End Summary - Correction Trial Correct Rejections] [float] NULL,
	[STDEV_End Summary - Correction Trial Mistakes] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 0.2s SD] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 0.5s SD] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 1s SD] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 2s SD] [float] NULL,
	[STDEV_End Summary - Corrects] [float] NULL,
	[STDEV_End Summary - CS - Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS + Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[STDEV_End Summary - Hits] [float] NULL,
	[STDEV_End Summary - Hits at 0.2s SD] [float] NULL,
	[STDEV_End Summary - Hits at 0.5s SD] [float] NULL,
	[STDEV_End Summary - Hits at 1s SD] [float] NULL,
	[STDEV_End Summary - Hits at 2s SD] [float] NULL,
	[STDEV_End Summary - Incorrect Touches] [float] NULL,
	[STDEV_End Summary - Left ITI Touches] [float] NULL,
	[STDEV_End Summary - Miss at 0.2s SD] [float] NULL,
	[STDEV_End Summary - Miss at 0.5s SD] [float] NULL,
	[STDEV_End Summary - Miss at 1s SD] [float] NULL,
	[STDEV_End Summary - Miss at 2s SD] [float] NULL,
	[STDEV_End Summary - Misses] [float] NULL,
	[STDEV_End Summary - Mistake at 0.2s SD] [float] NULL,
	[STDEV_End Summary - Mistake at 0.5s SD] [float] NULL,
	[STDEV_End Summary - Mistake at 1s SD] [float] NULL,
	[STDEV_End Summary - Mistake at 2s SD] [float] NULL,
	[STDEV_End Summary - Mistakes] [float] NULL,
	[STDEV_End Summary - No Correction Trials] [float] NULL,
	[STDEV_End Summary - No of non correction trials] [float] NULL,
	[STDEV_End Summary - No. images] [float] NULL,
	[STDEV_END SUMMARY - Number of target touches] [float] NULL,
	[STDEV_End Summary - Omissions - Omission] [float] NULL,
	[STDEV_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [float] NULL,
	[STDEV_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [float] NULL,
	[STDEV_END SUMMARY - Revised total response time] [float] NULL,
	[STDEV_END SUMMARY - REWARD COLLECTION LATENCY] [float] NULL,
	[STDEV_End Summary - Right ITI Touches] [float] NULL,
	[STDEV_END SUMMARY - Schedule Length] [float] NULL,
	[STDEV_End Summary - Stimulus Duration] [float] NULL,
	[STDEV_End Summary - Total Trials] [float] NULL,
	[STDEV_End Summary - Touches to lit CS-] [float] NULL,
	[STDEV_End Summary - Touches to lit CS+] [float] NULL,
	[STDEV_End Summary - Tray Beam Breaking] [float] NULL,
	[STDEV_End Summary - Tray CS - Beam Breaking] [float] NULL,
	[STDEV_End Summary - Tray CS + Beam Breaking] [float] NULL,
	[STDEV_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[STDEV_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[STDEV_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[STDEV_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[STDEV_End Summary - Trials Completed] [float] NULL,
	[STDEV_False Alarm Rate at 0.2s] [float] NULL,
	[STDEV_False Alarm Rate at 0.5s] [float] NULL,
	[STDEV_False Alarm Rate at 1s] [float] NULL,
	[STDEV_False Alarm Rate at 2s] [float] NULL,
	[STDEV_Hit Rate at 0.2s] [float] NULL,
	[STDEV_Hit Rate at 0.5s] [float] NULL,
	[STDEV_Hit Rate at 1s] [float] NULL,
	[STDEV_Hit Rate at 2s] [float] NULL,
	[STDEV_Incorrect Choice Latency] [float] NULL,
	[STDEV_Incorrect Touch Latency] [float] NULL,
	[STDEV_No Distractor Correct Rejection] [float] NULL,
	[STDEV_No Distractor False Alarm Latency] [float] NULL,
	[STDEV_No Distractor False Alarm Rate] [float] NULL,
	[STDEV_No Distractor Hit] [float] NULL,
	[STDEV_No Distractor Hit Latency] [float] NULL,
	[STDEV_No Distractor Hit Rate] [float] NULL,
	[STDEV_No Distractor Miss] [float] NULL,
	[STDEV_No Distractor Mistake] [float] NULL,
	[STDEV_No Distractor Response Bias] [float] NULL,
	[STDEV_No Distractor Reward Latency] [float] NULL,
	[STDEV_No Distractor Sensitivity (d)] [float] NULL,
	[STDEV_Normalized - End Summary - All CS- touches] [float] NULL,
	[STDEV_Normalized - End Summary - All CS+ touches] [float] NULL,
	[STDEV_Normalized - End Summary - CS - Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS + Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Touches to lit CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Touches to lit CS+] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Tray CS - Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Tray CS + Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[STDEV_Omission to Tray Latency] [float] NULL,
	[STDEV_Omissions - Total] [float] NULL,
	[STDEV_Perseverative Correct - Total] [float] NULL,
	[STDEV_Perseverative Incorrect - Total] [float] NULL,
	[STDEV_Premature Responses - Total] [float] NULL,
	[STDEV_Response Bias at 0.2s] [float] NULL,
	[STDEV_Response Bias at 0.5s] [float] NULL,
	[STDEV_Response Bias at 1s] [float] NULL,
	[STDEV_Response Bias at 2s] [float] NULL,
	[STDEV_Reward Collection Latency] [float] NULL,
	[STDEV_Reward IR Breaks - Reward Beam Cnt] [float] NULL,
	[STDEV_Reward Retrieval Latency] [float] NULL,
	[STDEV_Screen IR Breaks - Screen IR Cnt] [float] NULL,
	[STDEV_Threshold - Accuracy %] [float] NULL,
	[STDEV_Threshold - Condition] [float] NULL,
	[STDEV_Threshold - Omission %] [float] NULL,
	[STDEV_Threshold - Trials] [float] NULL,
	[STDEV_Tray entered - Cnt] [float] NULL,
	[STDEV_Trial Analysis - Accuracy%] [float] NULL,
	[STDEV_Trial Analysis - Condition] [float] NULL,
	[STDEV_Trial Analysis - Correct] [float] NULL,
	[STDEV_Trial Analysis - Correct Resp Latency with No Distract] [float] NULL,
	[STDEV_Trial Analysis - Correct Response Latency] [float] NULL,
	[STDEV_Trial Analysis - CS- Approach Latency] [float] NULL,
	[STDEV_Trial Analysis - CS- Touch Latency] [float] NULL,
	[STDEV_Trial Analysis - CS+ Approach Latency] [float] NULL,
	[STDEV_Trial Analysis - CS+ Touch Latency] [float] NULL,
	[STDEV_Trial Analysis - Incorrect] [float] NULL,
	[STDEV_Trial Analysis - Incorrect Response Latency] [float] NULL,
	[STDEV_Trial Analysis - Omission] [float] NULL,
	[STDEV_Trial Analysis - Omission%] [float] NULL,
	[STDEV_Trial Analysis - Omissions - Omission] [float] NULL,
	[STDEV_Trial Analysis - Premature] [float] NULL,
	[STDEV_Trial Analysis - Reward Collection Latency] [float] NULL,
	[STDEV_Trial Analysis - Reward Given - Condition] [float] NULL,
	[STDEV_Trial Analysis - Stimulus Duration] [float] NULL,
	[STDEV_Trial Analysis - Time] [float] NULL,
	[STDEV_Trial Analysis - Time To Distraction] [float] NULL,
	[STDEV_trial by trial anal - Correct Rejections] [float] NULL,
	[STDEV_trial by trial anal - Correction Trial Correct Rejections] [float] NULL,
	[STDEV_trial by trial anal - Correction Trial Mistakes] [float] NULL,
	[STDEV_trial by trial anal - Current image] [float] NULL,
	[STDEV_trial by trial anal - Distractor Time] [float] NULL,
	[STDEV_trial by trial anal - Hits] [float] NULL,
	[STDEV_trial by trial anal - ITI] [float] NULL,
	[STDEV_trial by trial anal - Misses] [float] NULL,
	[STDEV_trial by trial anal - Mistakes] [float] NULL,
	[STDEV_trial by trial anal - No of non correction trials] [float] NULL,
	[STDEV_trial by trial anal - Stimulus Duration] [float] NULL,
	[STDEV_Trial by trial analysis - Image response choice latency] [float] NULL,
	[STDEV_Trial by trial analysis - Left chosen] [float] NULL,
	[STDEV_Trial by trial analysis - One trial] [float] NULL,
	[STDEV_Trial by trial analysis - Optimal side chosen] [float] NULL,
	[STDEV_Trial by trial analysis - Optimal side chosen with milkshake given] [float] NULL,
	[STDEV_Trial by trial analysis - Optimal side chosen without milkshake given] [float] NULL,
	[STDEV_Trial by trial analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[STDEV_Trial by trial analysis - Right chosen] [float] NULL,
	[STDEV_Trial by trial analysis - Spurious feedback given] [float] NULL,
	[STDEV_Trial by trial analysis - Spurious no milkshake] [float] NULL,
	[STDEV_Trial by trial analysis - Spurious with milkshake] [float] NULL,
	[STDEV_Trial by trial analysis - Trial type] [float] NULL,
	[STDEV_True Blank Touches at Grid 1] [float] NULL,
	[STDEV_True Blank Touches at Grid 2] [float] NULL,
	[STDEV_True Blank Touches at Grid 3] [float] NULL,
	[STDEV_True Blank Touches at Grid 4] [float] NULL,
	[STDEV_True Blank Touches at Grid 5] [float] NULL,
	[STDEV_Whole session analysis - Condition] [float] NULL,
	[STDEV_Whole session analysis - Image response choice latency] [float] NULL,
	[STDEV_Whole session analysis - Number of Reversals] [float] NULL,
	[STDEV_Whole session analysis - Optimal side chosen] [float] NULL,
	[STDEV_Whole session analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[STDEV_Whole session analysis - Trials completed] [float] NULL,
	[SessionId3] [int] NOT NULL,
	[COUNT_Average - Correct Choice Latency at 0.2s SD] [int] NULL,
	[COUNT_Average - Correct Choice Latency at 0.5s SD] [int] NULL,
	[COUNT_Average - Correct Choice Latency at 1s SD] [int] NULL,
	[COUNT_Average - Correct Choice Latency at 2s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 0.2s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 0.5s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 1s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 2s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 0.2s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 0.5s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 1s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 2s SD] [int] NULL,
	[COUNT_Blank Touch Latency] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 1] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 2] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 3] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 4] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 5] [int] NULL,
	[COUNT_Correct Centre touch latency] [int] NULL,
	[COUNT_Correct Choice Latency] [int] NULL,
	[COUNT_Correct latency grid 1] [int] NULL,
	[COUNT_Correct latency grid 2] [int] NULL,
	[COUNT_Correct latency grid 3] [int] NULL,
	[COUNT_Correct latency grid 4] [int] NULL,
	[COUNT_Correct latency grid 5] [int] NULL,
	[COUNT_Correct Reward Collection] [int] NULL,
	[COUNT_Correct touch latency] [int] NULL,
	[COUNT_count at 0.2s] [int] NULL,
	[COUNT_count at 0.5s] [int] NULL,
	[COUNT_count at 1s] [int] NULL,
	[COUNT_count at 2s] [int] NULL,
	[COUNT_Discrimination Sensitivity at 0.2s ] [int] NULL,
	[COUNT_Discrimination Sensitivity at 0.5s ] [int] NULL,
	[COUNT_Discrimination Sensitivity at 1s ] [int] NULL,
	[COUNT_Discrimination Sensitivity at 2s ] [int] NULL,
	[COUNT_Distractor 0.5s Delay Correct Rejection] [int] NULL,
	[COUNT_Distractor 0.5s Delay False Alarm Latency] [int] NULL,
	[COUNT_Distractor 0.5s Delay False Alarm Rate] [int] NULL,
	[COUNT_Distractor 0.5s Delay Hit] [int] NULL,
	[COUNT_Distractor 0.5s Delay Hit Latency] [int] NULL,
	[COUNT_Distractor 0.5s Delay Hit Rate] [int] NULL,
	[COUNT_Distractor 0.5s Delay Miss] [int] NULL,
	[COUNT_Distractor 0.5s Delay Mistake] [int] NULL,
	[COUNT_Distractor 0.5s Delay Response Bias] [int] NULL,
	[COUNT_Distractor 0.5s Delay Reward Latency] [int] NULL,
	[COUNT_Distractor 0.5s Delay Sensitivity (d)] [int] NULL,
	[COUNT_Distractor 1s Delay Correct Rejection] [int] NULL,
	[COUNT_Distractor 1s Delay False Alarm Latency] [int] NULL,
	[COUNT_Distractor 1s Delay False Alarm Rate] [int] NULL,
	[COUNT_Distractor 1s Delay Hit] [int] NULL,
	[COUNT_Distractor 1s Delay Hit Latency] [int] NULL,
	[COUNT_Distractor 1s Delay Hit Rate] [int] NULL,
	[COUNT_Distractor 1s Delay Miss] [int] NULL,
	[COUNT_Distractor 1s Delay Mistake] [int] NULL,
	[COUNT_Distractor 1s Delay Response Bias] [int] NULL,
	[COUNT_Distractor 1s Delay Reward Latency] [int] NULL,
	[COUNT_Distractor 1s Delay Sensitivity (d)] [int] NULL,
	[COUNT_Distrator Presentation Correct Rejection] [int] NULL,
	[COUNT_Distrator Presentation False Alarm Latency] [int] NULL,
	[COUNT_Distrator Presentation False Alarm Rate] [int] NULL,
	[COUNT_Distrator Presentation Hit] [int] NULL,
	[COUNT_Distrator Presentation Hit Latency] [int] NULL,
	[COUNT_Distrator Presentation Hit Rate] [int] NULL,
	[COUNT_Distrator Presentation Miss] [int] NULL,
	[COUNT_Distrator Presentation Mistake] [int] NULL,
	[COUNT_Distrator Presentation Response Bias] [int] NULL,
	[COUNT_Distrator Presentation Reward Latency] [int] NULL,
	[COUNT_Distrator Presentation Sensitivity (d)] [int] NULL,
	[COUNT_End Summary - % Correct] [int] NULL,
	[COUNT_End Summary - After Reward Pause] [int] NULL,
	[COUNT_End Summary - All CS- touches] [int] NULL,
	[COUNT_End Summary - All CS+ touches] [int] NULL,
	[COUNT_End Summary - Blank Touches] [int] NULL,
	[COUNT_END SUMMARY - Breakpoint] [int] NULL,
	[COUNT_End Summary - Centre ITI Touches] [int] NULL,
	[COUNT_End Summary - Condition] [int] NULL,
	[COUNT_End Summary - Correct Image] [int] NULL,
	[COUNT_End Summary - Correct Rejections] [int] NULL,
	[COUNT_End Summary - Correction Trial Correct Rejections] [int] NULL,
	[COUNT_End Summary - Correction Trial Mistakes] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 0.2s SD] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 0.5s SD] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 1s SD] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 2s SD] [int] NULL,
	[COUNT_End Summary - Corrects] [int] NULL,
	[COUNT_End Summary - CS - Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS - Image Approach CS+ Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS - Image Presentation Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS + Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS + Image Approach CS- Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS + Image Presentation Beam Breaking] [int] NULL,
	[COUNT_End Summary - Hits] [int] NULL,
	[COUNT_End Summary - Hits at 0.2s SD] [int] NULL,
	[COUNT_End Summary - Hits at 0.5s SD] [int] NULL,
	[COUNT_End Summary - Hits at 1s SD] [int] NULL,
	[COUNT_End Summary - Hits at 2s SD] [int] NULL,
	[COUNT_End Summary - Incorrect Touches] [int] NULL,
	[COUNT_End Summary - Left ITI Touches] [int] NULL,
	[COUNT_End Summary - Miss at 0.2s SD] [int] NULL,
	[COUNT_End Summary - Miss at 0.5s SD] [int] NULL,
	[COUNT_End Summary - Miss at 1s SD] [int] NULL,
	[COUNT_End Summary - Miss at 2s SD] [int] NULL,
	[COUNT_End Summary - Misses] [int] NULL,
	[COUNT_End Summary - Mistake at 0.2s SD] [int] NULL,
	[COUNT_End Summary - Mistake at 0.5s SD] [int] NULL,
	[COUNT_End Summary - Mistake at 1s SD] [int] NULL,
	[COUNT_End Summary - Mistake at 2s SD] [int] NULL,
	[COUNT_End Summary - Mistakes] [int] NULL,
	[COUNT_End Summary - No Correction Trials] [int] NULL,
	[COUNT_End Summary - No of non correction trials] [int] NULL,
	[COUNT_End Summary - No. images] [int] NULL,
	[COUNT_END SUMMARY - Number of target touches] [int] NULL,
	[COUNT_End Summary - Omissions - Omission] [int] NULL,
	[COUNT_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [int] NULL,
	[COUNT_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [int] NULL,
	[COUNT_END SUMMARY - Revised total response time] [int] NULL,
	[COUNT_END SUMMARY - REWARD COLLECTION LATENCY] [int] NULL,
	[COUNT_End Summary - Right ITI Touches] [int] NULL,
	[COUNT_END SUMMARY - Schedule Length] [int] NULL,
	[COUNT_End Summary - Stimulus Duration] [int] NULL,
	[COUNT_End Summary - Total Trials] [int] NULL,
	[COUNT_End Summary - Touches to lit CS-] [int] NULL,
	[COUNT_End Summary - Touches to lit CS+] [int] NULL,
	[COUNT_End Summary - Tray Beam Breaking] [int] NULL,
	[COUNT_End Summary - Tray CS - Beam Breaking] [int] NULL,
	[COUNT_End Summary - Tray CS + Beam Breaking] [int] NULL,
	[COUNT_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [int] NULL,
	[COUNT_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [int] NULL,
	[COUNT_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [int] NULL,
	[COUNT_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [int] NULL,
	[COUNT_End Summary - Trials Completed] [int] NULL,
	[COUNT_False Alarm Rate at 0.2s] [int] NULL,
	[COUNT_False Alarm Rate at 0.5s] [int] NULL,
	[COUNT_False Alarm Rate at 1s] [int] NULL,
	[COUNT_False Alarm Rate at 2s] [int] NULL,
	[COUNT_Hit Rate at 0.2s] [int] NULL,
	[COUNT_Hit Rate at 0.5s] [int] NULL,
	[COUNT_Hit Rate at 1s] [int] NULL,
	[COUNT_Hit Rate at 2s] [int] NULL,
	[COUNT_Incorrect Choice Latency] [int] NULL,
	[COUNT_Incorrect Touch Latency] [int] NULL,
	[COUNT_No Distractor Correct Rejection] [int] NULL,
	[COUNT_No Distractor False Alarm Latency] [int] NULL,
	[COUNT_No Distractor False Alarm Rate] [int] NULL,
	[COUNT_No Distractor Hit] [int] NULL,
	[COUNT_No Distractor Hit Latency] [int] NULL,
	[COUNT_No Distractor Hit Rate] [int] NULL,
	[COUNT_No Distractor Miss] [int] NULL,
	[COUNT_No Distractor Mistake] [int] NULL,
	[COUNT_No Distractor Response Bias] [int] NULL,
	[COUNT_No Distractor Reward Latency] [int] NULL,
	[COUNT_No Distractor Sensitivity (d)] [int] NULL,
	[COUNT_Normalized - End Summary - All CS- touches] [int] NULL,
	[COUNT_Normalized - End Summary - All CS+ touches] [int] NULL,
	[COUNT_Normalized - End Summary - CS - Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS - Image Presentation Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS + Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS + Image Presentation Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Touches to lit CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Touches to lit CS+] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Tray CS - Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Tray CS + Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [int] NULL,
	[COUNT_Omission to Tray Latency] [int] NULL,
	[COUNT_Omissions - Total] [int] NULL,
	[COUNT_Perseverative Correct - Total] [int] NULL,
	[COUNT_Perseverative Incorrect - Total] [int] NULL,
	[COUNT_Premature Responses - Total] [int] NULL,
	[COUNT_Response Bias at 0.2s] [int] NULL,
	[COUNT_Response Bias at 0.5s] [int] NULL,
	[COUNT_Response Bias at 1s] [int] NULL,
	[COUNT_Response Bias at 2s] [int] NULL,
	[COUNT_Reward Collection Latency] [int] NULL,
	[COUNT_Reward IR Breaks - Reward Beam Cnt] [int] NULL,
	[COUNT_Reward Retrieval Latency] [int] NULL,
	[COUNT_Screen IR Breaks - Screen IR Cnt] [int] NULL,
	[COUNT_Threshold - Accuracy %] [int] NULL,
	[COUNT_Threshold - Condition] [int] NULL,
	[COUNT_Threshold - Omission %] [int] NULL,
	[COUNT_Threshold - Trials] [int] NULL,
	[COUNT_Tray entered - Cnt] [int] NULL,
	[COUNT_Trial Analysis - Accuracy%] [int] NULL,
	[COUNT_Trial Analysis - Condition] [int] NULL,
	[COUNT_Trial Analysis - Correct] [int] NULL,
	[COUNT_Trial Analysis - Correct Resp Latency with No Distract] [int] NULL,
	[COUNT_Trial Analysis - Correct Response Latency] [int] NULL,
	[COUNT_Trial Analysis - CS- Approach Latency] [int] NULL,
	[COUNT_Trial Analysis - CS- Touch Latency] [int] NULL,
	[COUNT_Trial Analysis - CS+ Approach Latency] [int] NULL,
	[COUNT_Trial Analysis - CS+ Touch Latency] [int] NULL,
	[COUNT_Trial Analysis - Incorrect] [int] NULL,
	[COUNT_Trial Analysis - Incorrect Response Latency] [int] NULL,
	[COUNT_Trial Analysis - Omission] [int] NULL,
	[COUNT_Trial Analysis - Omission%] [int] NULL,
	[COUNT_Trial Analysis - Omissions - Omission] [int] NULL,
	[COUNT_Trial Analysis - Premature] [int] NULL,
	[COUNT_Trial Analysis - Reward Collection Latency] [int] NULL,
	[COUNT_Trial Analysis - Reward Given - Condition] [int] NULL,
	[COUNT_Trial Analysis - Stimulus Duration] [int] NULL,
	[COUNT_Trial Analysis - Time] [int] NULL,
	[COUNT_Trial Analysis - Time To Distraction] [int] NULL,
	[COUNT_trial by trial anal - Correct Rejections] [int] NULL,
	[COUNT_trial by trial anal - Correction Trial Correct Rejections] [int] NULL,
	[COUNT_trial by trial anal - Correction Trial Mistakes] [int] NULL,
	[COUNT_trial by trial anal - Current image] [int] NULL,
	[COUNT_trial by trial anal - Distractor Time] [int] NULL,
	[COUNT_trial by trial anal - Hits] [int] NULL,
	[COUNT_trial by trial anal - ITI] [int] NULL,
	[COUNT_trial by trial anal - Misses] [int] NULL,
	[COUNT_trial by trial anal - Mistakes] [int] NULL,
	[COUNT_trial by trial anal - No of non correction trials] [int] NULL,
	[COUNT_trial by trial anal - Stimulus Duration] [int] NULL,
	[COUNT_Trial by trial analysis - Image response choice latency] [int] NULL,
	[COUNT_Trial by trial analysis - Left chosen] [int] NULL,
	[COUNT_Trial by trial analysis - One trial] [int] NULL,
	[COUNT_Trial by trial analysis - Optimal side chosen] [int] NULL,
	[COUNT_Trial by trial analysis - Optimal side chosen with milkshake given] [int] NULL,
	[COUNT_Trial by trial analysis - Optimal side chosen without milkshake given] [int] NULL,
	[COUNT_Trial by trial analysis - REWARD COLLECTION LATENCY] [int] NULL,
	[COUNT_Trial by trial analysis - Right chosen] [int] NULL,
	[COUNT_Trial by trial analysis - Spurious feedback given] [int] NULL,
	[COUNT_Trial by trial analysis - Spurious no milkshake] [int] NULL,
	[COUNT_Trial by trial analysis - Spurious with milkshake] [int] NULL,
	[COUNT_Trial by trial analysis - Trial type] [int] NULL,
	[COUNT_True Blank Touches at Grid 1] [int] NULL,
	[COUNT_True Blank Touches at Grid 2] [int] NULL,
	[COUNT_True Blank Touches at Grid 3] [int] NULL,
	[COUNT_True Blank Touches at Grid 4] [int] NULL,
	[COUNT_True Blank Touches at Grid 5] [int] NULL,
	[COUNT_Whole session analysis - Condition] [int] NULL,
	[COUNT_Whole session analysis - Image response choice latency] [int] NULL,
	[COUNT_Whole session analysis - Number of Reversals] [int] NULL,
	[COUNT_Whole session analysis - Optimal side chosen] [int] NULL,
	[COUNT_Whole session analysis - REWARD COLLECTION LATENCY] [int] NULL,
	[COUNT_Whole session analysis - Trials completed] [int] NULL,
	[SessionId4] [int] NOT NULL,
	[SUM_Average - Correct Choice Latency at 0.2s SD] [float] NULL,
	[SUM_Average - Correct Choice Latency at 0.5s SD] [float] NULL,
	[SUM_Average - Correct Choice Latency at 1s SD] [float] NULL,
	[SUM_Average - Correct Choice Latency at 2s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 0.2s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 0.5s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 1s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 2s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 0.2s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 0.5s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 1s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 2s SD] [float] NULL,
	[SUM_Blank Touch Latency] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 1] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 2] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 3] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 4] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 5] [float] NULL,
	[SUM_Correct Centre touch latency] [float] NULL,
	[SUM_Correct Choice Latency] [float] NULL,
	[SUM_Correct latency grid 1] [float] NULL,
	[SUM_Correct latency grid 2] [float] NULL,
	[SUM_Correct latency grid 3] [float] NULL,
	[SUM_Correct latency grid 4] [float] NULL,
	[SUM_Correct latency grid 5] [float] NULL,
	[SUM_Correct Reward Collection] [float] NULL,
	[SUM_Correct touch latency] [float] NULL,
	[SUM_count at 0.2s] [float] NULL,
	[SUM_count at 0.5s] [float] NULL,
	[SUM_count at 1s] [float] NULL,
	[SUM_count at 2s] [float] NULL,
	[SUM_Discrimination Sensitivity at 0.2s ] [float] NULL,
	[SUM_Discrimination Sensitivity at 0.5s ] [float] NULL,
	[SUM_Discrimination Sensitivity at 1s ] [float] NULL,
	[SUM_Discrimination Sensitivity at 2s ] [float] NULL,
	[SUM_Distractor 0.5s Delay Correct Rejection] [float] NULL,
	[SUM_Distractor 0.5s Delay False Alarm Latency] [float] NULL,
	[SUM_Distractor 0.5s Delay False Alarm Rate] [float] NULL,
	[SUM_Distractor 0.5s Delay Hit] [float] NULL,
	[SUM_Distractor 0.5s Delay Hit Latency] [float] NULL,
	[SUM_Distractor 0.5s Delay Hit Rate] [float] NULL,
	[SUM_Distractor 0.5s Delay Miss] [float] NULL,
	[SUM_Distractor 0.5s Delay Mistake] [float] NULL,
	[SUM_Distractor 0.5s Delay Response Bias] [float] NULL,
	[SUM_Distractor 0.5s Delay Reward Latency] [float] NULL,
	[SUM_Distractor 0.5s Delay Sensitivity (d)] [float] NULL,
	[SUM_Distractor 1s Delay Correct Rejection] [float] NULL,
	[SUM_Distractor 1s Delay False Alarm Latency] [float] NULL,
	[SUM_Distractor 1s Delay False Alarm Rate] [float] NULL,
	[SUM_Distractor 1s Delay Hit] [float] NULL,
	[SUM_Distractor 1s Delay Hit Latency] [float] NULL,
	[SUM_Distractor 1s Delay Hit Rate] [float] NULL,
	[SUM_Distractor 1s Delay Miss] [float] NULL,
	[SUM_Distractor 1s Delay Mistake] [float] NULL,
	[SUM_Distractor 1s Delay Response Bias] [float] NULL,
	[SUM_Distractor 1s Delay Reward Latency] [float] NULL,
	[SUM_Distractor 1s Delay Sensitivity (d)] [float] NULL,
	[SUM_Distrator Presentation Correct Rejection] [float] NULL,
	[SUM_Distrator Presentation False Alarm Latency] [float] NULL,
	[SUM_Distrator Presentation False Alarm Rate] [float] NULL,
	[SUM_Distrator Presentation Hit] [float] NULL,
	[SUM_Distrator Presentation Hit Latency] [float] NULL,
	[SUM_Distrator Presentation Hit Rate] [float] NULL,
	[SUM_Distrator Presentation Miss] [float] NULL,
	[SUM_Distrator Presentation Mistake] [float] NULL,
	[SUM_Distrator Presentation Response Bias] [float] NULL,
	[SUM_Distrator Presentation Reward Latency] [float] NULL,
	[SUM_Distrator Presentation Sensitivity (d)] [float] NULL,
	[SUM_End Summary - % Correct] [float] NULL,
	[SUM_End Summary - After Reward Pause] [float] NULL,
	[SUM_End Summary - All CS- touches] [float] NULL,
	[SUM_End Summary - All CS+ touches] [float] NULL,
	[SUM_End Summary - Blank Touches] [float] NULL,
	[SUM_END SUMMARY - Breakpoint] [float] NULL,
	[SUM_End Summary - Centre ITI Touches] [float] NULL,
	[SUM_End Summary - Condition] [float] NULL,
	[SUM_End Summary - Correct Image] [float] NULL,
	[SUM_End Summary - Correct Rejections] [float] NULL,
	[SUM_End Summary - Correction Trial Correct Rejections] [float] NULL,
	[SUM_End Summary - Correction Trial Mistakes] [float] NULL,
	[SUM_End Summary - CorrectRejection at 0.2s SD] [float] NULL,
	[SUM_End Summary - CorrectRejection at 0.5s SD] [float] NULL,
	[SUM_End Summary - CorrectRejection at 1s SD] [float] NULL,
	[SUM_End Summary - CorrectRejection at 2s SD] [float] NULL,
	[SUM_End Summary - Corrects] [float] NULL,
	[SUM_End Summary - CS - Beam Breaking] [float] NULL,
	[SUM_End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[SUM_End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[SUM_End Summary - CS + Beam Breaking] [float] NULL,
	[SUM_End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[SUM_End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[SUM_End Summary - Hits] [float] NULL,
	[SUM_End Summary - Hits at 0.2s SD] [float] NULL,
	[SUM_End Summary - Hits at 0.5s SD] [float] NULL,
	[SUM_End Summary - Hits at 1s SD] [float] NULL,
	[SUM_End Summary - Hits at 2s SD] [float] NULL,
	[SUM_End Summary - Incorrect Touches] [float] NULL,
	[SUM_End Summary - Left ITI Touches] [float] NULL,
	[SUM_End Summary - Miss at 0.2s SD] [float] NULL,
	[SUM_End Summary - Miss at 0.5s SD] [float] NULL,
	[SUM_End Summary - Miss at 1s SD] [float] NULL,
	[SUM_End Summary - Miss at 2s SD] [float] NULL,
	[SUM_End Summary - Misses] [float] NULL,
	[SUM_End Summary - Mistake at 0.2s SD] [float] NULL,
	[SUM_End Summary - Mistake at 0.5s SD] [float] NULL,
	[SUM_End Summary - Mistake at 1s SD] [float] NULL,
	[SUM_End Summary - Mistake at 2s SD] [float] NULL,
	[SUM_End Summary - Mistakes] [float] NULL,
	[SUM_End Summary - No Correction Trials] [float] NULL,
	[SUM_End Summary - No of non correction trials] [float] NULL,
	[SUM_End Summary - No. images] [float] NULL,
	[SUM_END SUMMARY - Number of target touches] [float] NULL,
	[SUM_End Summary - Omissions - Omission] [float] NULL,
	[SUM_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [float] NULL,
	[SUM_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [float] NULL,
	[SUM_END SUMMARY - Revised total response time] [float] NULL,
	[SUM_END SUMMARY - REWARD COLLECTION LATENCY] [float] NULL,
	[SUM_End Summary - Right ITI Touches] [float] NULL,
	[SUM_END SUMMARY - Schedule Length] [float] NULL,
	[SUM_End Summary - Stimulus Duration] [float] NULL,
	[SUM_End Summary - Total Trials] [float] NULL,
	[SUM_End Summary - Touches to lit CS-] [float] NULL,
	[SUM_End Summary - Touches to lit CS+] [float] NULL,
	[SUM_End Summary - Tray Beam Breaking] [float] NULL,
	[SUM_End Summary - Tray CS - Beam Breaking] [float] NULL,
	[SUM_End Summary - Tray CS + Beam Breaking] [float] NULL,
	[SUM_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[SUM_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[SUM_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[SUM_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[SUM_End Summary - Trials Completed] [float] NULL,
	[SUM_False Alarm Rate at 0.2s] [float] NULL,
	[SUM_False Alarm Rate at 0.5s] [float] NULL,
	[SUM_False Alarm Rate at 1s] [float] NULL,
	[SUM_False Alarm Rate at 2s] [float] NULL,
	[SUM_Hit Rate at 0.2s] [float] NULL,
	[SUM_Hit Rate at 0.5s] [float] NULL,
	[SUM_Hit Rate at 1s] [float] NULL,
	[SUM_Hit Rate at 2s] [float] NULL,
	[SUM_Incorrect Choice Latency] [float] NULL,
	[SUM_Incorrect Touch Latency] [float] NULL,
	[SUM_No Distractor Correct Rejection] [float] NULL,
	[SUM_No Distractor False Alarm Latency] [float] NULL,
	[SUM_No Distractor False Alarm Rate] [float] NULL,
	[SUM_No Distractor Hit] [float] NULL,
	[SUM_No Distractor Hit Latency] [float] NULL,
	[SUM_No Distractor Hit Rate] [float] NULL,
	[SUM_No Distractor Miss] [float] NULL,
	[SUM_No Distractor Mistake] [float] NULL,
	[SUM_No Distractor Response Bias] [float] NULL,
	[SUM_No Distractor Reward Latency] [float] NULL,
	[SUM_No Distractor Sensitivity (d)] [float] NULL,
	[SUM_Normalized - End Summary - All CS- touches] [float] NULL,
	[SUM_Normalized - End Summary - All CS+ touches] [float] NULL,
	[SUM_Normalized - End Summary - CS - Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS + Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Touches to lit CS-] [float] NULL,
	[SUM_Normalized - End Summary - Touches to lit CS+] [float] NULL,
	[SUM_Normalized - End Summary - Tray Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Tray CS - Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Tray CS + Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[SUM_Omission to Tray Latency] [float] NULL,
	[SUM_Omissions - Total] [float] NULL,
	[SUM_Perseverative Correct - Total] [float] NULL,
	[SUM_Perseverative Incorrect - Total] [float] NULL,
	[SUM_Premature Responses - Total] [float] NULL,
	[SUM_Response Bias at 0.2s] [float] NULL,
	[SUM_Response Bias at 0.5s] [float] NULL,
	[SUM_Response Bias at 1s] [float] NULL,
	[SUM_Response Bias at 2s] [float] NULL,
	[SUM_Reward Collection Latency] [float] NULL,
	[SUM_Reward IR Breaks - Reward Beam Cnt] [float] NULL,
	[SUM_Reward Retrieval Latency] [float] NULL,
	[SUM_Screen IR Breaks - Screen IR Cnt] [float] NULL,
	[SUM_Threshold - Accuracy %] [float] NULL,
	[SUM_Threshold - Condition] [float] NULL,
	[SUM_Threshold - Omission %] [float] NULL,
	[SUM_Threshold - Trials] [float] NULL,
	[SUM_Tray entered - Cnt] [float] NULL,
	[SUM_Trial Analysis - Accuracy%] [float] NULL,
	[SUM_Trial Analysis - Condition] [float] NULL,
	[SUM_Trial Analysis - Correct] [float] NULL,
	[SUM_Trial Analysis - Correct Resp Latency with No Distract] [float] NULL,
	[SUM_Trial Analysis - Correct Response Latency] [float] NULL,
	[SUM_Trial Analysis - CS- Approach Latency] [float] NULL,
	[SUM_Trial Analysis - CS- Touch Latency] [float] NULL,
	[SUM_Trial Analysis - CS+ Approach Latency] [float] NULL,
	[SUM_Trial Analysis - CS+ Touch Latency] [float] NULL,
	[SUM_Trial Analysis - Incorrect] [float] NULL,
	[SUM_Trial Analysis - Incorrect Response Latency] [float] NULL,
	[SUM_Trial Analysis - Omission] [float] NULL,
	[SUM_Trial Analysis - Omission%] [float] NULL,
	[SUM_Trial Analysis - Omissions - Omission] [float] NULL,
	[SUM_Trial Analysis - Premature] [float] NULL,
	[SUM_Trial Analysis - Reward Collection Latency] [float] NULL,
	[SUM_Trial Analysis - Reward Given - Condition] [float] NULL,
	[SUM_Trial Analysis - Stimulus Duration] [float] NULL,
	[SUM_Trial Analysis - Time] [float] NULL,
	[SUM_Trial Analysis - Time To Distraction] [float] NULL,
	[SUM_trial by trial anal - Correct Rejections] [float] NULL,
	[SUM_trial by trial anal - Correction Trial Correct Rejections] [float] NULL,
	[SUM_trial by trial anal - Correction Trial Mistakes] [float] NULL,
	[SUM_trial by trial anal - Current image] [float] NULL,
	[SUM_trial by trial anal - Distractor Time] [float] NULL,
	[SUM_trial by trial anal - Hits] [float] NULL,
	[SUM_trial by trial anal - ITI] [float] NULL,
	[SUM_trial by trial anal - Misses] [float] NULL,
	[SUM_trial by trial anal - Mistakes] [float] NULL,
	[SUM_trial by trial anal - No of non correction trials] [float] NULL,
	[SUM_trial by trial anal - Stimulus Duration] [float] NULL,
	[SUM_Trial by trial analysis - Image response choice latency] [float] NULL,
	[SUM_Trial by trial analysis - Left chosen] [float] NULL,
	[SUM_Trial by trial analysis - One trial] [float] NULL,
	[SUM_Trial by trial analysis - Optimal side chosen] [float] NULL,
	[SUM_Trial by trial analysis - Optimal side chosen with milkshake given] [float] NULL,
	[SUM_Trial by trial analysis - Optimal side chosen without milkshake given] [float] NULL,
	[SUM_Trial by trial analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[SUM_Trial by trial analysis - Right chosen] [float] NULL,
	[SUM_Trial by trial analysis - Spurious feedback given] [float] NULL,
	[SUM_Trial by trial analysis - Spurious no milkshake] [float] NULL,
	[SUM_Trial by trial analysis - Spurious with milkshake] [float] NULL,
	[SUM_Trial by trial analysis - Trial type] [float] NULL,
	[SUM_True Blank Touches at Grid 1] [float] NULL,
	[SUM_True Blank Touches at Grid 2] [float] NULL,
	[SUM_True Blank Touches at Grid 3] [float] NULL,
	[SUM_True Blank Touches at Grid 4] [float] NULL,
	[SUM_True Blank Touches at Grid 5] [float] NULL,
	[SUM_Whole session analysis - Condition] [float] NULL,
	[SUM_Whole session analysis - Image response choice latency] [float] NULL,
	[SUM_Whole session analysis - Number of Reversals] [float] NULL,
	[SUM_Whole session analysis - Optimal side chosen] [float] NULL,
	[SUM_Whole session analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[SUM_Whole session analysis - Trials completed] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[FamilyName] [nvarchar](max) NULL,
	[GivenName] [nvarchar](max) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[TermsConfirmed] [bit] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Task]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[OriginalName] [nvarchar](250) NULL,
	[ShortName] [nvarchar](50) NULL,
	[TaskDescription] [text] NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Experiment]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Experiment](
	[ExpID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](450) NULL,
	[PUSID] [int] NULL,
	[TaskID] [int] NOT NULL,
	[SpeciesID] [int] NULL,
	[ExpName] [varchar](500) NULL,
	[StartExpDate] [date] NOT NULL,
	[EndExpDate] [date] NOT NULL,
	[TaskDescription] [nvarchar](max) NULL,
	[DOI] [nvarchar](200) NULL,
	[Status] [bit] NOT NULL,
	[TaskBattery] [text] NULL,
	[MultipleSessions] [bit] NULL,
	[RepoGuid] [uniqueidentifier] NULL,
	[DataCiteURL] [nvarchar](200) NULL,
 CONSTRAINT [PK_Experiment] PRIMARY KEY CLUSTERED 
(
	[ExpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Age]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Age](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AgeInMonth] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Age] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Genotype]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Genotype](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Genotype] [nvarchar](50) NOT NULL,
	[Link] [nvarchar](100) NULL,
	[Description] [nvarchar](50) NULL,
 CONSTRAINT [PK_Genotype] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Animal]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Animal](
	[AnimalID] [int] IDENTITY(1,1) NOT NULL,
	[ExpID] [int] NOT NULL,
	[UserAnimalID] [nvarchar](50) NOT NULL,
	[SID] [int] NULL,
	[GID] [int] NULL,
	[Sex] [varchar](1) NULL,
 CONSTRAINT [PK_Animal] PRIMARY KEY CLUSTERED 
(
	[AnimalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PIUserSite]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PIUserSite](
	[PUSID] [int] IDENTITY(1,1) NOT NULL,
	[PSID] [int] NOT NULL,
	[UserID] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_PIUserSite] PRIMARY KEY CLUSTERED 
(
	[PUSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Upload]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Upload](
	[UploadID] [int] IDENTITY(1,1) NOT NULL,
	[ExpID] [int] NOT NULL,
	[AnimalID] [int] NOT NULL,
	[SubExpID] [int] NOT NULL,
	[UserFileName] [nvarchar](256) NULL,
	[SysFileName] [nvarchar](256) NULL,
	[SessionName] [nvarchar](200) NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[WarningMessage] [nvarchar](max) NULL,
	[IsUploaded] [bit] NOT NULL,
	[IsDismissed] [bit] NOT NULL,
	[DateUpload] [datetime] NULL,
	[DateFileCreated] [datetime] NULL,
	[FileSize] [int] NULL,
	[FileUniqueID] [nvarchar](500) NULL,
	[FileContent] [nvarchar](max) NULL,
	[IsQcPassed] [bit] NOT NULL,
	[IsIdentifierPassed] [bit] NOT NULL,
	[PermanentFilePath] [nvarchar](500) NULL,
	[DateCreated] [datetime] NULL,
	[IsDuplicateSession] [bit] NULL,
 CONSTRAINT [PK_Upload] PRIMARY KEY CLUSTERED 
(
	[UploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SessionInfo]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SessionInfo](
	[SessionID] [int] IDENTITY(1,1) NOT NULL,
	[ExpID] [int] NOT NULL,
	[UploadID] [int] NOT NULL,
	[AnimalID] [int] NOT NULL,
	[UserID] [nvarchar](450) NOT NULL,
	[Database_Name] [nvarchar](50) NULL,
	[Date_Time] [nvarchar](50) NULL,
	[Environment] [nvarchar](50) NULL,
	[Machine_Name] [nvarchar](50) NULL,
	[SessionName] [nvarchar](200) NULL,
	[Analysis_Name] [nvarchar](100) NULL,
	[Schedule_Name] [nvarchar](max) NULL,
	[Stimulus_Duration] [nvarchar](50) NULL,
	[Guid] [nchar](10) NULL,
	[Schedule_Run_ID] [nvarchar](50) NULL,
	[Version] [nvarchar](20) NULL,
	[Version_Name] [nvarchar](max) NULL,
	[Application_Version] [nvarchar](300) NULL,
	[Max_Number_Trials] [int] NULL,
	[Max_Schedule_Time] [nvarchar](50) NULL,
	[Schedule_Description] [nvarchar](max) NULL,
	[Schedule_Start_Time] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NULL,
 CONSTRAINT [PK_SessionInfo] PRIMARY KEY CLUSTERED 
(
	[SessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubExperiment]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubExperiment](
	[SubExpID] [int] IDENTITY(1,1) NOT NULL,
	[ExpID] [int] NOT NULL,
	[AgeID] [int] NOT NULL,
	[SubExpName] [nvarchar](100) NOT NULL,
	[IsPostProcessingPass] [bit] NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[IsIntervention] [bit] NULL,
	[IsDrug] [bit] NULL,
	[DrugName] [nvarchar](50) NULL,
	[DrugUnit] [nvarchar](30) NULL,
	[DrugQuantity] [nvarchar](10) NULL,
	[InterventionDescription] [text] NULL,
	[ImageIds] [nvarchar](50) NULL,
	[ImageDescription] [text] NULL,
	[Housing] [nvarchar](50) NULL,
	[LightCycle] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubExperiment] PRIMARY KEY CLUSTERED 
(
	[SubExpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[View_PD_DataAnalysis_AllData]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* Query for Data Analysis*/
CREATE VIEW [dbo].[View_PD_DataAnalysis_AllData]
AS
SELECT        TOP (100) PERCENT dbo.SessionInfo.SessionID, dbo.Task.Name AS TaskName, dbo.Experiment.TaskID AS TaskId, dbo.Experiment.ExpID, dbo.Experiment.ExpName, dbo.Experiment.Status, 
                         dbo.Animal.UserAnimalID, dbo.Animal.AnimalID , dbo.SubExperiment.SubExpID, dbo.SubExperiment.AgeID, SubExperiment.ImageIds, dbo.Age.AgeInMonth AS Age, dbo.Animal.Sex, dbo.Animal.GID, dbo.Genotype.Genotype, dbo.Animal.SID, dbo.Strain.Strain, 
                         tt2.UserName AS Username, tt2.PID, tt2.PIName, tt2.Institution, dbo.SessionInfo.Date_Time, dbo.SessionInfo.SessionName, tmp.S_ID, tmp.[AVG_Correct Reward Collection], 
                         tmp.[AVG_End Summary - No Correction Trials], tmp.[AVG_End Summary - % Correct], tmp.[AVG_Correct touch latency]
FROM            dbo.SessionInfo INNER JOIN
                         dbo.Animal ON dbo.Animal.AnimalID = dbo.SessionInfo.AnimalID INNER JOIN
                         dbo.Experiment ON dbo.Experiment.ExpID = dbo.SessionInfo.ExpID INNER JOIN
                         dbo.Genotype ON dbo.Genotype.ID = dbo.Animal.GID INNER JOIN
                         dbo.Strain ON dbo.Strain.ID = dbo.Animal.SID INNER JOIN
                         dbo.Upload ON dbo.Upload.UploadID = dbo.SessionInfo.UploadID INNER JOIN
                         dbo.SubExperiment ON dbo.SubExperiment.SubExpID = dbo.Upload.SubExpID INNER JOIN
                         dbo.Age ON dbo.Age.ID = dbo.SubExperiment.AgeID INNER JOIN
                         dbo.Task ON dbo.Task.ID = dbo.Experiment.TaskID INNER JOIN
                             (SELECT        dbo.PIUserSite.PUSID, tt.PID, tt.PIName, tt.Institution, dbo.AspNetUsers.GivenName + '-' + dbo.AspNetUsers.FamilyName AS UserName
                               FROM            dbo.PIUserSite INNER JOIN
                                                             (SELECT        dbo.PI.PID, dbo.PISite.PSID, dbo.PI.PName AS PIName, dbo.Site.Institution, dbo.PI.PName + ' - ' + dbo.Site.Institution AS PISiteName
                                                               FROM            dbo.PISite INNER JOIN
                                                                                         dbo.PI ON dbo.PI.PID = dbo.PISite.PID INNER JOIN
                                                                                         dbo.Site ON dbo.Site.SiteID = dbo.PISite.SiteID) AS tt ON tt.PSID = dbo.PIUserSite.PSID INNER JOIN
                                                         dbo.AspNetUsers ON dbo.AspNetUsers.Id = dbo.PIUserSite.UserID) AS tt2 ON tt2.PUSID = dbo.Experiment.PUSID INNER JOIN
                             (SELECT        SessionID AS S_ID, [AVG_Correct Reward Collection], [AVG_End Summary - No Correction Trials], [AVG_End Summary - % Correct], [AVG_Correct touch latency]
                               FROM            dbo.rbt_data_cached) AS tmp ON tmp.S_ID = dbo.SessionInfo.SessionID
WHERE        (dbo.Experiment.TaskID = 3) AND (dbo.SessionInfo.SessionName IN ('Acquisition', 'Reversal', 'Baseline'))
ORDER BY dbo.SessionInfo.ExpID, dbo.Animal.UserAnimalID
GO
/****** Object:  View [dbo].[vw_visualization_main]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[vw_visualization_main]
AS

SELECT      dbo.SessionInfo.SessionID as SessionInfoSessionID, dbo.SessionInfo.UploadID, dbo.Animal.UserAnimalID, dbo.SubExperiment.SubExpID, dbo.Age.AgeInMonth AS Age, dbo.Task.Name AS Task,
dbo.Animal.Sex, dbo.Animal.GID, dbo.Genotype.Genotype, dbo.Animal.SID, dbo.Strain.Strain,
dbo.Experiment.ExpID, dbo.Experiment.ExpName, dbo.Experiment.Status, 
tt2.PISiteName + ' - ' + tt2.UserName AS PISiteUser,
SubExperiment.AgeID, Age.AgeInMonth,
dbo.Experiment.TaskID,
dbo.SessionInfo.Database_Name,
dbo.SessionInfo.Date_Time,
dbo.SessionInfo.Environment, 
dbo.SessionInfo.Machine_Name,
dbo.SessionInfo.SessionName,
dbo.SessionInfo.Analysis_Name,
dbo.SessionInfo.Schedule_Name,
dbo.SessionInfo.Stimulus_Duration,
dbo.SessionInfo.Schedule_Run_ID,
dbo.SessionInfo.Version,
dbo.SessionInfo.Version_Name, 
dbo.SessionInfo.Application_Version,
dbo.SessionInfo.Max_Number_Trials,
dbo.SessionInfo.Max_Schedule_Time,
dbo.SessionInfo.Schedule_Description,
dbo.SessionInfo.Schedule_Start_Time,
dbo.SessionInfo.DateCreated 

FROM  dbo.SessionInfo
INNER JOIN dbo.Animal ON dbo.Animal.AnimalID = dbo.SessionInfo.AnimalID
INNER JOIN dbo.Experiment ON dbo.Experiment.ExpID = dbo.SessionInfo.ExpID
INNER JOIN dbo.Genotype ON dbo.Genotype.ID = dbo.Animal.GID
INNER JOIN dbo.Strain ON dbo.Strain.ID = dbo.Animal.SID
INNER JOIN Upload on Upload.UploadID = SessionInfo.UploadID
INNER JOIN SubExperiment on SubExperiment.SubExpID = Upload.SubExpID
INNER JOIN Age on Age.ID = SubExperiment.AgeID
INNER JOIN dbo.Task ON dbo.Task.ID = dbo.Experiment.TaskID 
INNER JOIN
    (SELECT        dbo.PIUserSite.PUSID, dbo.PIUserSite.PSID, tt.PISiteName, dbo.AspNetUsers.GivenName + '-' + dbo.AspNetUsers.FamilyName AS UserName
      FROM            dbo.PIUserSite INNER JOIN
           (SELECT        dbo.PISite.PSID, dbo.PI.PName, dbo.Site.Institution, dbo.PI.PName + ' - ' + dbo.Site.Institution AS PISiteName
             FROM            dbo.PISite
			 INNER JOIN dbo.PI ON dbo.PI.PID = dbo.PISite.PID
			 INNER JOIN dbo.Site ON dbo.Site.SiteID = dbo.PISite.SiteID) AS tt ON tt.PSID = dbo.PIUserSite.PSID
			 INNER JOIN dbo.AspNetUsers ON dbo.AspNetUsers.Id = dbo.PIUserSite.UserID) AS tt2 ON tt2.PUSID = dbo.Experiment.PUSID 
WHERE        dbo.Experiment.Status = 1



GO
/****** Object:  View [dbo].[vw_5choice_visualization_TrainingSessions]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










CREATE VIEW [dbo].[vw_5choice_visualization_TrainingSessions]
AS

SELECT        ExpName, Task, UserAnimalID, Age, Sex, Strain, Genotype, PISiteUser,  Schedule_Name, Stimulus_Duration, SessionName, CAST(COUNT(*) AS FLOAT) AS [Trials to Criteria]
FROM          dbo.vw_visualization_main
WHERE        (TaskID = 2) AND ( SessionName='Training' ) And Age = '3_6'
GROUP BY UserAnimalID, Age, Task, Sex, Genotype, Strain, ExpName, PISiteUser, Schedule_Name, Stimulus_Duration, SessionName

GO
/****** Object:  View [dbo].[vw_visualization]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_visualization]
AS
SELECT        dbo.Animal.UserAnimalID, dbo.Age.AgeInMonth AS Age, dbo.Task.Name AS Task, dbo.Animal.Sex, dbo.Genotype.Genotype, dbo.Strain.Strain, dbo.Experiment.ExpName, dbo.Experiment.Status, 
                         tt2.PISiteName + ' - ' + tt2.UserName AS PISiteUser, dbo.SessionInfo.SessionID AS SessionInfoSessionID, dbo.SessionInfo.Database_Name, dbo.SessionInfo.Date_Time, dbo.SessionInfo.Environment, 
                         dbo.SessionInfo.Machine_Name, dbo.SessionInfo.Analysis_Name, dbo.SessionInfo.Schedule_Name, dbo.SessionInfo.Schedule_Run_ID, dbo.SessionInfo.Version, dbo.SessionInfo.Version_Name, 
                         dbo.SessionInfo.Application_Version, dbo.SessionInfo.Max_Number_Trials, dbo.SessionInfo.Max_Schedule_Time, dbo.SessionInfo.Schedule_Description, dbo.SessionInfo.Schedule_Start_Time, dbo.SessionInfo.DateCreated, 
                         dbo.Experiment.TaskID
FROM            dbo.SessionInfo INNER JOIN
                         dbo.Animal ON dbo.Animal.AnimalID = dbo.SessionInfo.AnimalID INNER JOIN
                         dbo.Experiment ON dbo.Experiment.ExpID = dbo.SessionInfo.ExpID INNER JOIN
                         dbo.Genotype ON dbo.Genotype.ID = dbo.Animal.GID INNER JOIN
                         dbo.Strain ON dbo.Strain.ID = dbo.Animal.SID INNER JOIN
                         dbo.Task ON dbo.Task.ID = dbo.Experiment.TaskID INNER JOIN
                             (SELECT        dbo.PIUserSite.PUSID, dbo.PIUserSite.PSID, tt.PISiteName, dbo.AspNetUsers.GivenName + '-' + dbo.AspNetUsers.FamilyName AS UserName
                               FROM            dbo.PIUserSite INNER JOIN
                                                             (SELECT        dbo.PISite.PSID, dbo.PI.PName, dbo.Site.Institution, dbo.PI.PName + ' - ' + dbo.Site.Institution AS PISiteName
                                                               FROM            dbo.PISite INNER JOIN
                                                                                         dbo.PI ON dbo.PI.PID = dbo.PISite.PID INNER JOIN
                                                                                         dbo.Site ON dbo.Site.SiteID = dbo.PISite.SiteID) AS tt ON tt.PSID = dbo.PIUserSite.PSID INNER JOIN
                                                         dbo.AspNetUsers ON dbo.AspNetUsers.Id = dbo.PIUserSite.UserID) AS tt2 ON tt2.PUSID = dbo.Experiment.PUSID INNER JOIN
                             (SELECT        dbo.Upload.UploadID, dbo.SubExperiment.ExpID, dbo.SubExperiment.SubExpID, dbo.SubExperiment.AgeID
                               FROM            dbo.SubExperiment INNER JOIN
                                                         dbo.Upload ON dbo.SubExperiment.SubExpID = dbo.Upload.SubExpID) AS ss ON ss.UploadID = dbo.SessionInfo.UploadID INNER JOIN
                         dbo.Age ON dbo.Age.ID = ss.AgeID
WHERE        (dbo.Experiment.Status = 1)
GO
/****** Object:  View [dbo].[vw_pal_acquisition_visualization]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[vw_pal_acquisition_visualization]
AS

Select	ExpName, Task, UserAnimalID, Age, Sex, Strain, Genotype, PISiteUser, Schedule_Name, SessionName, CAST(COUNT(*) AS FLOAT) AS [Sessions to Criteria]
From vw_visualization_main 
Where Taskid = 4 and SessionName = 'PAL-Acquisition'
Group by UserAnimalID, Age, Task, Sex, Genotype, Strain, ExpName, PISiteUser, Schedule_Name, SessionName


GO
/****** Object:  View [dbo].[vw_pd_reversal_visualization]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[vw_pd_reversal_visualization]
AS
with C1 as
(
  select ExpName,
	     ExpID,
         UserAnimalID,
         age,
		 Date_Time,
		 Task, Sex, Genotype, Strain, PISiteUser, Schedule_Name, SessionName,
 		 [Number of Correction Trials],
		 [% Correct],
		 [Correct Touch Latency],
		 [Reward Collection Latency],
		 		 
         lag(age) over(order by UserAnimalID, CAST(Date_Time as datetime)) as lag_age
         ,lag(useranimalid) over(order by UserAnimalID, CAST(Date_Time as datetime)) as lag_user_animal_id
  from (
		  select vv.ExpName, vv.ExpID, vv.UserAnimalID, vv.Age , vv.Date_Time, 
			[AVG_End Summary - No Correction Trials] as [Number of Correction Trials], [AVG_End Summary - % Correct] as [% Correct], [AVG_Correct touch latency] as [Correct Touch Latency], [AVG_Correct Reward Collection] as [Reward Collection Latency],
			Task, Sex, Genotype, Strain,  PISiteUser, Schedule_Name, SessionName
			from vw_visualization_main vv
			inner join rbt_data_cached on vv.SessionInfoSessionID = SessionID 
			where vv.Taskid = 3 
			and (vv.SessionName = 'Reversal') 
			group by vv.UserAnimalID, vv.ExpName, vv.ExpID, vv.Age, vv.Date_Time,
			[AVG_End Summary - No Correction Trials], [AVG_End Summary - % Correct], [AVG_Correct touch latency], [AVG_Correct Reward Collection],
			Task, Sex, Genotype, Strain,  PISiteUser, Schedule_Name, SessionName
  ) tmp
),
C2 as
(
  select ExpName,
         ExpID,
		 Task,
         UserAnimalID,
         Age,
		 Date_Time,
		 Sex, Genotype, Strain, PISiteUser, Schedule_Name, SessionName,
 		 [Number of Correction Trials],
		 [% Correct],
		 [Correct Touch Latency],
		 [Reward Collection Latency],
		 
         sum(case when age = lag_age and useranimalid = lag_user_animal_id then 0 else 1 end) 
            over(order by UserAnimalID, CAST(Date_Time as datetime) rows unbounded preceding) as change 
  from C1
)
select 
		ExpName,
		ExpID,
		 Task,
         UserAnimalID,
         Age,
		 Sex, Strain, Genotype, PISiteUser, Schedule_Name, SessionName,  CAST(Date_Time as datetime) as Date_Time,
		 row_number() over(partition by change order by UserAnimalID, CAST(Date_Time as datetime)) as Session,
 		 [Number of Correction Trials],
		 [% Correct],
		 [Correct Touch Latency],
		 [Reward Collection Latency]
		 
		 
from C2





GO
/****** Object:  View [dbo].[vw_pd_acquisition_visualization]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[vw_pd_acquisition_visualization]
AS
SELECT        ExpName, Task, UserAnimalID, Age, Sex, Strain, Genotype, PISiteUser,  Schedule_Name, SessionName, CAST(COUNT(*) AS FLOAT) AS [Sessions to Criteria] 
FROM            dbo.vw_visualization_main
WHERE        (TaskID = 3) AND (SessionName = 'PD-Acquisition')
GROUP BY UserAnimalID, Age, Task, Sex, Genotype, Strain, ExpName, PISiteUser, Schedule_Name, SessionName



GO
/****** Object:  View [dbo].[vw_5choice_visualization]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










CREATE VIEW [dbo].[vw_5choice_visualization]
AS

WITH C1 AS (SELECT  UserAnimalID, Age, Task, Sex, Genotype, Strain, ExpName, Status, PISiteUser, SessionInfoSessionID, 
Analysis_Name, SessionName, Schedule_Name, Stimulus_Duration, TaskID, ExpID
FROM         dbo.vw_visualization_main
WHERE        (TaskID = 2) AND (SessionName='Intra_Probe' OR SessionName='Re_Baseline' OR SessionName='Training' OR SessionName='Probe' )
),

C2 as
(

SELECT     C1_1.SessionInfoSessionID, C1_1.ExpName, C1_1.ExpID, C1_1.Task, C1_1.UserAnimalID, C1_1.Age, C1_1.Sex, C1_1.Strain, C1_1.Genotype, C1_1.PISiteUser, C1_1.Schedule_Name, C1_1.SessionName, C1_1.Stimulus_Duration,
           dbo.rbt_data_cached.[AVG_Threshold - Accuracy %] AS [% Correct], dbo.rbt_data_cached.[AVG_Threshold - Omission %] AS [% Omission], 
           dbo.rbt_data_cached.[AVG_Trial Analysis - Correct Response Latency] AS [Correct Touch Latency (s)], dbo.rbt_data_cached.[AVG_Reward Collection Latency] AS [Reward Collection Latency (s)], 
           dbo.rbt_data_cached.[AVG_Premature Responses - Total] AS [Premature Response], dbo.rbt_data_cached.[AVG_Perseverative Correct - Total] AS [Perseverative Response]
FROM       C1 AS C1_1 INNER JOIN
           dbo.rbt_data_cached ON dbo.rbt_data_cached.SessionID = C1_1.SessionInfoSessionID
)

Select C2.ExpName, C2.ExpID, C2.Task, C2.UserAnimalID, C2.Age, C2.Sex, C2.Strain, C2.Genotype, C2.PISiteUser, C2.SessionName, C2.Stimulus_Duration,
           AVG(C2.[% Correct]) AS [% Correct],
		   AVG(C2.[% Omission]) AS [% Omission], 
           AVG(C2.[Correct Touch Latency (s)]) AS [Correct Touch Latency (s)],
		   AVG(C2.[Reward Collection Latency (s)]) AS [Reward Collection Latency (s)], 
           AVG(C2.[Premature Response]) AS [Premature Response],
		   AVG(C2.[Perseverative Response]) AS [Perseverative Response]

		   

From C2
Group by C2.ExpName, C2.ExpID, C2.Task, C2.UserAnimalID, C2.Age, C2.Sex, C2.Strain, C2.Genotype, C2.PISiteUser, C2.SessionName, C2.Stimulus_Duration

GO
/****** Object:  Table [dbo].[RBT_TouchScreen_Features]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RBT_TouchScreen_Features](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SessionID] [int] NOT NULL,
	[SourceTypeID] [int] NOT NULL,
	[FeatureName] [nvarchar](200) NOT NULL,
	[Results] [float] NULL,
	[Time] [float] NULL,
	[Duration] [float] NULL,
	[Count] [float] NULL,
 CONSTRAINT [PK_RBT_TouchScreen_Features] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_RBT_Cache]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_RBT_Cache]
AS
SELECT        FeatureName, SessionID
FROM            dbo.RBT_TouchScreen_Features
GROUP BY FeatureName, SessionID
GO
/****** Object:  View [dbo].[View_tmp_PD]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_tmp_PD]
AS
-- Query to add Session number to all files
with C1 as
(
	Select *, 
	lag(AgeID) over(order by AnimalID, ageID, CAST(Date_Time as datetime)) as lag_age
    ,lag(AnimalID) over(order by AnimalID, ageID, CAST(Date_Time as datetime)) as lag_user_animal_id

	FROM View_PD_DataAnalysis_AllData

	Where isnull([AVG_Correct Reward Collection], 0) <=5 and isnull([AVG_Correct touch latency], 0)<=8 --and animalid = 932

),

C2 as
(
	Select *,
	sum(case when AgeID = lag_age and AnimalID = lag_user_animal_id then 0 else 1 end) 
            over(order by AnimalID, ageID, CAST(Date_Time as datetime) rows unbounded preceding) as change
		From C1

)


Select SessionID, ExpID, Status, UserAnimalID, AnimalID, AgeID, Age, Sex, GID, Genotype, SID, Strain, ImageIds, Username, PIName, CAST(Date_Time as datetime) as Date_Time, SessionName, 

[AVG_Correct Reward Collection],
[AVG_End Summary - No Correction Trials],
[AVG_End Summary - % Correct],
[AVG_Correct touch latency],

row_number() over(partition by change order by AnimalID, ageID, CAST(Date_Time as datetime)) as Session

 From C2
 Where C2.SessionName = 'Acquisition'

 UNION

 Select SessionID, ExpID, Status, UserAnimalID, AnimalID, AgeID, Age, Sex, GID, Genotype, SID, Strain, ImageIds, Username, PIName, CAST(Date_Time as datetime) as Date_Time, SessionName, 

[AVG_Correct Reward Collection],
[AVG_End Summary - No Correction Trials],
[AVG_End Summary - % Correct],
[AVG_Correct touch latency],

row_number() over(partition by change order by AnimalID, ageID, CAST(Date_Time as datetime)) as Session

 From C2
 Where C2.SessionName = 'Baseline'

 UNION

 Select top 100 percent SessionID, ExpID, Status, UserAnimalID, AnimalID, AgeID, Age, Sex, GID, Genotype, SID, Strain, ImageIds, Username, PIName, CAST(Date_Time as datetime) as Date_Time, SessionName, 

[AVG_Correct Reward Collection],
[AVG_End Summary - No Correction Trials],
[AVG_End Summary - % Correct],
[AVG_Correct touch latency],

row_number() over(partition by change order by AnimalID, ageID, CAST(Date_Time as datetime)) as Session

 From C2
 Where C2.SessionName = 'Reversal' 

Order by EXPID , UserAnimalID

GO
/****** Object:  View [dbo].[View_MB_Data]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[View_MB_Data]
AS
SELECT        dbo.SessionInfo.SessionID AS SessionInfoSessionID, dbo.SessionInfo.UploadID, dbo.Animal.UserAnimalID, dbo.Animal.AnimalID, dbo.SubExperiment.SubExpID, dbo.Age.AgeInMonth AS Age, dbo.Task.Name AS Task, dbo.Task.ShortName AS TaskName,
						
						 (CAST(dbo.Experiment.ExpID as varchar) + '_' + dbo.Task.ShortName) As Experiment,
						 dbo.Animal.Sex,
                         dbo.Animal.GID, dbo.Genotype.Genotype, dbo.Animal.SID, dbo.Strain.Strain, dbo.Strain.Disease_Model, dbo.Experiment.ExpID, dbo.Experiment.ExpName, dbo.Experiment.Status, tt2.PName As PIName, tt2.UserName AS UserName,
						 tt2.Institution, 
                         dbo.SubExperiment.AgeID, dbo.Age.AgeInMonth, dbo.Experiment.TaskID, dbo.SessionInfo.Database_Name, dbo.SessionInfo.Date_Time, dbo.SessionInfo.Environment, dbo.SessionInfo.Machine_Name, 
                         dbo.SessionInfo.SessionName, dbo.SessionInfo.Analysis_Name, dbo.SessionInfo.Schedule_Name, dbo.SessionInfo.Stimulus_Duration, dbo.SessionInfo.Schedule_Run_ID, dbo.SessionInfo.Version, 
                         dbo.SessionInfo.Version_Name, dbo.SessionInfo.Application_Version, dbo.SessionInfo.Max_Number_Trials, dbo.SessionInfo.Max_Schedule_Time, dbo.SessionInfo.Schedule_Description, 
                         dbo.SessionInfo.Schedule_Start_Time, dbo.SessionInfo.DateCreated
FROM            dbo.SessionInfo INNER JOIN
                         dbo.Animal ON dbo.Animal.AnimalID = dbo.SessionInfo.AnimalID INNER JOIN
                         dbo.Experiment ON dbo.Experiment.ExpID = dbo.SessionInfo.ExpID INNER JOIN
                         dbo.Genotype ON dbo.Genotype.ID = dbo.Animal.GID INNER JOIN
                         dbo.Strain ON dbo.Strain.ID = dbo.Animal.SID INNER JOIN
                         dbo.Upload ON dbo.Upload.UploadID = dbo.SessionInfo.UploadID INNER JOIN
                         dbo.SubExperiment ON dbo.SubExperiment.SubExpID = dbo.Upload.SubExpID INNER JOIN
                         dbo.Age ON dbo.Age.ID = dbo.SubExperiment.AgeID INNER JOIN
                         dbo.Task ON dbo.Task.ID = dbo.Experiment.TaskID INNER JOIN
                             (SELECT        dbo.PIUserSite.PUSID, dbo.PIUserSite.PSID, tt.PName, dbo.AspNetUsers.FamilyName AS UserName, tt.Institution
                               FROM            dbo.PIUserSite INNER JOIN
                                                             (SELECT        dbo.PISite.PSID, dbo.PI.PName, dbo.Site.Institution
                                                               FROM            dbo.PISite INNER JOIN
                                                                                         dbo.PI ON dbo.PI.PID = dbo.PISite.PID INNER JOIN
                                                                                         dbo.Site ON dbo.Site.SiteID = dbo.PISite.SiteID) AS tt ON tt.PSID = dbo.PIUserSite.PSID INNER JOIN
                                                         dbo.AspNetUsers ON dbo.AspNetUsers.Id = dbo.PIUserSite.UserID) AS tt2 ON tt2.PUSID = dbo.Experiment.PUSID

GO
/****** Object:  View [dbo].[vw_dPALsPAL_visualization]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[vw_dPALsPAL_visualization]
AS

WITH C1 AS (
  select ExpName,
         UserAnimalID,
         age,
		 Date_Time,
		 Task, Sex, Genotype, Strain, PISiteUser, SessionName, Schedule_Name,
 		 [Number of Correction Trials],
		 [% Correct],
		 [Correct Touch Latency],
		 [Reward Collection Latency]		 		 
		,lag(ltrim(age)) over(order by UserAnimalID, CAST(Date_Time as datetime), SessionName) as lag_age
		,lag(useranimalid) over(order by UserAnimalID, CAST(Date_Time as datetime), SessionName) as lag_user_animal_id
	    ,lag(ltrim(SessionName))over(order by UserAnimalID, CAST(Date_Time as datetime)) as lag_SessionName
		from (
		  select vv.ExpName, vv.UserAnimalID, vv.Age , vv.Date_Time, 
			[AVG_End Summary - No Correction Trials] as [Number of Correction Trials], [AVG_End Summary - % Correct] as [% Correct], [AVG_Correct touch latency] as [Correct Touch Latency], [AVG_Correct Reward Collection] as [Reward Collection Latency],
			Task, Sex, Genotype, Strain,  PISiteUser,  SessionName, Schedule_Name
			from vw_visualization_main vv
			inner join rbt_data_cached on vv.SessionInfoSessionID = SessionID 
			where vv.Taskid = 4 
			and (vv.SessionName = 'Mouse_dpal_spal')
			group by vv.UserAnimalID, vv.ExpName, vv.Age, vv.Date_Time,
			[AVG_End Summary - No Correction Trials], [AVG_End Summary - % Correct], [AVG_Correct touch latency], [AVG_Correct Reward Collection],
			Task, Sex, Genotype, Strain,  PISiteUser, SessionName, Schedule_Name
			
  ) tmp
),
C2 as
(
  select  
		 ExpName,
         UserAnimalID,
         Age,
		 Date_Time,
		 Task, Sex, Genotype, Strain, PISiteUser,  SessionName, Schedule_Name,
 		 [Number of Correction Trials],
		 [% Correct],
		 [Correct Touch Latency],
		 [Reward Collection Latency],
         sum(case when age = lag_age and useranimalid = lag_user_animal_id and SessionName=lag_SessionName then 0 else 1 end) 
            over(order by UserAnimalID, age rows unbounded preceding) as change 
  from C1
),
C3 as
(
select
		--RANK() OVER (ORDER BY UserAnimalID, convert(datetime, Date_Time, 103), Schedule_name) Rank,
		 ExpName,
         UserAnimalID,
         Age,
		 Date_Time,
		 Task, Sex, Genotype, Strain, PISiteUser,  SessionName, Schedule_Name,
		 row_number() over(partition by change order by UserAnimalID, age, SessionName) as Session,
 		 [Number of Correction Trials],
		 [% Correct],
		 [Correct Touch Latency],
		 [Reward Collection Latency]		 
from C2
)

SELECT   ExpName,
         UserAnimalID,
         Age,
		 
		 Task, Sex, Genotype, Strain, PISiteUser,  SessionName, Schedule_Name, ((Session-1) / 5) + 1 as [Session],
   AVG([Number of Correction Trials]) AS [Number of Correction Trials], AVG([% Correct]) As [% Correct], AVG([Correct Touch Latency]) AS [Correct Touch Latency], AVG([Reward Collection Latency]) AS [Reward Collection Latency]
			
FROM C3
GROUP BY ((Session-1) / 5) + 1, ExpName, UserAnimalID, Age, SessionName, Task, Sex, Genotype, Strain, PISiteUser, Schedule_Name





GO
/****** Object:  UserDefinedFunction [dbo].[SUBSTRING_INDEX]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 --Function definition for SUBSTRING_INDEX
CREATE FUNCTION [dbo].[SUBSTRING_INDEX]
(
    @str NVARCHAR(4000),
    @delim NVARCHAR(1),
    @count INT
)
RETURNS NVARCHAR(4000)
WITH SCHEMABINDING
BEGIN
    DECLARE @XmlSourceString XML;
    SET @XmlSourceString = (SELECT N'<root><row>' + REPLACE( (SELECT @str AS '*' FOR XML PATH('')) , @delim, N'</row><row>' ) + N'</row></root>');

    RETURN STUFF
    (
        ((
            SELECT  @delim + x.XmlCol.value(N'(text())[1]', N'NVARCHAR(4000)') AS '*'
            FROM    @XmlSourceString.nodes(N'(root/row)[position() <= sql:variable("@count")]') x(XmlCol)
            FOR XML PATH(N''), TYPE
        ).value(N'.', N'NVARCHAR(4000)')), 
        1, 1, N''
    );
END
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Image]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Links]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Links](
	[LinkID] [int] IDENTITY(1,1) NOT NULL,
	[LinkGuid] [uniqueidentifier] NOT NULL,
	[IsSaved] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[TaskID] [int] NOT NULL,
	[TaskName] [varchar](50) NULL,
	[Species] [nvarchar](50) NULL,
	[SpeciesID] [int] NULL,
	[SubTaskId] [int] NOT NULL,
	[SubTaskName] [varchar](50) NULL,
	[ExpIdCsv] [varchar](500) NULL,
	[ExpNameCsv] [varchar](1000) NULL,
	[AnimalAgeCsv] [varchar](500) NULL,
	[AnimalSexCsv] [varchar](500) NULL,
	[AnimalGenotypeCsv] [varchar](500) NULL,
	[AnimalStrainCsv] [varchar](500) NULL,
	[PiSiteIdsCsv] [varchar](500) NULL,
	[PiSiteNameCsv] [varchar](1000) NULL,
	[SessionInfoNamesCsv] [varchar](500) NULL,
	[MarkerInfoNamesCsv] [text] NULL,
	[AggNamesCsv] [varchar](500) NULL,
	[IsTrialByTrial] [bit] NULL,
	[SubExpIDcsv] [varchar](500) NULL,
	[SessionNameCsv] [nvarchar](500) NULL,
 CONSTRAINT [PK_Links] PRIMARY KEY CLUSTERED 
(
	[LinkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mapping_MarkerData]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mapping_MarkerData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OriginalName] [nvarchar](400) NOT NULL,
	[MappedName] [nvarchar](400) NOT NULL,
	[SourceType] [nvarchar](50) NOT NULL,
	[IsDuplicate] [bit] NULL,
	[IsTaskSpecific] [bit] NULL,
	[SubTaskID] [int] NULL,
 CONSTRAINT [PK_Mapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mapping_SessionInfo]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mapping_SessionInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OriginalName] [nvarchar](50) NOT NULL,
	[MappedName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Mapping_SessionInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Metrics]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Metrics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [date] NOT NULL,
	[ButtonName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Metrics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rbt_cached]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rbt_cached](
	[FeatureName] [nvarchar](200) NOT NULL,
	[SessionID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rbt_data_cached_avg]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rbt_data_cached_avg](
	[SessionID] [int] NOT NULL,
	[AVG_Average - Correct Choice Latency at 0.2s SD] [float] NULL,
	[AVG_Average - Correct Choice Latency at 0.5s SD] [float] NULL,
	[AVG_Average - Correct Choice Latency at 1s SD] [float] NULL,
	[AVG_Average - Correct Choice Latency at 2s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 0.2s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 0.5s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 1s SD] [float] NULL,
	[AVG_Average - Incorrect Choice Latency at 2s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 0.2s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 0.5s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 1s SD] [float] NULL,
	[AVG_Average - Reward Retrieval Latency at 2s SD] [float] NULL,
	[AVG_Blank Touch Latency] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 1] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 2] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 3] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 4] [float] NULL,
	[AVG_Blank touches at grid specific - Grid 5] [float] NULL,
	[AVG_Correct Centre touch latency] [float] NULL,
	[AVG_Correct Choice Latency] [float] NULL,
	[AVG_Correct latency grid 1] [float] NULL,
	[AVG_Correct latency grid 2] [float] NULL,
	[AVG_Correct latency grid 3] [float] NULL,
	[AVG_Correct latency grid 4] [float] NULL,
	[AVG_Correct latency grid 5] [float] NULL,
	[AVG_Correct Reward Collection] [float] NULL,
	[AVG_Correct touch latency] [float] NULL,
	[AVG_count at 0.2s] [float] NULL,
	[AVG_count at 0.5s] [float] NULL,
	[AVG_count at 1s] [float] NULL,
	[AVG_count at 2s] [float] NULL,
	[AVG_Discrimination Sensitivity at 0.2s ] [float] NULL,
	[AVG_Discrimination Sensitivity at 0.5s ] [float] NULL,
	[AVG_Discrimination Sensitivity at 1s ] [float] NULL,
	[AVG_Discrimination Sensitivity at 2s ] [float] NULL,
	[AVG_Distractor 0.5s Delay Correct Rejection] [float] NULL,
	[AVG_Distractor 0.5s Delay False Alarm Latency] [float] NULL,
	[AVG_Distractor 0.5s Delay False Alarm Rate] [float] NULL,
	[AVG_Distractor 0.5s Delay Hit] [float] NULL,
	[AVG_Distractor 0.5s Delay Hit Latency] [float] NULL,
	[AVG_Distractor 0.5s Delay Hit Rate] [float] NULL,
	[AVG_Distractor 0.5s Delay Miss] [float] NULL,
	[AVG_Distractor 0.5s Delay Mistake] [float] NULL,
	[AVG_Distractor 0.5s Delay Response Bias] [float] NULL,
	[AVG_Distractor 0.5s Delay Reward Latency] [float] NULL,
	[AVG_Distractor 0.5s Delay Sensitivity (d)] [float] NULL,
	[AVG_Distractor 1s Delay Correct Rejection] [float] NULL,
	[AVG_Distractor 1s Delay False Alarm Latency] [float] NULL,
	[AVG_Distractor 1s Delay False Alarm Rate] [float] NULL,
	[AVG_Distractor 1s Delay Hit] [float] NULL,
	[AVG_Distractor 1s Delay Hit Latency] [float] NULL,
	[AVG_Distractor 1s Delay Hit Rate] [float] NULL,
	[AVG_Distractor 1s Delay Miss] [float] NULL,
	[AVG_Distractor 1s Delay Mistake] [float] NULL,
	[AVG_Distractor 1s Delay Response Bias] [float] NULL,
	[AVG_Distractor 1s Delay Reward Latency] [float] NULL,
	[AVG_Distractor 1s Delay Sensitivity (d)] [float] NULL,
	[AVG_Distrator Presentation Correct Rejection] [float] NULL,
	[AVG_Distrator Presentation False Alarm Latency] [float] NULL,
	[AVG_Distrator Presentation False Alarm Rate] [float] NULL,
	[AVG_Distrator Presentation Hit] [float] NULL,
	[AVG_Distrator Presentation Hit Latency] [float] NULL,
	[AVG_Distrator Presentation Hit Rate] [float] NULL,
	[AVG_Distrator Presentation Miss] [float] NULL,
	[AVG_Distrator Presentation Mistake] [float] NULL,
	[AVG_Distrator Presentation Response Bias] [float] NULL,
	[AVG_Distrator Presentation Reward Latency] [float] NULL,
	[AVG_Distrator Presentation Sensitivity (d)] [float] NULL,
	[AVG_End Summary - % Correct] [float] NULL,
	[AVG_End Summary - After Reward Pause] [float] NULL,
	[AVG_End Summary - All CS- touches] [float] NULL,
	[AVG_End Summary - All CS+ touches] [float] NULL,
	[AVG_End Summary - Blank Touches] [float] NULL,
	[AVG_END SUMMARY - Breakpoint] [float] NULL,
	[AVG_End Summary - Centre ITI Touches] [float] NULL,
	[AVG_End Summary - Condition] [float] NULL,
	[AVG_End Summary - Correct Image] [float] NULL,
	[AVG_End Summary - Correct Rejections] [float] NULL,
	[AVG_End Summary - Correction Trial Correct Rejections] [float] NULL,
	[AVG_End Summary - Correction Trial Mistakes] [float] NULL,
	[AVG_End Summary - CorrectRejection at 0.2s SD] [float] NULL,
	[AVG_End Summary - CorrectRejection at 0.5s SD] [float] NULL,
	[AVG_End Summary - CorrectRejection at 1s SD] [float] NULL,
	[AVG_End Summary - CorrectRejection at 2s SD] [float] NULL,
	[AVG_End Summary - Corrects] [float] NULL,
	[AVG_End Summary - CS - Beam Breaking] [float] NULL,
	[AVG_End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[AVG_End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[AVG_End Summary - CS + Beam Breaking] [float] NULL,
	[AVG_End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[AVG_End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[AVG_End Summary - Hits] [float] NULL,
	[AVG_End Summary - Hits at 0.2s SD] [float] NULL,
	[AVG_End Summary - Hits at 0.5s SD] [float] NULL,
	[AVG_End Summary - Hits at 1s SD] [float] NULL,
	[AVG_End Summary - Hits at 2s SD] [float] NULL,
	[AVG_End Summary - Incorrect Touches] [float] NULL,
	[AVG_End Summary - Left ITI Touches] [float] NULL,
	[AVG_End Summary - Miss at 0.2s SD] [float] NULL,
	[AVG_End Summary - Miss at 0.5s SD] [float] NULL,
	[AVG_End Summary - Miss at 1s SD] [float] NULL,
	[AVG_End Summary - Miss at 2s SD] [float] NULL,
	[AVG_End Summary - Misses] [float] NULL,
	[AVG_End Summary - Mistake at 0.2s SD] [float] NULL,
	[AVG_End Summary - Mistake at 0.5s SD] [float] NULL,
	[AVG_End Summary - Mistake at 1s SD] [float] NULL,
	[AVG_End Summary - Mistake at 2s SD] [float] NULL,
	[AVG_End Summary - Mistakes] [float] NULL,
	[AVG_End Summary - No Correction Trials] [float] NULL,
	[AVG_End Summary - No of non correction trials] [float] NULL,
	[AVG_End Summary - No. images] [float] NULL,
	[AVG_END SUMMARY - Number of target touches] [float] NULL,
	[AVG_End Summary - Omissions - Omission] [float] NULL,
	[AVG_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [float] NULL,
	[AVG_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [float] NULL,
	[AVG_END SUMMARY - Revised total response time] [float] NULL,
	[AVG_END SUMMARY - REWARD COLLECTION LATENCY] [float] NULL,
	[AVG_End Summary - Right ITI Touches] [float] NULL,
	[AVG_END SUMMARY - Schedule Length] [float] NULL,
	[AVG_End Summary - Stimulus Duration] [float] NULL,
	[AVG_End Summary - Total Trials] [float] NULL,
	[AVG_End Summary - Touches to lit CS-] [float] NULL,
	[AVG_End Summary - Touches to lit CS+] [float] NULL,
	[AVG_End Summary - Tray Beam Breaking] [float] NULL,
	[AVG_End Summary - Tray CS - Beam Breaking] [float] NULL,
	[AVG_End Summary - Tray CS + Beam Breaking] [float] NULL,
	[AVG_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[AVG_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[AVG_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[AVG_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[AVG_End Summary - Trials Completed] [float] NULL,
	[AVG_False Alarm Rate at 0.2s] [float] NULL,
	[AVG_False Alarm Rate at 0.5s] [float] NULL,
	[AVG_False Alarm Rate at 1s] [float] NULL,
	[AVG_False Alarm Rate at 2s] [float] NULL,
	[AVG_Hit Rate at 0.2s] [float] NULL,
	[AVG_Hit Rate at 0.5s] [float] NULL,
	[AVG_Hit Rate at 1s] [float] NULL,
	[AVG_Hit Rate at 2s] [float] NULL,
	[AVG_Incorrect Choice Latency] [float] NULL,
	[AVG_Incorrect Touch Latency] [float] NULL,
	[AVG_No Distractor Correct Rejection] [float] NULL,
	[AVG_No Distractor False Alarm Latency] [float] NULL,
	[AVG_No Distractor False Alarm Rate] [float] NULL,
	[AVG_No Distractor Hit] [float] NULL,
	[AVG_No Distractor Hit Latency] [float] NULL,
	[AVG_No Distractor Hit Rate] [float] NULL,
	[AVG_No Distractor Miss] [float] NULL,
	[AVG_No Distractor Mistake] [float] NULL,
	[AVG_No Distractor Response Bias] [float] NULL,
	[AVG_No Distractor Reward Latency] [float] NULL,
	[AVG_No Distractor Sensitivity (d)] [float] NULL,
	[AVG_Normalized - End Summary - All CS- touches] [float] NULL,
	[AVG_Normalized - End Summary - All CS+ touches] [float] NULL,
	[AVG_Normalized - End Summary - CS - Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS + Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Touches to lit CS-] [float] NULL,
	[AVG_Normalized - End Summary - Touches to lit CS+] [float] NULL,
	[AVG_Normalized - End Summary - Tray Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Tray CS - Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Tray CS + Beam Breaking] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[AVG_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[AVG_Omission to Tray Latency] [float] NULL,
	[AVG_Omissions - Total] [float] NULL,
	[AVG_Perseverative Correct - Total] [float] NULL,
	[AVG_Perseverative Incorrect - Total] [float] NULL,
	[AVG_Premature Responses - Total] [float] NULL,
	[AVG_Response Bias at 0.2s] [float] NULL,
	[AVG_Response Bias at 0.5s] [float] NULL,
	[AVG_Response Bias at 1s] [float] NULL,
	[AVG_Response Bias at 2s] [float] NULL,
	[AVG_Reward Collection Latency] [float] NULL,
	[AVG_Reward IR Breaks - Reward Beam Cnt] [float] NULL,
	[AVG_Reward Retrieval Latency] [float] NULL,
	[AVG_Screen IR Breaks - Screen IR Cnt] [float] NULL,
	[AVG_Threshold - Accuracy %] [float] NULL,
	[AVG_Threshold - Condition] [float] NULL,
	[AVG_Threshold - Omission %] [float] NULL,
	[AVG_Threshold - Trials] [float] NULL,
	[AVG_Tray entered - Cnt] [float] NULL,
	[AVG_Trial Analysis - Accuracy%] [float] NULL,
	[AVG_Trial Analysis - Condition] [float] NULL,
	[AVG_Trial Analysis - Correct] [float] NULL,
	[AVG_Trial Analysis - Correct Resp Latency with No Distract] [float] NULL,
	[AVG_Trial Analysis - Correct Response Latency] [float] NULL,
	[AVG_Trial Analysis - CS- Approach Latency] [float] NULL,
	[AVG_Trial Analysis - CS- Touch Latency] [float] NULL,
	[AVG_Trial Analysis - CS+ Approach Latency] [float] NULL,
	[AVG_Trial Analysis - CS+ Touch Latency] [float] NULL,
	[AVG_Trial Analysis - Incorrect] [float] NULL,
	[AVG_Trial Analysis - Incorrect Response Latency] [float] NULL,
	[AVG_Trial Analysis - Omission] [float] NULL,
	[AVG_Trial Analysis - Omission%] [float] NULL,
	[AVG_Trial Analysis - Omissions - Omission] [float] NULL,
	[AVG_Trial Analysis - Premature] [float] NULL,
	[AVG_Trial Analysis - Reward Collection Latency] [float] NULL,
	[AVG_Trial Analysis - Reward Given - Condition] [float] NULL,
	[AVG_Trial Analysis - Stimulus Duration] [float] NULL,
	[AVG_Trial Analysis - Time] [float] NULL,
	[AVG_Trial Analysis - Time To Distraction] [float] NULL,
	[AVG_trial by trial anal - Correct Rejections] [float] NULL,
	[AVG_trial by trial anal - Correction Trial Correct Rejections] [float] NULL,
	[AVG_trial by trial anal - Correction Trial Mistakes] [float] NULL,
	[AVG_trial by trial anal - Current image] [float] NULL,
	[AVG_trial by trial anal - Distractor Time] [float] NULL,
	[AVG_trial by trial anal - Hits] [float] NULL,
	[AVG_trial by trial anal - ITI] [float] NULL,
	[AVG_trial by trial anal - Misses] [float] NULL,
	[AVG_trial by trial anal - Mistakes] [float] NULL,
	[AVG_trial by trial anal - No of non correction trials] [float] NULL,
	[AVG_trial by trial anal - Stimulus Duration] [float] NULL,
	[AVG_Trial by trial analysis - Image response choice latency] [float] NULL,
	[AVG_Trial by trial analysis - Left chosen] [float] NULL,
	[AVG_Trial by trial analysis - One trial] [float] NULL,
	[AVG_Trial by trial analysis - Optimal side chosen] [float] NULL,
	[AVG_Trial by trial analysis - Optimal side chosen with milkshake given] [float] NULL,
	[AVG_Trial by trial analysis - Optimal side chosen without milkshake given] [float] NULL,
	[AVG_Trial by trial analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[AVG_Trial by trial analysis - Right chosen] [float] NULL,
	[AVG_Trial by trial analysis - Spurious feedback given] [float] NULL,
	[AVG_Trial by trial analysis - Spurious no milkshake] [float] NULL,
	[AVG_Trial by trial analysis - Spurious with milkshake] [float] NULL,
	[AVG_Trial by trial analysis - Trial type] [float] NULL,
	[AVG_True Blank Touches at Grid 1] [float] NULL,
	[AVG_True Blank Touches at Grid 2] [float] NULL,
	[AVG_True Blank Touches at Grid 3] [float] NULL,
	[AVG_True Blank Touches at Grid 4] [float] NULL,
	[AVG_True Blank Touches at Grid 5] [float] NULL,
	[AVG_Whole session analysis - Condition] [float] NULL,
	[AVG_Whole session analysis - Image response choice latency] [float] NULL,
	[AVG_Whole session analysis - Number of Reversals] [float] NULL,
	[AVG_Whole session analysis - Optimal side chosen] [float] NULL,
	[AVG_Whole session analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[AVG_Whole session analysis - Trials completed] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rbt_data_cached_cnt]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rbt_data_cached_cnt](
	[SessionID] [int] NOT NULL,
	[COUNT_Average - Correct Choice Latency at 0.2s SD] [int] NULL,
	[COUNT_Average - Correct Choice Latency at 0.5s SD] [int] NULL,
	[COUNT_Average - Correct Choice Latency at 1s SD] [int] NULL,
	[COUNT_Average - Correct Choice Latency at 2s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 0.2s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 0.5s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 1s SD] [int] NULL,
	[COUNT_Average - Incorrect Choice Latency at 2s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 0.2s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 0.5s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 1s SD] [int] NULL,
	[COUNT_Average - Reward Retrieval Latency at 2s SD] [int] NULL,
	[COUNT_Blank Touch Latency] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 1] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 2] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 3] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 4] [int] NULL,
	[COUNT_Blank touches at grid specific - Grid 5] [int] NULL,
	[COUNT_Correct Centre touch latency] [int] NULL,
	[COUNT_Correct Choice Latency] [int] NULL,
	[COUNT_Correct latency grid 1] [int] NULL,
	[COUNT_Correct latency grid 2] [int] NULL,
	[COUNT_Correct latency grid 3] [int] NULL,
	[COUNT_Correct latency grid 4] [int] NULL,
	[COUNT_Correct latency grid 5] [int] NULL,
	[COUNT_Correct Reward Collection] [int] NULL,
	[COUNT_Correct touch latency] [int] NULL,
	[COUNT_count at 0.2s] [int] NULL,
	[COUNT_count at 0.5s] [int] NULL,
	[COUNT_count at 1s] [int] NULL,
	[COUNT_count at 2s] [int] NULL,
	[COUNT_Discrimination Sensitivity at 0.2s ] [int] NULL,
	[COUNT_Discrimination Sensitivity at 0.5s ] [int] NULL,
	[COUNT_Discrimination Sensitivity at 1s ] [int] NULL,
	[COUNT_Discrimination Sensitivity at 2s ] [int] NULL,
	[COUNT_Distractor 0.5s Delay Correct Rejection] [int] NULL,
	[COUNT_Distractor 0.5s Delay False Alarm Latency] [int] NULL,
	[COUNT_Distractor 0.5s Delay False Alarm Rate] [int] NULL,
	[COUNT_Distractor 0.5s Delay Hit] [int] NULL,
	[COUNT_Distractor 0.5s Delay Hit Latency] [int] NULL,
	[COUNT_Distractor 0.5s Delay Hit Rate] [int] NULL,
	[COUNT_Distractor 0.5s Delay Miss] [int] NULL,
	[COUNT_Distractor 0.5s Delay Mistake] [int] NULL,
	[COUNT_Distractor 0.5s Delay Response Bias] [int] NULL,
	[COUNT_Distractor 0.5s Delay Reward Latency] [int] NULL,
	[COUNT_Distractor 0.5s Delay Sensitivity (d)] [int] NULL,
	[COUNT_Distractor 1s Delay Correct Rejection] [int] NULL,
	[COUNT_Distractor 1s Delay False Alarm Latency] [int] NULL,
	[COUNT_Distractor 1s Delay False Alarm Rate] [int] NULL,
	[COUNT_Distractor 1s Delay Hit] [int] NULL,
	[COUNT_Distractor 1s Delay Hit Latency] [int] NULL,
	[COUNT_Distractor 1s Delay Hit Rate] [int] NULL,
	[COUNT_Distractor 1s Delay Miss] [int] NULL,
	[COUNT_Distractor 1s Delay Mistake] [int] NULL,
	[COUNT_Distractor 1s Delay Response Bias] [int] NULL,
	[COUNT_Distractor 1s Delay Reward Latency] [int] NULL,
	[COUNT_Distractor 1s Delay Sensitivity (d)] [int] NULL,
	[COUNT_Distrator Presentation Correct Rejection] [int] NULL,
	[COUNT_Distrator Presentation False Alarm Latency] [int] NULL,
	[COUNT_Distrator Presentation False Alarm Rate] [int] NULL,
	[COUNT_Distrator Presentation Hit] [int] NULL,
	[COUNT_Distrator Presentation Hit Latency] [int] NULL,
	[COUNT_Distrator Presentation Hit Rate] [int] NULL,
	[COUNT_Distrator Presentation Miss] [int] NULL,
	[COUNT_Distrator Presentation Mistake] [int] NULL,
	[COUNT_Distrator Presentation Response Bias] [int] NULL,
	[COUNT_Distrator Presentation Reward Latency] [int] NULL,
	[COUNT_Distrator Presentation Sensitivity (d)] [int] NULL,
	[COUNT_End Summary - % Correct] [int] NULL,
	[COUNT_End Summary - After Reward Pause] [int] NULL,
	[COUNT_End Summary - All CS- touches] [int] NULL,
	[COUNT_End Summary - All CS+ touches] [int] NULL,
	[COUNT_End Summary - Blank Touches] [int] NULL,
	[COUNT_END SUMMARY - Breakpoint] [int] NULL,
	[COUNT_End Summary - Centre ITI Touches] [int] NULL,
	[COUNT_End Summary - Condition] [int] NULL,
	[COUNT_End Summary - Correct Image] [int] NULL,
	[COUNT_End Summary - Correct Rejections] [int] NULL,
	[COUNT_End Summary - Correction Trial Correct Rejections] [int] NULL,
	[COUNT_End Summary - Correction Trial Mistakes] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 0.2s SD] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 0.5s SD] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 1s SD] [int] NULL,
	[COUNT_End Summary - CorrectRejection at 2s SD] [int] NULL,
	[COUNT_End Summary - Corrects] [int] NULL,
	[COUNT_End Summary - CS - Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS - Image Approach CS+ Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS - Image Presentation Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS + Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS + Image Approach CS- Beam Breaking] [int] NULL,
	[COUNT_End Summary - CS + Image Presentation Beam Breaking] [int] NULL,
	[COUNT_End Summary - Hits] [int] NULL,
	[COUNT_End Summary - Hits at 0.2s SD] [int] NULL,
	[COUNT_End Summary - Hits at 0.5s SD] [int] NULL,
	[COUNT_End Summary - Hits at 1s SD] [int] NULL,
	[COUNT_End Summary - Hits at 2s SD] [int] NULL,
	[COUNT_End Summary - Incorrect Touches] [int] NULL,
	[COUNT_End Summary - Left ITI Touches] [int] NULL,
	[COUNT_End Summary - Miss at 0.2s SD] [int] NULL,
	[COUNT_End Summary - Miss at 0.5s SD] [int] NULL,
	[COUNT_End Summary - Miss at 1s SD] [int] NULL,
	[COUNT_End Summary - Miss at 2s SD] [int] NULL,
	[COUNT_End Summary - Misses] [int] NULL,
	[COUNT_End Summary - Mistake at 0.2s SD] [int] NULL,
	[COUNT_End Summary - Mistake at 0.5s SD] [int] NULL,
	[COUNT_End Summary - Mistake at 1s SD] [int] NULL,
	[COUNT_End Summary - Mistake at 2s SD] [int] NULL,
	[COUNT_End Summary - Mistakes] [int] NULL,
	[COUNT_End Summary - No Correction Trials] [int] NULL,
	[COUNT_End Summary - No of non correction trials] [int] NULL,
	[COUNT_End Summary - No. images] [int] NULL,
	[COUNT_END SUMMARY - Number of target touches] [int] NULL,
	[COUNT_End Summary - Omissions - Omission] [int] NULL,
	[COUNT_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [int] NULL,
	[COUNT_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [int] NULL,
	[COUNT_END SUMMARY - Revised total response time] [int] NULL,
	[COUNT_END SUMMARY - REWARD COLLECTION LATENCY] [int] NULL,
	[COUNT_End Summary - Right ITI Touches] [int] NULL,
	[COUNT_END SUMMARY - Schedule Length] [int] NULL,
	[COUNT_End Summary - Stimulus Duration] [int] NULL,
	[COUNT_End Summary - Total Trials] [int] NULL,
	[COUNT_End Summary - Touches to lit CS-] [int] NULL,
	[COUNT_End Summary - Touches to lit CS+] [int] NULL,
	[COUNT_End Summary - Tray Beam Breaking] [int] NULL,
	[COUNT_End Summary - Tray CS - Beam Breaking] [int] NULL,
	[COUNT_End Summary - Tray CS + Beam Breaking] [int] NULL,
	[COUNT_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [int] NULL,
	[COUNT_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [int] NULL,
	[COUNT_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [int] NULL,
	[COUNT_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [int] NULL,
	[COUNT_End Summary - Trials Completed] [int] NULL,
	[COUNT_False Alarm Rate at 0.2s] [int] NULL,
	[COUNT_False Alarm Rate at 0.5s] [int] NULL,
	[COUNT_False Alarm Rate at 1s] [int] NULL,
	[COUNT_False Alarm Rate at 2s] [int] NULL,
	[COUNT_Hit Rate at 0.2s] [int] NULL,
	[COUNT_Hit Rate at 0.5s] [int] NULL,
	[COUNT_Hit Rate at 1s] [int] NULL,
	[COUNT_Hit Rate at 2s] [int] NULL,
	[COUNT_Incorrect Choice Latency] [int] NULL,
	[COUNT_Incorrect Touch Latency] [int] NULL,
	[COUNT_No Distractor Correct Rejection] [int] NULL,
	[COUNT_No Distractor False Alarm Latency] [int] NULL,
	[COUNT_No Distractor False Alarm Rate] [int] NULL,
	[COUNT_No Distractor Hit] [int] NULL,
	[COUNT_No Distractor Hit Latency] [int] NULL,
	[COUNT_No Distractor Hit Rate] [int] NULL,
	[COUNT_No Distractor Miss] [int] NULL,
	[COUNT_No Distractor Mistake] [int] NULL,
	[COUNT_No Distractor Response Bias] [int] NULL,
	[COUNT_No Distractor Reward Latency] [int] NULL,
	[COUNT_No Distractor Sensitivity (d)] [int] NULL,
	[COUNT_Normalized - End Summary - All CS- touches] [int] NULL,
	[COUNT_Normalized - End Summary - All CS+ touches] [int] NULL,
	[COUNT_Normalized - End Summary - CS - Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS - Image Presentation Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS + Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - CS + Image Presentation Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Touches to lit CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Touches to lit CS+] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Tray CS - Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Tray CS + Beam Breaking] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [int] NULL,
	[COUNT_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [int] NULL,
	[COUNT_Omission to Tray Latency] [int] NULL,
	[COUNT_Omissions - Total] [int] NULL,
	[COUNT_Perseverative Correct - Total] [int] NULL,
	[COUNT_Perseverative Incorrect - Total] [int] NULL,
	[COUNT_Premature Responses - Total] [int] NULL,
	[COUNT_Response Bias at 0.2s] [int] NULL,
	[COUNT_Response Bias at 0.5s] [int] NULL,
	[COUNT_Response Bias at 1s] [int] NULL,
	[COUNT_Response Bias at 2s] [int] NULL,
	[COUNT_Reward Collection Latency] [int] NULL,
	[COUNT_Reward IR Breaks - Reward Beam Cnt] [int] NULL,
	[COUNT_Reward Retrieval Latency] [int] NULL,
	[COUNT_Screen IR Breaks - Screen IR Cnt] [int] NULL,
	[COUNT_Threshold - Accuracy %] [int] NULL,
	[COUNT_Threshold - Condition] [int] NULL,
	[COUNT_Threshold - Omission %] [int] NULL,
	[COUNT_Threshold - Trials] [int] NULL,
	[COUNT_Tray entered - Cnt] [int] NULL,
	[COUNT_Trial Analysis - Accuracy%] [int] NULL,
	[COUNT_Trial Analysis - Condition] [int] NULL,
	[COUNT_Trial Analysis - Correct] [int] NULL,
	[COUNT_Trial Analysis - Correct Resp Latency with No Distract] [int] NULL,
	[COUNT_Trial Analysis - Correct Response Latency] [int] NULL,
	[COUNT_Trial Analysis - CS- Approach Latency] [int] NULL,
	[COUNT_Trial Analysis - CS- Touch Latency] [int] NULL,
	[COUNT_Trial Analysis - CS+ Approach Latency] [int] NULL,
	[COUNT_Trial Analysis - CS+ Touch Latency] [int] NULL,
	[COUNT_Trial Analysis - Incorrect] [int] NULL,
	[COUNT_Trial Analysis - Incorrect Response Latency] [int] NULL,
	[COUNT_Trial Analysis - Omission] [int] NULL,
	[COUNT_Trial Analysis - Omission%] [int] NULL,
	[COUNT_Trial Analysis - Omissions - Omission] [int] NULL,
	[COUNT_Trial Analysis - Premature] [int] NULL,
	[COUNT_Trial Analysis - Reward Collection Latency] [int] NULL,
	[COUNT_Trial Analysis - Reward Given - Condition] [int] NULL,
	[COUNT_Trial Analysis - Stimulus Duration] [int] NULL,
	[COUNT_Trial Analysis - Time] [int] NULL,
	[COUNT_Trial Analysis - Time To Distraction] [int] NULL,
	[COUNT_trial by trial anal - Correct Rejections] [int] NULL,
	[COUNT_trial by trial anal - Correction Trial Correct Rejections] [int] NULL,
	[COUNT_trial by trial anal - Correction Trial Mistakes] [int] NULL,
	[COUNT_trial by trial anal - Current image] [int] NULL,
	[COUNT_trial by trial anal - Distractor Time] [int] NULL,
	[COUNT_trial by trial anal - Hits] [int] NULL,
	[COUNT_trial by trial anal - ITI] [int] NULL,
	[COUNT_trial by trial anal - Misses] [int] NULL,
	[COUNT_trial by trial anal - Mistakes] [int] NULL,
	[COUNT_trial by trial anal - No of non correction trials] [int] NULL,
	[COUNT_trial by trial anal - Stimulus Duration] [int] NULL,
	[COUNT_Trial by trial analysis - Image response choice latency] [int] NULL,
	[COUNT_Trial by trial analysis - Left chosen] [int] NULL,
	[COUNT_Trial by trial analysis - One trial] [int] NULL,
	[COUNT_Trial by trial analysis - Optimal side chosen] [int] NULL,
	[COUNT_Trial by trial analysis - Optimal side chosen with milkshake given] [int] NULL,
	[COUNT_Trial by trial analysis - Optimal side chosen without milkshake given] [int] NULL,
	[COUNT_Trial by trial analysis - REWARD COLLECTION LATENCY] [int] NULL,
	[COUNT_Trial by trial analysis - Right chosen] [int] NULL,
	[COUNT_Trial by trial analysis - Spurious feedback given] [int] NULL,
	[COUNT_Trial by trial analysis - Spurious no milkshake] [int] NULL,
	[COUNT_Trial by trial analysis - Spurious with milkshake] [int] NULL,
	[COUNT_Trial by trial analysis - Trial type] [int] NULL,
	[COUNT_True Blank Touches at Grid 1] [int] NULL,
	[COUNT_True Blank Touches at Grid 2] [int] NULL,
	[COUNT_True Blank Touches at Grid 3] [int] NULL,
	[COUNT_True Blank Touches at Grid 4] [int] NULL,
	[COUNT_True Blank Touches at Grid 5] [int] NULL,
	[COUNT_Whole session analysis - Condition] [int] NULL,
	[COUNT_Whole session analysis - Image response choice latency] [int] NULL,
	[COUNT_Whole session analysis - Number of Reversals] [int] NULL,
	[COUNT_Whole session analysis - Optimal side chosen] [int] NULL,
	[COUNT_Whole session analysis - REWARD COLLECTION LATENCY] [int] NULL,
	[COUNT_Whole session analysis - Trials completed] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rbt_data_cached_std]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rbt_data_cached_std](
	[SessionID] [int] NOT NULL,
	[STDEV_Average - Correct Choice Latency at 0.2s SD] [float] NULL,
	[STDEV_Average - Correct Choice Latency at 0.5s SD] [float] NULL,
	[STDEV_Average - Correct Choice Latency at 1s SD] [float] NULL,
	[STDEV_Average - Correct Choice Latency at 2s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 0.2s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 0.5s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 1s SD] [float] NULL,
	[STDEV_Average - Incorrect Choice Latency at 2s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 0.2s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 0.5s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 1s SD] [float] NULL,
	[STDEV_Average - Reward Retrieval Latency at 2s SD] [float] NULL,
	[STDEV_Blank Touch Latency] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 1] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 2] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 3] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 4] [float] NULL,
	[STDEV_Blank touches at grid specific - Grid 5] [float] NULL,
	[STDEV_Correct Centre touch latency] [float] NULL,
	[STDEV_Correct Choice Latency] [float] NULL,
	[STDEV_Correct latency grid 1] [float] NULL,
	[STDEV_Correct latency grid 2] [float] NULL,
	[STDEV_Correct latency grid 3] [float] NULL,
	[STDEV_Correct latency grid 4] [float] NULL,
	[STDEV_Correct latency grid 5] [float] NULL,
	[STDEV_Correct Reward Collection] [float] NULL,
	[STDEV_Correct touch latency] [float] NULL,
	[STDEV_count at 0.2s] [float] NULL,
	[STDEV_count at 0.5s] [float] NULL,
	[STDEV_count at 1s] [float] NULL,
	[STDEV_count at 2s] [float] NULL,
	[STDEV_Discrimination Sensitivity at 0.2s ] [float] NULL,
	[STDEV_Discrimination Sensitivity at 0.5s ] [float] NULL,
	[STDEV_Discrimination Sensitivity at 1s ] [float] NULL,
	[STDEV_Discrimination Sensitivity at 2s ] [float] NULL,
	[STDEV_Distractor 0.5s Delay Correct Rejection] [float] NULL,
	[STDEV_Distractor 0.5s Delay False Alarm Latency] [float] NULL,
	[STDEV_Distractor 0.5s Delay False Alarm Rate] [float] NULL,
	[STDEV_Distractor 0.5s Delay Hit] [float] NULL,
	[STDEV_Distractor 0.5s Delay Hit Latency] [float] NULL,
	[STDEV_Distractor 0.5s Delay Hit Rate] [float] NULL,
	[STDEV_Distractor 0.5s Delay Miss] [float] NULL,
	[STDEV_Distractor 0.5s Delay Mistake] [float] NULL,
	[STDEV_Distractor 0.5s Delay Response Bias] [float] NULL,
	[STDEV_Distractor 0.5s Delay Reward Latency] [float] NULL,
	[STDEV_Distractor 0.5s Delay Sensitivity (d)] [float] NULL,
	[STDEV_Distractor 1s Delay Correct Rejection] [float] NULL,
	[STDEV_Distractor 1s Delay False Alarm Latency] [float] NULL,
	[STDEV_Distractor 1s Delay False Alarm Rate] [float] NULL,
	[STDEV_Distractor 1s Delay Hit] [float] NULL,
	[STDEV_Distractor 1s Delay Hit Latency] [float] NULL,
	[STDEV_Distractor 1s Delay Hit Rate] [float] NULL,
	[STDEV_Distractor 1s Delay Miss] [float] NULL,
	[STDEV_Distractor 1s Delay Mistake] [float] NULL,
	[STDEV_Distractor 1s Delay Response Bias] [float] NULL,
	[STDEV_Distractor 1s Delay Reward Latency] [float] NULL,
	[STDEV_Distractor 1s Delay Sensitivity (d)] [float] NULL,
	[STDEV_Distrator Presentation Correct Rejection] [float] NULL,
	[STDEV_Distrator Presentation False Alarm Latency] [float] NULL,
	[STDEV_Distrator Presentation False Alarm Rate] [float] NULL,
	[STDEV_Distrator Presentation Hit] [float] NULL,
	[STDEV_Distrator Presentation Hit Latency] [float] NULL,
	[STDEV_Distrator Presentation Hit Rate] [float] NULL,
	[STDEV_Distrator Presentation Miss] [float] NULL,
	[STDEV_Distrator Presentation Mistake] [float] NULL,
	[STDEV_Distrator Presentation Response Bias] [float] NULL,
	[STDEV_Distrator Presentation Reward Latency] [float] NULL,
	[STDEV_Distrator Presentation Sensitivity (d)] [float] NULL,
	[STDEV_End Summary - % Correct] [float] NULL,
	[STDEV_End Summary - After Reward Pause] [float] NULL,
	[STDEV_End Summary - All CS- touches] [float] NULL,
	[STDEV_End Summary - All CS+ touches] [float] NULL,
	[STDEV_End Summary - Blank Touches] [float] NULL,
	[STDEV_END SUMMARY - Breakpoint] [float] NULL,
	[STDEV_End Summary - Centre ITI Touches] [float] NULL,
	[STDEV_End Summary - Condition] [float] NULL,
	[STDEV_End Summary - Correct Image] [float] NULL,
	[STDEV_End Summary - Correct Rejections] [float] NULL,
	[STDEV_End Summary - Correction Trial Correct Rejections] [float] NULL,
	[STDEV_End Summary - Correction Trial Mistakes] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 0.2s SD] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 0.5s SD] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 1s SD] [float] NULL,
	[STDEV_End Summary - CorrectRejection at 2s SD] [float] NULL,
	[STDEV_End Summary - Corrects] [float] NULL,
	[STDEV_End Summary - CS - Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS + Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[STDEV_End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[STDEV_End Summary - Hits] [float] NULL,
	[STDEV_End Summary - Hits at 0.2s SD] [float] NULL,
	[STDEV_End Summary - Hits at 0.5s SD] [float] NULL,
	[STDEV_End Summary - Hits at 1s SD] [float] NULL,
	[STDEV_End Summary - Hits at 2s SD] [float] NULL,
	[STDEV_End Summary - Incorrect Touches] [float] NULL,
	[STDEV_End Summary - Left ITI Touches] [float] NULL,
	[STDEV_End Summary - Miss at 0.2s SD] [float] NULL,
	[STDEV_End Summary - Miss at 0.5s SD] [float] NULL,
	[STDEV_End Summary - Miss at 1s SD] [float] NULL,
	[STDEV_End Summary - Miss at 2s SD] [float] NULL,
	[STDEV_End Summary - Misses] [float] NULL,
	[STDEV_End Summary - Mistake at 0.2s SD] [float] NULL,
	[STDEV_End Summary - Mistake at 0.5s SD] [float] NULL,
	[STDEV_End Summary - Mistake at 1s SD] [float] NULL,
	[STDEV_End Summary - Mistake at 2s SD] [float] NULL,
	[STDEV_End Summary - Mistakes] [float] NULL,
	[STDEV_End Summary - No Correction Trials] [float] NULL,
	[STDEV_End Summary - No of non correction trials] [float] NULL,
	[STDEV_End Summary - No. images] [float] NULL,
	[STDEV_END SUMMARY - Number of target touches] [float] NULL,
	[STDEV_End Summary - Omissions - Omission] [float] NULL,
	[STDEV_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [float] NULL,
	[STDEV_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [float] NULL,
	[STDEV_END SUMMARY - Revised total response time] [float] NULL,
	[STDEV_END SUMMARY - REWARD COLLECTION LATENCY] [float] NULL,
	[STDEV_End Summary - Right ITI Touches] [float] NULL,
	[STDEV_END SUMMARY - Schedule Length] [float] NULL,
	[STDEV_End Summary - Stimulus Duration] [float] NULL,
	[STDEV_End Summary - Total Trials] [float] NULL,
	[STDEV_End Summary - Touches to lit CS-] [float] NULL,
	[STDEV_End Summary - Touches to lit CS+] [float] NULL,
	[STDEV_End Summary - Tray Beam Breaking] [float] NULL,
	[STDEV_End Summary - Tray CS - Beam Breaking] [float] NULL,
	[STDEV_End Summary - Tray CS + Beam Breaking] [float] NULL,
	[STDEV_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[STDEV_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[STDEV_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[STDEV_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[STDEV_End Summary - Trials Completed] [float] NULL,
	[STDEV_False Alarm Rate at 0.2s] [float] NULL,
	[STDEV_False Alarm Rate at 0.5s] [float] NULL,
	[STDEV_False Alarm Rate at 1s] [float] NULL,
	[STDEV_False Alarm Rate at 2s] [float] NULL,
	[STDEV_Hit Rate at 0.2s] [float] NULL,
	[STDEV_Hit Rate at 0.5s] [float] NULL,
	[STDEV_Hit Rate at 1s] [float] NULL,
	[STDEV_Hit Rate at 2s] [float] NULL,
	[STDEV_Incorrect Choice Latency] [float] NULL,
	[STDEV_Incorrect Touch Latency] [float] NULL,
	[STDEV_No Distractor Correct Rejection] [float] NULL,
	[STDEV_No Distractor False Alarm Latency] [float] NULL,
	[STDEV_No Distractor False Alarm Rate] [float] NULL,
	[STDEV_No Distractor Hit] [float] NULL,
	[STDEV_No Distractor Hit Latency] [float] NULL,
	[STDEV_No Distractor Hit Rate] [float] NULL,
	[STDEV_No Distractor Miss] [float] NULL,
	[STDEV_No Distractor Mistake] [float] NULL,
	[STDEV_No Distractor Response Bias] [float] NULL,
	[STDEV_No Distractor Reward Latency] [float] NULL,
	[STDEV_No Distractor Sensitivity (d)] [float] NULL,
	[STDEV_Normalized - End Summary - All CS- touches] [float] NULL,
	[STDEV_Normalized - End Summary - All CS+ touches] [float] NULL,
	[STDEV_Normalized - End Summary - CS - Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS + Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Touches to lit CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Touches to lit CS+] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Tray CS - Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Tray CS + Beam Breaking] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[STDEV_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[STDEV_Omission to Tray Latency] [float] NULL,
	[STDEV_Omissions - Total] [float] NULL,
	[STDEV_Perseverative Correct - Total] [float] NULL,
	[STDEV_Perseverative Incorrect - Total] [float] NULL,
	[STDEV_Premature Responses - Total] [float] NULL,
	[STDEV_Response Bias at 0.2s] [float] NULL,
	[STDEV_Response Bias at 0.5s] [float] NULL,
	[STDEV_Response Bias at 1s] [float] NULL,
	[STDEV_Response Bias at 2s] [float] NULL,
	[STDEV_Reward Collection Latency] [float] NULL,
	[STDEV_Reward IR Breaks - Reward Beam Cnt] [float] NULL,
	[STDEV_Reward Retrieval Latency] [float] NULL,
	[STDEV_Screen IR Breaks - Screen IR Cnt] [float] NULL,
	[STDEV_Threshold - Accuracy %] [float] NULL,
	[STDEV_Threshold - Condition] [float] NULL,
	[STDEV_Threshold - Omission %] [float] NULL,
	[STDEV_Threshold - Trials] [float] NULL,
	[STDEV_Tray entered - Cnt] [float] NULL,
	[STDEV_Trial Analysis - Accuracy%] [float] NULL,
	[STDEV_Trial Analysis - Condition] [float] NULL,
	[STDEV_Trial Analysis - Correct] [float] NULL,
	[STDEV_Trial Analysis - Correct Resp Latency with No Distract] [float] NULL,
	[STDEV_Trial Analysis - Correct Response Latency] [float] NULL,
	[STDEV_Trial Analysis - CS- Approach Latency] [float] NULL,
	[STDEV_Trial Analysis - CS- Touch Latency] [float] NULL,
	[STDEV_Trial Analysis - CS+ Approach Latency] [float] NULL,
	[STDEV_Trial Analysis - CS+ Touch Latency] [float] NULL,
	[STDEV_Trial Analysis - Incorrect] [float] NULL,
	[STDEV_Trial Analysis - Incorrect Response Latency] [float] NULL,
	[STDEV_Trial Analysis - Omission] [float] NULL,
	[STDEV_Trial Analysis - Omission%] [float] NULL,
	[STDEV_Trial Analysis - Omissions - Omission] [float] NULL,
	[STDEV_Trial Analysis - Premature] [float] NULL,
	[STDEV_Trial Analysis - Reward Collection Latency] [float] NULL,
	[STDEV_Trial Analysis - Reward Given - Condition] [float] NULL,
	[STDEV_Trial Analysis - Stimulus Duration] [float] NULL,
	[STDEV_Trial Analysis - Time] [float] NULL,
	[STDEV_Trial Analysis - Time To Distraction] [float] NULL,
	[STDEV_trial by trial anal - Correct Rejections] [float] NULL,
	[STDEV_trial by trial anal - Correction Trial Correct Rejections] [float] NULL,
	[STDEV_trial by trial anal - Correction Trial Mistakes] [float] NULL,
	[STDEV_trial by trial anal - Current image] [float] NULL,
	[STDEV_trial by trial anal - Distractor Time] [float] NULL,
	[STDEV_trial by trial anal - Hits] [float] NULL,
	[STDEV_trial by trial anal - ITI] [float] NULL,
	[STDEV_trial by trial anal - Misses] [float] NULL,
	[STDEV_trial by trial anal - Mistakes] [float] NULL,
	[STDEV_trial by trial anal - No of non correction trials] [float] NULL,
	[STDEV_trial by trial anal - Stimulus Duration] [float] NULL,
	[STDEV_Trial by trial analysis - Image response choice latency] [float] NULL,
	[STDEV_Trial by trial analysis - Left chosen] [float] NULL,
	[STDEV_Trial by trial analysis - One trial] [float] NULL,
	[STDEV_Trial by trial analysis - Optimal side chosen] [float] NULL,
	[STDEV_Trial by trial analysis - Optimal side chosen with milkshake given] [float] NULL,
	[STDEV_Trial by trial analysis - Optimal side chosen without milkshake given] [float] NULL,
	[STDEV_Trial by trial analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[STDEV_Trial by trial analysis - Right chosen] [float] NULL,
	[STDEV_Trial by trial analysis - Spurious feedback given] [float] NULL,
	[STDEV_Trial by trial analysis - Spurious no milkshake] [float] NULL,
	[STDEV_Trial by trial analysis - Spurious with milkshake] [float] NULL,
	[STDEV_Trial by trial analysis - Trial type] [float] NULL,
	[STDEV_True Blank Touches at Grid 1] [float] NULL,
	[STDEV_True Blank Touches at Grid 2] [float] NULL,
	[STDEV_True Blank Touches at Grid 3] [float] NULL,
	[STDEV_True Blank Touches at Grid 4] [float] NULL,
	[STDEV_True Blank Touches at Grid 5] [float] NULL,
	[STDEV_Whole session analysis - Condition] [float] NULL,
	[STDEV_Whole session analysis - Image response choice latency] [float] NULL,
	[STDEV_Whole session analysis - Number of Reversals] [float] NULL,
	[STDEV_Whole session analysis - Optimal side chosen] [float] NULL,
	[STDEV_Whole session analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[STDEV_Whole session analysis - Trials completed] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rbt_data_cached_sum]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rbt_data_cached_sum](
	[SessionID] [int] NOT NULL,
	[SUM_Average - Correct Choice Latency at 0.2s SD] [float] NULL,
	[SUM_Average - Correct Choice Latency at 0.5s SD] [float] NULL,
	[SUM_Average - Correct Choice Latency at 1s SD] [float] NULL,
	[SUM_Average - Correct Choice Latency at 2s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 0.2s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 0.5s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 1s SD] [float] NULL,
	[SUM_Average - Incorrect Choice Latency at 2s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 0.2s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 0.5s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 1s SD] [float] NULL,
	[SUM_Average - Reward Retrieval Latency at 2s SD] [float] NULL,
	[SUM_Blank Touch Latency] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 1] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 2] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 3] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 4] [float] NULL,
	[SUM_Blank touches at grid specific - Grid 5] [float] NULL,
	[SUM_Correct Centre touch latency] [float] NULL,
	[SUM_Correct Choice Latency] [float] NULL,
	[SUM_Correct latency grid 1] [float] NULL,
	[SUM_Correct latency grid 2] [float] NULL,
	[SUM_Correct latency grid 3] [float] NULL,
	[SUM_Correct latency grid 4] [float] NULL,
	[SUM_Correct latency grid 5] [float] NULL,
	[SUM_Correct Reward Collection] [float] NULL,
	[SUM_Correct touch latency] [float] NULL,
	[SUM_count at 0.2s] [float] NULL,
	[SUM_count at 0.5s] [float] NULL,
	[SUM_count at 1s] [float] NULL,
	[SUM_count at 2s] [float] NULL,
	[SUM_Discrimination Sensitivity at 0.2s ] [float] NULL,
	[SUM_Discrimination Sensitivity at 0.5s ] [float] NULL,
	[SUM_Discrimination Sensitivity at 1s ] [float] NULL,
	[SUM_Discrimination Sensitivity at 2s ] [float] NULL,
	[SUM_Distractor 0.5s Delay Correct Rejection] [float] NULL,
	[SUM_Distractor 0.5s Delay False Alarm Latency] [float] NULL,
	[SUM_Distractor 0.5s Delay False Alarm Rate] [float] NULL,
	[SUM_Distractor 0.5s Delay Hit] [float] NULL,
	[SUM_Distractor 0.5s Delay Hit Latency] [float] NULL,
	[SUM_Distractor 0.5s Delay Hit Rate] [float] NULL,
	[SUM_Distractor 0.5s Delay Miss] [float] NULL,
	[SUM_Distractor 0.5s Delay Mistake] [float] NULL,
	[SUM_Distractor 0.5s Delay Response Bias] [float] NULL,
	[SUM_Distractor 0.5s Delay Reward Latency] [float] NULL,
	[SUM_Distractor 0.5s Delay Sensitivity (d)] [float] NULL,
	[SUM_Distractor 1s Delay Correct Rejection] [float] NULL,
	[SUM_Distractor 1s Delay False Alarm Latency] [float] NULL,
	[SUM_Distractor 1s Delay False Alarm Rate] [float] NULL,
	[SUM_Distractor 1s Delay Hit] [float] NULL,
	[SUM_Distractor 1s Delay Hit Latency] [float] NULL,
	[SUM_Distractor 1s Delay Hit Rate] [float] NULL,
	[SUM_Distractor 1s Delay Miss] [float] NULL,
	[SUM_Distractor 1s Delay Mistake] [float] NULL,
	[SUM_Distractor 1s Delay Response Bias] [float] NULL,
	[SUM_Distractor 1s Delay Reward Latency] [float] NULL,
	[SUM_Distractor 1s Delay Sensitivity (d)] [float] NULL,
	[SUM_Distrator Presentation Correct Rejection] [float] NULL,
	[SUM_Distrator Presentation False Alarm Latency] [float] NULL,
	[SUM_Distrator Presentation False Alarm Rate] [float] NULL,
	[SUM_Distrator Presentation Hit] [float] NULL,
	[SUM_Distrator Presentation Hit Latency] [float] NULL,
	[SUM_Distrator Presentation Hit Rate] [float] NULL,
	[SUM_Distrator Presentation Miss] [float] NULL,
	[SUM_Distrator Presentation Mistake] [float] NULL,
	[SUM_Distrator Presentation Response Bias] [float] NULL,
	[SUM_Distrator Presentation Reward Latency] [float] NULL,
	[SUM_Distrator Presentation Sensitivity (d)] [float] NULL,
	[SUM_End Summary - % Correct] [float] NULL,
	[SUM_End Summary - After Reward Pause] [float] NULL,
	[SUM_End Summary - All CS- touches] [float] NULL,
	[SUM_End Summary - All CS+ touches] [float] NULL,
	[SUM_End Summary - Blank Touches] [float] NULL,
	[SUM_END SUMMARY - Breakpoint] [float] NULL,
	[SUM_End Summary - Centre ITI Touches] [float] NULL,
	[SUM_End Summary - Condition] [float] NULL,
	[SUM_End Summary - Correct Image] [float] NULL,
	[SUM_End Summary - Correct Rejections] [float] NULL,
	[SUM_End Summary - Correction Trial Correct Rejections] [float] NULL,
	[SUM_End Summary - Correction Trial Mistakes] [float] NULL,
	[SUM_End Summary - CorrectRejection at 0.2s SD] [float] NULL,
	[SUM_End Summary - CorrectRejection at 0.5s SD] [float] NULL,
	[SUM_End Summary - CorrectRejection at 1s SD] [float] NULL,
	[SUM_End Summary - CorrectRejection at 2s SD] [float] NULL,
	[SUM_End Summary - Corrects] [float] NULL,
	[SUM_End Summary - CS - Beam Breaking] [float] NULL,
	[SUM_End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[SUM_End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[SUM_End Summary - CS + Beam Breaking] [float] NULL,
	[SUM_End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[SUM_End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[SUM_End Summary - Hits] [float] NULL,
	[SUM_End Summary - Hits at 0.2s SD] [float] NULL,
	[SUM_End Summary - Hits at 0.5s SD] [float] NULL,
	[SUM_End Summary - Hits at 1s SD] [float] NULL,
	[SUM_End Summary - Hits at 2s SD] [float] NULL,
	[SUM_End Summary - Incorrect Touches] [float] NULL,
	[SUM_End Summary - Left ITI Touches] [float] NULL,
	[SUM_End Summary - Miss at 0.2s SD] [float] NULL,
	[SUM_End Summary - Miss at 0.5s SD] [float] NULL,
	[SUM_End Summary - Miss at 1s SD] [float] NULL,
	[SUM_End Summary - Miss at 2s SD] [float] NULL,
	[SUM_End Summary - Misses] [float] NULL,
	[SUM_End Summary - Mistake at 0.2s SD] [float] NULL,
	[SUM_End Summary - Mistake at 0.5s SD] [float] NULL,
	[SUM_End Summary - Mistake at 1s SD] [float] NULL,
	[SUM_End Summary - Mistake at 2s SD] [float] NULL,
	[SUM_End Summary - Mistakes] [float] NULL,
	[SUM_End Summary - No Correction Trials] [float] NULL,
	[SUM_End Summary - No of non correction trials] [float] NULL,
	[SUM_End Summary - No. images] [float] NULL,
	[SUM_END SUMMARY - Number of target touches] [float] NULL,
	[SUM_End Summary - Omissions - Omission] [float] NULL,
	[SUM_END SUMMARY - Revised post reinf pause (from first head entry after reward delivery until first screen touch)] [float] NULL,
	[SUM_END SUMMARY - Revised post reinf pause (head out of mag to first screen touch)] [float] NULL,
	[SUM_END SUMMARY - Revised total response time] [float] NULL,
	[SUM_END SUMMARY - REWARD COLLECTION LATENCY] [float] NULL,
	[SUM_End Summary - Right ITI Touches] [float] NULL,
	[SUM_END SUMMARY - Schedule Length] [float] NULL,
	[SUM_End Summary - Stimulus Duration] [float] NULL,
	[SUM_End Summary - Total Trials] [float] NULL,
	[SUM_End Summary - Touches to lit CS-] [float] NULL,
	[SUM_End Summary - Touches to lit CS+] [float] NULL,
	[SUM_End Summary - Tray Beam Breaking] [float] NULL,
	[SUM_End Summary - Tray CS - Beam Breaking] [float] NULL,
	[SUM_End Summary - Tray CS + Beam Breaking] [float] NULL,
	[SUM_End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[SUM_End Summary - Tray Entries during first 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[SUM_End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[SUM_End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[SUM_End Summary - Trials Completed] [float] NULL,
	[SUM_False Alarm Rate at 0.2s] [float] NULL,
	[SUM_False Alarm Rate at 0.5s] [float] NULL,
	[SUM_False Alarm Rate at 1s] [float] NULL,
	[SUM_False Alarm Rate at 2s] [float] NULL,
	[SUM_Hit Rate at 0.2s] [float] NULL,
	[SUM_Hit Rate at 0.5s] [float] NULL,
	[SUM_Hit Rate at 1s] [float] NULL,
	[SUM_Hit Rate at 2s] [float] NULL,
	[SUM_Incorrect Choice Latency] [float] NULL,
	[SUM_Incorrect Touch Latency] [float] NULL,
	[SUM_No Distractor Correct Rejection] [float] NULL,
	[SUM_No Distractor False Alarm Latency] [float] NULL,
	[SUM_No Distractor False Alarm Rate] [float] NULL,
	[SUM_No Distractor Hit] [float] NULL,
	[SUM_No Distractor Hit Latency] [float] NULL,
	[SUM_No Distractor Hit Rate] [float] NULL,
	[SUM_No Distractor Miss] [float] NULL,
	[SUM_No Distractor Mistake] [float] NULL,
	[SUM_No Distractor Response Bias] [float] NULL,
	[SUM_No Distractor Reward Latency] [float] NULL,
	[SUM_No Distractor Sensitivity (d)] [float] NULL,
	[SUM_Normalized - End Summary - All CS- touches] [float] NULL,
	[SUM_Normalized - End Summary - All CS+ touches] [float] NULL,
	[SUM_Normalized - End Summary - CS - Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS - Image Approach CS+ Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS - Image Presentation Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS + Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS + Image Approach CS- Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - CS + Image Presentation Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Touches to lit CS-] [float] NULL,
	[SUM_Normalized - End Summary - Touches to lit CS+] [float] NULL,
	[SUM_Normalized - End Summary - Tray Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Tray CS - Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Tray CS + Beam Breaking] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during first 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count 1st 5s CS-] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS- lit - Tray Count last 5s CS-] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count 1st 5s CS+] [float] NULL,
	[SUM_Normalized - End Summary - Tray Entries during last 5s CS+ lit - Tray Count last 5s CS+] [float] NULL,
	[SUM_Omission to Tray Latency] [float] NULL,
	[SUM_Omissions - Total] [float] NULL,
	[SUM_Perseverative Correct - Total] [float] NULL,
	[SUM_Perseverative Incorrect - Total] [float] NULL,
	[SUM_Premature Responses - Total] [float] NULL,
	[SUM_Response Bias at 0.2s] [float] NULL,
	[SUM_Response Bias at 0.5s] [float] NULL,
	[SUM_Response Bias at 1s] [float] NULL,
	[SUM_Response Bias at 2s] [float] NULL,
	[SUM_Reward Collection Latency] [float] NULL,
	[SUM_Reward IR Breaks - Reward Beam Cnt] [float] NULL,
	[SUM_Reward Retrieval Latency] [float] NULL,
	[SUM_Screen IR Breaks - Screen IR Cnt] [float] NULL,
	[SUM_Threshold - Accuracy %] [float] NULL,
	[SUM_Threshold - Condition] [float] NULL,
	[SUM_Threshold - Omission %] [float] NULL,
	[SUM_Threshold - Trials] [float] NULL,
	[SUM_Tray entered - Cnt] [float] NULL,
	[SUM_Trial Analysis - Accuracy%] [float] NULL,
	[SUM_Trial Analysis - Condition] [float] NULL,
	[SUM_Trial Analysis - Correct] [float] NULL,
	[SUM_Trial Analysis - Correct Resp Latency with No Distract] [float] NULL,
	[SUM_Trial Analysis - Correct Response Latency] [float] NULL,
	[SUM_Trial Analysis - CS- Approach Latency] [float] NULL,
	[SUM_Trial Analysis - CS- Touch Latency] [float] NULL,
	[SUM_Trial Analysis - CS+ Approach Latency] [float] NULL,
	[SUM_Trial Analysis - CS+ Touch Latency] [float] NULL,
	[SUM_Trial Analysis - Incorrect] [float] NULL,
	[SUM_Trial Analysis - Incorrect Response Latency] [float] NULL,
	[SUM_Trial Analysis - Omission] [float] NULL,
	[SUM_Trial Analysis - Omission%] [float] NULL,
	[SUM_Trial Analysis - Omissions - Omission] [float] NULL,
	[SUM_Trial Analysis - Premature] [float] NULL,
	[SUM_Trial Analysis - Reward Collection Latency] [float] NULL,
	[SUM_Trial Analysis - Reward Given - Condition] [float] NULL,
	[SUM_Trial Analysis - Stimulus Duration] [float] NULL,
	[SUM_Trial Analysis - Time] [float] NULL,
	[SUM_Trial Analysis - Time To Distraction] [float] NULL,
	[SUM_trial by trial anal - Correct Rejections] [float] NULL,
	[SUM_trial by trial anal - Correction Trial Correct Rejections] [float] NULL,
	[SUM_trial by trial anal - Correction Trial Mistakes] [float] NULL,
	[SUM_trial by trial anal - Current image] [float] NULL,
	[SUM_trial by trial anal - Distractor Time] [float] NULL,
	[SUM_trial by trial anal - Hits] [float] NULL,
	[SUM_trial by trial anal - ITI] [float] NULL,
	[SUM_trial by trial anal - Misses] [float] NULL,
	[SUM_trial by trial anal - Mistakes] [float] NULL,
	[SUM_trial by trial anal - No of non correction trials] [float] NULL,
	[SUM_trial by trial anal - Stimulus Duration] [float] NULL,
	[SUM_Trial by trial analysis - Image response choice latency] [float] NULL,
	[SUM_Trial by trial analysis - Left chosen] [float] NULL,
	[SUM_Trial by trial analysis - One trial] [float] NULL,
	[SUM_Trial by trial analysis - Optimal side chosen] [float] NULL,
	[SUM_Trial by trial analysis - Optimal side chosen with milkshake given] [float] NULL,
	[SUM_Trial by trial analysis - Optimal side chosen without milkshake given] [float] NULL,
	[SUM_Trial by trial analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[SUM_Trial by trial analysis - Right chosen] [float] NULL,
	[SUM_Trial by trial analysis - Spurious feedback given] [float] NULL,
	[SUM_Trial by trial analysis - Spurious no milkshake] [float] NULL,
	[SUM_Trial by trial analysis - Spurious with milkshake] [float] NULL,
	[SUM_Trial by trial analysis - Trial type] [float] NULL,
	[SUM_True Blank Touches at Grid 1] [float] NULL,
	[SUM_True Blank Touches at Grid 2] [float] NULL,
	[SUM_True Blank Touches at Grid 3] [float] NULL,
	[SUM_True Blank Touches at Grid 4] [float] NULL,
	[SUM_True Blank Touches at Grid 5] [float] NULL,
	[SUM_Whole session analysis - Condition] [float] NULL,
	[SUM_Whole session analysis - Image response choice latency] [float] NULL,
	[SUM_Whole session analysis - Number of Reversals] [float] NULL,
	[SUM_Whole session analysis - Optimal side chosen] [float] NULL,
	[SUM_Whole session analysis - REWARD COLLECTION LATENCY] [float] NULL,
	[SUM_Whole session analysis - Trials completed] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Request]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Request](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[FullName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[TaskName] [nvarchar](50) NULL,
	[ScheduleName] [text] NULL,
	[PIFullName] [nvarchar](50) NULL,
	[PIEmail] [nvarchar](50) NULL,
	[PIInstitution] [nvarchar](50) NULL,
	[Age] [nchar](10) NULL,
	[MouseStrain] [nvarchar](100) NULL,
	[GeneticModification] [nvarchar](50) NULL,
	[StrainReference] [nvarchar](100) NULL,
	[ControlSuggestion] [text] NULL,
 CONSTRAINT [PK_Request] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SessionInfo_Dynamic]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SessionInfo_Dynamic](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SessionID] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_SessionInfo_Dynamic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SourceType]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SourceTYpe] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Species]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Species](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Species] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Species] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sub_Task]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sub_Task](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Task_ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[OriginalName] [nvarchar](50) NULL,
	[Show] [bit] NULL,
	[SubTaskDescription] [text] NULL,
 CONSTRAINT [PK_Sub_Task] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Test]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test](
	[TestID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nchar](10) NULL,
	[Last_Name] [nchar](10) NULL,
 CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED 
(
	[TestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Test_5C]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test_5C](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AnimalID] [nvarchar](20) NOT NULL,
	[Strain] [nvarchar](50) NOT NULL,
	[Genotype] [nvarchar](50) NOT NULL,
	[Sex] [nvarchar](1) NOT NULL,
	[Site] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Test_5C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Test_PAL]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test_PAL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AnimalID] [nvarchar](20) NOT NULL,
	[Strain] [nvarchar](50) NOT NULL,
	[Genotype] [nvarchar](50) NOT NULL,
	[Sex] [nvarchar](1) NOT NULL,
	[Site] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Test_PAL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Test_PD]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test_PD](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AnimalID] [nvarchar](20) NOT NULL,
	[Strain] [nvarchar](50) NOT NULL,
	[Genotype] [nvarchar](50) NOT NULL,
	[Sex] [nvarchar](1) NOT NULL,
	[Site] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Test_PD] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Upload_SessionInfo]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Upload_SessionInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskID] [int] NOT NULL,
	[TaskName] [nvarchar](50) NULL,
	[SessionName] [nvarchar](200) NULL,
	[SessionDescription] [text] NULL,
 CONSTRAINT [PK_Upload_SessionInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UploadErrorLog]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UploadErrorLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExpID] [int] NOT NULL,
	[SubExpID] [int] NOT NULL,
	[UserFileName] [nvarchar](250) NULL,
	[ErrorMessage] [nvarchar](450) NULL,
	[UploadDate] [datetime] NULL,
 CONSTRAINT [PK_UploadErrorLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Links]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Links] ON [dbo].[Links]
(
	[LinkGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RBT_TouchScreen_Features]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_RBT_TouchScreen_Features] ON [dbo].[RBT_TouchScreen_Features]
(
	[SessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RBT_TouchScreen_Features_1]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_RBT_TouchScreen_Features_1] ON [dbo].[RBT_TouchScreen_Features]
(
	[FeatureName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RBT_TouchScreen_Features_2]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_RBT_TouchScreen_Features_2] ON [dbo].[RBT_TouchScreen_Features]
(
	[SourceTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SessionInfo]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_SessionInfo] ON [dbo].[SessionInfo]
(
	[SessionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SessionInfo_1]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_SessionInfo_1] ON [dbo].[SessionInfo]
(
	[ExpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SessionInfo_UploadID]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SessionInfo_UploadID] ON [dbo].[SessionInfo]
(
	[UploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Upload]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_Upload] ON [dbo].[Upload]
(
	[SubExpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Upload_1]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_Upload_1] ON [dbo].[Upload]
(
	[AnimalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Upload_2]    Script Date: 2023-10-15 1:08:42 PM ******/
CREATE NONCLUSTERED INDEX [IX_Upload_2] ON [dbo].[Upload]
(
	[ExpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Experiment] ADD  CONSTRAINT [DF_Experiment_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_IsSaved]  DEFAULT ((0)) FOR [IsSaved]
GO
ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Metrics] ADD  CONSTRAINT [DF_Metrics_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[SessionInfo] ADD  CONSTRAINT [DF_SessionInfo_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SessionInfo] ADD  CONSTRAINT [DF_SessionInfo_DateCreated]  DEFAULT (getutcdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[SubExperiment] ADD  CONSTRAINT [DF_SubExperiment_IsPostProcessingPass]  DEFAULT ((0)) FOR [IsPostProcessingPass]
GO
ALTER TABLE [dbo].[SubExperiment] ADD  CONSTRAINT [DF_SubExperiment_IsIntervention]  DEFAULT ((0)) FOR [IsIntervention]
GO
ALTER TABLE [dbo].[SubExperiment] ADD  CONSTRAINT [DF_SubExperiment_IsDrug]  DEFAULT ((0)) FOR [IsDrug]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_IsCompleted]  DEFAULT ((0)) FOR [IsUploaded]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_IsDismissed]  DEFAULT ((0)) FOR [IsDismissed]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_DateUpload]  DEFAULT (NULL) FOR [DateUpload]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_IsQcPrePassed]  DEFAULT ((0)) FOR [IsQcPassed]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_IsIdentifierPassed]  DEFAULT ((0)) FOR [IsIdentifierPassed]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_DateCreated]  DEFAULT (getutcdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Upload] ADD  CONSTRAINT [DF_Upload_IsDuplicateSession]  DEFAULT ((0)) FOR [IsDuplicateSession]
GO
ALTER TABLE [dbo].[Animal]  WITH CHECK ADD  CONSTRAINT [FK_Animal_Experiment] FOREIGN KEY([ExpID])
REFERENCES [dbo].[Experiment] ([ExpID])
GO
ALTER TABLE [dbo].[Animal] CHECK CONSTRAINT [FK_Animal_Experiment]
GO
ALTER TABLE [dbo].[Animal]  WITH CHECK ADD  CONSTRAINT [FK_Animal_Genotype] FOREIGN KEY([GID])
REFERENCES [dbo].[Genotype] ([ID])
GO
ALTER TABLE [dbo].[Animal] CHECK CONSTRAINT [FK_Animal_Genotype]
GO
ALTER TABLE [dbo].[Animal]  WITH CHECK ADD  CONSTRAINT [FK_Animal_Strain] FOREIGN KEY([SID])
REFERENCES [dbo].[Strain] ([ID])
GO
ALTER TABLE [dbo].[Animal] CHECK CONSTRAINT [FK_Animal_Strain]
GO
ALTER TABLE [dbo].[PISite]  WITH CHECK ADD  CONSTRAINT [FK_PISite_PISite_PI] FOREIGN KEY([PID])
REFERENCES [dbo].[PI] ([PID])
GO
ALTER TABLE [dbo].[PISite] CHECK CONSTRAINT [FK_PISite_PISite_PI]
GO
ALTER TABLE [dbo].[PISite]  WITH CHECK ADD  CONSTRAINT [FK_PISite_PISite_Site] FOREIGN KEY([SiteID])
REFERENCES [dbo].[Site] ([SiteID])
GO
ALTER TABLE [dbo].[PISite] CHECK CONSTRAINT [FK_PISite_PISite_Site]
GO
ALTER TABLE [dbo].[PIUserSite]  WITH CHECK ADD  CONSTRAINT [FK_PIUserSite_] FOREIGN KEY([PSID])
REFERENCES [dbo].[PISite] ([PSID])
GO
ALTER TABLE [dbo].[PIUserSite] CHECK CONSTRAINT [FK_PIUserSite_]
GO
ALTER TABLE [dbo].[PIUserSite]  WITH CHECK ADD  CONSTRAINT [FK_PIUserSite_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[PIUserSite] CHECK CONSTRAINT [FK_PIUserSite_User]
GO
ALTER TABLE [dbo].[RBT_TouchScreen_Features]  WITH NOCHECK ADD  CONSTRAINT [FK_RBT_TouchScreen_Features_SessionInfo] FOREIGN KEY([SessionID])
REFERENCES [dbo].[SessionInfo] ([SessionID])
GO
ALTER TABLE [dbo].[RBT_TouchScreen_Features] CHECK CONSTRAINT [FK_RBT_TouchScreen_Features_SessionInfo]
GO
ALTER TABLE [dbo].[RBT_TouchScreen_Features]  WITH NOCHECK ADD  CONSTRAINT [FK_RBT_TouchScreen_Features_SourceType] FOREIGN KEY([SourceTypeID])
REFERENCES [dbo].[SourceType] ([ID])
GO
ALTER TABLE [dbo].[RBT_TouchScreen_Features] CHECK CONSTRAINT [FK_RBT_TouchScreen_Features_SourceType]
GO
ALTER TABLE [dbo].[SessionInfo]  WITH CHECK ADD  CONSTRAINT [FK_SessionInfo_Animal] FOREIGN KEY([AnimalID])
REFERENCES [dbo].[Animal] ([AnimalID])
GO
ALTER TABLE [dbo].[SessionInfo] CHECK CONSTRAINT [FK_SessionInfo_Animal]
GO
ALTER TABLE [dbo].[SessionInfo]  WITH CHECK ADD  CONSTRAINT [FK_SessionInfo_AspNetUsers] FOREIGN KEY([UserID])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SessionInfo] CHECK CONSTRAINT [FK_SessionInfo_AspNetUsers]
GO
ALTER TABLE [dbo].[SessionInfo]  WITH CHECK ADD  CONSTRAINT [FK_SessionInfo_Experiment] FOREIGN KEY([ExpID])
REFERENCES [dbo].[Experiment] ([ExpID])
GO
ALTER TABLE [dbo].[SessionInfo] CHECK CONSTRAINT [FK_SessionInfo_Experiment]
GO
ALTER TABLE [dbo].[SessionInfo]  WITH CHECK ADD  CONSTRAINT [FK_SessionInfo_Upload] FOREIGN KEY([UploadID])
REFERENCES [dbo].[Upload] ([UploadID])
GO
ALTER TABLE [dbo].[SessionInfo] CHECK CONSTRAINT [FK_SessionInfo_Upload]
GO
ALTER TABLE [dbo].[SessionInfo_Dynamic]  WITH CHECK ADD  CONSTRAINT [FK_SessionInfo_Dynamic_SessionInfo] FOREIGN KEY([SessionID])
REFERENCES [dbo].[SessionInfo] ([SessionID])
GO
ALTER TABLE [dbo].[SessionInfo_Dynamic] CHECK CONSTRAINT [FK_SessionInfo_Dynamic_SessionInfo]
GO
ALTER TABLE [dbo].[Sub_Task]  WITH CHECK ADD  CONSTRAINT [FK_Task_Sub_Task] FOREIGN KEY([Task_ID])
REFERENCES [dbo].[Task] ([ID])
GO
ALTER TABLE [dbo].[Sub_Task] CHECK CONSTRAINT [FK_Task_Sub_Task]
GO
ALTER TABLE [dbo].[SubExperiment]  WITH CHECK ADD  CONSTRAINT [FK_SubExperiment_Age] FOREIGN KEY([AgeID])
REFERENCES [dbo].[Age] ([ID])
GO
ALTER TABLE [dbo].[SubExperiment] CHECK CONSTRAINT [FK_SubExperiment_Age]
GO
ALTER TABLE [dbo].[SubExperiment]  WITH CHECK ADD  CONSTRAINT [FK_SubExperiment_Experiment] FOREIGN KEY([ExpID])
REFERENCES [dbo].[Experiment] ([ExpID])
GO
ALTER TABLE [dbo].[SubExperiment] CHECK CONSTRAINT [FK_SubExperiment_Experiment]
GO
ALTER TABLE [dbo].[Upload]  WITH CHECK ADD  CONSTRAINT [FK_Upload_Animal] FOREIGN KEY([AnimalID])
REFERENCES [dbo].[Animal] ([AnimalID])
GO
ALTER TABLE [dbo].[Upload] CHECK CONSTRAINT [FK_Upload_Animal]
GO
ALTER TABLE [dbo].[Upload]  WITH CHECK ADD  CONSTRAINT [FK_Upload_Experiment] FOREIGN KEY([ExpID])
REFERENCES [dbo].[Experiment] ([ExpID])
GO
ALTER TABLE [dbo].[Upload] CHECK CONSTRAINT [FK_Upload_Experiment]
GO
ALTER TABLE [dbo].[Upload]  WITH CHECK ADD  CONSTRAINT [FK_Upload_SubExperiment] FOREIGN KEY([SubExpID])
REFERENCES [dbo].[SubExperiment] ([SubExpID])
GO
ALTER TABLE [dbo].[Upload] CHECK CONSTRAINT [FK_Upload_SubExperiment]
GO
ALTER TABLE [dbo].[UploadErrorLog]  WITH CHECK ADD  CONSTRAINT [FK_UploadErrorLog_Experiment] FOREIGN KEY([ExpID])
REFERENCES [dbo].[Experiment] ([ExpID])
GO
ALTER TABLE [dbo].[UploadErrorLog] CHECK CONSTRAINT [FK_UploadErrorLog_Experiment]
GO
ALTER TABLE [dbo].[UploadErrorLog]  WITH CHECK ADD  CONSTRAINT [FK_UploadErrorLog_SubExperiment] FOREIGN KEY([SubExpID])
REFERENCES [dbo].[SubExperiment] ([SubExpID])
GO
ALTER TABLE [dbo].[UploadErrorLog] CHECK CONSTRAINT [FK_UploadErrorLog_SubExperiment]
GO
/****** Object:  StoredProcedure [dbo].[update_rbt_cache]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[update_rbt_cache] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @lastDateUpload datetime
    SELECT TOP 1 @lastDateUpload = DateUpload from Upload order by UploadID desc

    IF (DATEDIFF(hour, @lastDateUpload , getdate()) > 48)
    BEGIN
        RETURN
    END

	IF OBJECT_ID('dbo.rbt_cached', 'U') IS NOT NULL 
	DROP TABLE dbo.rbt_cached; 

	select * into rbt_cached from (select * from vw_rbt_cache  with(nolock)) tmp

END
GO
/****** Object:  StoredProcedure [dbo].[update_rbt_data_cache]    Script Date: 2023-10-15 1:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:   <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[update_rbt_data_cache] 
AS
BEGIN

      SET NOCOUNT ON;

      DECLARE @lastDateUpload datetime
    SELECT TOP 1 @lastDateUpload = DateUpload from Upload order by UploadID desc

    --IF (DATEDIFF(hour, @lastDateUpload , getdate()) > 48)
    --BEGIN
    --    RETURN
    --END

IF OBJECT_ID('dbo.rbt_data_cached_avg', 'U') IS NOT NULL 
  DROP TABLE dbo.rbt_data_cached_avg; 
IF OBJECT_ID('dbo.rbt_data_cached_std', 'U') IS NOT NULL 
  DROP TABLE dbo.rbt_data_cached_std; 
IF OBJECT_ID('dbo.rbt_data_cached_cnt', 'U') IS NOT NULL 
  DROP TABLE dbo.rbt_data_cached_cnt;
IF OBJECT_ID('dbo.rbt_data_cached_sum', 'U') IS NOT NULL 
  DROP TABLE dbo.rbt_data_cached_sum;


      DECLARE @cols AS NVARCHAR(MAX),
      @colsAVG AS NVARCHAR(MAX),
      @colsSTD AS NVARCHAR(MAX),
      @colsCnt as NVARCHAR(MAX),
      @colsSUM as NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX)

      select @cols = STUFF((SELECT ',' + QUOTENAME(FeatureName) 
                              from RBT_TouchScreen_Features with(nolock)
                    group by FeatureName
                    order by FeatureName
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

      select @colsAVG = STUFF((SELECT ',' + QUOTENAME(FeatureName) + ' as ' + QUOTENAME('AVG_' + FeatureName) 
                              from RBT_TouchScreen_Features with(nolock)
                              group by FeatureName
                    order by FeatureName
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

      select @colsSTD = STUFF((SELECT ',' + QUOTENAME(FeatureName) + ' as ' + QUOTENAME('STDEV_' + FeatureName) 
                    from RBT_TouchScreen_Features with(nolock)
                    group by FeatureName
                    order by FeatureName
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

                                    
      select @colsCnt = STUFF((SELECT ',' + QUOTENAME(FeatureName) + ' as ' + QUOTENAME('COUNT_' + FeatureName) 
                    from RBT_TouchScreen_Features with(nolock)
                    group by FeatureName
                    order by FeatureName
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

            
      select @colsSUM = STUFF((SELECT ',' + QUOTENAME(FeatureName) + ' as ' + QUOTENAME('SUM_' + FeatureName) 
                              from RBT_TouchScreen_Features with(nolock)
                              group by FeatureName
                    order by FeatureName
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

            
      set @query = 'SELECT * INTO rbt_data_cached_avg FROM (SELECT SessionID,' + @colsAVG  +    ' from 
       (
            select SessionID, FeatureName, 
            case SourceTypeID
            when 1 then Results
            when 2 then [Count]
            when 3 then Duration/1000000
            end 
            as val
            from RBT_TouchScreen_Features with(nolock)
      ) x
      pivot 
      (
            AVG(val)
            for FeatureName in (' + @cols + ')
      ) p1) tmp
      '

      execute(@query);


      set @query = 'SELECT * INTO rbt_data_cached_std FROM (SELECT SessionID,' + @colsSTD  +    ' from 
             (
                select SessionID, FeatureName, 
                        case SourceTypeID
                        when 1 then Results
                        when 2 then [Count]
                        when 3 then Duration/1000000
                        end 
                        as val
                from RBT_TouchScreen_Features with(nolock)
            ) x
                  pivot 
            (
                STDEV(val)
                for FeatureName in (' + @cols + ')
            ) p2) tmp
                  '

      execute(@query);

      set @query = 'SELECT * INTO rbt_data_cached_cnt FROM (SELECT SessionID,' + @colsCnt  +    ' from 
             (
                select SessionID, FeatureName
                from RBT_TouchScreen_Features with(nolock)
            ) x
                  pivot 
            (
                Count(FeatureName)
                for FeatureName in (' + @cols + ')
            ) p3) tmp
                  '
      execute(@query);

      set @query = 'SELECT * INTO rbt_data_cached_sum FROM (SELECT SessionID,' + @colsSUM  +    ' from 
            (
                  select SessionID, FeatureName, 
                  case SourceTypeID
                  when 1 then Results
                  when 2 then [Count]
                  when 3 then Duration/1000000
                  end 
                  as val
                  from RBT_TouchScreen_Features with(nolock)
            ) x
            pivot 
            (
                  SUM(val)
                  for FeatureName in (' + @cols + ')
            ) p1) tmp
            '

      execute(@query);

--IF OBJECT_ID('dbo.rbt_data_cached', 'U') IS NOT NULL 
--  DROP TABLE dbo.rbt_data_cached; 

      
--      select * into rbt_data_cached from (
--            select  * from rbt_data_cached_1 t1  with(nolock)
--            inner join rbt_data_cached_2 t2  with(nolock) on t1.SessionId = t2.SessionId2
--            inner join rbt_data_cached_3 t3  with(nolock) on t2.SessionId2 = t3.SessionId3
--            inner join rbt_data_cached_4 t4  with(nolock) on t3.SessionId3 = t4.SessionId4
--      ) tmp

--      IF OBJECT_ID('dbo.rbt_data_cached_1', 'U') IS NOT NULL 
--        DROP TABLE dbo.rbt_data_cached_1; 
--      IF OBJECT_ID('dbo.rbt_data_cached_2', 'U') IS NOT NULL 
--        DROP TABLE dbo.rbt_data_cached_2; 
--      IF OBJECT_ID('dbo.rbt_data_cached_3', 'U') IS NOT NULL 
--        DROP TABLE dbo.rbt_data_cached_3; 
--      IF OBJECT_ID('dbo.rbt_data_cached_4', 'U') IS NOT NULL 
--        DROP TABLE dbo.rbt_data_cached_4; 



END


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
         Begin Table = "SessionInfo"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 240
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Animal"
            Begin Extent = 
               Top = 6
               Left = 278
               Bottom = 136
               Right = 448
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Experiment"
            Begin Extent = 
               Top = 6
               Left = 486
               Bottom = 136
               Right = 659
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Genotype"
            Begin Extent = 
               Top = 6
               Left = 697
               Bottom = 136
               Right = 867
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Strain"
            Begin Extent = 
               Top = 6
               Left = 905
               Bottom = 119
               Right = 1075
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Upload"
            Begin Extent = 
               Top = 6
               Left = 1113
               Bottom = 136
               Right = 1302
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SubExperiment"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 251
            End
            DisplayFlags' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_MB_Data'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 280
            TopColumn = 0
         End
         Begin Table = "Age"
            Begin Extent = 
               Top = 120
               Left = 905
               Bottom = 216
               Right = 1075
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Task"
            Begin Extent = 
               Top = 138
               Left = 289
               Bottom = 268
               Right = 462
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tt2"
            Begin Extent = 
               Top = 138
               Left = 500
               Bottom = 268
               Right = 670
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_MB_Data'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_MB_Data'
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
         Begin Table = "SessionInfo"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 240
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Animal"
            Begin Extent = 
               Top = 6
               Left = 278
               Bottom = 136
               Right = 448
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Experiment"
            Begin Extent = 
               Top = 6
               Left = 486
               Bottom = 136
               Right = 659
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Genotype"
            Begin Extent = 
               Top = 6
               Left = 697
               Bottom = 136
               Right = 867
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Strain"
            Begin Extent = 
               Top = 6
               Left = 905
               Bottom = 119
               Right = 1075
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Upload"
            Begin Extent = 
               Top = 6
               Left = 1113
               Bottom = 136
               Right = 1302
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SubExperiment"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 251
            End
            DisplayFlags' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_PD_DataAnalysis_AllData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 280
            TopColumn = 0
         End
         Begin Table = "Age"
            Begin Extent = 
               Top = 120
               Left = 905
               Bottom = 216
               Right = 1075
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Task"
            Begin Extent = 
               Top = 138
               Left = 289
               Bottom = 268
               Right = 462
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tt2"
            Begin Extent = 
               Top = 138
               Left = 500
               Bottom = 268
               Right = 670
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tmp"
            Begin Extent = 
               Top = 216
               Left = 708
               Bottom = 346
               Right = 1016
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_PD_DataAnalysis_AllData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_PD_DataAnalysis_AllData'
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
         Begin Table = "PISite"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 119
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PI"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 102
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Site"
            Begin Extent = 
               Top = 6
               Left = 454
               Bottom = 119
               Right = 624
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_PI_Site'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_PI_Site'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[13] 4[20] 2[48] 3) )"
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
         Left = -1594
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 23
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_tmp_PD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_tmp_PD'
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
         Begin Table = "RBT_TouchScreen_Features"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_RBT_Cache'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_RBT_Cache'
GO
USE [master]
GO
ALTER DATABASE [Mousebytes] SET  READ_WRITE 
GO
