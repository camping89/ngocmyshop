using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Shipping
{
    public class ShelfService : IShelfService
    {
        private readonly IRepository<Shelf> _shelfRepository;
        private readonly IRepository<ShelfOrderItem> _shelfOrderItemRepository;

        public ShelfService(IRepository<Shelf> shelfRepository, IRepository<ShelfOrderItem> shelfOrderItemRepository)
        {
            _shelfRepository = shelfRepository;
            _shelfOrderItemRepository = shelfOrderItemRepository;
        }

        public void DeleteShelf(int shelfId)
        {
            var shelf = _shelfRepository.GetById(shelfId);
            if (shelf != null)
            {
                _shelfRepository.Delete(shelf);
            }
        }

        public IPagedList<Shelf> GetAllShelf(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool? shelfOrderItemIsActive = true,
            bool isShelfEmpty = false, bool isEmptyAssignedShelf = false,
            bool? isCustomerNotified = null, string shelfCode = null,
            int? shelfNoteId = null, bool? isPackageItemProcessedDatetime = null)
        {
            var query = _shelfRepository.Table;

            if (string.IsNullOrEmpty(shelfCode) == false)
            {
                shelfCode = shelfCode.TrimStart().TrimEnd().Trim().ToLowerInvariant();
                query = query.Where(_ => _.ShelfCode.ToLower().Contains(shelfCode));
            }

            if (customerId > 0)
            {
                query = query.Where(_ => _.CustomerId == customerId);
            }

            if (assignedFromUtc != null)
            {
                query = query.Where(_ => _.AssignedDate != null && _.AssignedDate >= assignedFromUtc);
            }

            if (assignedToUtc != null)
            {
                query = query.Where(_ => _.AssignedDate != null && _.AssignedDate <= assignedToUtc);
            }


            if (assignedOrderItemFromUtc != null && assignedOrderItemToUtc != null)
            {
                var subQuery = _shelfOrderItemRepository.Table;
                var shelfIds = subQuery.Where(_ => _.AssignedDate >= assignedOrderItemFromUtc && _.AssignedDate <= assignedOrderItemToUtc).Select(s => s.ShelfId).Distinct().ToList();
                query = query.Where(_ => shelfIds.Contains(_.Id));
            }

            if (shippedFromUtc != null)
            {
                query = query.Where(_ => _.ShippedDate != null && _.ShippedDate >= shippedFromUtc);
            }

            if (shippedToUtc != null)
            {
                query = query.Where(_ => _.ShippedDate != null && _.ShippedDate <= shippedToUtc);
            }

            if (isCustomerNotified != null)
            {
                query = query.Where(_ => _.IsCustomerNotified == isCustomerNotified);
            }

            if (shelfNoteId != null)
            {
                query = query.Where(_ => _.ShelfNoteId == shelfNoteId);
            }

            if (isPackageItemProcessedDatetime != null)
            {
                if (isPackageItemProcessedDatetime.Value)
                {
                    var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.OrderItem.PackageItemProcessedDatetime != null).Select(_ => _.ShelfId).Distinct().ToList();
                    query = query.Where(_ => shelfOrderItems.Contains(_.Id));
                }
                else
                {
                    var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.OrderItem.PackageItemProcessedDatetime == null).Select(_ => _.ShelfId).Distinct().ToList();
                    query = query.Where(_ => shelfOrderItems.Contains(_.Id));
                }
            }

            if (shelfOrderItemIsActive != null && shelfOrderItemIsActive == true)
            {
                var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.IsActived).Select(_ => _.ShelfId).Distinct().ToList();
                query = query.Where(_ => shelfOrderItems.Contains(_.Id));
            }
            else if (shelfOrderItemIsActive != null && shelfOrderItemIsActive == false)
            {
                var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.IsActived == false).Select(_ => _.ShelfId).Distinct().ToList();
                query = query.Where(_ => shelfOrderItems.Contains(_.Id));
            }


            if (isShelfEmpty)
            {
                var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.IsActived).Select(_ => _.ShelfId).Distinct().ToList();
                query = query.Where(_ => _.CustomerId == null || _.CustomerId == 0 || shelfOrderItems.Contains(_.Id) == false);
            }

            if (isEmptyAssignedShelf)
            {
                query = query.Where(_ => _.ShelfOrderItems.Count == 0 && _.CustomerId != null && _.CustomerId > 0);
            }
            query = query.OrderBy(_ => _.ShelfCode);
            var shelfList = new PagedList<Shelf>(query, pageIndex, pageSize);
            return shelfList;
        }

        public List<Shelf> GetAllShelfAvailable(int customerId = 0, string shelfCode = null)
        {
            var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.IsActived && (customerId == 0 || s.CustomerId != customerId)).Select(_ => _.ShelfId).Distinct().ToList();
            var query = _shelfRepository.Table;
            if (string.IsNullOrEmpty(shelfCode) == false)
            {
                shelfCode = shelfCode.TrimStart().TrimEnd().Trim().ToLowerInvariant();
                query = query.Where(_ => _.ShelfCode.ToLower().Contains(shelfCode));
            }
            query = query.Where(_ => shelfOrderItems.Contains(_.Id) == false);
            return query.OrderBy(_ => _.ShelfCode).ToList();
        }
        public void UpdateShelf(Shelf shelf)
        {
            if (shelf != null)
            {
                _shelfRepository.Update(shelf);
            }
        }

        public void InsertShelf(Shelf shelf)
        {
            if (shelf != null)
            {
                _shelfRepository.Insert(shelf);
            }
        }

        public Shelf GetShelfById(int id)
        {
            return _shelfRepository.GetById(id);
        }

        public Shelf GetShelfByCode(string shelfCode)
        {
            var query = _shelfRepository.Table;
            query = query.Where(_ => _.ShelfCode.Contains(shelfCode));
            return query.FirstOrDefault();
        }

        public void DeleteShelfOrderItem(int shelfOrderItemId)
        {
            var shelfOrderItem = _shelfOrderItemRepository.GetById(shelfOrderItemId);
            if (shelfOrderItem != null)
            {
                _shelfOrderItemRepository.Delete(shelfOrderItem);
            }
        }

        public IPagedList<ShelfOrderItem> GetAllShelfOrderItem(int shelfId = 0, int customerId = 0, DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null, int pageIndex = 0, int pageSize = Int32.MaxValue, bool? shelfOrderItemIsActive = null)
        {
            var query = _shelfOrderItemRepository.Table;
            if (shelfId > 0)
            {
                query = query.Where(_ => _.ShelfId == shelfId);
            }

            if (customerId > 0)
            {
                query = query.Where(_ => _.CustomerId == customerId);
            }

            if (assignedFromUtc != null)
            {
                query = query.Where(_ => _.AssignedDate >= assignedFromUtc);
            }

            if (assignedToUtc != null)
            {
                query = query.Where(_ => _.AssignedDate <= assignedToUtc);
            }

            if (shelfOrderItemIsActive.HasValue)
            {
                query = query.Where(_ => _.IsActived == shelfOrderItemIsActive);
            }

            query = query.OrderByDescending(_ => _.AssignedDate);
            var shelfOrderItems = new PagedList<ShelfOrderItem>(query, pageIndex, pageSize);
            return shelfOrderItems;
        }

        public void UpdateShelfOrderItem(ShelfOrderItem shelfOrderItem)
        {
            if (shelfOrderItem != null)
            {
                _shelfOrderItemRepository.Update(shelfOrderItem);
            }
        }

        public void InsertShelfOrderItem(ShelfOrderItem shelfOrderItem)
        {
            if (shelfOrderItem != null)
            {
                _shelfOrderItemRepository.Insert(shelfOrderItem);
            }
        }

        public ShelfOrderItem GetShelfOrderItemById(int id)
        {
            return _shelfOrderItemRepository.GetById(id);
        }
        public ShelfOrderItem GetShelfOrderItemByOrderItemId(int orderItemId)
        {
            var query = _shelfOrderItemRepository.Table;
            if (orderItemId > 0)
            {
                return query.FirstOrDefault(_ => _.OrderItemId == orderItemId);
            }

            return null;
        }
    }
}
