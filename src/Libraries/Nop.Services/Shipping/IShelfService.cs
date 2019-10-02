using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;

namespace Nop.Services.Shipping
{
    public interface IShelfService
    {
        void DeleteShelf(int shelfId);

        IPagedList<Shelf> GetShelves(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool isShelfEmpty = false,
            bool? isCustomerNotified = null,
            string shelfCode = null, int? shelfNoteId = null,
            bool isAscSortedAssignedDate = false, string customerPhone = null);

        IPagedList<Shelf> GetAllShelfByStore(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool isShelfEmpty = false, bool isEmptyAssignedShelf = false,
            bool? isCustomerNotified = null,
            string shelfCode = null, int? shelfNoteId = null,
            bool? isPackageItemProcessedDatetime = null);

        List<Shelf> GetAvailableShelf(string shelfCode = null);
        void UpdateShelf(Shelf shelf);
        void UpdateShelfves(IEnumerable<Shelf> items);
        void InsertShelf(Shelf shelf);
        Shelf GetShelfById(int id);
        Shelf GetShelfByCode(string shelfCode);
        void ClearShelfInfo(string shelfIdorShelfCode);
        IList<OrderItem> GetOrderItems(string shelfIdOrShelfCode, bool activeItem = true);

        void UpdateShelfTotalAmount(string shelfIdOrCode);
    }
}
