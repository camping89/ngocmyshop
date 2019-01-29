using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
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
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShelfController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IShelfService _shelfService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        public ShelfController(ICustomerService customerService, IShelfService shelfService, IOrderService orderService, IPermissionService permissionService, ILocalizationService localizationService)
        {
            _customerService = customerService;
            _shelfService = shelfService;
            _orderService = orderService;
            _permissionService = permissionService;
            _localizationService = localizationService;
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

            var customers = _customerService.GetAllCustomers().Where(_ => string.IsNullOrEmpty(_.GetFullName()) == false);
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

            return View(shelfListModel);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, ShelfListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            var shelfs = _shelfService.GetAllShelf(model.CustomerId, model.AssignedFromDate, model.AssignedToDate, command.Page - 1, command.PageSize, model.ShelfIsEmpty);
            var gridModel = new DataSourceResult
            {
                Data = shelfs.Select(x =>
                {
                    var m = x.ToModel();
                    m.AssignedDate = x.AssignedDate?.ToString("MM/dd/yyyy");
                    var customer = x.Customer;
                    if (customer != null)
                    {
                        m.CustomerPhone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);

                        var linkFacebook = customer.GetAttribute<string>(SystemCustomerAttributeNames.LinkFacebook1);
                        if (string.IsNullOrEmpty(linkFacebook))
                        {
                            linkFacebook = customer.GetAttribute<string>(SystemCustomerAttributeNames.LinkFacebook2);
                        }

                        m.CustomerFullName = customer.GetFullName();
                        m.CustomerLinkFacebook = linkFacebook;

                    }
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

        [HttpPost]
        public IActionResult GetShelfAvailable(int orderItemId = 0)
        {
            var orderItem = _orderService.GetOrderItemById(orderItemId);
            var customerId = orderItem != null ? orderItem.Order.CustomerId : 0;
            var shelfAvailable = _shelfService.GetAllShelfAvailable(customerId).Select(x => new
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
    }
}
