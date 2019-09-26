DELETE dbo.ShelfOrderItem
GO
UPDATE dbo.Shelf SET CustomerId = NULL, AssignedDate = NULL, ShippedDate = NULL, UpdatedNoteDate = NULL,ShelfNoteId = 0,IsCustomerNotified = 0;
GO
DELETE dbo.ShoppingCartItem
GO
DELETE dbo.ShipmentManualItem
GO
DELETE dbo.ShipmentManual
GO
DELETE dbo.OrderItem
GO
DELETE dbo.[Order]
GO
DELETE dbo.Product_SpecificationAttribute_Mapping
GO
DELETE dbo.Product_ProductTag_Mapping
GO
DELETE dbo.Product_ProductAttribute_Mapping
GO
DELETE dbo.Product_Picture_Mapping
GO
DELETE dbo.Picture
GO 
DELETE dbo.Product_Manufacturer_Mapping
GO
DELETE dbo.Product_Category_Mapping
GO
DELETE dbo.Product
GO
DELETE dbo.CustomerAttributeValue
GO
DELETE dbo.CustomerAttribute
GO
DELETE dbo.CustomerAddresses  WHERE	(SELECT COUNT(*) FROM	 dbo.Customer_CustomerRole_Mapping crm WHERE crm.Customer_Id = CustomerAddresses.Customer_Id AND crm.CustomerRole_Id = 1  ) = 0 
go
DELETE dbo.Customer WHERE	(SELECT COUNT(*) FROM	 dbo.Customer_CustomerRole_Mapping crm WHERE crm.Customer_Id = Customer.Id AND crm.CustomerRole_Id = 1  ) = 0 

