
CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO
  --  USER TABLE--
   

CREATE TABLE [User]
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,

    FirstName NVARCHAR(50) NOT NULL,

    LastName NVARCHAR(50) NOT NULL,

    Email NVARCHAR(100) NOT NULL UNIQUE,

    PasswordHash NVARCHAR(255) NOT NULL,

    PhoneNumber NVARCHAR(20) NULL,

    Role NVARCHAR(20) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_User_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT CK_User_Role
        CHECK (Role IN ('Organiser', 'Participant'))
);
GO
-- EVENT TYPE TABLE --
 

CREATE TABLE EventType
(
    EventTypeID INT IDENTITY(1,1) PRIMARY KEY,

    TypeName NVARCHAR(20) NOT NULL UNIQUE
);
GO



   -- EVENT TABLE--
   

CREATE TABLE Event
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,

    OrganiserID INT NOT NULL,

    EventTypeID INT NOT NULL,

    Name NVARCHAR(150) NOT NULL,

    Description NVARCHAR(1000) NULL,

    EventDate DATE NOT NULL,

    Location NVARCHAR(200) NOT NULL,

    Distance DECIMAL(6,2) NOT NULL,

    BannerImageUrl NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Event_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES [User](UserID),

    CONSTRAINT FK_Event_EventType
        FOREIGN KEY (EventTypeID)
        REFERENCES EventType(EventTypeID),

    CONSTRAINT CK_Event_Distance
        CHECK (Distance > 0)
);
GO
  -- CATEGORY TABLE --
   

CREATE TABLE Category
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    CategoryName NVARCHAR(100) NOT NULL,

    MinimumAge INT NULL,

    MaximumAge INT NULL,

    Distance DECIMAL(6,2) NULL,

    CONSTRAINT FK_Category_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID)
        ON DELETE CASCADE,

    CONSTRAINT CK_Category_MinimumAge
        CHECK (MinimumAge IS NULL OR MinimumAge >= 0),

    CONSTRAINT CK_Category_MaximumAge
        CHECK (MaximumAge IS NULL OR MaximumAge >= 0),

    CONSTRAINT CK_Category_AgeRange
        CHECK (
            MinimumAge IS NULL
            OR MaximumAge IS NULL
            OR MinimumAge <= MaximumAge
        ),

    CONSTRAINT CK_Category_Distance
        CHECK (Distance IS NULL OR Distance > 0),

    CONSTRAINT UQ_Category_Event_Name
        UNIQUE (EventID, CategoryName)
);
GO

-- ENROLMENT TABLE --
  

CREATE TABLE Enrolment
(
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,

    ParticipantID INT NOT NULL,

    EventID INT NOT NULL,

    CategoryID INT NOT NULL,

    EnrolmentDate DATETIME2 NOT NULL
        CONSTRAINT DF_Enrolment_Date DEFAULT SYSDATETIME(),

    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Enrolment_Status DEFAULT 'Pending',

    CONSTRAINT FK_Enrolment_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES [User](UserID),

    CONSTRAINT FK_Enrolment_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID),

    CONSTRAINT FK_Enrolment_Category
        FOREIGN KEY (CategoryID)
        REFERENCES Category(CategoryID),

    CONSTRAINT CK_Enrolment_Status
        CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),

    CONSTRAINT UQ_Enrolment_Participant_Event
        UNIQUE (ParticipantID, EventID)
);
GO


-- RESULT TABLE --
   

CREATE TABLE Result
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,

    EnrolmentID INT NOT NULL UNIQUE,

    FinishTime TIME NOT NULL,

    FinishingPosition INT NOT NULL,

    RecordedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Result_RecordedAt DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Result_Enrolment
        FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolment(EnrolmentID)
        ON DELETE CASCADE,

    CONSTRAINT CK_Result_Position
        CHECK (FinishingPosition > 0)
);
GO

 -- EVENT TYPES --
 

INSERT INTO EventType (TypeName)
VALUES
    ('Run'),
    ('Walk'),
    ('Cycle');
GO

 -- USERS--
  -- 2 Organisers --
--   2 Participants --
  

INSERT INTO [User]
(
    FirstName,
    LastName,
    Email,
    PasswordHash,
    PhoneNumber,
    Role
)
VALUES
(
    'Thabo',
    'Mokoena',
    'thabo@raceday.co.za',
    'HASHED_PASSWORD_001',
    '0821111111',
    'Organiser'
),
(
    'Lerato',
    'Mahlangu',
    'lerato@raceday.co.za',
    'HASHED_PASSWORD_002',
    '0832222222',
    'Organiser'
),
(
    'Tshilidzi',
    'Ndlovu',
    'tshilidzi@example.com',
    'HASHED_PASSWORD_003',
    '0843333333',
    'Participant'
),
(
    'Sipho',
    'Dlamini',
    'sipho@example.com',
    'HASHED_PASSWORD_004',
    '0854444444',
    'Participant'
);
GO

--  EVENTS --
 

INSERT INTO Event
(
    OrganiserID,
    EventTypeID,
    Name,
    Description,
    EventDate,
    Location,
    Distance
)
VALUES
(
    1,
    1,
    'Johannesburg City Run',
    'A road running event through Johannesburg.',
    '2026-10-10',
    'Johannesburg',
    10.00
),
(
    1,
    2,
    'Soweto Community Walk',
    'A community walking event in Soweto.',
    '2026-10-24',
    'Soweto',
    5.00
),
(
    2,
    3,
    'Gauteng Cycle Challenge',
    'A cycling event for participants across Gauteng.',
    '2026-11-07',
    'Pretoria',
    50.00
);
GO

-- CATEGORIES --


INSERT INTO Category
(
    EventID,
    CategoryName,
    MinimumAge,
    MaximumAge,
    Distance
)
VALUES
(1, 'Under 20', 13, 19, 10.00),
(1, 'Senior', 20, 39, 10.00),
(1, 'Veteran', 40, NULL, 10.00),

(2, 'Under 20', 13, 19, 5.00),
(2, 'Senior', 20, 39, 5.00),

(3, 'Under 20', 16, 19, 50.00),
(3, 'Senior', 20, 39, 50.00);
GO


 -- ENROLMENTS --
  

INSERT INTO Enrolment
(
    ParticipantID,
    EventID,
    CategoryID,
    Status
)
VALUES
(3, 1, 2, 'Confirmed'),
(4, 1, 2, 'Confirmed'),
(3, 2, 5, 'Confirmed'),
(4, 3, 7, 'Pending');
GO

-- RESULTS --


INSERT INTO Result
(
    EnrolmentID,
    FinishTime,
    FinishingPosition
)
VALUES
(1, '00:52:31', 47),
(2, '00:55:18', 62);
GO

 --VERIFICATION QUERIES--


SELECT * FROM [User];

SELECT * FROM EventType;

SELECT * FROM Event;

SELECT * FROM Category;

SELECT * FROM Enrolment;

SELECT * FROM Result;
GO
