using Nop.Core;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;

namespace Nop.Services.Shipping
{
    public interface IShelfService
    {
        void DeleteShelf(int shelfId);
        IPagedList<Shelf> GetAllShelf(int customerId = 0, DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool shelfIsEmpty = false);

        List<Shelf> GetAllShelfAvailable(int customerId = 0);
        void UpdateShelf(Shelf shelf);
        void InsertShelf(Shelf shelf);
        Shelf GetShelfById(int id);
        void DeleteShelfOrderItem(int shelfOrderItemId);
        IPagedList<ShelfOrderItem> GetAllShelfOrderItem(int shelfId = 0, int customerId = 0, DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool? shelfOrderItemIsActive = null);

        void UpdateShelfOrderItem(ShelfOrderItem shelfOrderItem);
        void InsertShelfOrderItem(ShelfOrderItem shelfOrderItem);
        ShelfOrderItem GetShelfOrderItemById(int id);
        ShelfOrderItem GetShelfOrderItemByOrderItemId(int orderItemId);

    }
}
