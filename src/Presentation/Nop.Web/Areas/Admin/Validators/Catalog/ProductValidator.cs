﻿using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public sealed partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));
            RuleFor(x => x.Sku).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Sku.Required"));
            RuleFor(x => x.VendorProductUrl).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.VendorProductUrl.Required"));
            RuleFor(x => x.VendorId).GreaterThanOrEqualTo(1).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.VendorId.Required"));
            RuleFor(x => x.UnitPriceUsd).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.BaseUnitPrice.Required"));
            RuleFor(x => x.OrderingFee).GreaterThanOrEqualTo(1).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.OrderingFee.Required"));

            SetDatabaseValidationRules<Product>(dbContext);
        }
    }
}