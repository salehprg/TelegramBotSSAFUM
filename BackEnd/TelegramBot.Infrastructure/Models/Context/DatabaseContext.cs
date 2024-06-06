using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TelegramBot.Core.Entities;
using TelegramBot.Infrastructure.Models;


namespace TelegramBot.Infrastructure;


public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

        return new DatabaseContext(optionsBuilder.Options);
    }
}

public class DatabaseContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
{
    public DatabaseContext(DbContextOptions options)
        : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.ConnectionString);
    }

    public DbSet<BotSettingsEntity> BotSettings { get; set; }
    public DbSet<PostInfoEntity> PostInfos { get; set; }
    public DbSet<PostInfoEvent> PostInfoEvents { get; set; }
    public DbSet<ChannelInfoEntity> Channels { get; set; }
    public DbSet<LogModel> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostInfoEntity>().Navigation(x => x.PostEvents).AutoInclude();
    }
}