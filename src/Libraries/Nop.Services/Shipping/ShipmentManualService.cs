using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Extensions;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentManualService : IShipmentManualService
    {
        #region Fields

        private readonly IRepository<ShipmentManual> _shipmentManualRepository;
        private readonly IRepository<ShipmentManualItem> _shipmentManualItemRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Shelf> _shelfRepository;
        private readonly IRepository<ShelfOrderItem> _shelfOrderItemRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipmentManualRepository">Shipment repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="gaRepository"></param>
        /// <param name="shipmentManualItemRepository"></param>
        public ShipmentManualService(IRepository<ShipmentManual> shipmentManualRepository,
            IRepository<OrderItem> orderItemRepository, IRepository<GenericAttribute> gaRepository, IRepository<ShipmentManualItem> shipmentManualItemRepository, IRepository<Shelf> shelfRepository, IRepository<ShelfOrderItem> shelfOrderItemRepository)
        {
            this._shipmentManualRepository = shipmentManualRepository;
            this._orderItemRepository = orderItemRepository;
            _gaRepository = gaRepository;
            _shipmentManualItemRepository = shipmentManualItemRepository;
            _shelfRepository = shelfRepository;
            _shelfOrderItemRepository = shelfOrderItemRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipmentManual">shipmentManual</param>
        public virtual void DeleteShipmentManual(ShipmentManual shipmentManual)
        {
            if (shipmentManual == null)
                throw new ArgumentNullException(nameof(shipmentManual));

            _shipmentManualRepository.Delete(shipmentManual);

        }

        /// <summary>
        /// Search shipments
        /// </summary>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
        /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
        /// <param name="shippingCity">Shipping city; null to load all records</param>
        /// <param name="trackingNumber">Search by tracking number</param>
        /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orderItemId"></param>
        /// <param name="phoneShipperNumber"></param>
        /// <param name="shipperId"></param>
        /// <param name="customerId"></param>
        /// <returns>Shipments</returns>
        public virtual IPagedList<ShipmentManual> GetAllShipmentsManual(int shipmentId = 0, int vendorId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string shippingDistrict = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            bool exceptCity = false,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            int orderItemId = 0,
            string shelfCode = null,
            string phoneShipperNumber = null,
            int shipperId = 0, int customerId = 0,
            bool isNotSetShippedDate = false,
            bool isAddressEmpty = false)
        {
            var query = _shipmentManualRepository.Table;
            if (shipmentId > 0)
            {
                query = query.Where(_ => _.Id == shipmentId);
            }
            if (!string.IsNullOrEmpty(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));
            if (isNotSetShippedDate)
            {
                query = query.Where(_ => _.ShippedDateUtc == null);
            }
            if (isAddressEmpty)
            {
                query = query.Where(_ => _.Address == null || _.Address == string.Empty || _.Address == @"Chưa xác định");
            }
            if (shipperId > 0)
            {
                query = query.Where(_ => _.ShipperId == shipperId);
            }
            else if (shipperId == -1)
            {
                query = query.Where(_ => _.ShipperId == 0);
            }

            if (customerId > 0)
            {
                query = query.Where(_ => _.CustomerId == customerId);
            }

            if (!string.IsNullOrWhiteSpace(phoneShipperNumber))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.ShipperId, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                                 z.Attribute.Key == SystemCustomerAttributeNames.Phone &&
                                 z.Attribute.Value.Contains(phoneShipperNumber)))
                    .Select(z => z.Customer);
            }

            if (orderItemId > 0)
            {
                query = query.Where(s => s.ShipmentManualItems.Any(m => m.OrderItemId == orderItemId));
            }
            if (shippingCountryId > 0)
                query = query.Where(s => s.Customer.ShippingAddress.CountryId == shippingCountryId);
            if (shippingStateId > 0)
                query = query.Where(s => s.Customer.ShippingAddress.StateProvinceId == shippingStateId);
            if (!string.IsNullOrWhiteSpace(shippingCity))
                query = query.Where(s => s.Province.Equals(shippingCity) == !exceptCity);

            if (!string.IsNullOrWhiteSpace(shippingDistrict))
                query = query.Where(s => s.District.Equals(shippingDistrict));

            if (loadNotShipped)
                query = query.Where(s => !s.DeliveryDateUtc.HasValue);
            if (createdFromUtc.HasValue)
                query = query.Where(s => s.ShippedDateUtc != null && createdFromUtc.Value <= s.ShippedDateUtc);
            if (createdToUtc.HasValue)
                query = query.Where(s => s.ShippedDateUtc != null && createdToUtc.Value >= s.ShippedDateUtc);
            if (vendorId > 0)
            {
                var queryVendorOrderItems = from orderItem in _orderItemRepository.TableNoTracking
                                            where orderItem.Product.VendorId == vendorId
                                            select orderItem.Id;

                query = from s in query
                        where s.ShipmentManualItems.Any(_ => queryVendorOrderItems.Contains(_.OrderItemId))
                        select s;
            }

            //if (shelfCode.IsNotNullOrEmpty())
            //{
            //    shelfCode = shelfCode.ToUpper();
            //    var orderItemIds = new List<int>();
            //    var shelf = _shelfRepository.TableNoTracking.FirstOrDefault(_ => _.ShelfCode != null && _.ShelfCode == shelfCode);
            //    if (shelf != null)
            //    {
            //        orderItemIds = _shelfOrderItemRepository.TableNoTracking.Where(_ => _.ShelfId == shelf.Id).Select(s => s.OrderItemId).ToList();
            //    }

            //    query = from s in query
            //        where s.ShipmentManualItems.Any(_ => orderItemIds.Contains(_.OrderItemId))
            //        select s;
            //}
            if (shelfCode.IsNotNullOrEmpty())
            {
                shelfCode = shelfCode.ToUpper();
                query = query.Where(_ => _.ShelfCode.Equals(shelfCode));
            }

            //query = query.OrderByDescending(s => s.CreatedOnUtc);
            query = query.OrderByDescending(o => o.Id).ThenByDescending(o => o.CreatedOnUtc);
            var shipments = new PagedList<ShipmentManual>(query, pageIndex, pageSize) { TotalIds = query.Select(_ => _.Id).ToList() };
            return shipments;
        }
        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentManualIds">Shipment identifiers</param>
        /// <returns>Shipments</returns>
        public virtual IList<ShipmentManual> GetShipmentsManualByIds(int[] shipmentManualIds)
        {
            if (shipmentManualIds == null || shipmentManualIds.Length == 0)
                return new List<ShipmentManual>();

            var query = from o in _shipmentManualRepository.Table
                        where shipmentManualIds.Contains(o.Id)
                        select o;
            var shipments = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<ShipmentManual>();
            foreach (var id in shipmentManualIds)
            {
                var shipment = shipments.Find(x => x.Id == id);
                if (shipment != null)
                    sortedOrders.Add(shipment);
            }
            return sortedOrders;
        }

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="id">Shipment identifier</param>
        public virtual ShipmentManual GetShipmentManualById(int id)
        {
            if (id == 0)
                return null;

            return _shipmentManualRepository.GetById(id);
        }

        public virtual bool CheckExistTrackingNumber(string trackingNumber)
        {
            return _shipmentManualRepository.Table.Any(_ => _.TrackingNumber.Equals(trackingNumber, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipmentManual">Shipment</param>
        public virtual void InsertShipmentManual(ShipmentManual shipmentManual)
        {
            if (shipmentManual == null)
                throw new ArgumentNullException(nameof(shipmentManual));

            _shipmentManualRepository.Insert(shipmentManual);
        }

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipmentManual">Shipment</param>
        public virtual void UpdateShipmentManual(ShipmentManual shipmentManual)
        {
            if (shipmentManual == null)
                throw new ArgumentNullException(nameof(shipmentManual));

            _shipmentManualRepository.Update(shipmentManual);
        }
        public virtual void UpdateShipmentManuals(List<ShipmentManual> shipmentManuals)
        {
            if (shipmentManuals == null)
                throw new ArgumentNullException(nameof(shipmentManuals));

            _shipmentManualRepository.Update(shipmentManuals);
        }

        public void DeleteShipmentManualItem(ShipmentManualItem shipmentManualItem)
        {
            if (shipmentManualItem == null)
                throw new ArgumentNullException(nameof(shipmentManualItem));

            _shipmentManualItemRepository.Delete(shipmentManualItem);
        }

        public ShipmentManualItem GetShipmentManualItemById(int id)
        {
            if (id == 0)
                return null;

            return _shipmentManualItemRepository.GetById(id);
        }

        public void InsertShipmentManualItem(ShipmentManualItem shipmentManualItem)
        {
            if (shipmentManualItem == null)
                throw new ArgumentNullException(nameof(shipmentManualItem));

            _shipmentManualItemRepository.Insert(shipmentManualItem);
        }

        public void UpdateShipmentManualItem(ShipmentManualItem shipmentManualItem)
        {
            if (shipmentManualItem == null)
                throw new ArgumentNullException(nameof(shipmentManualItem));

            _shipmentManualItemRepository.Update(shipmentManualItem);
        }

        #endregion
    }
}
