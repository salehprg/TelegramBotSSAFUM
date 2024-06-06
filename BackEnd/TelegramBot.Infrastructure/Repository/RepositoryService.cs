using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TelegramBot.Application.Interfaces;
using TelegramBot.Core.Entities;

namespace TelegramBot.Infrastructure;

public class RepositoryService<T> : IRepository<T> where T : BaseEntity
{
    DatabaseContext databaseContext;

    public RepositoryService(DatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public void Add(T entity)
    {
        databaseContext.Add(entity);
        SaveChanges();
    }
    public void Add(IEnumerable<T> entities)
    {
        databaseContext.AddRange(entities);
        SaveChanges();
    }

    public void Edit(T entity)
    {
        databaseContext.Update(entity);
        SaveChanges();
    }

    public T GetById(int id)
    {
        return databaseContext.Set<T>().Where(x => x.Id == id).FirstOrDefault();
    }

    public void Remove(T entity)
    {
        databaseContext.Remove(entity);
        SaveChanges();
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        databaseContext.RemoveRange(entities);
        SaveChanges();
    }

    public void StartTransaction()
    {
        databaseContext.Database.BeginTransaction();
    }

    public void RollbackTransaction()
    {
        databaseContext.Database.RollbackTransaction();
    }

    public void CommitTransaction()
    {
        databaseContext.Database.CommitTransaction();
    }

    public void SaveChanges()
    {
        databaseContext.SaveChanges();
    }

    virtual public IQueryable<T> GetAll()
    {
        return databaseContext.Set<T>().AsQueryable();
    }

    public T FirstOrDefault()
    {
        return databaseContext.Set<T>().FirstOrDefault();
    }

}