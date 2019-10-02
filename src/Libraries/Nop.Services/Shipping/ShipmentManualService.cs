using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

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

        #endregion

        #region Ctor

        public ShipmentManualService(IRepository<ShipmentManual> shipmentManualRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<GenericAttribute> gaRepository,
            IRepository<ShipmentManualItem> shipmentManualItemRepository,
            IRepository<Shelf> shelfRepository)
        {
            this._shipmentManualRepository = shipmentManualRepository;
            this._orderItemRepository = orderItemRepository;
            _gaRepository = gaRepository;
            _shipmentManualItemRepository = shipmentManualItemRepository;
            _shelfRepository = shelfRepository;
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
        /// <param name="shipmentId"></param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
        /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
        /// <param name="shippingCity">Shipping city; null to load all records</param>
        /// <param name="shippingDistrict"></param>
        /// <param name="isCityExcluded"></param>
        /// <param name="trackingNumber">Search by tracking number</param>
        /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orderItemId"></param>
        /// <param name="shelfCode"></param>
        /// <param name="phoneShipperNumber"></param>
        /// <param name="shipperId"></param>
        /// <param name="customerId"></param>
        /// <param name="isShipmentDateEmpty"></param>
        /// <param name="isAddressEmpty"></param>
        /// <param name="customerPhone"></param>
        /// <returns>Shipments</returns>
        public virtual IPagedList<ShipmentManual> GetShipmentManuals(int shipmentId = 0, int vendorId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string shippingDistrict = null,
            bool isCityExcluded = false,
            string trackingNumber = null,
            bool loadNotShipped = false,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            int orderItemId = 0,
            string shelfCode = null,
            string phoneShipperNumber = null,
            int shipperId = 0, int customerId = 0,
            bool isShipmentDateEmpty = false,
            bool isAddressEmpty = false, string customerPhone = null)
        {
            var query = _shipmentManualRepository.Table;
            if (shipmentId > 0)
            {
                query = query.Where(_ => _.Id == shipmentId);
            }
            if (!string.IsNullOrEmpty(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));
            if (isShipmentDateEmpty)
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

            if (customerPhone.IsNotNullOrEmpty())
            {
                query = query.Where(_ => _.Customer.Phone.EndsWith(customerPhone.Trim()));
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
                query = query.Where(s => s.Province.Equals(shippingCity) == !isCityExcluded);

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

            if (shelfCode.IsNotNullOrEmpty())
            {
                shelfCode = shelfCode.ToUpper();
                query = query.Where(_ => _.ShelfCode.Equals(shelfCode));
            }
            
            query = query.OrderByDescending(o => o.Id).ThenByDescending(o => o.CreatedOnUtc);
            var shipments = new PagedList<ShipmentManual>(query, pageIndex, pageSize) { TotalIds = query.Select(_ => _.Id).ToList() };
            return shipments;
        }

        public IList<ShipmentManual> GetShipmentManualsByShelfCode(string shelfCode)
        {
            return _shipmentManualRepository.Table.Where(_ => _.ShelfCode.Equals(shelfCode, StringComparison.CurrentCultureIgnoreCase)).ToList();
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

        public virtual IList<ShipmentManualItem> GetShipmentManualItemsByOrderItemIds(int[] orderItemIds)
        {
            if (orderItemIds == null || orderItemIds.Length == 0)
                return new List<ShipmentManualItem>();

            var query = from o in _shipmentManualItemRepository.Table
                        where orderItemIds.Contains(o.OrderItemId)
                        select o;
            return query.ToList();

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

        public ShipmentManualItem GetShipmentManualItemByOrderItemId(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return _shipmentManualItemRepository.Table.FirstOrDefault(_ => _.OrderItemId == orderItemId);
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
