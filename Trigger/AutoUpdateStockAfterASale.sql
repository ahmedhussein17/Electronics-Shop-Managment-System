CREATE TRIGGER trg_UpdateStock
ON SalesOrderDetails
AFTER INSERT
AS
BEGIN
    UPDATE p
    SET p.stock_quantity = p.stock_quantity - i.quantity
    FROM Product p
    INNER JOIN inserted i ON p.product_id = i.product_id;
END;