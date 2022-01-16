using Dapper.Application.Interfaces;
using Dapper.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var data = await unitOfWork.Categories.GetAllAsync();
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await unitOfWork.Categories.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Add(Category category)
        {
            var data = await unitOfWork.Categories.AddAsync(category);
            return Ok(data);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await unitOfWork.Categories.DeleteAsync(id);
            return Ok(data);
        }
        [HttpPut]
        public async Task<IActionResult> Update(Category category)
        {
            var data = await unitOfWork.Categories.UpdateAsync(category);
            return Ok(data);
        }
    }
}
