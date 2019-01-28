using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentCustomService : IShipmentCustomService
    {
        #region Fields

        private readonly IRepository<ShipmentManual> _shipmentCustomRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipmentCustomRepository">Shipment repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="gaRepository"></param>
        public ShipmentCustomService(IRepository<ShipmentManual> shipmentCustomRepository,
            IRepository<OrderItem> orderItemRepository,
            IEventPublisher eventPublisher, IRepository<GenericAttribute> gaRepository)
        {
            this._shipmentCustomRepository = shipmentCustomRepository;
            this._orderItemRepository = orderItemRepository;
            this._eventPublisher = eventPublisher;
            _gaRepository = gaRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void DeleteShipmentCustom(ShipmentManual shipmentCustom)
        {
            if (shipmentCustom == null)
                throw new ArgumentNullException(nameof(shipmentCustom));

            _shipmentCustomRepository.Delete(shipmentCustom);

            //event notification
            _eventPublisher.EntityDeleted(shipmentCustom);
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
        /// <returns>Shipments</returns>
        public virtual IPagedList<ShipmentManual> GetAllShipmentsCustom(int vendorId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, int orderItemId = 0, string phoneShipperNumber = null)
        {
            var query = _shipmentCustomRepository.Table;
            if (!string.IsNullOrEmpty(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));

            if (!string.IsNullOrWhiteSpace(phoneShipperNumber))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.CustomerId, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                                 z.Attribute.Key == SystemCustomerAttributeNames.Phone &&
                                 z.Attribute.Value.Contains(phoneShipperNumber)))
                    .Select(z => z.Customer);
            }

            //if (orderItemId > 0)
            //{
            //    query = query.Where(s => s.OrderItem.Id == orderItemId);
            //}
            //if (shippingCountryId > 0)
            //    query = query.Where(s => s.OrderItem.Order.ShippingAddress.CountryId == shippingCountryId);
            //if (shippingStateId > 0)
            //    query = query.Where(s => s.OrderItem.Order.ShippingAddress.StateProvinceId == shippingStateId);
            //if (!string.IsNullOrWhiteSpace(shippingCity))
            //    query = query.Where(s => s.OrderItem.Order.ShippingAddress.City.Contains(shippingCity));
            //if (loadNotShipped)
            //    query = query.Where(s => !s.ShippedDateUtc.HasValue);
            //if (createdFromUtc.HasValue)
            //    query = query.Where(s => s.ShippedDateUtc != null && createdFromUtc.Value <= s.ShippedDateUtc);
            //if (createdToUtc.HasValue)
            //    query = query.Where(s => s.ShippedDateUtc != null && createdToUtc.Value >= s.ShippedDateUtc);
            //query = query.Where(s => s.OrderItem.Order != null && !s.OrderItem.Order.Deleted);
            //if (vendorId > 0)
            //{
            //    var queryVendorOrderItems = from orderItem in _orderItemRepository.Table
            //                                where orderItem.Product.VendorId == vendorId
            //                                select orderItem.Id;

            //    query = from s in query
            //            where queryVendorOrderItems.Contains(s.OrderItemId)
            //            select s;
            //}

            //query = query.OrderByDescending(s => s.CreatedOnUtc);
            query = query.OrderBy(o => o.CustomerId).ThenByDescending(o => o.CreatedOnUtc);
            var shipments = new PagedList<ShipmentManual>(query, pageIndex, pageSize);
            return shipments;
        }
        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentCustomIds">Shipment identifiers</param>
        /// <returns>Shipments</returns>
        public virtual IList<ShipmentManual> GetShipmentsCustomByIds(int[] shipmentCustomIds)
        {
            if (shipmentCustomIds == null || shipmentCustomIds.Length == 0)
                return new List<ShipmentManual>();

            var query = from o in _shipmentCustomRepository.Table
                        where shipmentCustomIds.Contains(o.Id)
                        select o;
            var shipments = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<ShipmentManual>();
            foreach (var id in shipmentCustomIds)
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
        /// <param name="shipmentCustomId">Shipment identifier</param>
        public virtual ShipmentManual GetShipmentCustomById(int shipmentCustomId)
        {
            if (shipmentCustomId == 0)
                return null;

            return _shipmentCustomRepository.GetById(shipmentCustomId);
        }

        public virtual bool CheckExistTrackingNumber(string trackingNumber)
        {
            return _shipmentCustomRepository.Table.Any(_ => _.TrackingNumber.Equals(trackingNumber, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipmentCustom">Shipment</param>
        public virtual void InsertShipmentCustom(ShipmentManual shipmentCustom)
        {
            if (shipmentCustom == null)
                throw new ArgumentNullException(nameof(shipmentCustom));

            _shipmentCustomRepository.Insert(shipmentCustom);

            //event notification
            _eventPublisher.EntityInserted(shipmentCustom);
        }

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipmentCustom">Shipment</param>
        public virtual void UpdateShipmentCustom(ShipmentManual shipmentCustom)
        {
            if (shipmentCustom == null)
                throw new ArgumentNullException(nameof(shipmentCustom));

            _shipmentCustomRepository.Update(shipmentCustom);

            //event notification
            _eventPublisher.EntityUpdated(shipmentCustom);
        }

        #endregion
    }
}
