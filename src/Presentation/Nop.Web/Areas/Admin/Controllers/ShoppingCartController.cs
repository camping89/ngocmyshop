using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Extensions;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShoppingCartController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IDiscountService _discountService;
        private readonly IGiftCardService _giftCardService;
        private readonly IDateRangeService _dateRangeService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDownloadService _downloadService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly OrderSettings _orderSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IShippingService _shippingService;
        private readonly CurrencySettings _currencySettings;
        #endregion

        #region Ctor

        public ShoppingCartController(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IStoreService storeService,
            ITaxService taxService,
            IPriceCalculationService priceCalculationService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IProductAttributeFormatter productAttributeFormatter, IShoppingCartModelFactory shoppingCartModelFactory, IProductService productService, IWorkContext workContext, IStoreContext storeContext, IShoppingCartService shoppingCartService, IPictureService pictureService, IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser, ICurrencyService currencyService, ICheckoutAttributeParser checkoutAttributeParser, IDiscountService discountService, IGiftCardService giftCardService, IDateRangeService dateRangeService, ICheckoutAttributeService checkoutAttributeService, IWorkflowMessageService workflowMessageService, IDownloadService downloadService, IStaticCacheManager cacheManager, IWebHelper webHelper, ICustomerActivityService customerActivityService, IGenericAttributeService genericAttributeService, MediaSettings mediaSettings, ShoppingCartSettings shoppingCartSettings, OrderSettings orderSettings, CaptchaSettings captchaSettings, CustomerSettings customerSettings, IProductModelFactory productModelFactory, VendorSettings vendorSettings, ICategoryService categoryService, IVendorService vendorService, IManufacturerService manufacturerService, IShippingService shippingService, CurrencySettings currencySettings)
        {
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._storeService = storeService;
            this._taxService = taxService;
            this._priceCalculationService = priceCalculationService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._productAttributeFormatter = productAttributeFormatter;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _currencyService = currencyService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _discountService = discountService;
            _giftCardService = giftCardService;
            _dateRangeService = dateRangeService;
            _checkoutAttributeService = checkoutAttributeService;
            _workflowMessageService = workflowMessageService;
            _downloadService = downloadService;
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _customerActivityService = customerActivityService;
            _genericAttributeService = genericAttributeService;
            _mediaSettings = mediaSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _orderSettings = orderSettings;
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _productModelFactory = productModelFactory;
            _vendorSettings = vendorSettings;
            _categoryService = categoryService;
            _vendorService = vendorService;
            _manufacturerService = manufacturerService;
            _shippingService = shippingService;
            _currencySettings = currencySettings;
        }

        #endregion

        #region Methods

        //shopping carts
        public virtual IActionResult CurrentCarts()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult CurrentCarts(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedKendoGridJson();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.ShoppingCart,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new ShoppingCartModel
                {
                    CustomerId = x.Id,
                    CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    TotalItems = x.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList().GetTotalProducts()
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult GetCartDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedKendoGridJson();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    var store = _storeService.GetStoreById(sci.StoreId);
                    var sciModel = new ShoppingCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = sci.Product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml, sci.Customer),
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out decimal taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }

        //wishlists
        public virtual IActionResult CurrentWishlists()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult CurrentWishlists(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedKendoGridJson();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.Wishlist,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new ShoppingCartModel
                {
                    CustomerId = x.Id,
                    CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    TotalItems = x.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList().GetTotalProducts()
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult GetWishlistDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedKendoGridJson();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    var store = _storeService.GetStoreById(sci.StoreId);
                    var sciModel = new ShoppingCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = sci.Product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml, sci.Customer),
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out decimal taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }

        #endregion


        #region Utilities

        protected virtual void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, IFormCollection form, Customer customer = null)
        {
            if (customer == null)
            {
                customer = _workContext.CurrentCustomer;
            }
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !cart.RequiresShipping(_productService, _productAttributeParser);
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var controlId = $"checkout_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Parse product attributes on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="errors">Errors</param>
        /// <returns>Parsed attributes</returns>
        protected virtual string ParseProductAttributes(Product product, IFormCollection form, List<string> errors)
        {
            //product attributes
            var attributesXml = GetProductAttributesXml(product, form, errors);

            //gift cards
            AddGiftCardsAttributesXml(product, form, ref attributesXml);

            return attributesXml;
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        protected virtual void ParseRentalDates(Product product, IFormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var startControlId = $"rental_start_date_{product.Id}";
            var endControlId = $"rental_end_date_{product.Id}";
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        protected virtual void AddGiftCardsAttributesXml(Product product, IFormCollection form, ref string attributesXml)
        {
            if (!product.IsGiftCard) return;

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"giftcard_{product.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        }

        protected virtual string GetProductAttributesXml(Product product, IFormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                        (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[$"product_attribute_{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                            (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                    (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                            }
                            catch
                            {
                            }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }
            return attributesXml;
        }

        protected virtual void SaveItem(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, Product product,
           ShoppingCartType cartType, string attributes, decimal customerEnteredPriceConverted, DateTime? rentalStartDate,
           DateTime? rentalEndDate, int quantity, Customer customer, decimal adjustusdPrice = Decimal.Zero)
        {
            if (customer != null)
            {
                if (updatecartitem == null)
                {

                    //add to the cart
                    addToCartWarnings.AddRange(_shoppingCartService.AddToCart(customer,
                        product, cartType, _storeContext.CurrentStore.Id,
                        attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true));
                }
                else
                {
                    var cart = customer.ShoppingCartItems
                        .Where(x => x.ShoppingCartType == updatecartitem.ShoppingCartType)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();
                    var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                        cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate);
                    if (otherCartItemWithSameParameters != null &&
                        otherCartItemWithSameParameters.Id == updatecartitem.Id)
                    {
                        //ensure it's some other shopping cart item
                        otherCartItemWithSameParameters = null;
                    }
                    //update existing item
                    addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(customer,
                        updatecartitem.Id, attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true, currencyId: updatecartitem.CurrencyId));
                    if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                    {
                        //delete the same shopping cart item (the other one)
                        _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                    }
                }
            }
        }


        #endregion
        #region Admin Create Order Manual

        public virtual IActionResult CreateOrder(int customerId = 0, string activetab = "", bool udOrder = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var modelResult = new OrderCreateOrUpdateModel();
            var model = new ProductListModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null,
                AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts
            };
            modelResult.CustomerId = customerId;
            if (customerId > 0)
            {
                var customer = _customerService.GetCustomerById(customerId);
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var shoppingCartModel = new Web.Models.ShoppingCart.ShoppingCartModel();
                shoppingCartModel.CurrencyCurrent = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId, false);
                try
                {
                    var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId, false);
                    shoppingCartModel.PrimaryExchangeCurrency = primaryStoreCurrency ?? throw new NopException("Primary store currency is not set");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
                modelResult.ShoppingCartModel = _shoppingCartModelFactory.PrepareShoppingCartModel(shoppingCartModel, cart, customer: customer);
                modelResult.CustomerFullName = $"<strong>{customer.GetFullName()}</strong> - Phone: <strong>{customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone)}</strong> - Facebook: <strong>{customer.GetAttribute<string>(SystemCustomerAttributeNames.LinkFacebook1)}</strong>";
                var customerAddress = customer.Addresses.OrderBy(_ => _.CreatedOnUtc).FirstOrDefault();
                if (customerAddress != null)
                {
                    modelResult.CustomerAddress = customerAddress.Address1;
                    modelResult.CustomerWard = customerAddress.Ward;
                    modelResult.CustomerDistrict = customerAddress.District;
                    modelResult.CustomerCity = customerAddress.City;
                }
            }
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var wh in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });
            modelResult.ProductListModel = model;
            modelResult.SetUpdateOrder = udOrder;
            if (!string.IsNullOrEmpty(activetab))
            {
                SaveSelectedTabName(activetab);
            }

            modelResult.Customer = _customerService.GetCustomerById(customerId);
            ViewBag.CustomerId = customerId;
            return View(modelResult);
        }

        [HttpPost]
        public virtual IActionResult CreateOrder(OrderCreateOrUpdateModel model)
        {

            SaveSelectedTabName();
            return View("CreateOrder");
        }


        #endregion

        #region Shopping cart

        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public virtual IActionResult AddProductToCart_Catalog(ICollection<int> productIds, int customerId,
            int quantity, bool forceredirection = false)
        {
            var cartType = ShoppingCartType.ShoppingCart;

            foreach (var productId in productIds)
            {
                var product = _productService.GetProductById(productId);
                if (product == null)
                    //no product found
                    return Json(new
                    {
                        success = false,
                        message = "No product found with the specified ID"
                    });

                //we can add only simple products
                if (product.ProductType != ProductType.SimpleProduct)
                {
                    return Json(new
                    {
                        success = false,
                        message = "We can add only simple products"
                    });
                }

                //products with "minimum order quantity" more than a specified qty
                if (product.OrderMinimumQuantity > quantity)
                {
                    //we cannot add to the cart such products from category pages
                    //it can confuse customers. That's why we redirect customers to the product details page
                    return Json(new
                    {
                        success = false,
                        message = "Products with \"minimum order quantity\" more than a specified qty"
                    });
                }

                if (product.CustomerEntersPrice)
                {
                    //cannot be added to the cart (requires a customer to enter price)
                    return Json(new
                    {
                        success = false,
                        message = "Cannot be added to the cart (requires a customer to enter price)"
                    });
                }

                if (product.IsRental)
                {
                    //
                    return Json(new
                    {
                        success = false,
                        message = "Rental products require start/end dates to be entered"
                    });
                }

                var allowedQuantities = product.ParseAllowedQuantities();
                if (allowedQuantities.Length > 0)
                {
                    //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                    return Json(new
                    {
                        success = false,
                        message = "Cannot be added to the cart (requires a customer to select a quantity from dropdownlist)"
                    });
                }

                //allow a product to be added to the cart when all attributes are with "read-only checkboxes" type
                var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                if (productAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
                {
                    //product has some attributes. let a customer see them
                    return Json(new
                    {
                        success = false,
                        message = "Product has some attributes. let a customer see them"
                    });
                }

                //creating XML for "read-only checkboxes" attributes
                var attXml = productAttributes.Aggregate(string.Empty, (attributesXml, attribute) =>
                {
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    foreach (var selectedAttributeId in attributeValues
                        .Where(v => v.IsPreSelected)
                        .Select(v => v.Id)
                        .ToList())
                    {
                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }
                    return attributesXml;
                });
                //TODO: Get customer choosed
                Customer customer = _customerService.GetCustomerById(customerId);
                //get standard warnings without attribute validations
                //first, try to find existing shopping cart item
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == cartType)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
                //if we already have the same product in the cart, then use the total quantity to validate
                var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
                var addToCartWarnings = _shoppingCartService
                    .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, cartType,
                        product, _storeContext.CurrentStore.Id, string.Empty,
                        decimal.Zero, null, null, quantityToValidate, false, true, false, false, false);
                if (addToCartWarnings.Any())
                {
                    //cannot be added to the cart
                    //let's display standard warnings
                    return Json(new
                    {
                        success = false,
                        message = addToCartWarnings.ToArray()
                    });
                }

                //now let's try adding product to the cart (now including product attribute validation, etc)
                addToCartWarnings = _shoppingCartService.AddToCart(customer: customer,
                    product: product,
                    shoppingCartType: cartType,
                    storeId: _storeContext.CurrentStore.Id,
                    attributesXml: attXml,
                    quantity: quantity);
            }
            //added to the cart
            return Json(new
            {
                success = true
            });
        }

        public IActionResult AddProductToCart_Details(int productId, int customerId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);


            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("HomePage");

                return RedirectToRoute("Product", new { SeName = parentGroupedProduct.GetSeName() });
            }
            var customer = _customerService.GetCustomerById(customerId);
            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = customer.ShoppingCartItems
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            //model
            var model = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);
            model.CustomerId = customerId;
            model.MetaTitle = $"Thêm vào giỏ hàng - {customer.GetFullName()} - Sku: {product.Sku}";
            ViewData["CustomerId"] = customerId;
            var productTemplateViewPath = _productModelFactory.PrepareProductTemplateViewPath(product);
            return View(productTemplateViewPath, model);
        }
        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        public virtual IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, int customerId, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            var customer = _customerService.GetCustomerById(customerId);
            //update existing shopping cart item
            var updatecartitemid = 0;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                //search with the same cart type as specified
                var cart = customer.ShoppingCartItems
                    .Where(x => x.ShoppingCartTypeId == shoppingCartTypeId)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
            }

            //customer entered price
            var customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (var formKey in form.Keys)
                {
                    if (formKey.Equals($"addtocart_{productId}.CustomerEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (decimal.TryParse(form[formKey], out decimal customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            var adjustusdPrice = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (var formKey in form.Keys)
                {
                    if (formKey.Equals($"addtocart_{productId}.adjustusd", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (decimal.TryParse(form[formKey], out decimal adjustusd))
                        {
                            adjustusdPrice = adjustusd;
                        }
                        break;
                    }
                }
            }

            //quantity
            var quantity = 1;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            var addToCartWarnings = new List<string>();

            //product and gift card attributes
            var attributes = ParseProductAttributes(product, form, addToCartWarnings);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                updatecartitem.ShoppingCartType;

            //product.CurrencyId;
            SaveItem(updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity, customer, adjustusdPrice);
            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart",
                _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

            var productTemplateViewPath = _productModelFactory.PrepareProductTemplateViewPath(product);
            ViewBag.RefreshPage = true;
            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            //model
            var model = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);
            model.CustomerId = customerId;
            ViewData["CustomerId"] = customerId;
            //return View(productTemplateViewPath,model);
            //return result
            return View(productTemplateViewPath, model);
            //return GetProductToCartDetails(addToCartWarnings, cartType, product);
        }
        protected virtual IActionResult GetProductToCartDetails(List<string> addToCartWarnings, ShoppingCartType cartType,
           Product product)
        {
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist",
                            _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(
                            _localizationService.GetResource("Wishlist.HeaderQuantity"),
                            _workContext.CurrentCustomer.ShoppingCartItems
                                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                                .LimitPerStore(_storeContext.CurrentStore.Id)
                                .ToList()
                                .GetTotalProducts());

                        return Json(new
                        {
                            success = true,
                            message = string.Format(
                                _localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"),
                                Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart",
                            _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

                        return Json(new
                        {
                            redirect = Url.RouteUrl("ShoppingCart")
                        });
                    }
            }
        }
        //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the product details pages
        [HttpPost]
        public virtual IActionResult ProductDetails_AttributeChange(int productId, int customerId, bool validateAttributeConditions,
            bool loadPicture, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = ParseProductAttributes(product, form, errors);
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Customer not exist in system"
                });
            }
            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            var sku = product.FormatSku(attributeXml, _productAttributeParser);
            var mpn = product.FormatMpn(attributeXml, _productAttributeParser);
            var gtin = product.FormatGtin(attributeXml, _productAttributeParser);

            //price
            var price = "";
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                //we do not calculate price of "customer enters price" option is enabled
                List<DiscountForCaching> scDiscounts;
                var finalPrice = _priceCalculationService.GetUnitPrice(product,
                    customer,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out decimal _, out scDiscounts);
                //var finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out decimal _);
                //var finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                //price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                price = _priceFormatter.FormatPrice(finalPrice);
            }

            //base price adjustment
            var adjustBasePrice = decimal.Zero;
            var attributeBaseValues = _productAttributeParser.ParseProductAttributeValues(attributeXml);
            if (attributeBaseValues != null)
            {
                foreach (var attributeValue in attributeBaseValues)
                {
                    adjustBasePrice += _priceCalculationService.GetProductAttributeValueBasePriceAdjustment(attributeValue);
                }
            }


            //stock
            var stockAvailability = product.FormatStockMessage(attributeXml, _localizationService, _productAttributeParser, _dateRangeService);
            var stockNumber = product.GetStockNumber(attributeXml, _productAttributeParser);
            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            //picture. used when we want to override a default product picture when some attribute is selected
            var pictureFullSizeUrl = "";
            var pictureDefaultSizeUrl = "";
            if (loadPicture)
            {
                //just load (return) the first found picture (in case if we have several distinct attributes with associated pictures)
                //actually we're going to support pictures associated to attribute combinations (not attribute values) soon. it'll more flexible approach
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributeXml);
                var attributeValueWithPicture = attributeValues.FirstOrDefault(x => x.PictureId > 0);
                if (attributeValueWithPicture != null)
                {
                    var productAttributePictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTATTRIBUTE_PICTURE_MODEL_KEY,
                                    attributeValueWithPicture.PictureId,
                                    _webHelper.IsCurrentConnectionSecured(),
                                    _storeContext.CurrentStore.Id);
                    var pictureModel = _cacheManager.Get(productAttributePictureCacheKey, () =>
                    {
                        var valuePicture = _pictureService.GetPictureById(attributeValueWithPicture.PictureId);
                        if (valuePicture != null)
                        {
                            return new PictureModel
                            {
                                FullSizeImageUrl = _pictureService.GetPictureUrl(valuePicture),
                                ImageUrl = _pictureService.GetPictureUrl(valuePicture, _mediaSettings.ProductDetailsPictureSize)
                            };
                        }
                        return new PictureModel();
                    });
                    pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                    pictureDefaultSizeUrl = pictureModel.ImageUrl;
                }

            }

            var isFreeShipping = product.IsFreeShipping;
            if (isFreeShipping && !string.IsNullOrEmpty(attributeXml))
            {
                isFreeShipping = _productAttributeParser.ParseProductAttributeValues(attributeXml)
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    .Select(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId))
                    .All(associatedProduct => associatedProduct == null || !associatedProduct.IsShipEnabled || associatedProduct.IsFreeShipping);
            }

            var sellingPrice = product.UnitPriceUsd + adjustBasePrice;
            return Json(new
            {
                gtin,
                mpn,
                sku,
                price,
                adjustBasePrice,
                sellingPrice,
                stockAvailability,
                stockNumber,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                pictureFullSizeUrl,
                pictureDefaultSizeUrl,
                isFreeShipping,
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost]
        public virtual IActionResult CheckoutAttributeChange(IFormCollection form, bool isEditable, int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //save selected attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            var attributeXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes,
                _genericAttributeService, _storeContext.CurrentStore.Id);

            //conditions
            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var excludeShippableAttributes = !cart.RequiresShipping(_productService, _productAttributeParser);
            var attributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            //update blocks
            var ordetotalssectionhtml = this.RenderViewComponentToString("OrderTotals", new { isEditable, customerId });
            var selectedcheckoutattributesssectionhtml = this.RenderViewComponentToString("SelectedCheckoutAttributes");

            return Json(new
            {
                ordetotalssectionhtml,
                selectedcheckoutattributesssectionhtml,
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        public virtual IActionResult UploadFileProductAttribute(int attributeId)
        {
            var attribute = _productAttributeService.GetProductAttributeMappingById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = httpPostedFile.GetDownloadBits();

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = Path.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        [HttpPost]
        public virtual IActionResult UploadFileCheckoutAttribute(int attributeId)
        {
            var attribute = _checkoutAttributeService.GetCheckoutAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = httpPostedFile.GetDownloadBits();

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = Path.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }


        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult AdminCart(int customerId, string activetab)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToAction("Index", "Order");
            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new Web.Models.ShoppingCart.ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart, customer: customer);
            return RedirectToAction("CreateOrder", new { customerId = customerId, activetab = activetab });
        }


        [HttpPost, ActionName("AdminCart")]
        [FormValueRequired("updatecart")]
        public virtual IActionResult AdminUpdateCart(IFormCollection form, int customerId, string activetab)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToAction("Index", "Order");
            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form.ContainsKey("removefromcart") ?
                form["removefromcart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() :
                new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                var remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {
                    int newQuantity = sci.Quantity;
                    decimal newPrice = sci.CustomerEnteredPrice;
                    decimal unitPriceUsd = sci.UnitPriceUsd;
                    decimal exchangeRate = sci.ExchangeRate;
                    decimal orderingFee = sci.OrderingFee;
                    double saleOffPercent = sci.SaleOffPercent;
                    int currencyId = sci.CurrencyId;
                    decimal weightCost = sci.WeightCost;
                    foreach (var formKey in form.Keys)
                    {

                        if (formKey.Equals($"itemquantity{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out newQuantity);
                        }

                        if (formKey.Equals($"itemprice{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            decimal.TryParse(form[formKey], out newPrice);
                            newPrice = DecimalExtensions.RoundCustom(newPrice / 1000) * 1000;
                        }
                        if (formKey.Equals($"itempricebase{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            decimal.TryParse(form[formKey], out unitPriceUsd);
                        }
                        if (formKey.Equals($"itemexchangerate{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            decimal.TryParse(form[formKey], out exchangeRate);
                        }
                        if (formKey.Equals($"itemfeeship{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            decimal.TryParse(form[formKey], out orderingFee);
                        }
                        if (formKey.Equals($"itemsaleoff{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            double.TryParse(form[formKey], out saleOffPercent);
                        }
                        if (formKey.Equals($"itemcustomerCurrency{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out currencyId);
                        }
                        if (formKey.Equals($"itemweightcost{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            decimal.TryParse(form[formKey], out weightCost);
                        }
                    }

                    if (newPrice != sci.CustomerEnteredPrice || newQuantity != sci.Quantity)
                    {
                        var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(customer,
                            sci.Id, sci.AttributesXml, newPrice,
                            sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                            newQuantity, true, unitPriceUsd, exchangeRate, orderingFee, saleOffPercent, currencyId, weightCost);
                        innerWarnings.Add(sci.Id, currSciWarnings);
                    }
                }
            }

            customer = _customerService.GetCustomerById(customerId);
            cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form, customer);

            //updated cart

            var model = new Web.Models.ShoppingCart.ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart, customer: customer);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }

            return RedirectToAction("CreateOrder", new { customerId = customerId, activetab = activetab });
        }

        [HttpPost, ActionName("AdminCart")]
        [FormValueRequired("checkout")]
        public virtual IActionResult AdminStartCheckout(IFormCollection form, int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form, customer);

            //validate attributes
            var checkoutAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            var checkoutAttributeWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Any())
            {
                //something wrong, redisplay the page with warnings
                var model = new Web.Models.ShoppingCart.ShoppingCartModel();
                model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart, validateCheckoutAttributes: true, customer: customer);
                return View(model);
            }

            //everything is OK
            if (customer.IsGuest())
            {
                var downloadableProductsRequireRegistration =
                    _customerSettings.RequireRegistrationForDownloadableProducts && cart.Any(sci => sci.Product.IsDownload);

                if (!_orderSettings.AnonymousCheckoutAllowed
                    || downloadableProductsRequireRegistration)
                    return Challenge();

                return RedirectToRoute("LoginCheckoutAsGuest", new { returnUrl = Url.RouteUrl("ShoppingCart") });
            }

            return RedirectToAction("Index", "Checkout", new { customerId });
        }

        [HttpPost, ActionName("AdminCart")]
        [FormValueRequired("applydiscountcouponcode")]
        public virtual IActionResult ApplyDiscountCoupon(string discountcouponcode, int customerId, string activetab, IFormCollection form)
        {
            var customer = _customerService.GetCustomerById(customerId);
            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //cart
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form, customer);

            //var model = new Web.Models.ShoppingCart.ShoppingCartModel();
            //if (!string.IsNullOrWhiteSpace(discountcouponcode))
            //{
            //    //we find even hidden records here. this way we can display a user-friendly message if it's expired
            //    var discounts = _discountService.GetAllDiscountsForCaching(couponCode: discountcouponcode, showHidden: true)
            //        .Where(d => d.RequiresCouponCode)
            //        .ToList();
            //    if (discounts.Any())
            //    {
            //        var userErrors = new List<string>();
            //        var anyValidDiscount = discounts.Any(discount =>
            //        {
            //            var validationResult = _discountService.ValidateDiscount(discount, customer, new[] { discountcouponcode });
            //            userErrors.AddRange(validationResult.Errors);

            //            return validationResult.IsValid;
            //        });

            //        if (anyValidDiscount)
            //        {
            //            //valid
            //            customer.ApplyDiscountCouponCode(discountcouponcode);
            //            model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied"));
            //            model.DiscountBox.IsApplied = true;
            //        }
            //        else
            //        {
            //            if (userErrors.Any())
            //                //some user errors
            //                model.DiscountBox.Messages = userErrors;
            //            else
            //                //general error text
            //                model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
            //        }
            //    }
            //    else
            //        //discount cannot be found
            //        model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
            //}
            //else
            //    //empty coupon code
            //    model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));

            //model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart,customer:customer);

            return RedirectToAction("CreateOrder", new { customerId = customerId, activetab = activetab });
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public virtual IActionResult ApplyGiftCard(string giftcardcouponcode, int customerId, string activetab, IFormCollection form)
        {
            var customer = _customerService.GetCustomerById(customerId);
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            //cart
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form, customer);

            //var model = new Web.Models.ShoppingCart.ShoppingCartModel();
            //if (!cart.IsRecurring())
            //{
            //    if (!string.IsNullOrWhiteSpace(giftcardcouponcode))
            //    {
            //        var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
            //        var isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
            //        if (isGiftCardValid)
            //        {
            //            customer.ApplyGiftCardCouponCode(giftcardcouponcode);
            //            model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
            //            model.GiftCardBox.IsApplied = true;
            //        }
            //        else
            //        {
            //            model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
            //            model.GiftCardBox.IsApplied = false;
            //        }
            //    }
            //    else
            //    {
            //        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
            //        model.GiftCardBox.IsApplied = false;
            //    }
            //}
            //else
            //{
            //    model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");
            //    model.GiftCardBox.IsApplied = false;
            //}

            //model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart,customer:customer);
            //return View(model);
            return RedirectToAction("CreateOrder", new { customerId = customerId, activetab = activetab });
        }

        [PublicAntiForgery]
        [HttpPost]
        public virtual IActionResult GetEstimateShipping(int? countryId, int? stateProvinceId, string zipPostalCode, int customerId, IFormCollection form)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form, customer);

            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(zipPostalCode))
            {
                errors.Append(_localizationService.GetResource("ShoppingCart.EstimateShipping.ZipPostalCode.Required"));
            }

            if (countryId == null || countryId == 0)
            {
                if (errors.Length > 0)
                    errors.Append("<br>");

                errors.Append(_localizationService.GetResource("ShoppingCart.EstimateShipping.Country.Required"));
            }

            if (errors.Length > 0)
            {
                return Content(errors.ToString());
            }

            var model = _shoppingCartModelFactory.PrepareEstimateShippingResultModel(cart, countryId, stateProvinceId, zipPostalCode, customer: customer);
            return PartialView("_EstimateShippingResult", model);
        }

        [HttpPost, ActionName("ShoppingCart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removediscount-")]
        public virtual IActionResult RemoveDiscountCoupon(IFormCollection form, int customerId, string activetab)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var model = new Web.Models.ShoppingCart.ShoppingCartModel();

            ////get discount identifier
            var discountId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removediscount-", StringComparison.InvariantCultureIgnoreCase))
                    discountId = Convert.ToInt32(formValue.Substring("removediscount-".Length));
            var discount = _discountService.GetDiscountById(discountId);
            if (discount != null)
                customer.RemoveDiscountCouponCode(discount.CouponCode);


            //var cart = customer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            //model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart,customer:customer);
            //return View(model);
            return RedirectToAction("CreateOrder", new { customerId = customerId, activetab = activetab });
        }

        [HttpPost, ActionName("ShoppingCart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
        public virtual IActionResult RemoveGiftCardCode(IFormCollection form, int customerId, string activetab)
        {
            var customer = _customerService.GetCustomerById(customerId);
            //var model = new Web.Models.ShoppingCart.ShoppingCartModel();

            //get gift card identifier
            var giftCardId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                    giftCardId = Convert.ToInt32(formValue.Substring("removegiftcard-".Length));
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
                customer.RemoveGiftCardCouponCode(gc.GiftCardCouponCode);

            //var cart = customer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            //model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart, customer: customer);
            //return View(model);
            return RedirectToAction("CreateOrder", new { customerId = customerId, activetab = activetab });
        }

        #endregion
    }
}