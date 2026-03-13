CREATE TABLE Product (
product_id INT IDENTITY(1,1) PRIMARY KEY,
product_name VARCHAR(100) NOT NULL,
category_id INT FOREIGN KEY REFERENCES Category(category_id),
brand VARCHAR(50),
price DECIMAL(10,2) NOT NULL,
stock_quantity INT NOT NULL DEFAULT 0
);