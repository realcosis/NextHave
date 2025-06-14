using System.Data;
using MySqlConnector;
using Dolphin.Core.Database;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Dolphin.Core.Configurations.Models;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.DAL.MySQL
{
    [Service(ServiceLifetime.Scoped)]
    public class MySQLDbContext(IOptions<MySQLConfiguration> mysqlConfigurationOptions, IOptions<PoolConfiguration> poolConfigurationOptions) : MySQLDBContext(mysqlConfigurationOptions, poolConfigurationOptions)
    {
        public DbSet<ItemDefinitionEntity> ItemDefinitions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
            => base.OnConfiguring(builder);

        public async Task<int?> GetNextSequenceValue(string sequenceName)
        {
            using var command = Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetNextSequenceValue";
            command.CommandType = CommandType.StoredProcedure;

            var sequenceParam = new MySqlParameter("@sequenceName", MySqlDbType.VarChar)
            {
                Value = sequenceName,
                Direction = ParameterDirection.Input
            };
            var outputParam = new MySqlParameter("@nextValue", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(sequenceParam);
            command.Parameters.Add(outputParam);

            await command.ExecuteNonQueryAsync();

            return int.TryParse(outputParam.Value!.ToString(), out var nextValue) ? nextValue : default;
        }
    }
}