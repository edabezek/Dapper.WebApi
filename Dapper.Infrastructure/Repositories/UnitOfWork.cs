using Dapper.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace Dapper.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IConfiguration configuration { get; private set; }
        public UnitOfWork(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        public IProductRepository Products => _productRepository= _productRepository ??  new ProductRepository(this.configuration);

        public ICategoryRepository Categories => _categoryRepository = _categoryRepository ?? new CategoryRepository(this.configuration);
    }
}
