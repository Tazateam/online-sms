create database online_sms;
use online_sms;
--for user registration table 

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    MobileNumber NVARCHAR(10) UNIQUE NOT NULL,
	Gender NVARCHAR(10),
    DOB DATE,
    Address NVARCHAR(255),
    MaritalStatus NVARCHAR(20), 
    Hobbies NVARCHAR(255),
    Likes NVARCHAR(255),
    Dislikes NVARCHAR(255),
    Cuisines NVARCHAR(255),
    Sports NVARCHAR(255),
	Qualification NVARCHAR(255),
    School NVARCHAR(255),
    College NVARCHAR(255),
    WorkStatus NVARCHAR(50),
    Organization NVARCHAR(255),
    Designation NVARCHAR(255),
    VerificationCode NVARCHAR(10),
    IsVerified BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()


);

select * from Users;

    
