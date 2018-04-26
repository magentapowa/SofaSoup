--Create database
USE master
GO
ALTER DATABASE SofaSoup SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
IF EXISTS (
    SELECT name
    FROM sys.databases
    WHERE name = N'SofaSoup'
)
DROP DATABASE SofaSoup
GO
CREATE DATABASE SofaSoup
GO


-- Create Tables
USE SofaSoup

CREATE TABLE dbo.Users
(
  UserID INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- primary key column
  username [NVARCHAR](50) NOT NULL,
  password [NVARCHAR](50) NOT NULL,
  LVL INT NOT NULL,
  MemberSince [DATE] NOT NULL
);
GO

CREATE TABLE dbo.Events
(
  EventID INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- primary key column
  Description [NVARCHAR](MAX) NOT NULL,
  Address [NVARCHAR](MAX) NOT NULL,
  Date [DATETIME] NOT NULL,
  UserID INT NULL
  
    CONSTRAINT Events_UserID 
    FOREIGN KEY (UserID)
    REFERENCES Users(UserID) 
    ON DELETE SET NULL
);
GO

CREATE TABLE dbo.Saves
(
    UserID INT,
    EventID INT,
    PRIMARY KEY (UserID,EventID),

    CONSTRAINT fk_Saves_UserID
    FOREIGN KEY (UserID)
    REFERENCES Users (UserID)
    ON DELETE CASCADE,

    CONSTRAINT fk_Saves_EventID
    FOREIGN KEY (EventID)
    REFERENCES Events (EventID)
    ON DELETE CASCADE
);
GO

--Insert values
INSERT INTO Users
( [username], [password], [LVL],[MemberSince])
VALUES
('admin', 'aDmI3$', 4,'2018-04-01'),
('mike', '22510sA!', 4,'2018-04-02'),
('sofia', '22510sA!', 3,'2018-04-05'),
('anaconda', '22510sA!', 2,'2018-04-18'),
('kiko', '22510sA!', 1,'2018-04-23')
GO

INSERT INTO Events
( [Description], [Address], [Date],[UserID])
VALUES
('Funk,RnB', 'Aleksandras 90, Marousi', '2018-05-10 23:00', 1),
('Minor asia sounds, rebetiko', 'Kavetsou 10, Mytilene','2018-05-10 22:00:00.000' , 2),
('Jazz, Funk // BYOB. Need trombones', 'Korai 22, Athens','2018-05-05 22:00:00.000' ,3),
('Jazz, funk // BYOB // we need a drumer', 'Aleksandras 19, Athens','2018-05-20 22:00', NULL),
('Thash metal. BYOB// and the end we will play some Bacho', 'Korai 30,Athens','2018-05-10 22:00' ,5),
('60''s RnR' ,'Kavetsou 30, Pisw apo to synergeio' ,'2018-06-09 19:00:00.000' ,NULL ),
( 'gypsy swing, we need an accordion!!! // BYOB' ,'parko Xarhakos, L.Aleksandras, Thessaloniki' ,'2018-06-01 21:00:00.000' , 2),
( 'Afro funk, a little ska. BYOB' ,'Platanwn 30, Nikaia. sto  upogeio' ,'2018-05-29 22:00:00.000' , 1),
( 'soul, blues', 'Ioulianou 91, Pagkrati' ,'2018-06-05 21:00:00.000' ,4)
GO

INSERT INTO Saves
( [UserID], [EventID])
VALUES
(1,3),
(2,9),
(3,5),
(4,2),
(5,6),
(4,1),
(3,8),
(1,7)
GO