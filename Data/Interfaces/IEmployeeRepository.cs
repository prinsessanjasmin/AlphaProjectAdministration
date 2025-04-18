﻿using Data.Entities;

namespace Data.Interfaces;

public interface IEmployeeRepository : IBaseRepository<EmployeeEntity>
{
    Task<IEnumerable<EmployeeEntity>> SearchByTermAsync(string searchTerm);
}
