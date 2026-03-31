-- SQL Script for creating the WatchlistMovies table
-- Run this script inside your SQL database to set up the schema.

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WatchlistMovies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WatchlistMovies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImdbId] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Year] [int] NOT NULL,
	[AiPitch] [nvarchar](max) NULL,
	[AddedAt] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[IsWatched] [bit] NOT NULL DEFAULT (0),
 CONSTRAINT [PK_WatchlistMovies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
END
GO

-- Optional: Add a test movie to verify the setup
-- INSERT INTO [dbo].[WatchlistMovies] ([ImdbId], [Title], [Year], [AiPitch], [AddedAt], [IsWatched])
-- VALUES ('tt0133093', 'The Matrix', 1999, 'A hacker discovers reality is a simulation.', GETDATE(), 0);