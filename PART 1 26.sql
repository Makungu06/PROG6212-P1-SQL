

/* ============================================================
   1. CREATE DATABASE
   ============================================================ */

IF DB_ID('RaceDay') IS NULL
BEGIN
    CREATE DATABASE RaceDay;
END;
GO

USE RaceDay;
GO


/* ============================================================
   2. DROP EXISTING TABLES
   
   ============================================================ */

IF OBJECT_ID('dbo.RESULT', 'U') IS NOT NULL
    DROP TABLE dbo.RESULT;

IF OBJECT_ID('dbo.PAYMENT', 'U') IS NOT NULL
    DROP TABLE dbo.PAYMENT;

IF OBJECT_ID('dbo.RACE_ENTRY', 'U') IS NOT NULL
    DROP TABLE dbo.RACE_ENTRY;

IF OBJECT_ID('dbo.EVENT_SPONSOR', 'U') IS NOT NULL
    DROP TABLE dbo.EVENT_SPONSOR;

IF OBJECT_ID('dbo.SPONSOR', 'U') IS NOT NULL
    DROP TABLE dbo.SPONSOR;

IF OBJECT_ID('dbo.PARTICIPANT', 'U') IS NOT NULL
    DROP TABLE dbo.PARTICIPANT;

IF OBJECT_ID('dbo.RACE', 'U') IS NOT NULL
    DROP TABLE dbo.RACE;

IF OBJECT_ID('dbo.RACE_EVENT', 'U') IS NOT NULL
    DROP TABLE dbo.RACE_EVENT;

IF OBJECT_ID('dbo.[USER]', 'U') IS NOT NULL
    DROP TABLE dbo.[USER];
GO


/* ============================================================
   3. USER TABLE
   ============================================================ */

CREATE TABLE dbo.[USER]
(
    UserID       INT IDENTITY(1,1) NOT NULL,
    Username     NVARCHAR(50) NOT NULL,
    [Password]   NVARCHAR(255) NOT NULL,
    Email        NVARCHAR(100) NOT NULL,
    Role         NVARCHAR(30) NOT NULL
                 CONSTRAINT DF_USER_Role
                 DEFAULT ('Participant'),
    FullName     NVARCHAR(100) NOT NULL,

    CONSTRAINT PK_USER
        PRIMARY KEY (UserID),

    CONSTRAINT UQ_USER_Username
        UNIQUE (Username),

    CONSTRAINT UQ_USER_Email
        UNIQUE (Email),

    CONSTRAINT CK_USER_Role
        CHECK (Role IN
              ('Organizer', 'Participant', 'Admin'))
);
GO


/* ============================================================
   4. RACE_EVENT TABLE
   ============================================================ */

CREATE TABLE dbo.RACE_EVENT
(
    EventID       INT IDENTITY(1,1) NOT NULL,
    EventName     NVARCHAR(100) NOT NULL,
    EventDate     DATE NOT NULL,
    Venue         NVARCHAR(150) NOT NULL,
    Location      NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,

    CONSTRAINT PK_RACE_EVENT
        PRIMARY KEY (EventID),

    CONSTRAINT UQ_RACE_EVENT_EventName
        UNIQUE (EventName)
);
GO


/* ============================================================
   5. RACE TABLE
   ============================================================ */

CREATE TABLE dbo.RACE
(
    RaceID       INT IDENTITY(1,1) NOT NULL,
    EventID      INT NOT NULL,
    RaceName     NVARCHAR(100) NOT NULL,
    RaceNumber   NVARCHAR(20) NOT NULL,
    RaceType     NVARCHAR(50) NOT NULL,
    Distance     DECIMAL(6,2) NOT NULL,
    StartTime    TIME NOT NULL,

    CONSTRAINT PK_RACE
        PRIMARY KEY (RaceID),

    CONSTRAINT UQ_RACE_RaceNumber_Event
        UNIQUE (EventID, RaceNumber),

    CONSTRAINT CK_RACE_Distance
        CHECK (Distance > 0),

    CONSTRAINT FK_RACE_RACE_EVENT
        FOREIGN KEY (EventID)
        REFERENCES dbo.RACE_EVENT(EventID)
);
GO


/* ============================================================
   6. PARTICIPANT TABLE
   ============================================================ */

CREATE TABLE dbo.PARTICIPANT
(
    ParticipantID INT IDENTITY(1,1) NOT NULL,
    FirstName     NVARCHAR(50) NOT NULL,
    LastName      NVARCHAR(50) NOT NULL,
    DateOfBirth   DATE NOT NULL,
    Gender        NVARCHAR(20) NOT NULL,
    Email         NVARCHAR(100) NOT NULL,
    Phone         NVARCHAR(30) NOT NULL,

    CONSTRAINT PK_PARTICIPANT
        PRIMARY KEY (ParticipantID),

    CONSTRAINT UQ_PARTICIPANT_Email
        UNIQUE (Email),

    CONSTRAINT CK_PARTICIPANT_Gender
        CHECK (Gender IN
              ('Male', 'Female', 'Other'))
);
GO


/* ============================================================
   7. SPONSOR TABLE
   ============================================================ */

CREATE TABLE dbo.SPONSOR
(
    SponsorID     INT IDENTITY(1,1) NOT NULL,
    SponsorName   NVARCHAR(100) NOT NULL,
    ContactName   NVARCHAR(100) NOT NULL,
    Email         NVARCHAR(100) NOT NULL,
    Phone         NVARCHAR(30) NOT NULL,
    Website       NVARCHAR(200) NOT NULL,

    CONSTRAINT PK_SPONSOR
        PRIMARY KEY (SponsorID),

    CONSTRAINT UQ_SPONSOR_SponsorName
        UNIQUE (SponsorName),

    CONSTRAINT UQ_SPONSOR_Email
        UNIQUE (Email)
);
GO


/* ============================================================
   8. EVENT_SPONSOR TABLE
   Implements:
   RACE_EVENT * ----- * SPONSOR
   ============================================================ */

CREATE TABLE dbo.EVENT_SPONSOR
(
    EventID       INT NOT NULL,
    SponsorID     INT NOT NULL,

    CONSTRAINT PK_EVENT_SPONSOR
        PRIMARY KEY (EventID, SponsorID),

    CONSTRAINT FK_EVENT_SPONSOR_Event
        FOREIGN KEY (EventID)
        REFERENCES dbo.RACE_EVENT(EventID),

    CONSTRAINT FK_EVENT_SPONSOR_Sponsor
        FOREIGN KEY (SponsorID)
        REFERENCES dbo.SPONSOR(SponsorID)
);
GO


/* ============================================================
   9. RACE_ENTRY TABLE
   ============================================================ */

CREATE TABLE dbo.RACE_ENTRY
(
    EntryID       INT IDENTITY(1,1) NOT NULL,
    RaceID        INT NOT NULL,
    ParticipantID INT NOT NULL,
    UserID        INT NOT NULL,
    EntryDate     DATE NOT NULL
                  CONSTRAINT DF_RACE_ENTRY_EntryDate
                  DEFAULT (CAST(GETDATE() AS DATE)),
    Status        NVARCHAR(30) NOT NULL
                  CONSTRAINT DF_RACE_ENTRY_Status
                  DEFAULT ('Pending'),
    BibNumber     INT NOT NULL,

    CONSTRAINT PK_RACE_ENTRY
        PRIMARY KEY (EntryID),

    /* A participant cannot enter the same race twice */
    CONSTRAINT UQ_RACE_ENTRY_Race_Participant
        UNIQUE (RaceID, ParticipantID),

    /* Bib number must be unique within a race */
    CONSTRAINT UQ_RACE_ENTRY_BibNumber
        UNIQUE (RaceID, BibNumber),

    /* One USER has one RACE_ENTRY */
    CONSTRAINT UQ_RACE_ENTRY_User
        UNIQUE (UserID),

    CONSTRAINT CK_RACE_ENTRY_Status
        CHECK (Status IN
              ('Pending',
               'Confirmed',
               'Cancelled',
               'Completed')),

    CONSTRAINT CK_RACE_ENTRY_BibNumber
        CHECK (BibNumber > 0),

    CONSTRAINT FK_RACE_ENTRY_Race
        FOREIGN KEY (RaceID)
        REFERENCES dbo.RACE(RaceID),

    CONSTRAINT FK_RACE_ENTRY_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES dbo.PARTICIPANT(ParticipantID),

    CONSTRAINT FK_RACE_ENTRY_User
        FOREIGN KEY (UserID)
        REFERENCES dbo.[USER](UserID)
);
GO


/* ============================================================
   10. PAYMENT TABLE
   ============================================================ */

CREATE TABLE dbo.PAYMENT
(
    PaymentID      INT IDENTITY(1,1) NOT NULL,
    EntryID        INT NOT NULL,
    PaymentDate    DATE NOT NULL
                   CONSTRAINT DF_PAYMENT_PaymentDate
                   DEFAULT (CAST(GETDATE() AS DATE)),
    Amount         DECIMAL(10,2) NOT NULL,
    PaymentMethod  NVARCHAR(30) NOT NULL,
    PaymentStatus  NVARCHAR(30) NOT NULL
                   CONSTRAINT DF_PAYMENT_PaymentStatus
                   DEFAULT ('Pending'),
    TransactionRef NVARCHAR(100) NOT NULL,

    CONSTRAINT PK_PAYMENT
        PRIMARY KEY (PaymentID),

    /* One payment per race entry */
    CONSTRAINT UQ_PAYMENT_EntryID
        UNIQUE (EntryID),

    CONSTRAINT UQ_PAYMENT_TransactionRef
        UNIQUE (TransactionRef),

    CONSTRAINT CK_PAYMENT_Amount
        CHECK (Amount > 0),

    CONSTRAINT CK_PAYMENT_Method
        CHECK (PaymentMethod IN
              ('Card', 'EFT', 'Cash', 'Online')),

    CONSTRAINT CK_PAYMENT_Status
        CHECK (PaymentStatus IN
              ('Pending',
               'Paid',
               'Failed',
               'Refunded')),

    CONSTRAINT FK_PAYMENT_RaceEntry
        FOREIGN KEY (EntryID)
        REFERENCES dbo.RACE_ENTRY(EntryID)
);
GO


/* ============================================================
   11. RESULT TABLE
   ============================================================ */

CREATE TABLE dbo.RESULT
(
    ResultID     INT IDENTITY(1,1) NOT NULL,
    EntryID      INT NOT NULL,
    FinishTime   TIME NOT NULL,
    Position     INT NOT NULL,
    RaceStatus   NVARCHAR(30) NOT NULL
                 CONSTRAINT DF_RESULT_RaceStatus
                 DEFAULT ('Finished'),

    CONSTRAINT PK_RESULT
        PRIMARY KEY (ResultID),

    /* A race entry can have only one result */
    CONSTRAINT UQ_RESULT_EntryID
        UNIQUE (EntryID),

    CONSTRAINT CK_RESULT_Position
        CHECK (Position > 0),

    CONSTRAINT CK_RESULT_RaceStatus
        CHECK (RaceStatus IN
              ('Finished',
               'DNF',
               'DNS',
               'Disqualified')),

    CONSTRAINT FK_RESULT_RaceEntry
        FOREIGN KEY (EntryID)
        REFERENCES dbo.RACE_ENTRY(EntryID)
);
GO


/* ============================================================
   12. SAMPLE DATA
   ============================================================ */


/* ------------------------------------------------------------
   USERS / ORGANISERS
   ------------------------------------------------------------ */

INSERT INTO dbo.[USER]
    (Username, [Password], Email, Role, FullName)
VALUES
    ('jane.organizer',
     'RaceDay@123',
     'jane.organizer@raceday.com',
     'Organizer',
     'Jane Smith'),

    ('john.organizer',
     'RaceDay@456',
     'john.organizer@raceday.com',
     'Organizer',
     'John Williams'),

    ('alice.runner',
     'Runner@123',
     'alice.runner@email.com',
     'Participant',
     'Alice Brown'),

    ('bob.runner',
     'Runner@456',
     'bob.runner@email.com',
     'Participant',
     'Bob Johnson'),

    ('charlie.runner',
     'Runner@789',
     'charlie.runner@email.com',
     'Participant',
     'Charlie Davis'),

    ('diana.runner',
     'Runner@999',
     'diana.runner@email.com',
     'Participant',
     'Diana Wilson');
GO


/* ------------------------------------------------------------
   RACE EVENTS
   ------------------------------------------------------------ */

INSERT INTO dbo.RACE_EVENT
    (EventName,
     EventDate,
     Venue,
     Location,
     [Description])
VALUES
    ('Cape Town City Run',
     '2026-10-10',
     'Green Point Stadium',
     'Cape Town',
     'Annual city running event featuring multiple race categories.'),

    ('Johannesburg Spring Race',
     '2026-11-07',
     'Wanderers Stadium',
     'Johannesburg',
     'Spring road running event for recreational and competitive runners.'),

    ('Durban Coastal Challenge',
     '2026-12-05',
     'Moses Mabhida Stadium',
     'Durban',
     'Coastal running event with short and long distance categories.');
GO


/* ------------------------------------------------------------
   RACES / CATEGORIES
   3 races per event:
   5KM, 10KM and 21KM
   ------------------------------------------------------------ */

INSERT INTO dbo.RACE
    (EventID,
     RaceName,
     RaceNumber,
     RaceType,
     Distance,
     StartTime)
VALUES

    /* Cape Town City Run */
    (1,
     'Cape Town 5KM Fun Run',
     'CT-05',
     '5KM',
     5.00,
     '07:00'),

    (1,
     'Cape Town 10KM Road Race',
     'CT-10',
     '10KM',
     10.00,
     '07:30'),

    (1,
     'Cape Town Half Marathon',
     'CT-21',
     '21KM',
     21.10,
     '06:30'),

    /* Johannesburg Spring Race */
    (2,
     'Johannesburg 5KM Fun Run',
     'JHB-05',
     '5KM',
     5.00,
     '07:00'),

    (2,
     'Johannesburg 10KM Road Race',
     'JHB-10',
     '10KM',
     10.00,
     '07:30'),

    (2,
     'Johannesburg Half Marathon',
     'JHB-21',
     '21KM',
     21.10,
     '06:30'),

    /* Durban Coastal Challenge */
    (3,
     'Durban 5KM Fun Run',
     'DBN-05',
     '5KM',
     5.00,
     '07:00'),

    (3,
     'Durban 10KM Coastal Race',
     'DBN-10',
     '10KM',
     10.00,
     '07:30'),

    (3,
     'Durban Half Marathon',
     'DBN-21',
     '21KM',
     21.10,
     '06:30');
GO


/* ------------------------------------------------------------
   PARTICIPANTS
   ------------------------------------------------------------ */

INSERT INTO dbo.PARTICIPANT
    (FirstName,
     LastName,
     DateOfBirth,
     Gender,
     Email,
     Phone)
VALUES
    ('Alice',
     'Brown',
     '1995-04-15',
     'Female',
     'alice.brown@email.com',
     '0825551001'),

    ('Bob',
     'Johnson',
     '1992-08-21',
     'Male',
     'bob.johnson@email.com',
     '0825551002'),

    ('Charlie',
     'Davis',
     '1998-01-10',
     'Male',
     'charlie.davis@email.com',
     '0825551003'),

    ('Diana',
     'Wilson',
     '1990-11-30',
     'Female',
     'diana.wilson@email.com',
     '0825551004');
GO


/* ------------------------------------------------------------
   SPONSORS
   ------------------------------------------------------------ */

INSERT INTO dbo.SPONSOR
    (SponsorName,
     ContactName,
     Email,
     Phone,
     Website)
VALUES
    ('ActiveLife Sports',
     'Michael Adams',
     'michael@activelifesports.com',
     '0115552001',
     'https://www.activelifesports.com'),

    ('FitFuel Nutrition',
     'Sarah Green',
     'sarah@fitfuel.com',
     '0115552002',
     'https://www.fitfuel.com'),

    ('RunPro Athletics',
     'David Miller',
     'david@runpro.com',
     '0115552003',
     'https://www.runpro.com');
GO


/* ------------------------------------------------------------
   EVENT SPONSORS
   ------------------------------------------------------------ */

INSERT INTO dbo.EVENT_SPONSOR
    (EventID, SponsorID)
VALUES
    (1, 1),
    (1, 2),
    (2, 1),
    (2, 3),
    (3, 2),
    (3, 3);
GO


/* ------------------------------------------------------------
   RACE ENTRIES / SAMPLE ENROLMENTS
   ------------------------------------------------------------ */

INSERT INTO dbo.RACE_ENTRY
    (RaceID,
     ParticipantID,
     UserID,
     EntryDate,
     Status,
     BibNumber)
VALUES
    (1,
     1,
     3,
     '2026-08-20',
     'Confirmed',
     101),

    (2,
     2,
     4,
     '2026-08-21',
     'Confirmed',
     201),

    (4,
     3,
     5,
     '2026-08-22',
     'Confirmed',
     301),

    (5,
     4,
     6,
     '2026-08-23',
     'Pending',
     401);
GO


/* ------------------------------------------------------------
   PAYMENTS
   ------------------------------------------------------------ */

INSERT INTO dbo.PAYMENT
    (EntryID,
     PaymentDate,
     Amount,
     PaymentMethod,
     PaymentStatus,
     TransactionRef)
VALUES
    (1,
     '2026-08-20',
     150.00,
     'Card',
     'Paid',
     'TXN-2026-0001'),

    (2,
     '2026-08-21',
     200.00,
     'Online',
     'Paid',
     'TXN-2026-0002'),

    (3,
     '2026-08-22',
     150.00,
     'EFT',
     'Paid',
     'TXN-2026-0003'),

    (4,
     '2026-08-23',
     200.00,
     'Card',
     'Pending',
     'TXN-2026-0004');
GO


/* ------------------------------------------------------------
   RESULTS
   ------------------------------------------------------------ */

INSERT INTO dbo.RESULT
    (EntryID,
     FinishTime,
     Position,
     RaceStatus)
VALUES
    (1,
     '00:28:42',
     1,
     'Finished'),

    (2,
     '00:52:18',
     2,
     'Finished'),

    (3,
     '00:26:55',
     1,
     'Finished');
GO


/* ============================================================
   13. VERIFICATION QUERIES
   ============================================================ */

SELECT * FROM dbo.[USER];

SELECT * FROM dbo.RACE_EVENT;

SELECT * FROM dbo.RACE;

SELECT * FROM dbo.PARTICIPANT;

SELECT * FROM dbo.SPONSOR;

SELECT * FROM dbo.EVENT_SPONSOR;

SELECT * FROM dbo.RACE_ENTRY;

SELECT * FROM dbo.PAYMENT;

SELECT * FROM dbo.RESULT;
GO


/* ============================================================
   14. RELATIONSHIP TEST
   Displays the connected RaceDay information
   ============================================================ */

SELECT
    re.EventID,
    re.EventName,
    re.EventDate,
    re.Venue,
    re.Location,

    r.RaceID,
    r.RaceName,
    r.RaceNumber,
    r.RaceType,
    r.Distance,
    r.StartTime,

    p.ParticipantID,
    p.FirstName + ' ' + p.LastName AS Participant,
    p.Email AS ParticipantEmail,

    e.EntryID,
    e.EntryDate,
    e.Status AS EntryStatus,
    e.BibNumber,

    u.UserID,
    u.Username,
    u.Role,

    pay.PaymentID,
    pay.Amount,
    pay.PaymentMethod,
    pay.PaymentStatus,
    pay.TransactionRef,

    res.ResultID,
    res.FinishTime,
    res.Position,
    res.RaceStatus

FROM dbo.RACE_EVENT AS re

INNER JOIN dbo.RACE AS r
    ON re.EventID = r.EventID

INNER JOIN dbo.RACE_ENTRY AS e
    ON r.RaceID = e.RaceID

INNER JOIN dbo.PARTICIPANT AS p
    ON e.ParticipantID = p.ParticipantID

INNER JOIN dbo.[USER] AS u
    ON e.UserID = u.UserID

LEFT JOIN dbo.PAYMENT AS pay
    ON e.EntryID = pay.EntryID

LEFT JOIN dbo.RESULT AS res
    ON e.EntryID = res.EntryID

ORDER BY
    re.EventDate,
    r.StartTime,
    e.BibNumber;
GO


/* ============================================================
   15. EVENT AND SPONSOR RELATIONSHIP TEST
   ============================================================ */

SELECT
    re.EventID,
    re.EventName,
    re.EventDate,
    s.SponsorID,
    s.SponsorName,
    s.ContactName,
    s.Email,
    s.Phone,
    s.Website

FROM dbo.RACE_EVENT AS re

INNER JOIN dbo.EVENT_SPONSOR AS es
    ON re.EventID = es.EventID

INNER JOIN dbo.SPONSOR AS s
    ON es.SponsorID = s.SponsorID

ORDER BY
    re.EventName,
    s.SponsorName;
GO