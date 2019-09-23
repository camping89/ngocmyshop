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

WHILE @cnt <= 3
BEGIN
	DECLARE @i INT = 0;
	DECLARE @count int = 0;
	DECLARE @letterItem NCHAR(1);
	declare @code nvarchar(3);
	DECLARE @j INT = 100;
	declare @e int = 0;
   SELECT @count=  Count(*) FROM #LetterTmp
		WHILE @i <= @count
		BEGIN

			SELECT @letterItem = Letter FROM #LetterTmp
			ORDER BY Letter
			OFFSET @i ROWS   
			FETCH NEXT 1 ROWS ONLY  
			SET @i = @i + 1;
			set @j = 100;
			while @j <= 900
			begin 
				set @e = 0;
				WHILE @e <= 99
				BEGIN
					set @code = @j + @e;
					IF NOT EXISTS(SELECT * FROM dbo.Shelf WHERE ShelfCode = CONCAT(@cnt,@letterItem,@code))
					BEGIN
						
						PRINT(CONCAT(@cnt,@letterItem,@code));
							INSERT INTO dbo.Shelf
							(
								ShelfCode,
								CustomerId,
								AssignedDate,
								ShippedDate,
								IsCustomerNotified,
								ShelfNoteId,
								UpdatedNoteDate,
								Total,
								HasOrderItem,
								TotalWithoutDeposit,
								InActive
							)
							VALUES
							(   CONCAT(@cnt,@letterItem,@code),       -- ShelfCode - nvarchar(50)
								null,         -- CustomerId - int
								null, -- AssignedDate - datetime
								NULL, -- ShippedDate - datetime
								0,      -- IsCustomerNotified - bit
								0,         -- ShelfNoteId - int
								NULL,  -- UpdatedNoteDate - datetime
								0, -- total
								0, -- hasorderitem
								0, --total deposit
								0 -- inactive
								)
				   END
						
					SET @e = @e + 1;
				END
				set @j = @j + 100;
			end
		END
   SET @cnt = @cnt + 1;
END;
go
DROP TABLE #LetterTmp;
