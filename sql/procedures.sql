CREATE PROCEDURE RegisterUser
    @Username NVARCHAR(25),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
        THROW 50000, 'User already exists.', 1;

    INSERT INTO Users (Username, PasswordHash)
    VALUES (@Username, @PasswordHash);
END

-- SPLIT

CREATE PROCEDURE GetUserByUsername
    @Username NVARCHAR(25)
AS
BEGIN
    SELECT Id, PasswordHash FROM Users WHERE Username = @Username;
END

-- SPLIT

CREATE PROCEDURE ListAvailableApartments
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF @StartDate >= @EndDate
        THROW 50003, 'Start date must be before end date', 1;

    SELECT a.Id, a.Name, a.Description, a.PricePerNight
    FROM Apartments a
    WHERE NOT EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.ApartmentId = a.Id
        AND b.StartDate < @EndDate
        AND b.EndDate > @StartDate
    );
END

-- SPLIT

CREATE PROCEDURE CreateBooking
    @ApartmentId INT,
    @UserId INT = NULL,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    IF @StartDate >= @EndDate
        THROW 50003, 'Start date must be before end date', 1;

    IF EXISTS (
        SELECT 1
        FROM Bookings WITH (UPDLOCK, HOLDLOCK) -- Acquires an update lock for the whole transaction 
        WHERE ApartmentId = @ApartmentId
        AND StartDate < @EndDate
        AND EndDate > @StartDate
    )
    BEGIN
        ROLLBACK;
        THROW 50001, 'Apartment is booked in that date range', 1;
    END

    INSERT INTO Bookings (ApartmentId, UserId, StartDate, EndDate)
    VALUES (@ApartmentId, @UserId, @StartDate, @EndDate);

    COMMIT;
END

-- SPLIT

CREATE PROCEDURE DeleteBooking
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    IF NOT EXISTS (
        SELECT 1
        FROM Bookings WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @BookingId
    )
    BEGIN
        ROLLBACK;
        THROW 50002, 'Booking not found.', 1;
    END

    DELETE FROM Bookings WHERE Id = @BookingId;

    COMMIT;
END

-- SPLIT

CREATE PROCEDURE CheckApartmentAvailability
    @ApartmentId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF @StartDate >= @EndDate
        THROW 50003, 'Start date must be before end date', 1;

    IF EXISTS (
        SELECT 1
        FROM Bookings 
        WHERE Id = @ApartmentId
        AND StartDate < @EndDate
        AND EndDate > @StartDate
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS IsAvailable;
    END
    ELSE
    BEGIN
        SELECT CAST(1 AS BIT) AS IsAvailable;
    END
END
