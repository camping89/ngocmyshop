using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Checkout;

namespace Nop.Web.Areas.Admin.Components
{
    public class CheckoutProgressViewComponent : NopViewComponent
    {
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
        {
            this._checkoutModelFactory = checkoutModelFactory;
        }

        public IViewComponentResult Invoke(CheckoutProgressStep step)
        {
            var model = _checkoutModelFactory.PrepareCheckoutProgressModel(step);
            return View(model);
        }
    }
}
