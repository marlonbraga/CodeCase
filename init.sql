CREATE DATABASE MyDatabase;

USE MyDatabase;

CREATE TABLE HardSkills (
    HardSkillId INT PRIMARY KEY,
    HardSkillName NVARCHAR(50),
    TempoDeExperiencia DATE
);

CREATE TABLE SoftSkills (
    SoftSkillId INT PRIMARY KEY,
    SoftSkillName NVARCHAR(50)
);

--INSERT INTO HardSkills VALUES ('C-Sharp', '2015-01-01');
--INSERT INTO HardSkills VALUES ('.NET 8+', '2015-01-01');