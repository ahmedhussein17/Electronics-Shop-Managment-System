--UPDATE THE PRODUCT STOCK
CREATE PROCEDURE IncreaseProductStock
  @ProductID INT,
  @QuantityAdded INT
AS
BEGIN
  Update Product
  SET stock_quantity = stock_quantity + @QuantityAdded
  Where product_id = @ProductID;
  END;