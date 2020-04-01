﻿USE [master];

IF EXISTS (SELECT * FROM sys.databases WHERE NAME = '@DBNAME')
    DROP DATABASE [@DBNAME];

CREATE DATABASE [@DBNAME];

USE [@DBNAME];

CREATE TABLE USER_TEST (
    ID_USER INT IDENTITY (1, 1) NOT NULL,
    COD_USER INT NOT NULL,
    USER_NAME VARCHAR (64) NOT NULL,
    COD_USER_REF BIGINT NULL,
    PASSWORD_MD5 CHAR(32) NOT NULL,
    INPUT_DATE DATETIME NOT NULL,
    USER_STATUS CHAR(1) NOT NULL,
    PRIMARY KEY(ID_USER)
);

INSERT INTO USER_TEST (COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS)
    VALUES(44, 'First User Used For Tests', 1234567890123456, HASHBYTES('MD5', 'password123'), '2020-03-29 18:03:12', 'A');
