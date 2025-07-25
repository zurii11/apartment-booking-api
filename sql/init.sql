CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(25) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Apartments (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    PricePerNight DECIMAL(10, 0),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Bookings (
    Id INT IDENTITY PRIMARY KEY,
    ApartmentId INT NOT NULL,
    UserId INT,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Bookings_Apartments FOREIGN KEY (ApartmentId) REFERENCES Apartments(Id),
    CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT CK_Bookings_Dates CHECK (StartDate < EndDate)
);

CREATE INDEX IX_Bookings_Apartment_Date
ON Bookings (ApartmentId, StartDate, EndDate);

INSERT INTO Apartments (Name, Description, PricePerNight)
VALUES
('City Studio', 'Compact studio in the city center.', 55.00),
('Luxury Loft', 'Spacious loft with modern amenities.', 100.00),
('Budget Room', 'Affordable room near public transport.', 25.00),
('Mountain Retreat', 'Cabin with a mountain view and fireplace.', 145.00),
('Seaside Apartment', 'Cozy 2-bedroom near the beach.', 120.00);
