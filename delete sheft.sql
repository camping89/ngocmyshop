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
			DECLARE @j INT = 131;
			WHILE @j < 1000
				BEGIN
				PRINT(CONCAT(@cnt,@letterItem,@j))
					DELETE dbo.Shelf WHERE ShelfCode = CONCAT(@cnt,@letterItem,@j)
					SET @j = @j + 1;
				END
		END
   SET @cnt = @cnt + 1;
END;
go
DROP TABLE #LetterTmp;
