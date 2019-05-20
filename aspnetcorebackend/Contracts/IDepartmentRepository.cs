using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aspnetcorebackend.Models.Entities;

namespace aspnetcorebackend.Contracts
{
    public interface IDepartmentRepository
    {
        bool Exists(Guid id);
        IEnumerable<Department> GetAll();
        Department GetById(Guid id);
        Task<Department> CreateAsync(Department department);
        Task<Department> UpdateAsync(Department department);
        Task DeleteAsync(Guid id);
    }
}