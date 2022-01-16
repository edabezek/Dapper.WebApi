using Dapper.Application.Interfaces;
using Dapper.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IConfiguration configuration;
        public ProductRepository(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
        }

    }
}
