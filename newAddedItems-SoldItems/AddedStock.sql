UPDATE Product SET stock_quantity = stock_quantity + 20
WHERE product_id IN (1, 3, 4, 5, 8, 10);
-- updating stock quantity