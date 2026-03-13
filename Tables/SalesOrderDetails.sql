CREATE TABLE SalesOrderDetails (
    order_id INT NOT NULL FOREIGN KEY REFERENCES SalesOrder(order_id),
    product_id INT NOT NULL FOREIGN KEY REFERENCES Product(product_id),
    quantity INT NOT NULL,
    item_price DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (order_id, product_id)
);