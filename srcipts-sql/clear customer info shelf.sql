--SELECT s.ShelfCode
--FROM dbo.Shelf s
--WHERE
--(
--    SELECT COUNT(*)
--    FROM dbo.OrderItem oi
--    WHERE oi.ShelfId IS NOT NULL
--          AND oi.ShelfId = s.Id
--          AND oi.DeliveryDateUtc IS NULL
--)   = 0
--AND s.CustomerId IS NOT NULL
--AND s.CustomerId > 0
--GROUP BY s.ShelfCode;


UPDATE dbo.Shelf
SET CustomerId = NULL,
    AssignedDate = NULL,
    ShippedDate = NULL,
    ShelfNoteId = 0,
    IsCustomerNotified = 0,
    Total = 0,
    HasOrderItem = 0,
    TotalWithoutDeposit = 0
WHERE ShelfCode IN (
                       SELECT s.ShelfCode
                       FROM dbo.Shelf s
                       WHERE
                       (
                           SELECT COUNT(*)
                           FROM dbo.OrderItem oi
                           WHERE oi.ShelfId IS NOT NULL
                                 AND oi.ShelfId = s.Id
                                 AND oi.DeliveryDateUtc IS NULL
                       )   = 0
                       AND s.CustomerId IS NOT NULL
                       AND s.CustomerId > 0
                       GROUP BY s.ShelfCode
                   );

