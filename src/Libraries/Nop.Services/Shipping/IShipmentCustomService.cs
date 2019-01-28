using Nop.Core;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service interface
    /// </summary>
    public partial interface IShipmentCustomService
    {
        void DeleteShipmentCustom(ShipmentManual shipmentCustom);


        IPagedList<ShipmentManual> GetAllShipmentsCustom(int vendorId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, int orderItemId = 0, string phoneShipperNumber = null);

        IList<ShipmentManual> GetShipmentsCustomByIds(int[] shipmentCustomIds);

        ShipmentManual GetShipmentCustomById(int shipmentId);

        bool CheckExistTrackingNumber(string trackingNumber);

        void InsertShipmentCustom(ShipmentManual shipmentCustom);

        void UpdateShipmentCustom(ShipmentManual shipmentCustom);

    }
}
