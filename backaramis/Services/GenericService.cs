using backaramis.Interfaces;
using backaramis.Models;
using Microsoft.EntityFrameworkCore;

namespace backaramis.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
    {
        private readonly AramisContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericService(AramisContext context)
        {
            try
            {
                _context = context;
                _dbSet = _context.Set<TEntity>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Add(TEntity data)
        {
            try
            {
                _dbSet.Add(data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public void Add(List<TEntity> data)
        {
            try
            {
                _dbSet.AddRange(data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public bool Delete(long id)
        {
            try
            {

                TEntity? dataToDelete = _dbSet.Find(id);
                if (dataToDelete is null)
                {
                    return false;
                }

                _dbSet.Remove(dataToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                TEntity? dataToDelete = _dbSet.Find(id);
                if (dataToDelete is null)
                {
                    return false;
                }

                _dbSet.Remove(dataToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public List<TEntity> Get()
        {
            try
            {
                return _dbSet.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public TEntity Get(int id)
        {
            try
            {
                return _dbSet.Find(id)!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }

        public void Update(List<TEntity> data)
        {
            try
            {
                _dbSet.UpdateRange(data);

                foreach (TEntity? item in data)
                {
                    _context.Entry(item).State = EntityState.Modified;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
