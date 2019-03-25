

--DELETE dbo.ShelfOrderItem
--DELETE dbo.Shelf

DECLARE @Letters NVARCHAR(MAX) = 'A,B,C,D,E,F,G,H,I,K,L,M,N,O,P,Q,R,S,T,V,X,Y,Z';
CREATE TABLE #LetterTmp(
	Letter NCHAR(1)
);
INSERT INTO #LetterTmp (Letter)
	SELECT CAST(data as NCHAR(1)) FROM [nop_splitstring_to_table](@Letters, ',');
go
DECLARE @cnt INT = 1;

WHILE @cnt < 6
BEGIN
	DECLARE @i INT = 0;
	DECLARE @count int = 0;
	DECLARE @letterItem NCHAR(1);
   SELECT @count=  Count(*) FROM #LetterTmp
		WHILE @i <= @count
		BEGIN

			SELECT @letterItem = Letter FROM #LetterTmp
			ORDER BY Letter
			OFFSET @i ROWS   
			FETCH NEXT 1 ROWS ONLY  
			SET @i = @i + 1;
			DECLARE @j INT = 101;
			WHILE @j < 131
				BEGIN
					IF NOT EXISTS(SELECT * FROM dbo.Shelf WHERE ShelfCode = CONCAT(@cnt,@letterItem,@j))
					BEGIN
						PRINT(CONCAT(@cnt,@letterItem,@j));
							INSERT INTO dbo.Shelf
							(
								ShelfCode,
								CustomerId,
								AssignedDate,
								ShippedDate,
								IsCustomerNotified,
								ShelfNoteId,
								UpdatedNoteDate
							)
							VALUES
							(   CONCAT(@cnt,@letterItem,@j),       -- ShelfCode - nvarchar(50)
								null,         -- CustomerId - int
								null, -- AssignedDate - datetime
								NULL, -- ShippedDate - datetime
								0,      -- IsCustomerNotified - bit
								0,         -- ShelfNoteId - int
								NULL  -- UpdatedNoteDate - datetime
								)
				   END
						
					SET @j = @j + 1;
				END
		END
   SET @cnt = @cnt + 1;
END;
go
DROP TABLE #LetterTmp;
