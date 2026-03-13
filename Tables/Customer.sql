CREATE TABLE Customer (
customer_id INT IDENTITY(1,1) PRIMARY KEY,
customer_name VARCHAR(100) NOT NULL,
phone VARCHAR(20),
email VARCHAR(100)
);