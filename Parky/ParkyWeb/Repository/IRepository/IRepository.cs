﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParkyWeb.Repository.IRepository
{
    interface IRepository<T> where T : class
    {
        Task<T> GetAsync(string url, int Id);
        Task<IEnumerable<T>> GetAllAsync(string url);
        Task<bool> CreateAsync(string url, T objToCreate);
        Task<bool> UpdateAsync(string url, T objToCreate);
        Task<bool> DeleteAsync(string url, int Id);

    }
}