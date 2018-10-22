using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class EstimateShippingViewComponent : NopViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;

        public EstimateShippingViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext, ICustomerService customerService)
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._storeContext = storeContext;
            this._workContext = workContext;
            _customerService = customerService;
        }

        public IViewComponentResult Invoke(bool? prepareAndDisplayOrderReviewData, int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
            {
                customer = _workContext.CurrentCustomer;
            }
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = _shoppingCartModelFactory.PrepareEstimateShippingModel(cart,customer:customer);
            if (!model.Enabled)
                return Content("");

            return View(model);
        }
    }
}
