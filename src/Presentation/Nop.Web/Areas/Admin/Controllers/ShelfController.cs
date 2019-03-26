using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShelfController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IShelfService _shelfService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IShipmentManualService _shipmentManualService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkContext _workContext;
        public ShelfController(ICustomerService customerService, IShelfService shelfService, IOrderService orderService, IPermissionService permissionService, ILocalizationService localizationService, IShipmentManualService shipmentManualService, IWorkContext workContext, IPriceFormatter priceFormatter)
        {
            _customerService = customerService;
            _shelfService = shelfService;
            _orderService = orderService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _shipmentManualService = shipmentManualService;
            _workContext = workContext;
            _priceFormatter = priceFormatter;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            var shelfListModel = new ShelfListModel();
            shelfListModel.ShelfOrderItemsStatus.AddRange(new List<SelectListItem>()
            {
                new SelectListItem() {Value = "True",Text = _localizationService.GetResource("Admin.ShelfOrderItem.IsActive.True"), Selected = true},
                new SelectListItem() {Value = "False",Text = _localizationService.GetResource("Admin.ShelfOrderItem.IsActive.False")},
            });

            shelfListModel.CustomerNotifiedStatus.AddRange(new List<SelectListItem>()
            {
                new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All"), Selected = true},
                new SelectListItem() {Value = "True",Text = _localizationService.GetResource("Admin.ShelfOrderItem.IsCustomerNotified.True")},
                new SelectListItem() {Value = "False",Text = _localizationService.GetResource("Admin.ShelfOrderItem.IsCustomerNotified.False")},
            });

            shelfListModel.PackageItemProcessedDatetimeStatus.AddRange(new List<SelectListItem>()
            {
                new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All"), Selected = true},
                new SelectListItem() {Value = "True",Text = _localizationService.GetResource("Admin.ShelfOrderItem.IsPackageItemProcessedDatetimeStatus.True")},
                new SelectListItem() {Value = "False",Text = _localizationService.GetResource("Admin.ShelfOrderItem.IsPackageItemProcessedDatetimeStatus.False")},
            });

            var customers = _customerService.GetAllCustomers(customerRoleIds: new[] { CustomerRoleEnum.Customer.ToInt(), CustomerRoleEnum.Registered.ToInt() }).Where(_ => string.IsNullOrEmpty(_.GetFullName()) == false);
            shelfListModel.Customers = customers.Select(x =>
            {
                var model = new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.GetFullName()} - {x.GetAttribute<string>(SystemCustomerAttributeNames.Phone)}"
                };
                return model;
            }).ToList();

            shelfListModel.Customers.Insert(0, new SelectListItem { Value = "0", Text = _localizationService.GetResource("Admin.Common.All") });

            shelfListModel.ShelfNoteStatus = ShelfNoteStatus.NoReply.ToSelectList(false).ToList();
            shelfListModel.ShelfNoteStatus.Insert(0, new SelectListItem { Value = "", Text = _localizationService.GetResource("Admin.Common.All"), Selected = true });

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
            var shelfs = _shelfService.GetAllShelf(model.CustomerId,
                model.AssignedFromDate,
                model.AssignedToDate,
                model.AssignedOrderItemFromDate,
                model.AssignedOrderItemToDate,
                model.ShippedFromDate,
                model.ShippedToDate,
                command.Page - 1, command.PageSize,
                model.IsShelfEmpty,
                shelfCode: model.ShelfCode,
                isCustomerNotified: model.IsCustomerNotified,
                shelfNoteId: model.ShelfNoteId,
                isPackageItemProcessedDatetime: model.IsPackageItemProcessedDatetime);
            var gridModel = new DataSourceResult
            {
                Data = shelfs.Select(x =>
                {
                    var m = x.ToModel();
                    m.ShelfNoteStatus = x.ShelfNoteStatus.GetLocalizedEnum(_localizationService, _workContext);
                    m.AssignedDate = x.AssignedDate?.ToString("MM/dd/yyyy");
                    m.ShippedDate = x.ShippedDate?.ToString("MM/dd/yyyy");
                    m.UpdatedNoteDate = x.UpdatedNoteDate?.ToString("MM/dd/yyyy");
                    var customer = x.Customer;
                    if (customer != null)
                    {
                        m.CustomerPhone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);

                        var linkFacebook = customer.GetAttribute<string>(SystemCustomerAttributeNames.LinkFacebook1);
                        if (string.IsNullOrEmpty(linkFacebook))
                        {
                            linkFacebook = customer.GetAttribute<string>(SystemCustomerAttributeNames.LinkFacebook2);
                        }

                        if (customer.Addresses != null && customer.Addresses.Count > 0)
                        {

                            var customerAddress = customer.Addresses.OrderBy(_ => _.CreatedOnUtc).FirstOrDefault();
                            if (customerAddress != null)
                            {
                                m.CustomerAddress = $"{customerAddress.Address1}, {customerAddress.Ward}, {customerAddress.District}, {customerAddress.City}";
                            }
                        }

                        m.CustomerFullName = customer.GetFullName();
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

                    var orderItems = _shelfService.GetAllShelfOrderItem(x.Id, shelfOrderItemIsActive: true).Select(_ => _.OrderItem).ToList();
                    foreach (var orderItem in orderItems)
                    {
                        orderItem.PriceInclTax = Math.Ceiling(orderItem.PriceInclTax / 1000) * 1000;
                    }
                    m.Total = _priceFormatter.FormatPrice(orderItems.Sum(_ => _.PriceInclTax), true, false);
                    return m;
                }),
                Total = shelfs.TotalCount
            };

            return Json(gridModel);
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
            var entity = _shelfService.GetShelfById(model.Id);
            entity = model.ToEntity(entity);

            if (string.IsNullOrEmpty(model.AssignedDate) == false)
            {
                entity.AssignedDate = StringExtensions.StringToDateTime(model.AssignedDate);
            }

            if (string.IsNullOrEmpty(model.ShippedDate) == false)
            {
                entity.ShippedDate = StringExtensions.StringToDateTime(model.ShippedDate);
            }
            else
            {
                entity.ShippedDate = null;
            }

            if (string.IsNullOrEmpty(model.UpdatedNoteDate) == false)
            {
                entity.UpdatedNoteDate = StringExtensions.StringToDateTime(model.UpdatedNoteDate);
            }

            _shelfService.UpdateShelf(entity);
            return Json(new { Success = true });
        }


        [HttpPost]
        public IActionResult DeleteAjax(ShelfModel model)
        {
            if (model.Id > 0)
            {
                var shelfOrderItems = _shelfService.GetAllShelfOrderItem(model.Id);
                foreach (var shelfOrderItem in shelfOrderItems)
                {
                    _shelfService.DeleteShelfOrderItem(shelfOrderItem.Id);
                }
                _shelfService.DeleteShelf(model.Id);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
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
            var orderItem = _orderService.GetOrderItemById(orderItemId);
            var customerId = orderItem != null ? orderItem.Order.CustomerId : 0;
            var shelfAvailable = _shelfService.GetAllShelfAvailable(customerId, shelfCode: shelfCode).Select(x => new
            {
                ShelfId = x.Id,
                ShelfCode = x.ShelfCode
            }).ToList();
            shelfAvailable.Insert(0, new { ShelfId = 0, ShelfCode = "Chọn ngăn" });
            return Json(shelfAvailable);
        }

        [HttpPost]
        public IActionResult DeleteOrderItemId(int shelfOrderItemId)
        {
            var shelfOrderItem = _shelfService.GetShelfOrderItemById(shelfOrderItemId);

            if (shelfOrderItem != null)
            {

                _shelfService.DeleteShelfOrderItem(shelfOrderItemId);

                var shelfItems = _shelfService.GetAllShelfOrderItem(shelfOrderItem.ShelfId, shelfOrderItem.CustomerId, shelfOrderItemIsActive: true).ToList();
                if (shelfItems.Count == 0)
                {
                    var shelf = _shelfService.GetShelfById(shelfOrderItem.ShelfId);
                    shelf.AssignedDate = null;
                    shelf.CustomerId = null;
                    _shelfService.UpdateShelf(shelf);
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public IActionResult GetCustShelf(int orderItemId)
        {
            var orderItem = _orderService.GetOrderItemById(orderItemId);
            var shelfsList = _shelfService.GetAllShelf(orderItem.Order.CustomerId).Select(_ => new { ShelfCode = _.ShelfCode, ShelfId = _.Id }).ToList();

            if (shelfsList.Count > 0)
            {
                shelfsList.Add(new { ShelfCode = "Chọn ngăn", ShelfId = 0, });
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
            var shelfOrderItems = _shelfService.GetAllShelfOrderItem(shelfId, shelfOrderItemIsActive: true);
            var shelf = _shelfService.GetShelfById(shelfId);
            if (shelfOrderItems.Count > 0 && shelf != null)
            {

                CreateShipment(shelf, shelfOrderItems.Select(_ => _.OrderItemId).ToList(), customerId);
            }

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
            return Json(new { Success = true });
        }

        private void CreateShipment(Shelf shelf, ICollection<int> orderItemIds, int customerId)
        {
            if (customerId > 0 && orderItemIds != null)
            {
                var orderItems = _orderService.GetOrderItemsByIds(orderItemIds.ToArray());
                var totalWeight = orderItems.Sum(_ => _.ItemWeight);
                var customer = _customerService.GetCustomerById(customerId);
                if (customer != null)
                {
                    var customerAddress = customer.Addresses.OrderBy(_ => _.CreatedOnUtc).FirstOrDefault();
                    var shipmentEntity = new ShipmentManual
                    {
                        CustomerId = customerId,
                        CreatedOnUtc = DateTime.UtcNow,
                        BagId = StringExtensions.RandomString(6, false),
                        TrackingNumber = StringExtensions.RandomString(6, false),
                        ShippingAddressId = customerAddress?.Id,
                        TotalShippingFee = 0,
                        TotalWeight = totalWeight,
                        ShippedDateUtc = shelf.ShippedDate,
                        Address = customerAddress?.Address1,
                        Province = customerAddress?.City,
                        District = customerAddress?.District,
                        Ward = customerAddress?.Ward
                    };

                    _shipmentManualService.InsertShipmentManual(shipmentEntity);
                    if (shipmentEntity.Id > 0)
                    {
                        foreach (var orderItem in orderItems)
                        {
                            var shipmentManualItem = PrepareShipmentManualItemEntity(orderItem, shipmentEntity.Id);
                            _shipmentManualService.InsertShipmentManualItem(shipmentManualItem);
                        }

                        foreach (var orderItemId in orderItemIds)
                        {

                            var shelfOrderItem = _shelfService.GetShelfOrderItemByOrderItemId(orderItemId);
                            if (shelfOrderItem != null)
                            {
                                var shelfOrderItemEntity = _shelfService.GetShelfOrderItemById(shelfOrderItem.Id);
                                shelfOrderItemEntity.IsActived = false;
                                _shelfService.UpdateShelfOrderItem(shelfOrderItemEntity);
                            }
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
                shipmentManual.ShipperId = model.ShipperId;
                shipmentManual.Province = model.ShipmentCityId;
                shipmentManual.District = model.ShipmentDistrictId;
                shipmentManual.Address = model.ShipmentAddress;
                shipmentManual.Ward = model.ShipmentWard;
                shipmentManual.ShipmentNote = model.ShipmentNote;

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
                foreach (var shipmentManualItem in shipmentManual.ShipmentManualItems)
                {
                    //update status order
                    var order = _orderService.GetOrderById(shipmentManualItem.OrderItem.OrderId);
                    //check whether we have more items to ship
                    if (order.HasItemsToAddToShipment() || order.HasItemsToShip())
                        order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
                    else
                        order.ShippingStatusId = (int)ShippingStatus.NotYetShipped;
                    _orderService.UpdateOrder(order);

                    var shelfOrderItem = _shelfService.GetShelfOrderItemByOrderItemId(shipmentManualItem.OrderItemId);
                    if (shelfOrderItem != null)
                    {
                        shelfOrderItem.IsActived = true;
                        _shelfService.UpdateShelfOrderItem(shelfOrderItem);
                    }
                }
                _shipmentManualService.DeleteShipmentManual(shipmentManual);
            }

            return Json(new { Success = true });
        }


        //public IActionResult CreateAuto()
        //{
        //    var shelfItems = _shelfService.GetAllShelfOrderItem();
        //    foreach (var shelfItem in shelfItems)
        //    {
        //        _shelfService.DeleteShelfOrderItem(shelfItem.Id);
        //    }

        //    var shelfs = _shelfService.GetAllShelf().Select(_ => _.Id);
        //    foreach (var shelf in shelfs)
        //    {
        //        _shelfService.DeleteShelf(shelf);
        //    }

        //    var listLetter = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "V", "X", "Y", "Z" };
        //    var listResult = new List<string>();
        //    for (int i = 1; i < 6; i++)
        //    {
        //        foreach (var letter in listLetter)
        //        {
        //            for (int j = 101; j < 1000; j++)
        //            {
        //                if (j != 200 && j != 300 && j != 400 && j != 500 && j != 600 && j != 700 && j != 800 && j != 900)
        //                {
        //                    listResult.Add($"{i}{letter}{j}");
        //                }
        //            }
        //        }
        //    }

        //    var count = listResult.Count;

        //    foreach (var item in listResult)
        //    {
        //        _shelfService.InsertShelf(new Shelf() { ShelfCode = item, ShelfNoteStatus = ShelfNoteStatus.NoReply });
        //    }

        //    return Json(true);
        //}
        #endregion
    }
}
