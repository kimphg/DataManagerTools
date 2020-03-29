CREATE DATABASE SEAMAP
GO

USE SEAMAP

CREATE TABLE SHIP(
	MMSI VARCHAR(15) PRIMARY KEY,
	VSNM NVARCHAR(50),
	TYPE INT,
	CLASS CHAR(2)
)
GO

CREATE TABLE SHIPJOURNEY(
	MMSI VARCHAR(15),
	LAT FLOAT,
	LNG FLOAT,
	SOG CHAR(20),
	COG CHAR(20),
	TIME VARCHAR(20) 
	PRIMARY KEY(MMSI,TIME)
)
GO

DROP TABLE SHIPJOURNEY
DROP TABLE SHIP



use seamap
select * from Ship 
select * from ShipJourney