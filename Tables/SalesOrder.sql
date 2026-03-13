CREATE TABLE SalesOrder (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT FOREIGN KEY REFERENCES Customer(customer_id),
    employee_id INT FOREIGN KEY REFERENCES Employee(employee_id),
    order_date DATETIME DEFAULT GETDATE(),
    total_amount DECIMAL(10,2)
);