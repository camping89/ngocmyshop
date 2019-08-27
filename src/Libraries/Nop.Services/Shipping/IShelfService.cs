﻿using Nop.Core;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;

namespace Nop.Services.Shipping
{
    public interface IShelfService
    {
        void DeleteShelf(int shelfId);

        IPagedList<Shelf> GetAllShelf(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool? shelfOrderItemIsActive = true,
            bool isShelfEmpty = false, bool isEmptyAssignedShelf = false,
            bool? isCustomerNotified = null,
            string shelfCode = null, int? shelfNoteId = null,
            bool? isPackageItemProcessedDatetime = null, bool inActive = false);

        IPagedList<Shelf> GetAllShelfByStore(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool? shelfOrderItemIsActive = true,
            bool isShelfEmpty = false, bool isEmptyAssignedShelf = false,
            bool? isCustomerNotified = null,
            string shelfCode = null, int? shelfNoteId = null,
            bool? isPackageItemProcessedDatetime = null);

        List<Shelf> GetAllShelfAvailable(int customerId = 0, string shelfCode = null);
        void UpdateShelf(Shelf shelf);
        void UpdateShelfs(IEnumerable<Shelf> shelfs);
        void InsertShelf(Shelf shelf);
        Shelf GetShelfById(int id);
        Shelf GetShelfByCode(string shelfCode);
        void DeleteShelfOrderItem(int shelfOrderItemId);
        IPagedList<ShelfOrderItem> GetAllShelfOrderItem(int shelfId = 0, int customerId = 0, DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool? shelfOrderItemIsActive = null);

        List<int> GetOrderItemIdsByShelf(int shelfId, bool? shelfOrderItemIsActive = null);

        void UpdateShelfOrderItem(ShelfOrderItem shelfOrderItem);
        void InsertShelfOrderItem(ShelfOrderItem shelfOrderItem);
        ShelfOrderItem GetShelfOrderItemById(int id);
        ShelfOrderItem GetShelfOrderItemByOrderItemId(int orderItemId);

    }
}
