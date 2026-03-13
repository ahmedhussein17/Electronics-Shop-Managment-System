CREATE TABLE Employee (
employee_id INT IDENTITY(1,1) PRIMARY KEY,
employee_name VARCHAR(100) NOT NULL,
role VARCHAR(50),
salary DECIMAL(10,2)
);