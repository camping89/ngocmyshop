using Nop.Core;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;

namespace Nop.Services.Orders
{
    public interface IPackageOrderService
    {
        IPagedList<PackageOrder> SearchPackageOrders(string code, string name, int pageIndex = 0, int pageSize = int.MaxValue);
        IList<PackageOrder> GetPackageOrders(bool loadIsShipped = true);
        PackageOrder GetById(int id);
        void Create(PackageOrder entity);
        void Update(PackageOrder entity);

        void SetIsShipped(int packageId, bool isShipped);

    }
}
