﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public class VillaRepository<T> :Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}

//q: what does .net means?

