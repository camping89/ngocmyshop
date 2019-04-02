using Nop.Core;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service interface
    /// </summary>
    public partial interface IShipmentManualService
    {
        void DeleteShipmentManual(ShipmentManual shipmentManual);


        IPagedList<ShipmentManual> GetAllShipmentsManual(int vendorId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string shippingDistrict = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            bool exceptCity = false,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, int orderItemId = 0, string phoneShipperNumber = null, int shipperId = 0);

        IList<ShipmentManual> GetShipmentsManualByIds(int[] shipmentManualIds);

        ShipmentManual GetShipmentManualById(int id);

        bool CheckExistTrackingNumber(string trackingNumber);

        void InsertShipmentManual(ShipmentManual shipmentManual);

        void UpdateShipmentManual(ShipmentManual shipmentManual);


        void DeleteShipmentManualItem(ShipmentManualItem shipmentManualItem);

        ShipmentManualItem GetShipmentManualItemById(int id);

        void InsertShipmentManualItem(ShipmentManualItem shipmentManualItem);

        void UpdateShipmentManualItem(ShipmentManualItem shipmentManualItem);
    }
}
