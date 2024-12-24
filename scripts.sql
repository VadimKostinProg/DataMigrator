CREATE DATABASE TripsDB;

USE TripsDB;

CREATE TABLE Trips (
    tpep_pickup_datetime DATETIME NULL,  
    tpep_dropoff_datetime DATETIME NULL, 
    passenger_count INT NULL,            
    trip_distance FLOAT NULL,            
    store_and_fwd_flag VARCHAR(3) NULL,  
    PULocationID INT NULL,               
    DOLocationID INT NULL,               
    fare_amount DECIMAL(18, 2) NULL,     
    tip_amount DECIMAL(18, 2) NULL       
);

-- Find out which `PULocationId` (Pick-up location ID) has the highest tip_amount on average.
CREATE INDEX idx_PULocationID_TipAmount ON Trips (PULocationID, tip_amount);

SELECT TOP(1) PULocationID, AVG(tip_amount) AS AverageTipAmount
FROM Trips
GROUP BY PULocationID
ORDER BY AverageTipAmount DESC;

-- Find the top 100 longest fares in terms of `trip_distance`.
CREATE INDEX idx_TripDistance ON Trips (trip_distance DESC);

SELECT TOP 100 *
FROM Trips
ORDER BY trip_distance DESC;

-- Find the top 100 longest fares in terms of time spent traveling.
CREATE CLUSTERED INDEX idx_TripTime ON Trips (tpep_pickup_datetime, tpep_dropoff_datetime);

SELECT TOP 100 *, DATEDIFF(MINUTE, tpep_pickup_datetime, tpep_dropoff_datetime) AS TravelTimeMinutes
FROM Trips
ORDER BY TravelTimeMinutes DESC;

-- Search, where part of the conditions is `PULocationId`.
CREATE INDEX idx_PULocationID ON Trips (PULocationID);

SELECT *
FROM Trips
WHERE PULocationID =262;

-- To detect duplicates by the combination of tpep_pickup_datetime, tpep_dropoff_datetime, passenger_count
CREATE UNIQUE INDEX idx_unique_trip_datetime_passenger_count
ON Trips (tpep_pickup_datetime, tpep_dropoff_datetime, passenger_count);