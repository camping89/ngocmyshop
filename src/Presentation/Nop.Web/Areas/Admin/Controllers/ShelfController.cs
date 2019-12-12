using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Extensions;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Shelfs;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using StringExtensions = Nop.Web.Extensions.StringExtensions;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class ShelfController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IShelfService _shelfService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IShipmentManualService _shipmentManualService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IVendorService _vendorService;
        public ShelfController(ICustomerService customerService, IShelfService shelfService, IOrderService orderService, IPermissionService permissionService, ILocalizationService localizationService, IShipmentManualService shipmentManualService, IWorkContext workContext, IPriceFormatter priceFormatter, ISettingService settingService, IStateProvinceService stateProvinceService, ICustomerActivityService customerActivityService, IPictureService pictureService, IProductAttributeParser productAttributeParser, IVendorService vendorService)
        {
            _customerService = customerService;
            _shelfService = shelfService;
            _orderService = orderService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _shipmentManualService = shipmentManualService;
            _workContext = workContext;
            _priceFormatter = priceFormatter;
            _settingService = settingService;
            _stateProvinceService = stateProvinceService;
            _customerActivityService = customerActivityService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _vendorService = vendorService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            var shelfListModel = new ShelfListModel();
            shelfListModel.OrderItemsStatus.AddRange(new List<SelectListItem>()
            {
                new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All")},
                new SelectListItem() {Value = "True",Text = _localizationService.GetResource("Admin.OrderItem.IsActive.True"), Selected = true},
                new SelectListItem() {Value = "False",Text = _localizationService.GetResource("Admin.OrderItem.IsActive.False")},
            });

            shelfListModel.CustomerNotifiedStatus.AddRange(new List<SelectListItem>()
            {
                new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All"), Selected = true},
                new SelectListItem() {Value = "True",Text = _localizationService.GetResource("Admin.OrderItem.IsCustomerNotified.True")},
                new SelectListItem() {Value = "False",Text = _localizationService.GetResource("Admin.OrderItem.IsCustomerNotified.False")},
            });

            shelfListModel.PackageItemProcessedDatetimeStatus.AddRange(new List<SelectListItem>()
            {
                new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All"), Selected = true},
                new SelectListItem() {Value = "True",Text = _localizationService.GetResource("Admin.OrderItem.IsPackageItemProcessedDatetimeStatus.True")},
                new SelectListItem() {Value = "False",Text = _localizationService.GetResource("Admin.OrderItem.IsPackageItemProcessedDatetimeStatus.False")},
            });

            //var customers = _customerService.GetAllCustomersByCache(customerRoleIds: new[] { CustomerRoleEnum.Customer.ToInt(), CustomerRoleEnum.Registered.ToInt() }).Where(_ => string.IsNullOrEmpty(_.GetFullName()) == false);
            //shelfListModel.Customers = customers.Select(x =>
            //{
            //    var model = new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = $"{x.GetFullName()} - {x.GetAttribute<string>(SystemCustomerAttributeNames.Phone)}"
            //    };
            //    return model;
            //}).ToList();

            //shelfListModel.Customers.Insert(0, new SelectListItem { Value = "0", Text = _localizationService.GetResource("Admin.Common.All") });

            shelfListModel.ShelfNoteStatus = ShelfNoteStatus.NoReply.ToSelectList(false).ToList();
            shelfListModel.ShelfNoteStatus.Insert(0, new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All"), Selected = true });
            shelfListModel.IsAdmin = _workContext.CurrentCustomer.IsAdmin();
            return View(shelfListModel);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, ShelfListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();
            if (model.AssignedOrderItemToDate != null)
            {
                model.AssignedOrderItemToDate = model.AssignedOrderItemToDate.Value.AddDays(1).AddMinutes(-1);
            }
            var shelfs = _shelfService.GetShelves(model.CustomerId,
                model.AssignedFromDate,
                model.AssignedToDate,
                model.AssignedOrderItemFromDate,
                model.AssignedOrderItemToDate,
                model.ShippedFromDate,
                model.ShippedToDate,
                command.Page - 1, command.PageSize,
                isShelfEmpty: model.IsShelfEmpty,
                isCustomerNotified: model.IsCustomerNotified,
                shelfCode: model.ShelfCode,
                orderItemId:model.OrderItemId.ToIntODefault(),
                shelfNoteId: model.ShelfNoteId,
                isAscSortedAssignedDate: model.IsAscSortedAssignedDate,
                customerPhone: model.CustomerPhone);
            var gridModel = new DataSourceResult
            {
                Data =
                    shelfs.Select(x =>
                {
                    var m = x.ToModel();
                    m.ShelfNoteStatus = x.ShelfNoteStatus.GetLocalizedEnum(_localizationService, _workContext);
                    m.AssignedDate = x.AssignedDate?.ToString("MM/dd/yyyy");
                    m.ShippedDate = x.ShippedDate?.ToString("MM/dd/yyyy");
                    m.UpdatedNoteDate = x.UpdatedNoteDate?.ToString("MM/dd/yyyy");
                    var customer = x.Customer;
                    if (customer != null)
                    {
                        m.CustomerPhone = customer.Phone;

                        var linkFacebook = customer.LinkFacebook1;
                        if (string.IsNullOrEmpty(linkFacebook))
                        {
                            linkFacebook = customer.LinkFacebook2;
                        }

                        if (customer.Addresses != null && customer.Addresses.Count > 0)
                        {

                            var customerAddress = customer.Addresses.OrderByDescending(_ => _.CreatedOnUtc).FirstOrDefault();
                            if (customerAddress != null)
                            {
                                m.CustomerAddress = $"{customerAddress.Address1}, {customerAddress.Ward}, {customerAddress.District}, {customerAddress.City}";
                            }
                        }

                        m.CustomerFullName = customer.FullName;
                        m.CustomerLinkFacebook = linkFacebook;
                        if (string.IsNullOrEmpty(linkFacebook) == false)
                        {
                            linkFacebook = linkFacebook.Split('/').ToList().LastOrDefault();
                            if (string.IsNullOrEmpty(linkFacebook) == false)
                            {
                                m.CustomerLinkShortFacebook = linkFacebook.Split('?').FirstOrDefault();
                            }
                        }
                    }

                    m.Total = _priceFormatter.FormatPrice(x.Total, true, false);
                    m.TotalWithoutDeposit = _priceFormatter.FormatPrice(x.TotalWithoutDeposit, true, false);

                    return m;
                }),
                Total = shelfs.TotalCount
            };

            return Json(gridModel);
        }



        [HttpPost]
        public virtual IActionResult OrderItemsByShelfId(int shelfId, string orderItemId, bool? isActive, DataSourceRequest command)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var orderItems = _shelfService.GetOrderItems(shelfId.ToString()).ToList();
            if (orderItemId.IsNotNullOrEmpty())
            {
                orderItems = orderItems.Where(_ => _.Id == orderItemId.ToIntODefault()).ToList();
            }
            var model = orderItems.OrderBy(_ => _.ShelfAssignedDate).Select(PrepareOrderItemInShelfModel).ToList();

            var gridModel = new DataSourceResult
            {
                Data = model,
            };

            return Json(gridModel);
        }

        private OrderItemInShelfModel PrepareOrderItemInShelfModel(OrderItem orderItem)
        {
            var primaryStoreCurrency = _workContext.WorkingCurrency;
            orderItem.PriceInclTax = DecimalExtensions.RoundCustom(orderItem.PriceExclTax / 1000) * 1000;
            orderItem.PriceInclTax = DecimalExtensions.RoundCustom(orderItem.PriceInclTax / 1000) * 1000;
            //picture
            var orderItemPicture =
                orderItem.Product.GetProductPicture(orderItem.AttributesXml, _pictureService, _productAttributeParser);
            var pictureThumbnailUrl = _pictureService.GetPictureUrl(orderItemPicture, 150);

            var model = new OrderItemInShelfModel
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ShelfCode = orderItem.Shelf.ShelfCode,
                ProductName = orderItem.Product.Name,
                Sku = orderItem.Product.Sku,
                PictureThumbnailUrl = pictureThumbnailUrl,
                AttributeInfo = orderItem.AttributeDescription,
                WeightCostDec = orderItem.WeightCost,
                WeightCost = _priceFormatter.FormatPrice(orderItem.WeightCost, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true),
                Quantity = orderItem.Quantity,
                DeliveryDateUtc = orderItem.DeliveryDateUtc?.ToString("MM/dd/yyyy"),
                PackageOrderId = orderItem.PackageOrderId ?? 0,
                TotalWithoutWeightCost = _priceFormatter.FormatPrice((orderItem.PriceInclTax - orderItem.WeightCost), true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true),
                DepositStr = _priceFormatter.FormatPrice(orderItem.Deposit, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true),
                SubTotalInclTax = _priceFormatter.FormatPrice(orderItem.PriceInclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, true, true),
                PackageOrderCode = orderItem.PackageOrder?.PackageCode,
                AssignedShelfDate = orderItem.ShelfAssignedDate,
                PrimaryStoreCurrencyCode = primaryStoreCurrency.CurrencyCode,
                Note = orderItem.Note
            };
            var vendor = _vendorService.GetVendorById(orderItem.Product.VendorId);
            if (vendor != null)
            {
                model.VendorName = vendor.Name;
            }
            
            model.ExistShipment =  _shipmentManualService.ExistShipmentItem(orderItem.Id);
            return model;
        }

        public IActionResult Create()
        {
            var shelfModel = new ShelfModel();
            PrepareShelfModel(shelfModel);

            return View(shelfModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(ShelfModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                _shelfService.InsertShelf(entity);
                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = model.Id });
                }
                return RedirectToAction("List");
            }
            PrepareShelfModel(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateAjax(ShelfModel model)
        {
            var shelf = _shelfService.GetShelfByCode(model.ShelfCode);
            if (shelf != null)
            {
                return Json(new { errors = _localizationService.GetResource("ShelfExist.Message") });
            }

            var entity = model.ToEntity();

            if (string.IsNullOrEmpty(model.AssignedDate) == false)
            {
                entity.AssignedDate = StringExtensions.StringToDateTime(model.AssignedDate);
            }

            if (string.IsNullOrEmpty(model.ShippedDate) == false)
            {
                entity.ShippedDate = StringExtensions.StringToDateTime(model.ShippedDate);
            }

            if (string.IsNullOrEmpty(model.UpdatedNoteDate) == false)
            {
                entity.UpdatedNoteDate = StringExtensions.StringToDateTime(model.UpdatedNoteDate);
            }
            _shelfService.InsertShelf(entity);

            _customerActivityService.InsertActivity("InsertShelf", _localizationService.GetResource("activitylog.insertshelf"), model.ShelfCode);
            return Json(new { Success = true });
        }

        public IActionResult Edit(int id)
        {
            var shelfModel = _shelfService.GetShelfById(id).ToModel();
            PrepareShelfModel(shelfModel);

            return View(shelfModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Edit(int id, ShelfModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var entity = _shelfService.GetShelfById(model.Id);

                if (string.IsNullOrEmpty(model.AssignedDate) == false)
                {
                    entity.AssignedDate = StringExtensions.StringToDateTime(model.AssignedDate);
                }

                if (string.IsNullOrEmpty(model.ShippedDate) == false)
                {
                    entity.ShippedDate = StringExtensions.StringToDateTime(model.ShippedDate);
                }

                if (string.IsNullOrEmpty(model.UpdatedNoteDate) == false)
                {
                    entity.UpdatedNoteDate = StringExtensions.StringToDateTime(model.UpdatedNoteDate);
                }


                entity = model.ToEntity(entity);

                _shelfService.UpdateShelf(entity);

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = model.Id });
                }
                return RedirectToAction("List");
            }

            PrepareShelfModel(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditAjax(ShelfModel model)
        {
            var shelf = _shelfService.GetShelfById(model.Id);
            shelf = model.ToEntity(shelf);
            shelf.ShippedDate = model.ShippedDate.IsNotNullOrEmpty() ? StringExtensions.StringToDateTime(model.ShippedDate) : null;

            if (model.UpdatedNoteDate.IsNotNullOrEmpty())
            {
                shelf.UpdatedNoteDate = StringExtensions.StringToDateTime(model.UpdatedNoteDate);
            }

            _shelfService.UpdateShelf(shelf);
            _shelfService.UpdateShelfTotalAmount(shelf.Id.ToString());
            _customerActivityService.InsertActivity("UpdateShelf", _localizationService.GetResource("activitylog.updateshelf"), model.ShelfCode);
            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult SetInActiveShelfs(ICollection<int> shelfIds)
        {
            foreach (var shelfId in shelfIds)
            {
                var shelf = _shelfService.GetShelfById(shelfId);
                if (shelf != null)
                {
                    shelf.InActive = true;
                    _shelfService.UpdateShelf(shelf);
                }
            }
            return Json(new { Success = true });
        }


        [HttpPost]
        public IActionResult SetActiveShelfs(ICollection<int> shelfIds)
        {
            foreach (var shelfId in shelfIds)
            {
                var shelf = _shelfService.GetShelfById(shelfId);
                if (shelf != null)
                {
                    shelf.InActive = false;
                    _shelfService.UpdateShelf(shelf);
                }
            }
            return Json(new { Success = true });
        }


        [HttpPost]
        public IActionResult CustomerAssignedShelf(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var customers = new List<Customer>() { customer };
            var result = customers.Select(_ => new { CustomerId = _.Id, CustomerName = _.FullName + " - " + _.Phone }).ToArray();
            return Json(new { Data = result });
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                _shelfService.DeleteShelf(id);
            }

            return RedirectToAction("List");
        }


        private void PrepareShelfModel(ShelfModel shelfModel)
        {
            var customers = _customerService.GetAllCustomers();
            shelfModel.Customers = customers.Select(x =>
            {
                var customer = new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.GetFullName(),
                    Selected = x.Id == shelfModel.CustomerId
                };
                return customer;
            }).ToList();
            shelfModel.Customers.Insert(0, new SelectListItem { Value = "0", Text = "Chọn khách" });

        }

        [HttpGet]
        public IActionResult GetShelfAvailable(int orderItemId = 0)
        {
            var shelfCode = Request.Query.FirstOrDefault(_ => _.Key.Contains("filter[filters][0][value]")).Value;

            var availableShelf = _shelfService.GetAvailableShelf(shelfCode: shelfCode).Select(x => new
            {
                ShelfId = x.Id,
                ShelfCode = x.ShelfCode
            }).ToList();
            availableShelf.Insert(0, new { ShelfId = 0, ShelfCode = _localizationService.GetResource("shelf.edit.chooseself") });
            return Json(availableShelf);
        }

        [HttpPost]
        public IActionResult DeleteShelfOrderItemId(OrderItemInShelfModel model)
        {
            var orderItem = _orderService.GetOrderItemById(model.Id);

            if (orderItem != null)
            {
                var shelfId = orderItem.ShelfId;

                orderItem.ShelfId = null;
                orderItem.ShelfAssignedDate = null;
                _orderService.UpdateOrderItem(orderItem);

                _shelfService.UpdateShelfTotalAmount(shelfId.ToString());
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public IActionResult GetCustShelf(int orderItemId)
        {
            var orderItem = _orderService.GetOrderItemById(orderItemId);
            var shelfsList = _shelfService
                .GetShelves(orderItem.Order.CustomerId)
                .OrderByDescending(s => s.AssignedDate)
                .Take(1)
                .Select(_ => new { _.ShelfCode, ShelfId = _.Id })
                .ToList();

            if (shelfsList.Count > 0)
            {
                shelfsList.Add(new { ShelfCode = _localizationService.GetResource("shelf.edit.chooseself"), ShelfId = 0, });
                return Json(new { Exist = true, Shelfs = shelfsList });
            }
            else
            {
                return Json(new { Exist = false });
            }
        }

        #region Shipment

        [HttpPost]
        public IActionResult CreateShipAll(int shelfId, int customerId)
        {
            var orderItems = _shelfService.GetOrderItems(shelfId.ToString());
            var shelf = _shelfService.GetShelfById(shelfId);
            if (orderItems.Count > 0 && shelf != null)
            {

                CreateShipment(shelf, orderItems.Select(_ => _.Id).ToList(), customerId);
            }

            _shelfService.UpdateShelfTotalAmount(shelfId.ToString());

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult CreateShipSelectedIds(int shelfId, ICollection<int> orderItemIds, int customerId)
        {
            var shelf = _shelfService.GetShelfById(shelfId);
            if (shelf != null)
            {
                CreateShipment(shelf, orderItemIds, customerId);
            }

            _shelfService.UpdateShelfTotalAmount(shelfId.ToString());
            return Json(new { Success = true });
        }

        private void CreateShipment(Shelf shelf, ICollection<int> orderItemIds, int customerId)
        {
            var ids = new List<int>();
            if (customerId > 0 && orderItemIds != null)
            {
                // 1. get undelivered shipment by orderitem id
                var shipmentItemIds = _shipmentManualService.GetShipmentManualItemsByOrderItemIds(orderItemIds.ToArray())
                    .Select(_ => _.OrderItemId).Distinct().ToList();
                foreach (var orderItemId in orderItemIds)
                {
                    if (!shipmentItemIds.Contains(orderItemId))
                    {
                        ids.Add(orderItemId);

                    }
                }

                var orderItems = _orderService.GetOrderItemsByIds(ids.ToArray());
                foreach (var orderItem in orderItems)
                {
                    orderItem.PriceInclTax = DecimalExtensions.RoundCustom(orderItem.PriceInclTax / 1000) * 1000;
                }
                var totalMoney = orderItems.Sum(_ => _.PriceInclTax);
                var deposit = orderItems.Sum(_ => _.Deposit);

                var totalWeight = orderItems.Sum(_ => _.ItemWeight);
                var customer = _customerService.GetCustomerById(customerId);
                var orderItemFirst = orderItems.FirstOrDefault();
                if (customer != null && orderItemFirst != null)
                {
                    var order = orderItemFirst.Order;
                    var customerAddress = order.Customer.Addresses.OrderByDescending(_ => _.CreatedOnUtc).FirstOrDefault();
                    var shipmentEntity = new ShipmentManual
                    {
                        CustomerId = customerId,
                        CreatedOnUtc = DateTime.UtcNow,
                        BagId = StringExtensions.RandomString(6, false),
                        TrackingNumber = StringExtensions.RandomString(6, false),
                        ShippingAddressId = customerAddress?.Id,
                        //TotalShippingFee = configShippingFee,
                        TotalWeight = totalWeight,
                        ShippedDateUtc = shelf.ShippedDate,
                        Address = customerAddress?.Address1,
                        Province = customerAddress?.City,
                        District = customerAddress?.District,
                        Ward = customerAddress?.Ward,
                        Total = totalMoney,
                        Deposit = deposit,
                        ShelfCode = shelf.ShelfCode
                    };

                    shipmentEntity.HasShippingFee = true;
                    if (customerAddress != null && string.IsNullOrEmpty(customerAddress.City) == false && customerAddress.City.ToLower().Equals("đà nẵng"))
                    {
                        shipmentEntity.TotalShippingFee = _settingService.GetSettingByKey("Admin.Shipment.ShippingFeeDaNang", 10000.0m);
                    }
                    else
                    {
                        shipmentEntity.TotalShippingFee = _settingService.GetSettingByKey("Admin.Shipment.DefaultShippingFee", 1000.0m);
                    }
                    _shipmentManualService.InsertShipmentManual(shipmentEntity);
                    if (shipmentEntity.Id > 0)
                    {
                        foreach (var orderItem in orderItems)
                        {
                            var shipmentManualItem = PrepareShipmentManualItemEntity(orderItem, shipmentEntity.Id);
                            _shipmentManualService.InsertShipmentManualItem(shipmentManualItem);
                        }
                    }

                }
            }
        }
        private ShipmentManualItem PrepareShipmentManualItemEntity(OrderItem orderItem, int shipmentManualId)
        {
            var shipmentManualItem = new ShipmentManualItem
            {
                OrderItemId = orderItem.Id,
                Quantity = orderItem.Quantity,
                ShipmentManualId = shipmentManualId,
                ShippingFee = 0,
                WarehouseId = 0

            };
            return shipmentManualItem;
        }

        public IActionResult EditShipmentManualAjax(ShipmentManualModel model)
        {
            if (model.Id > 0)
            {
                var shipmentManual = _shipmentManualService.GetShipmentManualById(model.Id);

                if (string.IsNullOrEmpty(model.ShippedDate) == false)
                {
                    shipmentManual.ShippedDateUtc = StringExtensions.StringToDateTime(model.ShippedDate);
                }
                if (string.IsNullOrEmpty(model.DeliveryDate) == false)
                {
                    shipmentManual.DeliveryDateUtc = StringExtensions.StringToDateTime(model.DeliveryDate);
                }
                shipmentManual.BagId = model.BagId;
                if (model.ShipperId != null) shipmentManual.ShipperId = model.ShipperId.Value;
                shipmentManual.Province = model.ShipmentCityId;
                shipmentManual.District = model.ShipmentDistrictId == "0" ? string.Empty : model.ShipmentDistrictId;
                shipmentManual.Address = model.ShipmentAddress;
                shipmentManual.Ward = model.ShipmentWard;
                shipmentManual.ShipmentNote = model.ShipmentNote;
                if (model.HasShippingFee)
                {
                    shipmentManual.HasShippingFee = true;
                    if (string.IsNullOrEmpty(model.ShipmentCity) == false && model.ShipmentCity.ToLower().Equals("đà nẵng"))
                    {
                        shipmentManual.TotalShippingFee = _settingService.GetSettingByKey("Admin.Shipment.ShippingFeeDaNang", 10000.0m);
                    }
                    else
                    {
                        shipmentManual.TotalShippingFee = _settingService.GetSettingByKey("Admin.Shipment.DefaultShippingFee", 1000.0m);
                    }
                }
                else
                {
                    shipmentManual.HasShippingFee = false;
                }
                _shipmentManualService.UpdateShipmentManual(shipmentManual);
            }
            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult DeleteShipmentManualAjax(int id)
        {
            var shipmentManual = _shipmentManualService.GetShipmentManualById(id);
           
            if (shipmentManual != null)
            {
                var shelfCode = shipmentManual.ShelfCode;
                
                //_customerActivityService.InsertActivity("DeleteShipmentManual", _localizationService.GetResource("activitylog.DeleteShipmentManual"), shipmentManual.Id);

                foreach (var shipmentManualItem in shipmentManual.ShipmentManualItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentManualItem.OrderItemId);
                    if (orderItem != null)
                    {
                        orderItem.DeliveryDateUtc = null;
                        _orderService.UpdateOrderItem(orderItem);
                    }
                }
                var shelf = _shelfService.GetShelfByCode(shelfCode);
                if (shelf != null)
                {
                    //update customer
                    if (shelf.CustomerId == null || shelf.CustomerId == 0)
                    {
                        shelf.CustomerId = shipmentManual.CustomerId;
                        _shelfService.UpdateShelf(shelf);
                    }
                    _shelfService.UpdateShelfTotalAmount(shelf.Id.ToString());
                }

                _shipmentManualService.DeleteShipmentManual(shipmentManual);
            }

            return Json(new { Success = true });
        }




        public ActionResult UpdateTotalShelfByCode(string shelfCode)
        {
            _shelfService.UpdateShelfTotalAmount(shelfCode);
            return new NullJsonResult();
        }

        [HttpPost]
        public IActionResult EditDepositShipmentItem(int id, decimal deposit)
        {
            var shipmentManualItem = _shipmentManualService.GetShipmentManualItemById(id);
            if (shipmentManualItem != null)
            {
                var shipmentManualId = shipmentManualItem.ShipmentManualId;
                var orderItem = _orderService.GetOrderItemById(shipmentManualItem.OrderItemId);
                if (orderItem != null)
                {
                    orderItem.Deposit = deposit;
                    _orderService.UpdateOrderItem(orderItem);

                    _shipmentManualService.UpdateTotalShipmentManual(shipmentManualId);
                }
            }


            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult DeleteShipmentItem(int id)
        {
            var shipmentManualItem = _shipmentManualService.GetShipmentManualItemById(id);
            if (shipmentManualItem != null)
            {
                var shipmentManualId = shipmentManualItem.ShipmentManualId;
                var orderItemId = shipmentManualItem.OrderItemId;
                _shipmentManualService.DeleteShipmentManualItem(shipmentManualItem);

                var orderItem = _orderService.GetOrderItemById(orderItemId);
                if (orderItem != null)
                {
                    orderItem.DeliveryDateUtc = null;
                    _orderService.UpdateOrderItem(orderItem);
                    _shipmentManualService.UpdateTotalShipmentManual(shipmentManualId);
                }
            }
            return Json(new { Success = true });
        }


        public IActionResult UpdateShelvesTotal()
        {
            var shelves = _shelfService.GetShelves();
            foreach (var shelf in shelves)
            {
                if (shelf != null)
                {
                    decimal total = 0;
                    decimal totalWithoutDeposit = 0;
                    if (shelf.OrderItems != null)
                    {
                        var orderItems = shelf.OrderItems.Where(_ => _.DeliveryDateUtc == null).ToList();
                        foreach (var item in orderItems)
                        {
                            var itemTotal = DecimalExtensions.RoundCustom(item.PriceInclTax / 1000) * 1000;
                            total += itemTotal;
                            totalWithoutDeposit += itemTotal - DecimalExtensions.RoundCustom(item.Deposit / 1000) * 1000;
                        }
                    }
                    shelf.Total = total;
                    shelf.TotalWithoutDeposit = totalWithoutDeposit;
                }
            }

            _shelfService.UpdateShelfves(shelves);
            return new NullJsonResult();
        }

        public IActionResult UpdateShipmentsManual()
        {
            var shipmentManuals = _shipmentManualService.GetShipmentManuals().ToList();
            foreach (var shipmentManual in shipmentManuals)
            {
                if (shipmentManual.ShipmentManualItems != null)
                {
                    var orderItems = shipmentManual.ShipmentManualItems.Where(_ => _.OrderItem != null).Select(_ => _.OrderItem).ToList();
                    foreach (var orderItem in orderItems)
                    {
                        shipmentManual.Total += DecimalExtensions.RoundCustom(orderItem.PriceInclTax / 1000) * 1000;
                        shipmentManual.Deposit += DecimalExtensions.RoundCustom(orderItem.Deposit / 1000) * 1000;
                    }

                    var first = shipmentManual.ShipmentManualItems.FirstOrDefault();
                    if (first != null)
                    {
                        shipmentManual.ShelfCode = first.OrderItem.Shelf?.ShelfCode;
                    }

                    _shipmentManualService.UpdateShipmentManual(shipmentManual);
                }
            }
            return new NullJsonResult();
        }


        public IActionResult ClearCustomerInfoShelfEmpty()
        {
            var shelfs = _shelfService.GetShelves(isShelfEmpty: true);
            foreach (var shelf in shelfs)
            {
                shelf.CustomerId = null;
                shelf.AssignedDate = null;
                shelf.ShippedDate = null;
                shelf.Total = 0;
                shelf.TotalWithoutDeposit = 0;
                shelf.ShelfNoteStatus = ShelfNoteStatus.NoReply;
            }
            _shelfService.UpdateShelfves(shelfs);
            return new NullJsonResult();
        }
        #endregion
    }
}
