using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SCADA
{
    public class MMCSDbContext : DbContext
    {
        public MMCSDbContext() : base() { }

        public MMCSDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Users> users { get; set; }
        public DbSet<Log> logs { get; set; }
        public DbSet<PLCAlarm> plcalarms { get; set; }

        public DbSet<KeyValueData> keyvaluedatas { get; set; }

        public DbSet<RackUnitData> rackunitdatas { get; set; }

        public DbSet<WorkOrderData> workorderdatas { get; set; }
        public DbSet<TableUnitData> tableuintdatas { get; set; }

        public DbSet<StaticsData> staticsdatas { get; set; }

        public DbSet<MeterMode> metermodes { get; set; }

        public DbSet<MeterResult> meterresults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies().UseNpgsql(Config.GetConnectionString("WHRJ"));
            }
        }
    }
    
    public static class EntityFrameworkCoreExtensions
    {
        public static readonly bool TableNameLower = true;

        public static ModelBuilder AddModels(this ModelBuilder modelBuilder, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            #region 自动绑定Entity
            var entityTypes = assembly.GetTypes()
                .Where(type => !string.IsNullOrWhiteSpace(type.Namespace)
                && !type.IsAbstract
               && type.GetTypeInfo().IsClass
               && type.GetTypeInfo().BaseType != null
               && typeof(Entity).IsAssignableFrom(type));
            foreach (var entityType in entityTypes)
            {
                if (modelBuilder.Model.FindEntityType(entityType) == null)
                {
                    modelBuilder.Model.AddEntityType(entityType);
                }
            }
            #endregion
            #region 名称小写
            if (TableNameLower)
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    var currentTableName = modelBuilder.Entity(entity.Name).Metadata.Relational().TableName;
                    modelBuilder.Entity(entity.Name).ToTable(currentTableName.ToLower());
                    foreach (var property in entity.GetProperties())
                    {
                        modelBuilder.Entity(entity.Name).Property(property.Name).HasColumnName(property.Name.ToLower());
                    }
                }
            }
            #endregion
            return modelBuilder;
        }

        /// <summary>
        /// 生成Model时添加注释
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static ModelBuilder AddComment(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType?.IsDefined(typeof(DisplayAttribute)) == true)
                {
                    entityType.Npgsql().Comment = entityType.ClrType?.GetCustomAttribute<DisplayAttribute>().Name;
                }
                foreach (var property in entityType.GetProperties())
                {
                    if (property.PropertyInfo?.IsDefined(typeof(DisplayAttribute)) == true)
                    {
                        property.Npgsql().Comment = property.PropertyInfo?.GetCustomAttribute<DisplayAttribute>().Name;
                    }
                }
            }
            return modelBuilder;
        }
    }
}
