DECLARE @ShelfCode AS NVARCHAR(50);

DECLARE @OldShelfId AS INT;
DECLARE @NewShelfId AS INT;
DECLARE @count AS INT = 1;
DECLARE ShelfCode_Cursor CURSOR FOR
SELECT ShelfCode
FROM dbo.Shelf
WHERE InActive = 0
GROUP BY ShelfCode
HAVING COUNT(ShelfCode)  = 2;
OPEN ShelfCode_Cursor;
FETCH NEXT FROM ShelfCode_Cursor
INTO @ShelfCode;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT @ShelfCode;
    PRINT 'count:' + CONVERT(NVARCHAR(10), @count);
    IF @ShelfCode IS NOT NULL
    BEGIN
        SELECT TOP 1
               @NewShelfId = Id
        FROM dbo.Shelf s
        WHERE ShelfCode = @ShelfCode
        ORDER BY s.AssignedDate DESC;

        SELECT TOP 1
               @OldShelfId = Id
        FROM dbo.Shelf s
        WHERE ShelfCode = @ShelfCode
        ORDER BY s.AssignedDate;

        PRINT 'shelf new id :' + CONVERT(NVARCHAR(50), @NewShelfId);
        PRINT 'shelf old id :' + CONVERT(NVARCHAR(50), @OldShelfId);
        PRINT '===================';
        IF @NewShelfId > 0
           AND @OldShelfId > 0
           AND @NewShelfId != @OldShelfId
        BEGIN
            PRINT 'Update shelf';

            UPDATE dbo.OrderItem
            SET ShelfId = @NewShelfId, -- new shelfid
                ShelfOrderItemId = soi.Id,
				ShelfCode = @ShelfCode
            FROM dbo.ShelfOrderItem soi
            WHERE soi.ShelfId = @OldShelfId -- old shelfid
                  AND OrderItem.Id = soi.OrderItemId;

            UPDATE dbo.ShelfOrderItem
            SET ShelfId = @NewShelfId -- new shelfid
            WHERE ShelfId = @OldShelfId; -- old shelfid


            DELETE dbo.Shelf
            WHERE Id = @OldShelfId; -- old shelfid

        END;

    END;
    SET @count = @count + 1;
    FETCH NEXT FROM ShelfCode_Cursor
    INTO @ShelfCode;
END;
CLOSE ShelfCode_Cursor;
DEALLOCATE ShelfCode_Cursor;
GO