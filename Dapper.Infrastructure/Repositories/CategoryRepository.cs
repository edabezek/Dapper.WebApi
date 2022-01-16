using Dapper.Application.Interfaces;
using Dapper.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IConfiguration configuration;
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
        }
    }
}
