using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Nop.Services.Shipping
{
    public class ShelfService : IShelfService
    {
        private readonly IRepository<Shelf> _shelfRepository;
        private readonly IRepository<ShelfOrderItem> _shelfOrderItemRepository;
        private readonly IRepository<ShipmentManual> _shipmentRepository;
        public ShelfService(IRepository<Shelf> shelfRepository, IRepository<ShelfOrderItem> shelfOrderItemRepository, IRepository<ShipmentManual> shipmentRepository)
        {
            _shelfRepository = shelfRepository;
            _shelfOrderItemRepository = shelfOrderItemRepository;
            _shipmentRepository = shipmentRepository;
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
            int? shelfNoteId = null, bool? isPackageItemProcessedDatetime = null, bool inActive = false, bool isAscSortedAssignedDate = false, string customerPhone = null)
        {
            var query = _shelfRepository.Table.Where(_ => _.InActive == inActive);

            if (isShelfEmpty)
            {
                var hasShipmentShelf = _shipmentRepository.Table.Where(_ => _.DeliveryDateUtc == null).Select(_ => _.ShelfCode.ToLower()).Distinct();
                query = query.Where(_ =>
                        _.ShelfOrderItems.All(soi => !soi.IsActived)
                        && !hasShipmentShelf.Contains(_.ShelfCode.ToLower()))
                    .OrderBy(_ => _.ShelfCode);
                
                return new PagedList<Shelf>(query, pageIndex, pageSize);
            }


            if (string.IsNullOrEmpty(shelfCode) == false)
            {
                shelfCode = shelfCode.TrimStart().TrimEnd().Trim().ToLowerInvariant();
                query = query.Where(_ => _.ShelfCode.ToLower().Contains(shelfCode));
            }

            if (customerId > 0)
            {
                query = query.Where(_ => _.CustomerId == customerId);
            }
            if (!string.IsNullOrEmpty(customerPhone))
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

            if (isPackageItemProcessedDatetime != null)
            {
                if (isPackageItemProcessedDatetime.Value)
                {
                    //var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.OrderItem.PackageItemProcessedDatetime != null).Select(_ => _.ShelfId).Distinct().ToList();
                    //query = query.Where(_ => shelfOrderItems.Contains(_.Id));
                    query = query.Where(_ => _.ShelfOrderItems.Any(soi => soi.CustomerId == _.CustomerId && soi.OrderItem.PackageItemProcessedDatetime != null));
                }
                else
                {
                    //var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.OrderItem.PackageItemProcessedDatetime == null).Select(_ => _.ShelfId).Distinct().ToList();
                    //query = query.Where(_ => shelfOrderItems.Contains(_.Id));
                    query = query.Where(_ => _.ShelfOrderItems.Any(soi => soi.CustomerId == _.CustomerId && soi.OrderItem.PackageItemProcessedDatetime == null));
                }
            }

            // query active soi only without caring about user (to display wrong shelf placement)
            query = query.Where(_ => _.ShelfOrderItems.Any(soi => soi.IsActived));

            //if (shelfOrderItemIsActive != null && shelfOrderItemIsActive == true)
            //{
            //    //var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.IsActived).Select(_ => _.ShelfId).Distinct().ToList();
            //    //query = query.Where(_ => shelfOrderItems.Contains(_.Id));
            //    query = query.Where(_ => _.ShelfOrderItems.Any(soi => soi.IsActived && soi.CustomerId == _.CustomerId));
            //}
            //else if (shelfOrderItemIsActive != null && shelfOrderItemIsActive == false)
            //{
            //    //var shelfOrderItems = _shelfOrderItemRepository.Table.Where(s => s.IsActived == false).Select(_ => _.ShelfId).Distinct().ToList();
            //    //query = query.Where(_ => shelfOrderItems.Contains(_.Id));

            //    query = query.Where(_ => _.ShelfOrderItems.Any(soi => soi.IsActived == false && soi.CustomerId == _.CustomerId));
            //}

            if (assignedOrderItemFromUtc != null && assignedOrderItemToUtc != null)
            {
                //var subQuery = _shelfOrderItemRepository.Table;
                //var shelfIds = subQuery.Where(_ => _.AssignedDate >= assignedOrderItemFromUtc && _.AssignedDate <= assignedOrderItemToUtc).Select(s => s.ShelfId).Distinct().ToList();
                //query = query.Where(_ => shelfIds.Contains(_.Id));

                query = query.Where(_ => _.ShelfOrderItems.Any(soi => soi.CustomerId == _.CustomerId &&
                                                                      (soi.AssignedDate >= assignedOrderItemFromUtc && soi.AssignedDate <= assignedOrderItemToUtc)
                                                                      && (shelfOrderItemIsActive == null || soi.IsActived == shelfOrderItemIsActive)));
            }



            
            if (isEmptyAssignedShelf)
            {
                query = query.Where(_ => _.ShelfOrderItems.Count == 0 && _.CustomerId != null && _.CustomerId > 0);
            }

            if (isAscSortedAssignedDate)
            {
                query = query.Where(_ => _.AssignedDate != null).OrderBy(_ => _.AssignedDate);
            }
            else
            {
                query = query.OrderBy(_ => _.ShelfCode);
            }


            var shelfList = new PagedList<Shelf>(query, pageIndex, pageSize);
            return shelfList;
        }

        public IPagedList<Shelf> GetAllShelfByStore(int customerId = 0,
             DateTime? assignedFromUtc = null, DateTime? assignedToUtc = null,
             DateTime? assignedOrderItemFromUtc = null, DateTime? assignedOrderItemToUtc = null,
             DateTime? shippedFromUtc = null, DateTime? shippedToUtc = null,
             int pageIndex = 0, int pageSize = int.MaxValue,
             bool? shelfOrderItemIsActive = true,
             bool isShelfEmpty = false, bool isEmptyAssignedShelf = false,
             bool? isCustomerNotified = null, string shelfCode = null,
             int? shelfNoteId = null, bool? isPackageItemProcessedDatetime = null)
        {

            var shelfList = new PagedList<Shelf>(new List<Shelf>(), pageIndex, pageSize);
            return shelfList;
        }

        public List<Shelf> GetAvailableShelf(string shelfCode = null)
        {
            var hasShipmentShelf = _shipmentRepository.Table.Where(_ => _.DeliveryDateUtc == null).Select(_ => _.ShelfCode.ToLower()).Distinct().ToList();
            var availableShelf = _shelfRepository.Table.Where(s => !hasShipmentShelf.Contains(s.ShelfCode) && s.ShelfOrderItems.All(_ => !_.IsActived));

            var x = hasShipmentShelf.Contains("1a203");

            if (!string.IsNullOrEmpty(shelfCode))
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
        public void UpdateShelfs(IEnumerable<Shelf> shelfs)
        {
            if (shelfs != null)
            {
                _shelfRepository.Update(shelfs);
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

        public List<ShelfOrderItem> GetOrderItemIdsByShelf(int shelfId, bool? shelfOrderItemIsActive = null, int customerId = 0)
        {
            var query = _shelfOrderItemRepository.Table.AsNoTracking();
            if (customerId > 0 && shelfOrderItemIsActive.HasValue && shelfOrderItemIsActive == true)
            {
                query = query.Where(_ => _.CustomerId == customerId);
            }
            if (shelfId > 0)
            {
                query = query.Where(_ => _.ShelfId == shelfId);
            }

            if (shelfOrderItemIsActive.HasValue)
            {
                query = query.Where(_ => _.IsActived == shelfOrderItemIsActive);
            }
            query = query.OrderByDescending(_ => _.AssignedDate);
            return query.ToList();
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
