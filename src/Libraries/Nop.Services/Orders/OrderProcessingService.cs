using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order processing service
    /// </summary>
    public partial class OrderProcessingService : IOrderProcessingService
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ITaxService _taxService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IEncryptionService _encryptionService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly IAffiliateService _affiliateService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPdfService _pdfService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShipmentManualService _shipmentManualService;
        private readonly ShippingSettings _shippingSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderService">Order service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="productService">Product service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="orderTotalCalculationService">Order total calculation service</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="productAttributeFormatter">Product attribute formatter</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="shoppingCartService">Shopping cart service</param>
        /// <param name="checkoutAttributeFormatter">Checkout attribute service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shipmentService">Shipment service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="vendorService">Vendor service</param>
        /// <param name="customerActivityService">Customer activity service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="affiliateService">Affiliate service</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="pdfService">PDF service</param>
        /// <param name="rewardPointService">Reward point service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="countryService">Country service</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="stateProvinceService">State province service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="orderSettings">Order settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="customNumberFormatter">Custom number formatter</param>
        public OrderProcessingService(IOrderService orderService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IProductService productService,
            IPaymentService paymentService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter,
            IGiftCardService giftCardService,
            IShoppingCartService shoppingCartService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ITaxService taxService,
            ICustomerService customerService,
            IDiscountService discountService,
            IEncryptionService encryptionService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            ICurrencyService currencyService,
            IAffiliateService affiliateService,
            IEventPublisher eventPublisher,
            IPdfService pdfService,
            IRewardPointService rewardPointService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ShippingSettings shippingSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings,
            ICustomNumberFormatter customNumberFormatter, IShipmentManualService shipmentManualService, IPictureService pictureService)
        {
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._productService = productService;
            this._paymentService = paymentService;
            this._logger = logger;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._giftCardService = giftCardService;
            this._shoppingCartService = shoppingCartService;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._taxService = taxService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._encryptionService = encryptionService;
            this._customerActivityService = customerActivityService;
            this._currencyService = currencyService;
            this._affiliateService = affiliateService;
            this._eventPublisher = eventPublisher;
            this._pdfService = pdfService;
            this._rewardPointService = rewardPointService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;

            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._currencySettings = currencySettings;
            this._customNumberFormatter = customNumberFormatter;
            _shipmentManualService = shipmentManualService;
            _pictureService = pictureService;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// PlaceOrder container
        /// </summary>
        protected class PlaceOrderContainer
        {
            /// <summary>
            /// Ctor
            /// </summary>
            public PlaceOrderContainer()
            {
                this.Cart = new List<ShoppingCartItem>();
                this.AppliedDiscounts = new List<DiscountForCaching>();
                this.AppliedGiftCards = new List<AppliedGiftCard>();
            }

            /// <summary>
            /// Customer
            /// </summary>
            public Customer Customer { get; set; }
            /// <summary>
            /// Customer language
            /// </summary>
            public Language CustomerLanguage { get; set; }
            /// <summary>
            /// Affiliate identifier
            /// </summary>
            public int AffiliateId { get; set; }
            /// <summary>
            /// TAx display type
            /// </summary>
            public TaxDisplayType CustomerTaxDisplayType { get; set; }
            /// <summary>
            /// Selected currency
            /// </summary>
            public string CustomerCurrencyCode { get; set; }
            /// <summary>
            /// Customer currency rate
            /// </summary>
            public decimal CustomerCurrencyRate { get; set; }

            /// <summary>
            /// Billing address
            /// </summary>
            public Address BillingAddress { get; set; }
            /// <summary>
            /// Shipping address
            /// </summary>
            public Address ShippingAddress { get; set; }
            /// <summary>
            /// Shipping status
            /// </summary>
            public ShippingStatus ShippingStatus { get; set; }
            /// <summary>
            /// Selected shipping method
            /// </summary>
            public string ShippingMethodName { get; set; }
            /// <summary>
            /// Shipping rate computation method system name
            /// </summary>
            public string ShippingRateComputationMethodSystemName { get; set; }
            /// <summary>
            /// Is pickup in store selected?
            /// </summary>
            public bool PickUpInStore { get; set; }
            /// <summary>
            /// Selected pickup address
            /// </summary>
            public Address PickupAddress { get; set; }

            /// <summary>
            /// Is recurring shopping cart
            /// </summary>
            public bool IsRecurringShoppingCart { get; set; }
            /// <summary>
            /// Initial order (used with recurring payments)
            /// </summary>
            public Order InitialOrder { get; set; }

            /// <summary>
            /// Checkout attributes
            /// </summary>
            public string CheckoutAttributeDescription { get; set; }
            /// <summary>
            /// Shopping cart
            /// </summary>
            public string CheckoutAttributesXml { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public IList<ShoppingCartItem> Cart { get; set; }
            /// <summary>
            /// Applied discounts
            /// </summary>
            public List<DiscountForCaching> AppliedDiscounts { get; set; }
            /// <summary>
            /// Applied gift cards
            /// </summary>
            public List<AppliedGiftCard> AppliedGiftCards { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal OrderSubTotalInclTax { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public decimal OrderSubTotalExclTax { get; set; }
            /// <summary>
            /// Subtotal discount (incl tax)
            /// </summary>
            public decimal OrderSubTotalDiscountInclTax { get; set; }
            /// <summary>
            /// Subtotal discount (excl tax)
            /// </summary>
            public decimal OrderSubTotalDiscountExclTax { get; set; }
            /// <summary>
            /// Shipping (incl tax)
            /// </summary>
            public decimal OrderShippingTotalInclTax { get; set; }
            /// <summary>
            /// Shipping (excl tax)
            /// </summary>
            public decimal OrderShippingTotalExclTax { get; set; }
            /// <summary>
            /// Payment additional fee (incl tax)
            /// </summary>
            public decimal PaymentAdditionalFeeInclTax { get; set; }
            /// <summary>
            /// Payment additional fee (excl tax)
            /// </summary>
            public decimal PaymentAdditionalFeeExclTax { get; set; }
            /// <summary>
            /// Tax
            /// </summary>
            public decimal OrderTaxTotal { get; set; }
            /// <summary>
            /// VAT number
            /// </summary>
            public string VatNumber { get; set; }
            /// <summary>
            /// Tax rates
            /// </summary>
            public string TaxRates { get; set; }
            /// <summary>
            /// Order total discount amount
            /// </summary>
            public decimal OrderDiscountAmount { get; set; }
            /// <summary>
            /// Redeemed reward points
            /// </summary>
            public int RedeemedRewardPoints { get; set; }
            /// <summary>
            /// Redeemed reward points amount
            /// </summary>
            public decimal RedeemedRewardPointsAmount { get; set; }
            /// <summary>
            /// Order total
            /// </summary>
            public decimal OrderTotal { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Add order note
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="note">Note text</param>
        protected virtual void AddOrderNote(Order order, string note)
        {
            order.OrderNotes.Add(new OrderNote
            {
                Note = note,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Prepare details to place an order. It also sets some properties to "processPaymentRequest"
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceOrderContainer PreparePlaceOrderDetails(ProcessPaymentRequest processPaymentRequest)
        {

            var details = new PlaceOrderContainer
            {

                //customer
                Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId)
            };
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //check whether customer is guest
            //if (details.Customer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
            //    throw new NopException("Anonymous checkout is not allowed");

            //customer currency
            var currencyTmp = _currencyService.GetCurrencyById(
                details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId, processPaymentRequest.StoreId));
            var customerCurrency = (currencyTmp != null && currencyTmp.Published) ? currencyTmp : _workContext.WorkingCurrency;
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            details.CustomerCurrencyCode = customerCurrency.CurrencyCode;
            details.CustomerCurrencyRate = customerCurrency.Rate / primaryStoreCurrency.Rate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(
                details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, processPaymentRequest.StoreId));
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            var lastOrderCust = _orderService.GetLastOrderByCustomerId(customerId: details.Customer.Id);

            details.BillingAddress = details.Customer.Addresses.LastOrDefault();

            //payment method 
            if (lastOrderCust != null)
            {
                processPaymentRequest.PaymentMethodSystemName = lastOrderCust.PaymentMethodSystemName;
            }

            //checkout attributes
            details.CheckoutAttributesXml = details.Customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, processPaymentRequest.StoreId);
            details.CheckoutAttributeDescription = _checkoutAttributeFormatter.FormatAttributes(details.CheckoutAttributesXml, details.Customer);

            //load shopping cart
            details.Cart = details.Customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(processPaymentRequest.StoreId).ToList();

            if (!details.Cart.Any())
                throw new NopException("Cart is empty");

            //validate the entire shopping cart
            var warnings = _shoppingCartService.GetShoppingCartWarnings(details.Cart, details.CheckoutAttributesXml, true);
            if (warnings.Any())
                throw new NopException(warnings.Aggregate(string.Empty, (current, next) => $"{current}{next};"));

            //validate individual cart items
            foreach (var sci in details.Cart)
            {
                var sciWarnings = _shoppingCartService.GetShoppingCartItemWarnings(details.Customer,
                    sci.ShoppingCartType, sci.Product, processPaymentRequest.StoreId, sci.AttributesXml,
                    sci.CustomerEnteredPrice, sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
                if (sciWarnings.Any())
                    throw new NopException(sciWarnings.Aggregate(string.Empty, (current, next) => $"{current}{next};"));
            }

            //min totals validation
            if (!ValidateMinOrderSubtotalAmount(details.Cart))
            {
                var minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"),
                    _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false)));
            }

            if (!ValidateMinOrderTotalAmount(details.Cart))
            {
                var minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"),
                    _priceFormatter.FormatPrice(minOrderTotalAmount, true, false)));
            }

            //tax display type
            if (_taxSettings.AllowCustomersToSelectTaxDisplayType)
                details.CustomerTaxDisplayType = (TaxDisplayType)details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, processPaymentRequest.StoreId);
            else
                details.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;

            //sub total (incl tax)
            _orderTotalCalculationService.GetShoppingCartSubTotal(details.Cart, true, out decimal orderSubTotalDiscountAmount, out List<DiscountForCaching> orderSubTotalAppliedDiscounts, out decimal subTotalWithoutDiscountBase, out decimal _);
            details.OrderSubTotalInclTax = subTotalWithoutDiscountBase;
            details.OrderSubTotalDiscountInclTax = orderSubTotalDiscountAmount;

            //discount history
            foreach (var disc in orderSubTotalAppliedDiscounts)
                if (!details.AppliedDiscounts.ContainsDiscount(disc))
                    details.AppliedDiscounts.Add(disc);

            //sub total (excl tax)
            _orderTotalCalculationService.GetShoppingCartSubTotal(details.Cart, false, out orderSubTotalDiscountAmount,
                out orderSubTotalAppliedDiscounts, out subTotalWithoutDiscountBase, out _);
            details.OrderSubTotalExclTax = subTotalWithoutDiscountBase;
            details.OrderSubTotalDiscountExclTax = orderSubTotalDiscountAmount;

            //shipping info
            if (details.Cart.RequiresShipping(_productService, _productAttributeParser))
            {
                var pickupPoint = details.Customer.GetAttribute<PickupPoint>(SystemCustomerAttributeNames.SelectedPickupPoint, processPaymentRequest.StoreId);
                if (_shippingSettings.AllowPickUpInStore && pickupPoint != null)
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                    var state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id);

                    details.PickUpInStore = true;
                    if (lastOrderCust == null)
                    {
                        details.PickupAddress = new Address
                        {
                            Address1 = pickupPoint.Address,
                            City = pickupPoint.City,
                            Country = country,
                            StateProvince = state,
                            ZipPostalCode = pickupPoint.ZipPostalCode,
                            CreatedOnUtc = DateTime.UtcNow,
                        };
                    }
                    else
                    {
                        details.PickupAddress = lastOrderCust.PickupAddress;
                    }
                }
                else
                {
                    details.ShippingAddress = details.BillingAddress;
                }


                if (lastOrderCust == null)
                {
                    var shippingOption = details.Customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, processPaymentRequest.StoreId);
                    if (shippingOption != null)
                    {
                        details.ShippingMethodName = shippingOption.Name;
                        details.ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName;
                    }
                }
                else
                {
                    details.ShippingMethodName = lastOrderCust.ShippingMethod;
                    details.ShippingRateComputationMethodSystemName = lastOrderCust.ShippingRateComputationMethodSystemName;
                }
                details.ShippingStatus = ShippingStatus.NotYetShipped;
            }
            else
                details.ShippingStatus = ShippingStatus.ShippingNotRequired;

            //shipping total
            var orderShippingTotalInclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(details.Cart, true, out decimal _, out List<DiscountForCaching> shippingTotalDiscounts);
            var orderShippingTotalExclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(details.Cart, false);
            //if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
            //    throw new NopException("Shipping total couldn't be calculated");

            //details.OrderShippingTotalInclTax = orderShippingTotalInclTax.Value;
            //details.OrderShippingTotalExclTax = orderShippingTotalExclTax.Value;

            foreach (var disc in shippingTotalDiscounts)
                if (!details.AppliedDiscounts.ContainsDiscount(disc))
                    details.AppliedDiscounts.Add(disc);

            //payment total
            var paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(details.Cart, processPaymentRequest.PaymentMethodSystemName);
            details.PaymentAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, details.Customer);
            details.PaymentAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, details.Customer);

            //tax amount
            details.OrderTaxTotal = _orderTotalCalculationService.GetTaxTotal(details.Cart, out SortedDictionary<decimal, decimal> taxRatesDictionary);

            //VAT number
            var customerVatStatus = (VatNumberStatus)details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId);
            if (_taxSettings.EuVatEnabled && customerVatStatus == VatNumberStatus.Valid)
                details.VatNumber = details.Customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

            //tax rates
            details.TaxRates = taxRatesDictionary.Aggregate(string.Empty, (current, next) =>
                $"{current}{next.Key.ToString(CultureInfo.InvariantCulture)}:{next.Value.ToString(CultureInfo.InvariantCulture)};   ");

            //order total (and applied discounts, gift cards, reward points)
            var orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(details.Cart, out decimal orderDiscountAmount, out List<DiscountForCaching> orderAppliedDiscounts, out List<AppliedGiftCard> appliedGiftCards, out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount);
            if (!orderTotal.HasValue)
                throw new NopException("Order total couldn't be calculated");

            details.OrderDiscountAmount = orderDiscountAmount;
            details.RedeemedRewardPoints = redeemedRewardPoints;
            details.RedeemedRewardPointsAmount = redeemedRewardPointsAmount;
            details.AppliedGiftCards = appliedGiftCards;
            details.OrderTotal = orderTotal.Value;

            //discount history
            foreach (var disc in orderAppliedDiscounts)
                if (!details.AppliedDiscounts.ContainsDiscount(disc))
                    details.AppliedDiscounts.Add(disc);

            processPaymentRequest.OrderTotal = details.OrderTotal;

            //recurring or standard shopping cart?
            details.IsRecurringShoppingCart = details.Cart.IsRecurring();
            if (details.IsRecurringShoppingCart)
            {
                var recurringCyclesError = details.Cart.GetRecurringCycleInfo(_localizationService, out int recurringCycleLength, out RecurringProductCyclePeriod recurringCyclePeriod, out int recurringTotalCycles);
                if (!string.IsNullOrEmpty(recurringCyclesError))
                    throw new NopException(recurringCyclesError);

                processPaymentRequest.RecurringCycleLength = recurringCycleLength;
                processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
                processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
            }

            return details;
        }

        /// <summary>
        /// Prepare details to place order based on the recurring payment.
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceOrderContainer PrepareRecurringOrderDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceOrderContainer
            {
                IsRecurringShoppingCart = true,

                //Load initial order
                InitialOrder = _orderService.GetOrderById(processPaymentRequest.InitialOrderId)
            };
            if (details.InitialOrder == null)
                throw new ArgumentException("Initial order is not set for recurring payment");

            processPaymentRequest.PaymentMethodSystemName = details.InitialOrder.PaymentMethodSystemName;

            //customer
            details.Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //check whether customer is guest
            if (details.Customer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                throw new NopException("Anonymous checkout is not allowed");

            //customer currency
            details.CustomerCurrencyCode = details.InitialOrder.CustomerCurrencyCode;
            details.CustomerCurrencyRate = details.InitialOrder.CurrencyRate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(details.InitialOrder.CustomerLanguageId);
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            //billing address
            if (details.InitialOrder.BillingAddress == null)
                throw new NopException("Billing address is not available");

            details.BillingAddress = (Address)details.InitialOrder.BillingAddress.Clone();
            if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                throw new NopException($"Country '{details.BillingAddress.Country.Name}' is not allowed for billing");

            //checkout attributes
            details.CheckoutAttributesXml = details.InitialOrder.CheckoutAttributesXml;
            details.CheckoutAttributeDescription = details.InitialOrder.CheckoutAttributeDescription;

            //tax display type
            details.CustomerTaxDisplayType = details.InitialOrder.CustomerTaxDisplayType;

            //sub total
            details.OrderSubTotalInclTax = details.InitialOrder.OrderSubtotalInclTax;
            details.OrderSubTotalExclTax = details.InitialOrder.OrderSubtotalExclTax;
            details.OrderSubTotalDiscountExclTax = details.InitialOrder.OrderSubTotalDiscountExclTax;
            details.OrderSubTotalDiscountInclTax = details.InitialOrder.OrderSubTotalDiscountInclTax;

            //shipping info
            if (details.InitialOrder.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                details.PickUpInStore = details.InitialOrder.PickUpInStore;
                if (!details.PickUpInStore)
                {
                    if (details.InitialOrder.ShippingAddress == null)
                        throw new NopException("Shipping address is not available");

                    //clone shipping address
                    details.ShippingAddress = (Address)details.InitialOrder.ShippingAddress.Clone();
                    if (details.ShippingAddress.Country != null && !details.ShippingAddress.Country.AllowsShipping)
                        throw new NopException($"Country '{details.ShippingAddress.Country.Name}' is not allowed for shipping");
                }
                else
                    if (details.InitialOrder.PickupAddress != null)
                    details.PickupAddress = (Address)details.InitialOrder.PickupAddress.Clone();
                details.ShippingMethodName = details.InitialOrder.ShippingMethod;
                details.ShippingRateComputationMethodSystemName = details.InitialOrder.ShippingRateComputationMethodSystemName;
                details.ShippingStatus = ShippingStatus.NotYetShipped;
            }
            else
                details.ShippingStatus = ShippingStatus.ShippingNotRequired;

            //shipping total
            details.OrderShippingTotalInclTax = details.InitialOrder.OrderShippingInclTax;
            details.OrderShippingTotalExclTax = details.InitialOrder.OrderShippingExclTax;

            //payment total
            details.PaymentAdditionalFeeInclTax = details.InitialOrder.PaymentMethodAdditionalFeeInclTax;
            details.PaymentAdditionalFeeExclTax = details.InitialOrder.PaymentMethodAdditionalFeeExclTax;

            //tax total
            details.OrderTaxTotal = details.InitialOrder.OrderTax;

            //VAT number
            details.VatNumber = details.InitialOrder.VatNumber;

            //discount history (the same)
            foreach (var duh in details.InitialOrder.DiscountUsageHistory)
            {
                var d = _discountService.GetDiscountById(duh.DiscountId);
                if (d != null)
                    details.AppliedDiscounts.Add(d.MapDiscount());
            }

            //order total
            details.OrderDiscountAmount = details.InitialOrder.OrderDiscount;
            details.OrderTotal = details.InitialOrder.OrderTotal;
            processPaymentRequest.OrderTotal = details.OrderTotal;

            return details;
        }

        /// <summary>
        /// Save order and add order notes
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="processPaymentResult">Process payment result</param>
        /// <param name="details">Details</param>
        /// <returns>Order</returns>
        protected virtual Order SaveOrderDetails(ProcessPaymentRequest processPaymentRequest,
            ProcessPaymentResult processPaymentResult, PlaceOrderContainer details)
        {
            var order = new Order
            {
                StoreId = processPaymentRequest.StoreId,
                OrderGuid = processPaymentRequest.OrderGuid,
                CustomerId = details.Customer.Id,
                CustomerLanguageId = details.CustomerLanguage.Id,
                CustomerTaxDisplayType = details.CustomerTaxDisplayType,
                CustomerIp = _webHelper.GetCurrentIpAddress(),
                OrderSubtotalInclTax = details.OrderSubTotalInclTax,
                OrderSubtotalExclTax = details.OrderSubTotalExclTax,
                OrderSubTotalDiscountInclTax = details.OrderSubTotalDiscountInclTax,
                OrderSubTotalDiscountExclTax = details.OrderSubTotalDiscountExclTax,
                OrderShippingInclTax = details.OrderShippingTotalInclTax,
                OrderShippingExclTax = details.OrderShippingTotalExclTax,
                PaymentMethodAdditionalFeeInclTax = details.PaymentAdditionalFeeInclTax,
                PaymentMethodAdditionalFeeExclTax = details.PaymentAdditionalFeeExclTax,
                TaxRates = details.TaxRates,
                OrderTax = details.OrderTaxTotal,
                OrderTotal = details.OrderTotal,
                RefundedAmount = decimal.Zero,
                OrderDiscount = details.OrderDiscountAmount,
                CheckoutAttributeDescription = details.CheckoutAttributeDescription,
                CheckoutAttributesXml = details.CheckoutAttributesXml,
                CustomerCurrencyCode = details.CustomerCurrencyCode,
                CurrencyRate = details.CustomerCurrencyRate,
                AffiliateId = details.AffiliateId,
                OrderStatus = OrderStatus.Confirmed,
                AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
                CardType = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardType) : string.Empty,
                CardName = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardName) : string.Empty,
                CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardNumber) : string.Empty,
                MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(processPaymentRequest.CreditCardNumber)),
                CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardCvv2) : string.Empty,
                CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireMonth.ToString()) : string.Empty,
                CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireYear.ToString()) : string.Empty,
                PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                PaymentStatus = processPaymentResult.NewPaymentStatus,
                PaidDateUtc = null,
                BillingAddress = details.BillingAddress,
                ShippingAddress = details.ShippingAddress,
                ShippingStatus = details.ShippingStatus,
                ShippingMethod = details.ShippingMethodName,
                PickUpInStore = details.PickUpInStore,
                PickupAddress = details.PickupAddress,
                ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
                CustomValuesXml = processPaymentRequest.SerializeCustomValues(),
                VatNumber = details.VatNumber,
                CreatedOnUtc = DateTime.UtcNow,
                EstimatedTimeArrival = DateTime.UtcNow.AddDays(21),
                CustomOrderNumber = string.Empty
            };

            _orderService.InsertOrder(order);

            //generate and set custom order number
            order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
            _orderService.UpdateOrder(order);

            //reward points history
            if (details.RedeemedRewardPointsAmount > decimal.Zero)
            {
                _rewardPointService.AddRewardPointsHistoryEntry(details.Customer, -details.RedeemedRewardPoints, order.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
                    order, details.RedeemedRewardPointsAmount);
                _customerService.UpdateCustomer(details.Customer);
            }

            return order;
        }

        /// <summary>
        /// Send "order placed" notifications and save order notes
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void SendNotificationsAndSaveNotes(Order order)
        {
            //notes, messages
            AddOrderNote(order, _workContext.OriginalCustomerIfImpersonated != null
                ? $"Order placed by a store owner ('{_workContext.OriginalCustomerIfImpersonated.Email}'. ID = {_workContext.OriginalCustomerIfImpersonated.Id}) impersonating the customer."
                : "Order placed");

            //send email notifications
            var orderPlacedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderPlacedStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
            if (orderPlacedStoreOwnerNotificationQueuedEmailId > 0)
            {
                AddOrderNote(order, $"\"Order placed\" email (to store owner) has been queued. Queued email identifier: {orderPlacedStoreOwnerNotificationQueuedEmailId}.");
            }

            var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                _pdfService.PrintOrderToPdf(order) : null;
            var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                "order.pdf" : null;
            var orderPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
                .SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
            if (orderPlacedCustomerNotificationQueuedEmailId > 0)
            {
                AddOrderNote(order, $"\"Order placed\" email (to customer) has been queued. Queued email identifier: {orderPlacedCustomerNotificationQueuedEmailId}.");
            }

            var vendors = GetVendorsInOrder(order);
            foreach (var vendor in vendors)
            {
                var orderPlacedVendorNotificationQueuedEmailId = _workflowMessageService.SendOrderPlacedVendorNotification(order, vendor, _localizationSettings.DefaultAdminLanguageId);
                if (orderPlacedVendorNotificationQueuedEmailId > 0)
                {
                    AddOrderNote(order, $"\"Order placed\" email (to vendor) has been queued. Queued email identifier: {orderPlacedVendorNotificationQueuedEmailId}.");
                }
            }
        }
        /// <summary>
        /// Award (earn) reward points (for placing a new order)
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void AwardRewardPoints(Order order)
        {
            var totalForRewardPoints = _orderTotalCalculationService.CalculateApplicableOrderTotalForRewardPoints(order.OrderShippingInclTax, order.OrderTotal);
            var points = _orderTotalCalculationService.CalculateRewardPoints(order.Customer, totalForRewardPoints);
            if (points == 0)
                return;

            //Ensure that reward points were not added (earned) before. We should not add reward points if they were already earned for this order
            if (order.RewardPointsHistoryEntryId.HasValue)
                return;

            //check whether delay is set
            DateTime? activatingDate = null;
            if (_rewardPointsSettings.ActivationDelay > 0)
            {
                var delayPeriod = (RewardPointsActivatingDelayPeriod)_rewardPointsSettings.ActivationDelayPeriodId;
                var delayInHours = delayPeriod.ToHours(_rewardPointsSettings.ActivationDelay);
                activatingDate = DateTime.UtcNow.AddHours(delayInHours);
            }

            //add reward points
            order.RewardPointsHistoryEntryId = _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.EarnedForOrder"), order.CustomOrderNumber), activatingDate: activatingDate);

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Reduce (cancel) reward points (previously awarded for placing an order)
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ReduceRewardPoints(Order order)
        {
            var totalForRewardPoints = _orderTotalCalculationService.CalculateApplicableOrderTotalForRewardPoints(order.OrderShippingInclTax, order.OrderTotal);
            var points = _orderTotalCalculationService.CalculateRewardPoints(order.Customer, totalForRewardPoints);
            if (points == 0)
                return;

            //ensure that reward points were already earned for this order before
            if (!order.RewardPointsHistoryEntryId.HasValue)
                return;

            //get appropriate history entry
            var rewardPointsHistoryEntry = _rewardPointService.GetRewardPointsHistoryEntryById(order.RewardPointsHistoryEntryId.Value);
            if (rewardPointsHistoryEntry != null && rewardPointsHistoryEntry.CreatedOnUtc > DateTime.UtcNow)
            {
                //just delete the upcoming entry (points were not granted yet)
                _rewardPointService.DeleteRewardPointsHistoryEntry(rewardPointsHistoryEntry);
            }
            else
            {
                //or reduce reward points if the entry already exists
                _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -points, order.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.ReducedForOrder"), order.CustomOrderNumber));
            }

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Return back redeemded reward points to a customer (spent when placing an order)
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ReturnBackRedeemedRewardPoints(Order order)
        {
            //were some points redeemed when placing an order?
            if (order.RedeemedRewardPointsEntry == null)
                return;

            //return back
            _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -order.RedeemedRewardPointsEntry.Points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForOrder"), order.CustomOrderNumber));
            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Set IsActivated value for purchase gift cards for particular order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="activate">A value indicating whether to activate gift cards; true - activate, false - deactivate</param>
        protected virtual void SetActivatedValueForPurchasedGiftCards(Order order, bool activate)
        {
            var giftCards = _giftCardService.GetAllGiftCards(purchasedWithOrderId: order.Id,
                isGiftCardActivated: !activate);
            foreach (var gc in giftCards)
            {
                if (activate)
                {
                    //activate
                    var isRecipientNotified = gc.IsRecipientNotified;
                    if (gc.GiftCardType == GiftCardType.Virtual)
                    {
                        //send email for virtual gift card
                        if (!string.IsNullOrEmpty(gc.RecipientEmail) &&
                            !string.IsNullOrEmpty(gc.SenderEmail))
                        {
                            var customerLang = _languageService.GetLanguageById(order.CustomerLanguageId);
                            if (customerLang == null)
                                customerLang = _languageService.GetAllLanguages().FirstOrDefault();
                            if (customerLang == null)
                                throw new Exception("No languages could be loaded");
                            var queuedEmailId = _workflowMessageService.SendGiftCardNotification(gc, customerLang.Id);
                            if (queuedEmailId > 0)
                                isRecipientNotified = true;
                        }
                    }
                    gc.IsGiftCardActivated = true;
                    gc.IsRecipientNotified = isRecipientNotified;
                    _giftCardService.UpdateGiftCard(gc);
                }
                else
                {
                    //deactivate
                    gc.IsGiftCardActivated = false;
                    _giftCardService.UpdateGiftCard(gc);
                }
            }
        }

        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="os">New order status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        protected virtual void SetOrderStatus(Order order, OrderStatus os, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var prevOrderStatus = order.OrderStatus;
            if (prevOrderStatus == os)
                return;

            //set and save new order status
            order.OrderStatusId = (int)os;
            _orderService.UpdateOrder(order);

            //order notes, notifications
            AddOrderNote(order, $"Order status has been changed to {os.ToString()}");

            if (prevOrderStatus != OrderStatus.Complete &&
                os == OrderStatus.Complete
                && notifyCustomer)
            {
                //notification
                var orderCompletedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                    _pdfService.PrintOrderToPdf(order) : null;
                var orderCompletedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                    "order.pdf" : null;
                var orderCompletedCustomerNotificationQueuedEmailId = _workflowMessageService
                    .SendOrderCompletedCustomerNotification(order, order.CustomerLanguageId, orderCompletedAttachmentFilePath,
                    orderCompletedAttachmentFileName);
                if (orderCompletedCustomerNotificationQueuedEmailId > 0)
                {
                    AddOrderNote(order, $"\"Order completed\" email (to customer) has been queued. Queued email identifier: {orderCompletedCustomerNotificationQueuedEmailId}.");
                }
            }

            if (prevOrderStatus != OrderStatus.Cancelled &&
                os == OrderStatus.Cancelled
                && notifyCustomer)
            {
                //notification
                var orderCancelledCustomerNotificationQueuedEmailId = _workflowMessageService.SendOrderCancelledCustomerNotification(order, order.CustomerLanguageId);
                if (orderCancelledCustomerNotificationQueuedEmailId > 0)
                {
                    AddOrderNote(order, $"\"Order cancelled\" email (to customer) has been queued. Queued email identifier: {orderCancelledCustomerNotificationQueuedEmailId}.");

                }
            }

            //reward points
            if (order.OrderStatus == OrderStatus.Complete)
            {
                AwardRewardPoints(order);
            }
            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                ReduceRewardPoints(order);
            }

            //gift cards activation
            if (_orderSettings.ActivateGiftCardsAfterCompletingOrder && order.OrderStatus == OrderStatus.Complete)
            {
                SetActivatedValueForPurchasedGiftCards(order, true);
            }

            //gift cards deactivation
            if (_orderSettings.DeactivateGiftCardsAfterCancellingOrder && order.OrderStatus == OrderStatus.Cancelled)
            {
                SetActivatedValueForPurchasedGiftCards(order, false);
            }
        }

        /// <summary>
        /// Process order paid status
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ProcessOrderPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //raise event
            _eventPublisher.Publish(new OrderPaidEvent(order));

            //order paid email notification
            if (order.OrderTotal != decimal.Zero)
            {
                //we should not send it for free ($0 total) orders?
                //remove this "if" statement if you want to send it in this case

                var orderPaidAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                    _pdfService.PrintOrderToPdf(order) : null;
                var orderPaidAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                    "order.pdf" : null;
                _workflowMessageService.SendOrderPaidCustomerNotification(order, order.CustomerLanguageId,
                    orderPaidAttachmentFilePath, orderPaidAttachmentFileName);

                _workflowMessageService.SendOrderPaidStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
                var vendors = GetVendorsInOrder(order);
                foreach (var vendor in vendors)
                {
                    _workflowMessageService.SendOrderPaidVendorNotification(order, vendor, _localizationSettings.DefaultAdminLanguageId);
                }
                //TODO add "order paid email sent" order note
            }

            //customer roles with "purchased with product" specified
            ProcessCustomerRolesWithPurchasedProductSpecified(order, true);
        }

        /// <summary>
        /// Process customer roles with "Purchased with Product" property configured
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="add">A value indicating whether to add configured customer role; true - add, false - remove</param>
        protected virtual void ProcessCustomerRolesWithPurchasedProductSpecified(Order order, bool add)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //purchased product identifiers
            var purchasedProductIds = new List<int>();
            foreach (var orderItem in order.OrderItems)
            {
                //standard items
                purchasedProductIds.Add(orderItem.ProductId);

                //bundled (associated) products
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(orderItem.AttributesXml);
                foreach (var attributeValue in attributeValues)
                {
                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    {
                        purchasedProductIds.Add(attributeValue.AssociatedProductId);
                    }
                }
            }

            //list of customer roles
            var customerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Where(cr => purchasedProductIds.Contains(cr.PurchasedWithProductId))
                .ToList();

            if (!customerRoles.Any())
                return;

            var customer = order.Customer;
            foreach (var customerRole in customerRoles)
            {
                if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) == 0)
                {
                    //not in the list yet
                    if (add)
                    {
                        //add
                        customer.CustomerRoles.Add(customerRole);
                    }
                }
                else
                {
                    //already in the list
                    if (!add)
                    {
                        //remove
                        customer.CustomerRoles.Remove(customerRole);
                    }
                }
            }
            _customerService.UpdateCustomer(customer);
        }

        /// <summary>
        /// Get a list of vendors in order (order items)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Vendors</returns>
        protected virtual IList<Vendor> GetVendorsInOrder(Order order)
        {
            var vendors = new List<Vendor>();
            foreach (var orderItem in order.OrderItems)
            {
                var vendorId = orderItem.Product.VendorId;
                //find existing
                var vendor = vendors.FirstOrDefault(v => v.Id == vendorId);
                if (vendor == null)
                {
                    //not found. load by Id
                    vendor = _vendorService.GetVendorById(vendorId);
                    if (vendor != null && !vendor.Deleted && vendor.Active)
                    {
                        vendors.Add(vendor);
                    }
                }
            }

            return vendors;
        }

        /// <summary>
        /// Create recurring payment (the first payment)
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="order">Order</param>
        protected virtual void CreateFirstRecurringPayment(ProcessPaymentRequest processPaymentRequest, Order order)
        {
            var rp = new RecurringPayment
            {
                CycleLength = processPaymentRequest.RecurringCycleLength,
                CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
                TotalCycles = processPaymentRequest.RecurringTotalCycles,
                StartDateUtc = DateTime.UtcNow,
                IsActive = true,
                CreatedOnUtc = DateTime.UtcNow,
                InitialOrder = order,
            };
            _orderService.InsertRecurringPayment(rp);

            switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
            {
                case RecurringPaymentType.NotSupported:
                    //not supported
                    break;
                case RecurringPaymentType.Manual:
                    rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                    {
                        RecurringPayment = rp,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                    });
                    _orderService.UpdateRecurringPayment(rp);
                    break;
                case RecurringPaymentType.Automatic:
                    //will be created later (process is automated)
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Move shopping cart items to order items
        /// </summary>
        /// <param name="details">Place order container</param>
        /// <param name="order">Order</param>
        protected virtual void MoveShoppingCartItemsToOrderItems(PlaceOrderContainer details, Order order)
        {
            decimal weightCost = 0;
            foreach (var sc in details.Cart)
            {
                //prices
                var scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                var scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out decimal discountAmount,
                    out List<DiscountForCaching> scDiscounts, out int? _);
                var scUnitPriceInclTax =
                    _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.Customer, out decimal _);
                var scUnitPriceExclTax =
                    _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.Customer, out _);
                var scSubTotalInclTax =
                    _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.Customer, out _);
                var scSubTotalExclTax =
                    _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.Customer, out _);
                var discountAmountInclTax =
                    _taxService.GetProductPrice(sc.Product, discountAmount, true, details.Customer, out _);
                var discountAmountExclTax =
                    _taxService.GetProductPrice(sc.Product, discountAmount, false, details.Customer, out _);
                foreach (var disc in scDiscounts)
                    if (!details.AppliedDiscounts.ContainsDiscount(disc))
                        details.AppliedDiscounts.Add(disc);

                //attributes
                var attributeDescription =
                    _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.Customer);

                //var itemWeight = _shippingService.GetShoppingCartItemWeight(sc);
                var itemWeight = sc.Product.Weight;

                //save order item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    Order = order,
                    ProductId = sc.ProductId,
                    UnitPriceInclTax = scUnitPriceInclTax,
                    UnitPriceExclTax = scUnitPriceExclTax,
                    PriceInclTax = scSubTotalInclTax,
                    PriceExclTax = scSubTotalExclTax,
                    OriginalProductCost = _priceCalculationService.GetProductCost(sc.Product, sc.AttributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = sc.AttributesXml,
                    Quantity = sc.Quantity,
                    DiscountAmountInclTax = discountAmountInclTax,
                    DiscountAmountExclTax = discountAmountExclTax,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = sc.RentalStartDateUtc,
                    RentalEndDateUtc = sc.RentalEndDateUtc,
                    UnitPriceUsd = sc.UnitPriceUsd,
                    ExchangeRate = sc.ExchangeRate,
                    OrderingFee = sc.OrderingFee,
                    SaleOffPercent = sc.SaleOffPercent,
                    CurrencyId = sc.CurrencyId,
                    WeightCost = sc.WeightCost
                };
                order.OrderItems.Add(orderItem);
                _orderService.UpdateOrder(order);

                //gift cards
                //AddGiftCards(sc.Product, sc.AttributesXml, sc.Quantity, orderItem, scUnitPriceExclTax);

                //inventory
                _productService.AdjustInventory(sc.Product, -sc.Quantity, sc.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.PlaceOrder"),
                        order.Id));
                weightCost += sc.WeightCost;
            }

            order.WeightCost = weightCost;
            _orderService.UpdateOrder(order);
            //clear shopping cart
            details.Cart.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
        }

        /// <summary>
        /// Add gift cards
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">attributes XML</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="unitPriceExclTax">Unit price exclude tax, it set as amount if not set specific amount and product.OverriddenGiftCardAmount isn't set to</param>
        /// <param name="amount">Amount</param>
        protected virtual void AddGiftCards(Product product, string attributesXml, int quantity, OrderItem orderItem, decimal? unitPriceExclTax = null, decimal? amount = null)
        {
            if (!product.IsGiftCard)
                return;

            _productAttributeParser.GetGiftCardAttribute(attributesXml, out string giftCardRecipientName, out string giftCardRecipientEmail, out string giftCardSenderName, out string giftCardSenderEmail, out string giftCardMessage);

            for (var i = 0; i < quantity; i++)
            {
                _giftCardService.InsertGiftCard(new GiftCard
                {
                    GiftCardType = product.GiftCardType,
                    PurchasedWithOrderItem = orderItem,
                    Amount = amount ?? product.OverriddenGiftCardAmount ?? unitPriceExclTax ?? 0,
                    IsGiftCardActivated = false,
                    GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                    RecipientName = giftCardRecipientName,
                    RecipientEmail = giftCardRecipientEmail,
                    SenderName = giftCardSenderName,
                    SenderEmail = giftCardSenderEmail,
                    Message = giftCardMessage,
                    IsRecipientNotified = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get process payment result
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="details">Place order container</param>
        /// <returns></returns>
        protected virtual ProcessPaymentResult GetProcessPaymentResult(ProcessPaymentRequest processPaymentRequest, PlaceOrderContainer details)
        {
            //process payment
            ProcessPaymentResult processPaymentResult;
            //skip payment workflow if order total equals zero
            var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
            if (!skipPaymentWorkflow)
            {
                var paymentMethod =
                    _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                if (paymentMethod == null)
                    throw new NopException("Payment method couldn't be loaded");

                //ensure that payment method is active
                if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
                    throw new NopException("Payment method is not active");

                if (details.IsRecurringShoppingCart)
                {
                    //recurring cart
                    switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                    {
                        case RecurringPaymentType.NotSupported:
                            throw new NopException("Recurring payments are not supported by selected payment method");
                        case RecurringPaymentType.Manual:
                        case RecurringPaymentType.Automatic:
                            processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                            break;
                        default:
                            throw new NopException("Not supported recurring payment type");
                    }
                }
                else
                    //standard cart
                    processPaymentResult = _paymentService.ProcessPayment(processPaymentRequest);
            }
            else
                //payment is not required
                processPaymentResult = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };
            return processPaymentResult;
        }

        /// <summary>
        /// Save gift card usage history
        /// </summary>
        /// <param name="details">Place order container</param>
        /// <param name="order">Order</param>
        protected virtual void SaveGiftCardUsageHistory(PlaceOrderContainer details, Order order)
        {
            if (details.AppliedGiftCards == null || !details.AppliedGiftCards.Any())
                return;

            foreach (var agc in details.AppliedGiftCards)
            {
                agc.GiftCard.GiftCardUsageHistory.Add(new GiftCardUsageHistory
                {
                    GiftCard = agc.GiftCard,
                    UsedWithOrder = order,
                    UsedValue = agc.AmountCanBeUsed,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _giftCardService.UpdateGiftCard(agc.GiftCard);
            }
        }

        /// <summary>
        /// Save discount usage history
        /// </summary>
        /// <param name="details">PlaceOrderContainer</param>
        /// <param name="order">Order</param>
        protected virtual void SaveDiscountUsageHistory(PlaceOrderContainer details, Order order)
        {
            if (details.AppliedDiscounts == null || !details.AppliedDiscounts.Any())
                return;

            foreach (var discount in details.AppliedDiscounts)
            {
                var d = _discountService.GetDiscountById(discount.Id);
                if (d != null)
                {
                    _discountService.InsertDiscountUsageHistory(new DiscountUsageHistory
                    {
                        Discount = d,
                        Order = order,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void CheckOrderStatus(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.PaymentStatus == PaymentStatus.Paid && !order.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                order.PaidDateUtc = DateTime.UtcNow;
                _orderService.UpdateOrder(order);
            }

            switch (order.OrderStatus)
            {
                case OrderStatus.Confirmed:
                    if (order.PaymentStatus == PaymentStatus.Authorized ||
                        order.PaymentStatus == PaymentStatus.Paid)
                    {
                        SetOrderStatus(order, OrderStatus.Complete, false);
                    }

                    if (order.ShippingStatus == ShippingStatus.PartiallyShipped ||
                        order.ShippingStatus == ShippingStatus.Shipped ||
                        order.ShippingStatus == ShippingStatus.Delivered)
                    {
                        SetOrderStatus(order, OrderStatus.Complete, false);
                    }
                    break;
                //is order complete?
                case OrderStatus.Cancelled:
                case OrderStatus.Complete:
                    return;
            }

            if (order.PaymentStatus != PaymentStatus.Paid)
                return;

            bool completed;

            if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
            {
                //shipping is not required
                completed = true;
            }
            else
            {
                //shipping is required
                if (_orderSettings.CompleteOrderWhenDelivered)
                {
                    completed = order.ShippingStatus == ShippingStatus.Delivered;
                }
                else
                {
                    completed = order.ShippingStatus == ShippingStatus.Shipped ||
                                order.ShippingStatus == ShippingStatus.Delivered;
                }
            }

            if (completed)
            {
                SetOrderStatus(order, OrderStatus.Complete, true);
            }
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        public virtual PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));

            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);

                //var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                //if (processPaymentResult == null)
                //    throw new NopException("processPaymentResult is not available");

                //if (processPaymentResult.Success)
                //{
                //    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                //    result.PlacedOrder = order;

                //    //move shopping cart items to order items
                //    MoveShoppingCartItemsToOrderItems(details, order);

                //    //discount usage history
                //    SaveDiscountUsageHistory(details, order);

                //    //gift card usage history
                //    SaveGiftCardUsageHistory(details, order);

                //    //recurring orders
                //    if (details.IsRecurringShoppingCart)
                //    {
                //        CreateFirstRecurringPayment(processPaymentRequest, order);
                //    }

                //    //notifications
                //    SendNotificationsAndSaveNotes(order);

                //    //reset checkout data
                //    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                //    _customerActivityService.InsertActivity("PublicStore.PlaceOrder", _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id);

                //    //check order status
                //    CheckOrderStatus(order);

                //    //raise event       
                //    _eventPublisher.Publish(new OrderPlacedEvent(order));

                //    if (order.PaymentStatus == PaymentStatus.Paid)
                //        ProcessOrderPaid(order);
                //}
                //else
                //    foreach (var paymentError in processPaymentResult.Errors)
                //        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));

                var order = SaveOrderDetails(processPaymentRequest, new ProcessPaymentResult(), details);
                result.PlacedOrder = order;

                //move shopping cart items to order items
                MoveShoppingCartItemsToOrderItems(details, order);

                ////discount usage history
                //SaveDiscountUsageHistory(details, order);

                ////gift card usage history
                //SaveGiftCardUsageHistory(details, order);

                ////recurring orders
                //if (details.IsRecurringShoppingCart)
                //{
                //    CreateFirstRecurringPayment(processPaymentRequest, order);
                //}

                //notifications
                //SendNotificationsAndSaveNotes(order);

                //reset checkout data
                _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                _customerActivityService.InsertActivity("PublicStore.PlaceOrder", _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id);

                //check order status
                CheckOrderStatus(order);

                //raise event       
                _eventPublisher.Publish(new OrderPlacedEvent(order));

                if (order.PaymentStatus == PaymentStatus.Paid)
                    ProcessOrderPaid(order);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            if (result.Success)
                return result;

            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            _logger.Error(logError, customer: customer);

            return result;
        }

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        public virtual void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters)
        {
            if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                return;

            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

            //restore shopping cart from order items
            var restoredCart = updatedOrder.OrderItems.Select(orderItem => new ShoppingCartItem
            {
                Id = orderItem.Id,
                AttributesXml = orderItem.AttributesXml,
                Customer = updatedOrder.Customer,
                Product = orderItem.Product,
                Quantity = orderItem.Id == updatedOrderItem.Id ? updateOrderParameters.Quantity : orderItem.Quantity,
                RentalEndDateUtc = orderItem.RentalEndDateUtc,
                RentalStartDateUtc = orderItem.RentalStartDateUtc,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = updatedOrder.StoreId
            }).ToList();

            //get shopping cart item which has been updated
            var updatedShoppingCartItem = restoredCart.FirstOrDefault(shoppingCartItem => shoppingCartItem.Id == updatedOrderItem.Id);
            var itemDeleted = updatedShoppingCartItem == null;

            //validate shopping cart for warnings
            updateOrderParameters.Warnings.AddRange(_shoppingCartService.GetShoppingCartWarnings(restoredCart, string.Empty, false));
            if (!itemDeleted)
                updateOrderParameters.Warnings.AddRange(_shoppingCartService.GetShoppingCartItemWarnings(updatedOrder.Customer, updatedShoppingCartItem.ShoppingCartType,
                    updatedShoppingCartItem.Product, updatedOrder.StoreId, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.CustomerEnteredPrice,
                    updatedShoppingCartItem.RentalStartDateUtc, updatedShoppingCartItem.RentalEndDateUtc, updatedShoppingCartItem.Quantity, false));

            _orderTotalCalculationService.UpdateOrderTotals(updateOrderParameters, restoredCart);

            if (updateOrderParameters.PickupPoint != null)
            {
                updatedOrder.PickUpInStore = true;
                updatedOrder.PickupAddress = new Address
                {
                    Address1 = updateOrderParameters.PickupPoint.Address,
                    City = updateOrderParameters.PickupPoint.City,
                    Country = _countryService.GetCountryByTwoLetterIsoCode(updateOrderParameters.PickupPoint.CountryCode),
                    ZipPostalCode = updateOrderParameters.PickupPoint.ZipPostalCode,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                updatedOrder.ShippingMethod = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), updateOrderParameters.PickupPoint.Name);
                updatedOrder.ShippingRateComputationMethodSystemName = updateOrderParameters.PickupPoint.ProviderSystemName;
            }

            if (!itemDeleted)
            {
                updatedOrderItem.ItemWeight = _shippingService.GetShoppingCartItemWeight(updatedShoppingCartItem);
                updatedOrderItem.OriginalProductCost = _priceCalculationService.GetProductCost(updatedShoppingCartItem.Product, updatedShoppingCartItem.AttributesXml);
                updatedOrderItem.AttributeDescription = _productAttributeFormatter.FormatAttributes(updatedShoppingCartItem.Product,
                    updatedShoppingCartItem.AttributesXml, updatedOrder.Customer);

                //gift cards
                AddGiftCards(updatedShoppingCartItem.Product, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.Quantity, updatedOrderItem, updatedOrderItem.UnitPriceExclTax);
            }

            _orderService.UpdateOrder(updatedOrder);

            //discount usage history
            var discountUsageHistoryForOrder = _discountService.GetAllDiscountUsageHistory(null, updatedOrder.Customer.Id, updatedOrder.Id);
            foreach (var discount in updateOrderParameters.AppliedDiscounts)
            {
                if (!discountUsageHistoryForOrder.Any(history => history.DiscountId == discount.Id))
                {
                    var d = _discountService.GetDiscountById(discount.Id);
                    if (d != null)
                    {
                        _discountService.InsertDiscountUsageHistory(new DiscountUsageHistory
                        {
                            Discount = d,
                            Order = updatedOrder,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                    }
                }
            }

            CheckOrderStatus(updatedOrder);
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //check whether the order wasn't cancelled before
            //if it already was cancelled, then there's no need to make the following adjustments
            //(such as reward points, inventory, recurring payments)
            //they already was done when cancelling the order
            if (order.OrderStatus != OrderStatus.Cancelled)
            {
                //return (add) back redeemded reward points
                ReturnBackRedeemedRewardPoints(order);
                //reduce (cancel) back reward points (previously awarded for this order)
                ReduceRewardPoints(order);

                //cancel recurring payments
                var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: order.Id);
                foreach (var rp in recurringPayments)
                {
                    CancelRecurringPayment(rp);
                }

                //Adjust inventory for already shipped shipments
                //only products with "use multiple warehouses"
                foreach (var shipment in order.Shipments)
                {
                    foreach (var shipmentItem in shipment.ShipmentItems)
                    {
                        var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                        if (orderItem == null)
                            continue;

                        _productService.ReverseBookedInventory(orderItem.Product, shipmentItem,
                            string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
                    }
                }

                //Adjust inventory
                foreach (var orderItem in order.OrderItems)
                {
                    _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                        string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
                }
            }

            //deactivate gift cards
            if (_orderSettings.DeactivateGiftCardsAfterDeletingOrder)
                SetActivatedValueForPurchasedGiftCards(order, false);

            //add a note
            AddOrderNote(order, "Order has been deleted");

            //now delete an order
            _orderService.DeleteOrder(order);
        }

        /// <summary>
        /// Process next recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="paymentResult">Process payment result (info about last payment for automatic recurring payments)</param>
        /// <returns>Collection of errors</returns>
        public virtual IEnumerable<string> ProcessNextRecurringPayment(RecurringPayment recurringPayment, ProcessPaymentResult paymentResult = null)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            try
            {
                if (!recurringPayment.IsActive)
                    throw new NopException("Recurring payment is not active");

                var initialOrder = recurringPayment.InitialOrder;
                if (initialOrder == null)
                    throw new NopException("Initial order could not be loaded");

                var customer = initialOrder.Customer;
                if (customer == null)
                    throw new NopException("Customer could not be loaded");

                var nextPaymentDate = recurringPayment.NextPaymentDate;
                if (!nextPaymentDate.HasValue)
                    throw new NopException("Next payment date could not be calculated");

                //payment info
                var processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = initialOrder.StoreId,
                    CustomerId = customer.Id,
                    OrderGuid = Guid.NewGuid(),
                    InitialOrderId = initialOrder.Id,
                    RecurringCycleLength = recurringPayment.CycleLength,
                    RecurringCyclePeriod = recurringPayment.CyclePeriod,
                    RecurringTotalCycles = recurringPayment.TotalCycles,
                    CustomValues = initialOrder.DeserializeCustomValues()
                };

                //prepare order details
                var details = PrepareRecurringOrderDetails(processPaymentRequest);

                ProcessPaymentResult processPaymentResult;
                //skip payment workflow if order total equals zero
                var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
                if (!skipPaymentWorkflow)
                {
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        throw new NopException("Payment method couldn't be loaded");

                    if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
                        throw new NopException("Payment method is not active");

                    //Old credit card info
                    if (details.InitialOrder.AllowStoringCreditCardNumber)
                    {
                        processPaymentRequest.CreditCardType = _encryptionService.DecryptText(details.InitialOrder.CardType);
                        processPaymentRequest.CreditCardName = _encryptionService.DecryptText(details.InitialOrder.CardName);
                        processPaymentRequest.CreditCardNumber = _encryptionService.DecryptText(details.InitialOrder.CardNumber);
                        processPaymentRequest.CreditCardCvv2 = _encryptionService.DecryptText(details.InitialOrder.CardCvv2);
                        try
                        {
                            processPaymentRequest.CreditCardExpireMonth = Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationMonth));
                            processPaymentRequest.CreditCardExpireYear = Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationYear));
                        }
                        catch { }
                    }

                    //payment type
                    switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                    {
                        case RecurringPaymentType.NotSupported:
                            throw new NopException("Recurring payments are not supported by selected payment method");
                        case RecurringPaymentType.Manual:
                            processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                            break;
                        case RecurringPaymentType.Automatic:
                            //payment is processed on payment gateway site, info about last transaction in paymentResult parameter
                            processPaymentResult = paymentResult ?? new ProcessPaymentResult();
                            break;
                        default:
                            throw new NopException("Not supported recurring payment type");
                    }
                }
                else
                    processPaymentResult = paymentResult ?? new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    //save order details
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);

                    foreach (var orderItem in details.InitialOrder.OrderItems)
                    {
                        //save item
                        var newOrderItem = new OrderItem
                        {
                            OrderItemGuid = Guid.NewGuid(),
                            Order = order,
                            ProductId = orderItem.ProductId,
                            UnitPriceInclTax = orderItem.UnitPriceInclTax,
                            UnitPriceExclTax = orderItem.UnitPriceExclTax,
                            PriceInclTax = orderItem.PriceInclTax,
                            PriceExclTax = orderItem.PriceExclTax,
                            OriginalProductCost = orderItem.OriginalProductCost,
                            AttributeDescription = orderItem.AttributeDescription,
                            AttributesXml = orderItem.AttributesXml,
                            Quantity = orderItem.Quantity,
                            DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
                            DiscountAmountExclTax = orderItem.DiscountAmountExclTax,
                            DownloadCount = 0,
                            IsDownloadActivated = false,
                            LicenseDownloadId = 0,
                            ItemWeight = orderItem.ItemWeight,
                            RentalStartDateUtc = orderItem.RentalStartDateUtc,
                            RentalEndDateUtc = orderItem.RentalEndDateUtc
                        };
                        order.OrderItems.Add(newOrderItem);
                        _orderService.UpdateOrder(order);

                        //gift cards
                        AddGiftCards(orderItem.Product, orderItem.AttributesXml, orderItem.Quantity, newOrderItem, amount: orderItem.UnitPriceExclTax);

                        //inventory
                        _productService.AdjustInventory(orderItem.Product, -orderItem.Quantity, orderItem.AttributesXml,
                            string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.PlaceOrder"), order.Id));
                    }

                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));

                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                    //last payment succeeded
                    recurringPayment.LastPaymentFailed = false;

                    //next recurring payment
                    recurringPayment.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                    {
                        RecurringPayment = recurringPayment,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                    });
                    _orderService.UpdateRecurringPayment(recurringPayment);

                    return new List<string>();
                }

                //log errors
                var logError = processPaymentResult.Errors.Aggregate("Error while processing recurring order. ",
                    (current, next) => $"{current}Error {processPaymentResult.Errors.IndexOf(next) + 1}: {next}. ");
                _logger.Error(logError, customer: customer);

                if (!processPaymentResult.RecurringPaymentFailed)
                    return processPaymentResult.Errors;

                //set flag that last payment failed
                recurringPayment.LastPaymentFailed = true;
                _orderService.UpdateRecurringPayment(recurringPayment);

                if (_paymentSettings.CancelRecurringPaymentsAfterFailedPayment)
                {
                    //cancel recurring payment
                    CancelRecurringPayment(recurringPayment).ToList().ForEach(error => _logger.Error(error));

                    //notify a customer about cancelled payment
                    _workflowMessageService.SendRecurringPaymentCancelledCustomerNotification(recurringPayment, initialOrder.CustomerLanguageId);
                }
                else
                    //notify a customer about failed payment
                    _workflowMessageService.SendRecurringPaymentFailedCustomerNotification(recurringPayment, initialOrder.CustomerLanguageId);

                return processPaymentResult.Errors;
            }
            catch (Exception exc)
            {
                _logger.Error($"Error while processing recurring order. {exc.Message}", exc);
                throw;
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual IList<string> CancelRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return new List<string> { "Initial order could not be loaded" };

            var request = new CancelRecurringPaymentRequest();
            CancelRecurringPaymentResult result = null;
            try
            {
                request.Order = initialOrder;
                result = _paymentService.CancelRecurringPayment(request);
                if (result.Success)
                {
                    //update recurring payment
                    recurringPayment.IsActive = false;
                    _orderService.UpdateRecurringPayment(recurringPayment);

                    //add a note
                    initialOrder.OrderNotes.Add(new OrderNote
                    {
                        Note = "Recurring payment has been cancelled",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(initialOrder);

                    //notify a store owner
                    _workflowMessageService
                        .SendRecurringPaymentCancelledStoreOwnerNotification(recurringPayment,
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CancelRecurringPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = "";
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            initialOrder.OrderNotes.Add(new OrderNote
            {
                Note = $"Unable to cancel recurring payment. {error}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(initialOrder);

            //log it
            var logError = $"Error cancelling recurring payment. Order #{initialOrder.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public virtual bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return false;

            if (customerToValidate == null)
                return false;

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return false;

            var customer = recurringPayment.InitialOrder.Customer;
            if (customer == null)
                return false;

            if (initialOrder.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (!customerToValidate.IsAdmin())
            {
                if (customer.Id != customerToValidate.Id)
                    return false;
            }

            if (!recurringPayment.NextPaymentDate.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can retry last failed recurring payment
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>True if a customer can retry payment; otherwise false</returns>
        public virtual bool CanRetryLastRecurringPayment(Customer customer, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null || customer == null)
                return false;

            if (recurringPayment.InitialOrder == null || recurringPayment.InitialOrder.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (!recurringPayment.LastPaymentFailed || _paymentService.GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName) != RecurringPaymentType.Manual)
                return false;

            if (recurringPayment.InitialOrder.Customer == null || (!customer.IsAdmin() && recurringPayment.InitialOrder.Customer.Id != customer.Id))
                return false;

            return true;
        }

        /// <summary>
        /// Send a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void Ship(Shipment shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);
            order.OrderShippingInclTax += shipment.TotalShippingFee;
            order.OrderShippingExclTax += shipment.TotalShippingFee;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is already shipped");

            shipment.ShippedDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            //process products with "Multiple warehouse" support enabled
            foreach (var item in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(item.OrderItemId);
                _productService.BookReservedInventory(orderItem.Product, item.WarehouseId, -item.Quantity,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.Ship"), shipment.OrderId));

                order.OrderCurrentSubtotal += orderItem.UnitPriceInclTax * item.Quantity;
            }

            //check whether we have more items to ship
            if (order.HasItemsToAddToShipment() || order.HasItemsToShip())
                order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
            else
                order.ShippingStatusId = (int)ShippingStatus.Shipped;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Shipment# {shipment.Id} has been sent");

            if (notifyCustomer)
            {
                //notify customer
                var queuedEmailId = _workflowMessageService.SendShipmentSentCustomerNotification(shipment, order.CustomerLanguageId);
                if (queuedEmailId > 0)
                {
                    AddOrderNote(order, $"\"Shipped\" email (to customer) has been queued. Queued email identifier: {queuedEmailId}.");
                }
            }

            //event
            _eventPublisher.PublishShipmentSent(shipment);

            //check order status
            CheckOrderStatus(order);
        }
        public virtual void ShipManual(ShipmentManual shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));


            if (shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is already shipped");

            shipment.ShippedDateUtc = DateTime.UtcNow;
            _shipmentManualService.UpdateShipmentManual(shipment);

            //process products with "Multiple warehouse" support enabled
            foreach (var item in shipment.ShipmentManualItems)
            {
                var orderItem = _orderService.GetOrderItemById(item.OrderItemId);
                _productService.BookReservedInventory(orderItem.Product, item.WarehouseId, -item.Quantity,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.Ship"), orderItem.Order.Id));

                var order = _orderService.GetOrderById(orderItem.OrderId);

                if (order != null)
                {
                    order.OrderCurrentSubtotal += orderItem.UnitPriceInclTax * item.Quantity;

                    //check whether we have more items to ship
                    if (order.HasItemsToAddToShipment() || order.HasItemsToShip())
                        order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
                    else
                        order.ShippingStatusId = (int)ShippingStatus.Shipped;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, $"Shipment# {shipment.Id} has been sent");

                    //check order status
                    CheckOrderStatus(order);
                }
            }


        }

        /// <summary>
        /// Marks a shipment as delivered
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void Deliver(Shipment shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = shipment.Order;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (!shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is not shipped yet");

            if (shipment.DeliveryDateUtc.HasValue)
                throw new Exception("This shipment is already delivered");

            shipment.DeliveryDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            if (!order.HasItemsToAddToShipment() && !order.HasItemsToShip() && !order.HasItemsToDeliver())
                order.ShippingStatusId = (int)ShippingStatus.Delivered;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Shipment# {shipment.Id} has been delivered");

            if (notifyCustomer)
            {
                //send email notification
                var queuedEmailId = _workflowMessageService.SendShipmentDeliveredCustomerNotification(shipment, order.CustomerLanguageId);
                if (queuedEmailId > 0)
                {
                    AddOrderNote(order, $"\"Delivered\" email (to customer) has been queued. Queued email identifier: {queuedEmailId}.");
                }
            }

            //event
            _eventPublisher.PublishShipmentDelivered(shipment);

            //check order status
            CheckOrderStatus(order);
        }

        public virtual void DeliverManual(ShipmentManual shipment, bool notifyCustomer)
        {
            
            shipment.DeliveryDateUtc = DateTime.UtcNow;
            _shipmentManualService.UpdateShipmentManual(shipment);

            foreach (var shipmentManualItem in shipment.ShipmentManualItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentManualItem.OrderItemId);
                if (orderItem != null)
                {
                    orderItem.DeliveryDateUtc = DateTime.UtcNow;
                    _orderService.UpdateOrderItem(orderItem);

                }
                //var order = _orderService.GetOrderById(shipmentManualItem.OrderItem.OrderId);
                //if (order != null)
                //{
                //    if (!order.HasItemsToAddToShipment() && !order.HasItemsToShip() && !order.HasItemsToDeliver())
                //        order.ShippingStatusId = (int)ShippingStatus.Delivered;
                //    _orderService.UpdateOrder(order);

                //    //add a note
                //    AddOrderNote(order, $"Shipment# {shipment.Id} has been delivered");

                //    //check order status
                //    CheckOrderStatus(order);
                //}

            }
        }

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public virtual bool CanCancelOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            return true;
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void CancelOrder(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanCancelOrder(order))
                throw new NopException("Cannot do cancel for order.");

            //cancel order
            SetOrderStatus(order, OrderStatus.Cancelled, notifyCustomer);

            //add a note
            AddOrderNote(order, "Order has been cancelled");

            //return (add) back redeemded reward points
            ReturnBackRedeemedRewardPoints(order);

            //cancel recurring payments
            var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: order.Id);
            foreach (var rp in recurringPayments)
            {
                CancelRecurringPayment(rp);
            }

            //Adjust inventory for already shipped shipments
            //only products with "use multiple warehouses"
            foreach (var shipment in order.Shipments)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    _productService.ReverseBookedInventory(orderItem.Product, shipmentItem,
                        string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
                }
            }
            //Adjust inventory
            foreach (var orderItem in order.OrderItems)
            {
                _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
            }

            _eventPublisher.Publish(new OrderCancelledEvent(order));
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public virtual bool CanMarkOrderAsAuthorized(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Pending)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void MarkAsAuthorized(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.PaymentStatusId = (int)PaymentStatus.Authorized;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, "Order has been marked as authorized");

            //check order status
            CheckOrderStatus(order);
        }

        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public virtual bool CanCapture(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.Confirmed)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportCapture(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Capture an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Capture(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanCapture(order))
                throw new NopException("Cannot do capture for order.");

            var request = new CapturePaymentRequest();
            CapturePaymentResult result = null;
            try
            {
                //old info from placing order
                request.Order = order;
                result = _paymentService.Capture(request);

                if (result.Success)
                {
                    var paidDate = order.PaidDateUtc;
                    if (result.NewPaymentStatus == PaymentStatus.Paid)
                        paidDate = DateTime.UtcNow;

                    order.CaptureTransactionId = result.CaptureTransactionId;
                    order.CaptureTransactionResult = result.CaptureTransactionResult;
                    order.PaymentStatus = result.NewPaymentStatus;
                    order.PaidDateUtc = paidDate;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, "Order has been captured");

                    CheckOrderStatus(order);

                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        ProcessOrderPaid(order);
                    }
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CapturePaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = "";
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to capture order. {error}");

            //log it
            var logError = $"Error capturing order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public virtual bool CanMarkOrderAsPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.Refunded ||
                order.PaymentStatus == PaymentStatus.Voided)
                return false;

            return true;
        }

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void MarkOrderAsPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanMarkOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            order.PaymentStatusId = (int)PaymentStatus.Paid;
            order.PaidDateUtc = DateTime.UtcNow;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, "Order has been marked as paid");

            CheckOrderStatus(order);

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                ProcessOrderPaid(order);
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanRefund(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (order.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in order to disallow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Paid &&
                _paymentService.SupportRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Refund(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanRefund(order))
                throw new NopException("Cannot do refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Order = order;
                request.AmountToRefund = order.OrderTotal;
                request.IsPartialRefund = false;
                result = _paymentService.Refund(request);
                if (result.Success)
                {
                    //total amount refunded
                    var totalAmountRefunded = order.RefundedAmount + request.AmountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.PaymentStatus = result.NewPaymentStatus;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, $"Order has been refunded. Amount = {request.AmountToRefund}");

                    //check order status
                    CheckOrderStatus(order);

                    //notifications
                    var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, request.AmountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifier: {orderRefundedStoreOwnerNotificationQueuedEmailId}.");
                    }
                    var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedCustomerNotification(order, request.AmountToRefund, order.CustomerLanguageId);
                    if (orderRefundedCustomerNotificationQueuedEmailId > 0)
                    {
                        AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifier: {orderRefundedCustomerNotificationQueuedEmailId}.");
                    }

                    //raise event       
                    _eventPublisher.Publish(new OrderRefundedEvent(order, request.AmountToRefund));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = "";
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to refund order. {error}");

            //log it
            var logError = $"Error refunding order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        public virtual bool CanRefundOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (order.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in order to disallow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //     return false;

            if (order.PaymentStatus == PaymentStatus.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void RefundOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanRefundOffline(order))
                throw new NopException("You can't refund this order");

            //amout to refund
            var amountToRefund = order.OrderTotal;

            //total amount refunded
            var totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatus = PaymentStatus.Refunded;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Order has been marked as refunded. Amount = {amountToRefund}");

            //check order status
            CheckOrderStatus(order);

            //notifications
            var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
            {
                AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifier: {orderRefundedStoreOwnerNotificationQueuedEmailId}.");
            }
            var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
            if (orderRefundedCustomerNotificationQueuedEmailId > 0)
            {
                AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifier: {orderRefundedCustomerNotificationQueuedEmailId}.");
            }

            //raise event       
            _eventPublisher.Publish(new OrderRefundedEvent(order, amountToRefund));
        }

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanPartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            var canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded) &&
                _paymentService.SupportPartiallyRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> PartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanPartiallyRefund(order, amountToRefund))
                throw new NopException("Cannot do partial refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Order = order;
                request.AmountToRefund = amountToRefund;
                request.IsPartialRefund = true;

                result = _paymentService.Refund(request);

                if (result.Success)
                {
                    //total amount refunded
                    var totalAmountRefunded = order.RefundedAmount + amountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    //mark payment status as 'Refunded' if the order total amount is fully refunded
                    order.PaymentStatus = order.OrderTotal == totalAmountRefunded && result.NewPaymentStatus == PaymentStatus.PartiallyRefunded ? PaymentStatus.Refunded : result.NewPaymentStatus;
                    _orderService.UpdateOrder(order);


                    //add a note
                    AddOrderNote(order, $"Order has been partially refunded. Amount = {amountToRefund}");

                    //check order status
                    CheckOrderStatus(order);

                    //notifications
                    var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifier: {orderRefundedStoreOwnerNotificationQueuedEmailId}.");
                    }
                    var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
                    if (orderRefundedCustomerNotificationQueuedEmailId > 0)
                    {
                        AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifier: {orderRefundedCustomerNotificationQueuedEmailId}.");
                    }

                    //raise event       
                    _eventPublisher.Publish(new OrderRefundedEvent(order, amountToRefund));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = "";
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to partially refund order. {error}");

            //log it
            var logError = $"Error refunding order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        public virtual bool CanPartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            var canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public virtual void PartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanPartiallyRefundOffline(order, amountToRefund))
                throw new NopException("You can't partially refund (offline) this order");

            //total amount refunded
            var totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            //mark payment status as 'Refunded' if the order total amount is fully refunded
            order.PaymentStatus = order.OrderTotal == totalAmountRefunded ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Order has been marked as partially refunded. Amount = {amountToRefund}");

            //check order status
            CheckOrderStatus(order);

            //notifications
            var orderRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (orderRefundedStoreOwnerNotificationQueuedEmailId > 0)
            {
                AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifier: {orderRefundedStoreOwnerNotificationQueuedEmailId}.");
            }
            var orderRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
            if (orderRefundedCustomerNotificationQueuedEmailId > 0)
            {
                AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifier: {orderRefundedCustomerNotificationQueuedEmailId}.");
            }
            //raise event       
            _eventPublisher.Publish(new OrderRefundedEvent(order, amountToRefund));
        }

        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public virtual bool CanVoid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportVoid(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Voided order</returns>
        public virtual IList<string> Void(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanVoid(order))
                throw new NopException("Cannot do void for order.");

            var request = new VoidPaymentRequest();
            VoidPaymentResult result = null;
            try
            {
                request.Order = order;
                result = _paymentService.Void(request);

                if (result.Success)
                {
                    //update order info
                    order.PaymentStatus = result.NewPaymentStatus;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, "Order has been voided");

                    //check order status
                    CheckOrderStatus(order);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new VoidPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = "";
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to voiding order. {error}");

            //log it
            var logError = $"Error voiding order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        public virtual bool CanVoidOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void VoidOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order.PaymentStatusId = (int)PaymentStatus.Voided;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, "Order has been marked as voided");

            //check order status
            CheckOrderStatus(order);
        }

        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void ReOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //move shopping cart items (if possible)
            foreach (var orderItem in order.OrderItems)
            {
                _shoppingCartService.AddToCart(order.Customer, orderItem.Product,
                    ShoppingCartType.ShoppingCart, order.StoreId,
                    orderItem.AttributesXml, orderItem.UnitPriceExclTax,
                    orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                    orderItem.Quantity, false);
            }

            //set checkout attributes
            //comment the code below if you want to disable this functionality
            _genericAttributeService.SaveAttribute(order.Customer, SystemCustomerAttributeNames.CheckoutAttributes, order.CheckoutAttributesXml, order.StoreId);
        }

        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public virtual bool IsReturnRequestAllowed(Order order)
        {
            if (!_orderSettings.ReturnRequestsEnabled)
                return false;

            if (order == null || order.Deleted)
                return false;

            //status should be complete
            if (order.OrderStatus != OrderStatus.Complete)
                return false;

            //validate allowed number of days
            if (_orderSettings.NumberOfDaysReturnRequestAvailable > 0)
            {
                var daysPassed = (DateTime.UtcNow - order.CreatedOnUtc).TotalDays;
                if (daysPassed >= _orderSettings.NumberOfDaysReturnRequestAvailable)
                    return false;
            }

            //ensure that we have at least one returnable product
            return order.OrderItems.Any(oi => !oi.Product.NotReturnable);
        }

        /// <summary>
        /// Validate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        public virtual bool ValidateMinOrderSubtotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            //min order amount sub-total validation
            if (cart.Any() && _orderSettings.MinOrderSubtotalAmount > decimal.Zero)
            {
                //subtotal
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, _orderSettings.MinOrderSubtotalAmountIncludingTax, out decimal _, out List<DiscountForCaching> _, out decimal subTotalWithoutDiscountBase, out decimal _);

                if (subTotalWithoutDiscountBase < _orderSettings.MinOrderSubtotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public virtual bool ValidateMinOrderTotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (cart.Any() && _orderSettings.MinOrderTotalAmount > decimal.Zero)
            {
                var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart);
                if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value < _orderSettings.MinOrderTotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether payment workflow is required
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool? useRewardPoints = null)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var result = true;

            //check whether order total equals zero
            var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, useRewardPoints: useRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        #endregion
    }
}
