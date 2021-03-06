--query sai ngan

SELECT 
		s.Id AS 'ShelfId',
	   shelfOwner.Phone AS 'Sđt chủ ngăn' ,shelfOwner.FullName AS 'Họ tên chủ ngăn',
       LOWER(s.ShelfCode) AS 'Mã ngăn', 
	   o.id as 'Mã đơn', oi.Id AS 'Mã món',
       itemOwner.Phone AS 'Sđt chủ món', itemOwner.FullName AS 'Họ tên chủ món',
	   (SELECT TOP 1 itemOwnerShelf.ShelfCode FROM dbo.Shelf itemOwnerShelf WHERE itemOwnerShelf.CustomerId = itemOwner.Id) AS ItemOwnerCurrentShelf,
	   shipment.Id as 'Mã lệnh ship'

	   --DISTINCT shipment.Id
FROM  dbo.OrderItem oi    
    JOIN dbo.[Order] o        ON o.Id = oi.OrderId
    JOIN dbo.Shelf s        ON s.Id = oi.ShelfId
    JOIN dbo.Customer itemOwner        ON itemOwner.Id = o.CustomerId
    JOIN dbo.Customer shelfOwner        ON shelfOwner.Id = s.CustomerId
	LEFT JOIN dbo.ShipmentManualItem shipmentItem ON shipmentItem.OrderItemId = oi.Id
	LEFT JOIN dbo.ShipmentManual shipment ON shipment.Id = shipmentItem.ShipmentManualId
WHERE itemOwner.Id != shelfOwner.Id -- (chủ món khác chủ ngăn)
      AND o.Deleted = 0
	  AND oi.DeliveryDateUtc is NULL -- món chưa giao (active)
	  --AND s.shelfcode = '1a408'
	  --AND shipment.DeliveryDateUtc is NULL
ORDER BY  ItemOwnerCurrentShelf DESC, s.ShelfCode DESC;
