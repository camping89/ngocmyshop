﻿using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Shipping
{
    public class ShelfService : IShelfService
    {
        private readonly IRepository<Shelf> _shelfRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<ShipmentManual> _shipmentRepository;
        public ShelfService(IRepository<Shelf> shelfRepository, IRepository<ShipmentManual> shipmentRepository, IRepository<OrderItem> orderItemRepository)
        {
            _shelfRepository = shelfRepository;
            _shipmentRepository = shipmentRepository;
            _orderItemRepository = orderItemRepository;
        }

        public void DeleteShelf(int shelfId)
        {
            var shelf = _shelfRepository.GetById(shelfId);
            if (shelf != null)
            {
                _shelfRepository.Delete(shelf);
            }
        }

        public IPagedList<Shelf> GetShelves(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool isShelfEmpty = false,
            bool? isCustomerNotified = null, string shelfCode = null,
            int? shelfNoteId = null, bool isAscSortedAssignedDate = false, string customerPhone = null)
        {
            var query = _shelfRepository.Table;

            if (isShelfEmpty)
            {
                query = query.Where(_ => _.OrderItems == null || _.OrderItems.All(oi => oi.DeliveryDateUtc != null)).OrderBy(_ => _.ShelfCode);
                return new PagedList<Shelf>(query, pageIndex, pageSize);
            }


            if (shelfCode.IsNotNullOrEmpty())
            {
                shelfCode = shelfCode.TrimStart().TrimEnd().Trim().ToLowerInvariant();
                query = query.Where(_ => _.ShelfCode.ToLower().Contains(shelfCode));
            }

            if (customerId > 0)
            {
                query = query.Where(_ => _.CustomerId == customerId);
            }
            if (customerPhone.IsNotNullOrEmpty())
            {
                query = query.Where(_ => _.Customer.Phone.EndsWith(customerPhone.Trim()));
            }

            if (assignedFromUtc != null)
            {
                query = query.Where(_ => _.AssignedDate != null && _.AssignedDate >= assignedFromUtc);
            }

            if (assignedToUtc != null)
            {
                query = query.Where(_ => _.AssignedDate != null && _.AssignedDate <= assignedToUtc);
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
            
            if (assignedOrderItemFromUtc != null && assignedOrderItemToUtc != null)
            {
                query = query.Where(shelf => shelf.OrderItems!=null && shelf.OrderItems.Any(oi => 
                                                 (oi.ShelfAssignedDate >= assignedOrderItemFromUtc && oi.ShelfAssignedDate <= assignedOrderItemToUtc)
                                                && oi.DeliveryDateUtc !=null));
            }
            
            query = isAscSortedAssignedDate ? query.Where(_ => _.AssignedDate != null).OrderBy(_ => _.AssignedDate) : query.OrderBy(_ => _.ShelfCode);


            var shelfList = new PagedList<Shelf>(query, pageIndex, pageSize);
            return shelfList;
        }

        public IPagedList<Shelf> GetAllShelfByStore(int customerId = 0,
            DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
            DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
            DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool isShelfEmpty = false, bool isEmptyAssignedShelf = false,
            bool? isCustomerNotified = null, string shelfCode = null,
            int? shelfNoteId = null, bool? isPackageItemProcessedDatetime = null)
        {

            var shelfList = new PagedList<Shelf>(new List<Shelf>(), pageIndex, pageSize);
            return shelfList;
        }

        public List<Shelf> GetAvailableShelf(string shelfCode = null)
        {
            var availableShelf= _shelfRepository.Table.Where(_ => _.OrderItems == null || _.OrderItems.All(oi => oi.DeliveryDateUtc != null));
            
            if (shelfCode.IsNotNullOrEmpty())
            {
                shelfCode = shelfCode.TrimStart().TrimEnd().Trim().ToLowerInvariant();
                availableShelf = availableShelf.Where(_ => _.ShelfCode.ToLower().Contains(shelfCode));
            }

            return availableShelf.OrderBy(_ => _.ShelfCode).ToList();
        }

        public void UpdateShelf(Shelf shelf)
        {
            if (shelf != null)
            {
                _shelfRepository.Update(shelf);
            }
        }

        public void UpdateShelfves(IEnumerable<Shelf> items)
        {
            if (items != null)
            {
                _shelfRepository.Update(items);
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
        

        public IList<OrderItem> GetOrderItems(int shelfId, bool activeItem = true)
        {
            var shelf = _shelfRepository.Table.FirstOrDefault(_ => _.Id == shelfId);
            var orderItems = new List<OrderItem>();
            if (shelf != null)
            {
                if (activeItem)
                {
                    orderItems = shelf.OrderItems.Where(_ => _.DeliveryDateUtc != null).OrderByDescending(oi=>oi.ShelfAssignedDate).ToList();
                }

            }

            return orderItems;
        }
    }
}
