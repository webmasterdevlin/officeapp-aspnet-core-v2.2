using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Helpers;
using aspnetcorebackend.Models.Dtos;
using aspnetcorebackend.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace aspnetcorebackend.Controllers
{
    [Produces("application/json")]
    [Route("departments")]
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _repo;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        
        [HttpGet] // GET: /departments
        public IActionResult GetAllDepartments()
        {
            var departments = _repo.GetAll();
            var departmentsToReturn = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
            return Ok(departmentsToReturn);
        }

        [HttpGet("{id}")] // GET: /departments/1
        public IActionResult GetDepartment([FromRoute] Guid id)
        {
            var department = _repo.GetById(id);
            var departmentToReturn = _mapper.Map<DepartmentDto>(department);
            return Ok(departmentToReturn);
        }

        [HttpPost] // POST: /departments
        public async Task<IActionResult> CreateDepartment([FromBody] Department obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj);

            var department = JsonConvert.DeserializeObject<Department>(jsonString);

            if (!ModelState.IsValid) 
                return BadRequest();

            try
            {
                await _repo.CreateAsync(department);
                return Ok();
            }
            catch (AppException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPut("{id}")] // PUT: /departments/1
        public async Task<IActionResult> UpdateDepartment([FromRoute] Guid id, [FromBody] Department department)
        {
            if (id != department.Id) 
                return BadRequest();
            
            try
            {
                await _repo.UpdateAsync(department);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.Exists(id)) 
                    return NotFound();
                
                throw;
            }
        }

        [HttpDelete("{id}")] // DELETE: /departments/1
        public async Task<IActionResult> DeleteDepartment([FromRoute] Guid id)
        {
            await _repo.DeleteAsync(id);
            return Ok();
        }
    }
}