namespace backaramis.Interfaces
{
    public interface IGenericService<TEntity>
    {
        List<TEntity> Get();
        TEntity Get(int id);
        void Add(TEntity data);
        void Add(List<TEntity> data);
        bool Delete(long id);
        bool Delete(int id); 
        void Update(List<TEntity> data);

    }
}
