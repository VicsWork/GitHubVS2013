USE [ManufacturingStore_v2]
GO

/****** Object:  Table [dbo].[CalibrationResults]    Script Date: 8/16/2017 5:01:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CalibrationResults](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EuiId] [int] NOT NULL,
	[VoltageGain] [int] NULL,
	[CurrentGain] [int] NULL,
	[DateCalibrated] [datetime] NOT NULL,
	[MachineId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CalibrationResults] ADD  DEFAULT (getdate()) FOR [DateCalibrated]
GO

ALTER TABLE [dbo].[CalibrationResults]  WITH CHECK ADD  CONSTRAINT [FK_CalibrationResults_EuiList] FOREIGN KEY([EuiId])
REFERENCES [dbo].[EuiList] ([Id])
GO

ALTER TABLE [dbo].[CalibrationResults] CHECK CONSTRAINT [FK_CalibrationResults_EuiList]
GO

ALTER TABLE [dbo].[CalibrationResults]  WITH CHECK ADD  CONSTRAINT [FK_CalibrationResults_TestStationMachines] FOREIGN KEY([MachineId])
REFERENCES [dbo].[TestStationMachines] ([Id])
GO

ALTER TABLE [dbo].[CalibrationResults] CHECK CONSTRAINT [FK_CalibrationResults_TestStationMachines]
GO

