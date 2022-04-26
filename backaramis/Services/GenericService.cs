using backaramis.Interfaces;
using backaramis.Models;
using Microsoft.EntityFrameworkCore;

namespace backaramis.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
    {
        private readonly aramisContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericService(aramisContext context)
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
                throw new Exception(ex.Message);
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
                throw new Exception(ex.Message);
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
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<TEntity> Get()
        {
            try
            {
                return _dbSet.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TEntity Get(int id)
        {
            try
            {
                return _dbSet.Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(TEntity data)
        {
            try
            {
                _dbSet.Attach(data);
                _context.Entry(data).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
