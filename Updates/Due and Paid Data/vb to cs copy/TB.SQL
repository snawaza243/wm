CREATE TABLE MappedData (
    FileType VARCHAR(50),
    MaskedStr VARCHAR(500),
    CreatedTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedUser VARCHAR(50),
    LoginUser VARCHAR(50),
    ModifiedTime DATETIME,
    Action INT IDENTITY(1,1) PRIMARY KEY
);