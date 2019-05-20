using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Models;
using aspnetcorebackend.Models.Entities;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace aspnetcorebackend.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Exists(Guid id)
        {
            return _context.Departments.Any(d => d.Id == id);
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments;
        }

        public Department GetById(Guid id)
        {
            return _context.Departments.Find(id);
        }

        public async Task<Department> CreateAsync(Department department)
        {
            department.Id = Guid.NewGuid();
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department> UpdateAsync(Department department)
        {
            _context.Update(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task DeleteAsync(Guid id)
        {
            _context.Remove(_context.Departments.Find(id));
            await _context.SaveChangesAsync();
        }
    }
}