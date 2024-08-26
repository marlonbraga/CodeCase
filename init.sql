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
