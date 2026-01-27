/* ============================
   DISABLE FK
============================ */
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL";

/* ============================
   DROP TABLES
============================ */
IF OBJECT_ID('UserRoles') IS NOT NULL DROP TABLE UserRoles;
IF OBJECT_ID('Users') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles') IS NOT NULL DROP TABLE Roles;

/* ============================
   ROLES & USERS
============================ */
CREATE TABLE Roles (
    Id BIGINT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);

CREATE TABLE Users (
    Id BIGINT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(255),
    PhoneNumber NVARCHAR(50),
    Status NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);

CREATE TABLE UserRoles (
    UserId BIGINT NOT NULL,
    RoleId BIGINT NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);