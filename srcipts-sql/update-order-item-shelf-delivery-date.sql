
SELECT 
	--DISTINCT oi.id
	oi.Id as OrderItemId, 
	oi.DeliveryDateUtc as OrderItemDeliveryDate,
	sm.Id AS ShipmentId,
	sm.DeliveryDateUtc as ShipmentDeliveryDate
FROM dbo.OrderItem oi
 JOIN dbo.ShipmentManualItem smi ON smi.OrderItemId = oi.Id
 JOIN dbo.ShipmentManual sm ON sm.Id = smi.ShipmentManualId
WHERE oi.DeliveryDateUtc IS NOT NULL AND sm.DeliveryDateUtc IS NULL

UPDATE ShipmentManual
SET DeliveryDateUtc = oi.DeliveryDateUtc
FROM dbo.OrderItem oi
 JOIN dbo.ShipmentManualItem smi ON smi.OrderItemId = oi.Id
 JOIN dbo.ShipmentManual sm ON sm.Id = smi.ShipmentManualId
WHERE oi.DeliveryDateUtc IS NOT NULL AND sm.DeliveryDateUtc IS NULL

GO

SELECT 
	--DISTINCT oi.id
	oi.Id as OrderItemId, 
	oi.DeliveryDateUtc as OrderItemDeliveryDate,
	sm.Id AS ShipmentId,
	sm.DeliveryDateUtc as ShipmentDeliveryDate
FROM dbo.OrderItem oi
 JOIN dbo.ShipmentManualItem smi ON smi.OrderItemId = oi.Id
 JOIN dbo.ShipmentManual sm ON sm.Id = smi.ShipmentManualId
WHERE oi.DeliveryDateUtc IS NULL AND sm.DeliveryDateUtc IS NOT NULL

UPDATE OrderItem
SET DeliveryDateUtc = sm.DeliveryDateUtc
FROM dbo.OrderItem oi
 JOIN dbo.ShipmentManualItem smi ON smi.OrderItemId = oi.Id
 JOIN dbo.ShipmentManual sm ON sm.Id = smi.ShipmentManualId
WHERE oi.DeliveryDateUtc IS NULL AND sm.DeliveryDateUtc IS NOT NULL

