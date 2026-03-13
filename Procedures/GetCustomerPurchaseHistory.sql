--GET CUSTOMER PURCHASE HISTORY
CREATE PROCEDURE GetCustomerOrders
    @CustomerID INT
AS
BEGIN
    SELECT s.order_id, s.total_amount, s.order_date,
           p.product_name, d.quantity, d.item_price
    FROM SalesOrder s
    JOIN SalesOrderDetails d ON s.order_id = d.order_id
    JOIN Product p ON d.product_id = p.product_id
    WHERE s.customer_id = @CustomerID;
END;