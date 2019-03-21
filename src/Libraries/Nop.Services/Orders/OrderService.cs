using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderService : IOrderService
    {
        #region Fields

        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<ShelfOrderItem> _shelfOrderItemRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="orderNoteRepository">Order note repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="eventPublisher">Event published</param>
        public OrderService(IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<Customer> customerRepository,
            IEventPublisher eventPublisher, IRepository<ShelfOrderItem> shelfOrderItemRepository, IRepository<GenericAttribute> gaRepository)
        {
            this._orderRepository = orderRepository;
            this._orderItemRepository = orderItemRepository;
            this._orderNoteRepository = orderNoteRepository;
            this._productRepository = productRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
            _shelfOrderItemRepository = shelfOrderItemRepository;
            _gaRepository = gaRepository;
        }

        #endregion

        #region Methods

        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderRepository.GetById(orderId);
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByCustomOrderNumber(string customOrderNumber)
        {
            if (string.IsNullOrEmpty(customOrderNumber))
                return null;

            return _orderRepository.Table.FirstOrDefault(o => o.CustomOrderNumber == customOrderNumber);
        }

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>Order</returns>
        public virtual IList<Order> GetOrdersByIds(int[] orderIds)
        {
            if (orderIds == null || orderIds.Length == 0)
                return new List<Order>();

            var query = from o in _orderRepository.Table
                        where orderIds.Contains(o.Id) && !o.Deleted
                        select o;
            var orders = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Order>();
            foreach (var id in orderIds)
            {
                var order = orders.Find(x => x.Id == id);
                if (order != null)
                    sortedOrders.Add(order);
            }
            return sortedOrders;
        }

        public virtual IList<OrderItem> GetOrderItemsByIds(int[] orderItemIds)
        {
            if (orderItemIds == null || orderItemIds.Length == 0)
                return new List<OrderItem>();

            var query = from o in _orderItemRepository.Table
                        where orderItemIds.Contains(o.Id)
                        select o;
            var orderItems = query.ToList();
            //sort by passed identifiers
            var sortedOrderItems = new List<OrderItem>();
            foreach (var id in orderItemIds)
            {
                var order = orderItems.Find(x => x.Id == id);
                if (order != null)
                    sortedOrderItems.Add(order);
            }
            return sortedOrderItems;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var query = from o in _orderRepository.Table
                        where o.OrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        public Order GetLastOrderByCustomerId(int customerId)
        {
            var query = from o in _orderRepository.Table
                        where o.CustomerId == customerId
                        orderby o.Id descending
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.Deleted = true;
            UpdateOrder(order);

            //event notification
            _eventPublisher.EntityDeleted(order);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Order status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="billingPhone"></param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orderBy"></param>
        /// <param name="isOrderCheckout"></param>
        /// <returns>Orders</returns>
        public virtual IPagedList<Order> SearchOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null, List<int> procIds = null,
            string billingEmail = null, List<int> custIdsByLinkFace = null, string billingFullName = null, string billingPhone = null,
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, OrderSortingEnum orderBy = OrderSortingEnum.CreatedOnDesc, string orderId = null)
        {
            var query = _orderRepository.Table;

            if (string.IsNullOrEmpty(orderId) == false)
            {
                orderId = orderId.TrimStart().TrimEnd().Trim();
                query = query.Where(_ => _.Id.ToString().Contains(orderId));
            }
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.VendorId == vendorId));
            }
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (productId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.Id == productId));
            }

            if (procIds != null && procIds.Any())
            {
                query = query
                    .Where(o => o.OrderItems
                        .Any(orderItem => procIds.Contains(orderItem.Product.Id)));
            }

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem =>
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (orderItem.Product.ManageInventoryMethodId == manageStockInventoryMethodId &&
                        orderItem.Product.UseMultipleWarehouses &&
                        orderItem.Product.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                        ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((orderItem.Product.ManageInventoryMethodId != manageStockInventoryMethodId ||
                        !orderItem.Product.UseMultipleWarehouses) &&
                        orderItem.Product.WarehouseId == warehouseId))
                        );
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (custIdsByLinkFace != null)
            {
                if (custIdsByLinkFace.Count > 0)
                {
                    query = query.Where(o => custIdsByLinkFace.Contains(o.CustomerId));
                }
                else
                {
                    query = query.Where(o => 1 == 0);
                }
            }

            if (!string.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!string.IsNullOrEmpty(billingFullName))
            {
                var listKeywords = billingFullName.Split(' ').ToList();
                query = query.Where(o => o.BillingAddress != null && (listKeywords.Contains(o.BillingAddress.FirstName) || listKeywords.Contains(o.BillingAddress.LastName)));
            }

            //query = query.Where(o => o.BillingAddress != null && (!string.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName)) || !string.IsNullOrEmpty(o.BillingAddress.FirstName) && o.BillingAddress.FirstName.Contains(billingLastName));
            if (!string.IsNullOrEmpty(billingPhone))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.PhoneNumber) && o.BillingAddress.PhoneNumber.Contains(billingPhone));
            if (!string.IsNullOrEmpty(orderNotes))
                if (!string.IsNullOrEmpty(orderNotes))
                    query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));
            query = query.Where(o => !o.Deleted);

            //filter is checkout order
            //if (isOrderCheckout.HasValue)
            //{
            //    query = query.Where(o => o.IsOrderCheckout == isOrderCheckout.Value);
            //}

            switch (orderBy)
            {
                case OrderSortingEnum.CreatedOnAsc:
                    query = query.OrderBy(o => o.CreatedOnUtc);
                    break;
                case OrderSortingEnum.CreatedOnDesc:
                    query = query.OrderByDescending(o => o.CreatedOnUtc);
                    break;
                case OrderSortingEnum.StatusAsc:
                    query = query.OrderBy(o => o.OrderStatusId);
                    break;
                case OrderSortingEnum.StatusDesc:
                    query = query.OrderByDescending(o => o.OrderStatusId);
                    break;
                case OrderSortingEnum.TotalAsc:
                    query = query.OrderBy(o => o.OrderTotal);
                    break;
                case OrderSortingEnum.TotalDesc:
                    query = query.OrderByDescending(o => o.OrderTotal);
                    break;
            }

            //database layer paging
            return new PagedList<Order>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void InsertOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _orderRepository.Insert(order);

            //event notification
            _eventPublisher.EntityInserted(order);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _orderRepository.Update(order);

            //event notification
            _eventPublisher.EntityUpdated(order);
        }

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId,
            string paymentMethodSystemName)
        {
            var query = _orderRepository.Table;
            if (!string.IsNullOrWhiteSpace(authorizationTransactionId))
                query = query.Where(o => o.AuthorizationTransactionId == authorizationTransactionId);

            if (!string.IsNullOrWhiteSpace(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            query = query.OrderByDescending(o => o.CreatedOnUtc);
            var order = query.FirstOrDefault();
            return order;
        }

        #endregion

        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemById(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return _orderItemRepository.GetById(orderItemId);
        }

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="orderItemGuid">Order identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemByGuid(Guid orderItemGuid)
        {
            if (orderItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _orderItemRepository.Table
                        where orderItem.OrderItemGuid == orderItemGuid
                        select orderItem;
            var item = query.FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>Order items</returns>
        public virtual IList<OrderItem> GetDownloadableOrderItems(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException("customerId");

            var query = from orderItem in _orderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                        join p in _productRepository.Table on orderItem.ProductId equals p.Id
                        where customerId == o.CustomerId &&
                        p.IsDownload &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }


        public virtual IList<OrderItem> GetOrderItemsByPackageId(int packageId)
        {
            if (packageId == 0)
                throw new ArgumentOutOfRangeException("packageId");

            var query = from orderItem in _orderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                        where packageId == orderItem.PackageOrderId && !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }

        public virtual IPagedList<OrderItem> GetOrderItemsVendorCheckout(string vendorProductUrl, string orderId = null,
            string orderItemId = null, int pageIndex = 0,
            int pageSize = int.MaxValue, OrderSortingEnum orderBy = OrderSortingEnum.CreatedOnDesc, bool todayFilter = false,
            string customerPhone = null, string packageOrderCode = null,
            int vendorId = 0, bool? isSetPackageOrderId = null,
            bool? isSetShelfId = null, int orderItemStatusId = -1, bool? isPackageItemProcessedDatetime = null)
        {
            var query = _orderItemRepository.Table;
            if (string.IsNullOrEmpty(vendorProductUrl) == false)
            {
                query = query.Where(_ => string.IsNullOrEmpty(_.Product.VendorProductUrl) == false
                                         && _.Product.VendorProductUrl.Contains(vendorProductUrl));
            }

            if (string.IsNullOrEmpty(orderItemId) == false)
            {
                orderItemId = orderItemId.TrimStart().TrimEnd().Trim();
                query = query.Where(_ => _.Id.ToString().Contains(orderItemId));
            }

            if (string.IsNullOrEmpty(orderId) == false)
            {
                orderId = orderId.TrimStart().TrimEnd().Trim();
                query = query.Where(_ => _.OrderId.ToString().Contains(orderId));
            }

            if (string.IsNullOrEmpty(packageOrderCode) == false)
            {
                packageOrderCode = packageOrderCode.TrimStart().TrimEnd().Trim();
                query = query.Where(_ => _.PackageOrder.PackageCode.Contains(packageOrderCode));
            }
            else
            {
                if (isSetPackageOrderId.HasValue)
                {
                    if (isSetPackageOrderId.Value)
                    {
                        query = query.Where(_ => _.PackageOrderId != null && _.PackageOrderId > 0);
                    }
                    else
                    {
                        query = query.Where(_ => _.PackageOrderId == null || _.PackageOrderId == 0);
                    }
                }
            }

            if (isSetShelfId.HasValue)
            {
                var shelfOrderItemIds = _shelfOrderItemRepository.Table.Select(_ => _.OrderItemId).ToList();
                if (isSetShelfId.Value)
                {
                    query = query.Where(_ => shelfOrderItemIds.Contains(_.Id));
                }
                else
                {
                    query = query.Where(_ => shelfOrderItemIds.Contains(_.Id) == false);
                }
            }

            if (isPackageItemProcessedDatetime.HasValue)
            {
                query = query.Where(_ => _.PackageItemProcessedDatetime != null == isPackageItemProcessedDatetime);
            }

            if (vendorId > 0)
            {
                query = query.Where(_ => _.Product.VendorId == vendorId);
            }

            //if (isOrderCheckout.HasValue)
            //{
            //    query = query.Where(_ => _.IsOrderCheckout == isOrderCheckout);
            //}

            //if (isPackageItemProcessed)
            //{
            //    query = query.Where(_ => _.PackageItemProcessedDatetime != null);
            //}

            if (orderItemStatusId != -1)
            {
                query = query.Where(_ => _.OrderItemStatusId == orderItemStatusId);
            }

            if (todayFilter)
            {
                var startDate = DateTime.UtcNow.Date;
                var endDate = DateTime.UtcNow.Date.AddDays(1);
                query = query.Where(_ => _.Order.CreatedOnUtc != null && startDate <= _.Order.CreatedOnUtc && endDate > _.Order.CreatedOnUtc);
            }

            //if (!string.IsNullOrEmpty(customerPhone))
            //    query = query.Where(o => o.Order.BillingAddress != null && !string.IsNullOrEmpty(o.Order.BillingAddress.PhoneNumber) && o.Order.BillingAddress.PhoneNumber.Contains(customerPhone));
            //if (!string.IsNullOrWhiteSpace(customerPhone))
            //{
            //    customerPhone = customerPhone.TrimStart().TrimEnd().Trim();
            //    query = query
            //        .Join(_gaRepository.Table, x => x.Order.CustomerId, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
            //        .Where(z => z.Attribute.KeyGroup == "Customer" &&
            //                    z.Attribute.Key == SystemCustomerAttributeNames.Phone &&
            //                    z.Attribute.Value.Contains(customerPhone))
            //        .Select(z => z.Customer);
            //}

            if (!string.IsNullOrWhiteSpace(customerPhone))
            {
                customerPhone = customerPhone.TrimStart().TrimEnd().Trim();
                query = query
                    .Join(_customerRepository.Table, x => x.Order.CustomerId, y => y.Id, (x, y) => new { OrderItem = x, Customer = y })
                    .Where(z => z.Customer.Phone.Contains(customerPhone))
                    .Select(z => z.OrderItem);
            }

            switch (orderBy)
            {
                case OrderSortingEnum.CreatedOnAsc:
                    query = query.OrderBy(o => o.Order.CreatedOnUtc);
                    break;
                case OrderSortingEnum.CreatedOnDesc:
                    query = query.OrderByDescending(o => o.Order.CreatedOnUtc);
                    break;
                case OrderSortingEnum.StatusAsc:
                    query = query.OrderBy(o => o.Order.OrderStatusId);
                    break;
                case OrderSortingEnum.StatusDesc:
                    query = query.OrderByDescending(o => o.Order.OrderStatusId);
                    break;
                case OrderSortingEnum.TotalAsc:
                    query = query.OrderBy(o => o.Order.OrderTotal);
                    break;
                case OrderSortingEnum.TotalDesc:
                    query = query.OrderByDescending(o => o.Order.OrderTotal);
                    break;
            }

            var results = new PagedList<OrderItem>(query, pageIndex, pageSize) { TotalIds = query.Select(_ => _.Id).ToList() };
            return results;
        }
        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        public virtual void DeleteOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            _orderItemRepository.Delete(orderItem);

            //event notification
            _eventPublisher.EntityDeleted(orderItem);
        }
        public virtual void UpdateOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            _orderItemRepository.Update(orderItem);

            //event notification
            _eventPublisher.EntityUpdated(orderItem);
        }

        #endregion

        #region Orders notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>Order note</returns>
        public virtual OrderNote GetOrderNoteById(int orderNoteId)
        {
            if (orderNoteId == 0)
                return null;

            return _orderNoteRepository.GetById(orderNoteId);
        }

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        public virtual void DeleteOrderNote(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            _orderNoteRepository.Delete(orderNote);

            //event notification
            _eventPublisher.EntityDeleted(orderNote);
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void DeleteRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            recurringPayment.Deleted = true;
            UpdateRecurringPayment(recurringPayment);

            //event notification
            _eventPublisher.EntityDeleted(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public virtual RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

            return _recurringPaymentRepository.GetById(recurringPaymentId);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            _recurringPaymentRepository.Insert(recurringPayment);

            //event notification
            _eventPublisher.EntityInserted(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            _recurringPaymentRepository.Update(recurringPayment);

            //event notification
            _eventPublisher.EntityUpdated(recurringPayment);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payments</returns>
        public virtual IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;

            var query1 = from rp in _recurringPaymentRepository.Table
                         join c in _customerRepository.Table on rp.InitialOrder.CustomerId equals c.Id
                         where
                         (!rp.Deleted) &&
                         (showHidden || !rp.InitialOrder.Deleted) &&
                         (showHidden || !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || rp.InitialOrder.CustomerId == customerId) &&
                         (storeId == 0 || rp.InitialOrder.StoreId == storeId) &&
                         (initialOrderId == 0 || rp.InitialOrder.Id == initialOrderId) &&
                         (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 ||
                          rp.InitialOrder.OrderStatusId == initialOrderStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;

            var recurringPayments = new PagedList<RecurringPayment>(query2, pageIndex, pageSize);
            return recurringPayments;
        }

        #endregion

        #endregion
    }
}
