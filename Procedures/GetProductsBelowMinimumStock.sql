--GET PRODUCTS BELOW MINIMUM STOCK
CREATE PROCEDURE GetLowStockProducts
    @MinStock INT
AS
BEGIN
    SELECT product_id, product_name, stock_quantity
    FROM Product
    WHERE stock_quantity < @MinStock;
END;