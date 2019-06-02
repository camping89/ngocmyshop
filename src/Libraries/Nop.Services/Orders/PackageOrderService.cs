using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Orders
{
    public class PackageOrderService : IPackageOrderService
    {
        private readonly IRepository<PackageOrder> _packageOrdeRepository;

        public PackageOrderService(IRepository<PackageOrder> packageOrdeRepository)
        {
            _packageOrdeRepository = packageOrdeRepository;
        }

        public IPagedList<PackageOrder> SearchPackageOrders(string code, string name, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = _packageOrdeRepository.Table;
            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(_ => _.PackageCode.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(_ => _.PackageCode.Contains(name));
            }

            query = query.OrderByDescending(_ => _.Id);

            return new PagedList<PackageOrder>(query, pageIndex, pageSize);
        }

        public IList<PackageOrder> GetPackageOrders(bool loadIsShipped = true)
        {
            if (loadIsShipped)
            {
                return _packageOrdeRepository.Table.ToList();

            }
            return (_packageOrdeRepository.Table.Where(_ => _.IsShipped == loadIsShipped)).ToList();
        }

        public PackageOrder GetById(int id)
        {
            return _packageOrdeRepository.GetById(id);
        }

        public PackageOrder GetByCode(string code)
        {
            return _packageOrdeRepository.Table.FirstOrDefault(_ => _.PackageCode == code);
        }

        public void Create(PackageOrder entity)
        {
            if (entity != null)
            {
                _packageOrdeRepository.Insert(entity);
            }
        }

        public void Update(PackageOrder entity)
        {
            if (entity != null && entity.Id > 0)
            {
                _packageOrdeRepository.Update(entity);
            }
        }

        public void SetIsShipped(int packageId, bool isShipped)
        {
            var entity = _packageOrdeRepository.GetById(packageId);
            if (entity != null)
            {
                entity.IsShipped = isShipped;
            }
        }
    }
}
