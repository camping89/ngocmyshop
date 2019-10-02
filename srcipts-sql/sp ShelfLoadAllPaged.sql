GO

/****** Object:  StoredProcedure [dbo].[CategoryLoadAllPaged]    Script Date: 7/5/2019 1:51:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[ShelfAllPaged]
(
	@CustomerId INT = 0,
	@AssignedFromUtc DATETIME NULL = NULL,
	@AssignedToUtc DATETIME NULL = NULL,
	@AssignedOrderItemFromUtc DATETIME NULL = null,
	@AssignedOrderItemToUtc DATETIME NULL = null,
	@ShippedFromUtc DATETIME NULL = NULL,
	@ShippedToUtc DATETIME NULL = NULL,
	@ShelfOrderItemIsActive BIT NULL = 1,
	@IsShelfEmpty BIT = 0,
	@IsEmptyAssignedShelf BIT = 0,
	@IsCustomerNotified BIT NULL = NULL,
	@ShelfCode NVARCHAR(50) = '',
	@ShelfNoteId INT NULL = NULL,
	@IsPackageItemProcessedDatetime BIT	 NULL = NULL,
    @PageIndex			INT = 0,
	@PageSize			INT = 2147483644,
    @TotalRecords		INT = NULL OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @sql NVARCHAR(MAX) = '';
    DECLARE @sqlShelfOrderItem NVARCHAR(MAX) = '';
	DECLARE @Quote NVARCHAR(5) = '';
	SET @ShelfCode = '%' + @ShelfCode + '%'
	--ordered categories
    CREATE TABLE #ShelfIds
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ShelfId] int NOT NULL
	)
	SET @sql = '
		INSERT INTO #ShelfIds ([ShelfId])
		SELECT s.Id
		FROM Shelf s with (NOLOCK)
		WHERE s.[ShelfCode] IS NOT NULL '

    IF(@ShelfCode IS NOT NULL)
	BEGIN
		
		SET @sql = @sql + ' AND PATINDEX(@ShelfCode, s.[ShelfCode]) > 0 '
	END

	 IF(@CustomerId > 0)
	BEGIN
		SET @sql = @sql + ' AND s.CustomerId = @CustomerId '
	END
	
	 IF(@AssignedFromUtc IS NOT NULL)
	BEGIN
		SET @sql = @sql + ' AND (s.AssignedDate IS NOT NULL AND s.AssignedDate >= @AssignedFromUtc) '
	END
	
	 IF(@AssignedToUtc IS NOT NULL)
	BEGIN
		SET @sql = @sql + ' AND (s.AssignedDate IS NOT NULL AND s.AssignedDate <= @AssignedToUtc) '
	END
	
	 IF(@IsCustomerNotified IS NOT NULL)
	BEGIN
		SET @sql = @sql + ' AND s.IsCustomerNotified = @IsCustomerNotified '
	END
	
	 IF(@ShelfNoteId IS NOT NULL)
	BEGIN
		SET @sql = @sql + ' AND s.ShelfNoteId = @ShelfNoteId '
	END

	EXEC sp_executesql @sql, N'@CustomerId INT,
	@AssignedFromUtc DATETIME NULL,
	@AssignedToUtc DATETIME NULL,
	@AssignedOrderItemFromUtc DATETIME NULL,
	@AssignedOrderItemToUtc DATETIME NULL,
	@ShippedFromUtc DATETIME NULL,
	@ShippedToUtc DATETIME NULL,
	@ShelfOrderItemIsActive BIT NULL,
	@IsShelfEmpty BIT,
	@IsEmptyAssignedShelf BIT,
	@IsCustomerNotified BIT NULL ,
	@ShelfCode NVARCHAR(50),
	@ShelfNoteId INT NULL,
	@IsPackageItemProcessedDatetime BIT	NULL',
	@CustomerId,
	@AssignedFromUtc,
	@AssignedToUtc,
	@AssignedOrderItemFromUtc ,
	@AssignedOrderItemToUtc,
	@ShippedFromUtc,
	@ShippedToUtc,
	@ShelfOrderItemIsActive,
	@IsShelfEmpty,
	@IsEmptyAssignedShelf,
	@IsCustomerNotified,
	@ShelfCode,
	@ShelfNoteId,
	@IsPackageItemProcessedDatetime

	 SET @TotalRecords = @@ROWCOUNT


	SET @sqlShelfOrderItem = 'SELECT DISTINCT ShelfId FROM dbo.ShelfOrderItem soi with (NOLOCK)  '
	SET @sqlShelfOrderItem += ' INNER JOIN OrderItem oi with (NOLOCK) ON soi.OrderItemId = soi.OrderItemId '

	IF(@IsPackageItemProcessedDatetime IS NOT NULL)
	BEGIN
		IF (@IsPackageItemProcessedDatetime = 1)
		BEGIN
			
		END
	END

    --total records
   
    --paging
    SELECT s.* FROM #ShelfIds AS [Result] INNER JOIN [Shelf] s ON [Result].[ShelfId] = s.[Id]
	LEFT JOIN dbo.Customer c ON s.CustomerId IS NULL OR s.CustomerId = c.Id
    WHERE ([Result].[Id] > @PageSize * @PageIndex AND [Result].[Id] <= @PageSize * (@PageIndex + 1))
    ORDER BY [Result].[Id]

    DROP TABLE #ShelfIds
END
GO

