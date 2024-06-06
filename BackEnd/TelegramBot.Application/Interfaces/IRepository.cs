using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Interfaces;
public interface IRepository<T> where T : BaseEntity
{
    public IQueryable<T> GetAll();
    public T GetById(int id);
    public T FirstOrDefault();
    public void Add(T entity);
    public void Add(IEnumerable<T> entity);
    public void Edit(T entity);
    public void Remove(T entity);
    public void RemoveRange(IEnumerable<T> entity);
    public void SaveChanges();
    public void StartTransaction();
    public void CommitTransaction();
    public void RollbackTransaction();
}