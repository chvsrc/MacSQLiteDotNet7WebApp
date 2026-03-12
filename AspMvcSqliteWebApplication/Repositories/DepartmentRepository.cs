using System;
using System.Data;
using AspMvcSqliteWebApplication.DatabaseContexts;
using AspMvcSqliteWebApplication.Entities;
using AspMvcSqliteWebApplication.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AspMvcSqliteWebApplication.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly CompanyContext context;
        private readonly DbSet<Department> dbSet;

        public DepartmentRepository(CompanyContext context)
        {
            this.context = context;
            dbSet = context.Set<Department>();
        }

        public async Task<IEnumerable<Department>> GetAll()
        {
            if (await dbSet.AnyAsync() == false)
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Name", typeof(string));
                dataTable.Columns.Add("Location", typeof(string));
                for (int i = 0; i < 100000; i++)
                {
                    dataTable.Rows.Add("Dep Name" + i, "Dep Name" + i);
                }

                // SqlBulkCopy only works with SQL Server.
                // SQLite cannot work with SqlBulkCopy.
                /*
                using SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = "Department";
                await bulkCopy.WriteToServerAsync(dataTable);
                */


                // option - 2

                var departments = new List<Department>();
                for (int i = 1; i <= 100000; i++)
                {
                    departments.Add(new Department { Name = $"Employee {i}", Location = "Location " + i });
                }

                // Bulk insert 100k rows in SQLite
                //await context.BulkInsertAsync(departments);

                context.ChangeTracker.AutoDetectChangesEnabled = false;
            }
            return await dbSet.ToListAsync();
        }

        public async Task<Department?> GetById(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task Add(Department entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task Update(Department entity)
        {
            Department? entityData = await dbSet.FindAsync(entity.Id);
            if (entityData != null)
            {
                dbSet.Remove(entityData);
                await dbSet.AddAsync(entity);
            }
        }

        public async Task Delete(Guid id)
        {
            Department? entity = await dbSet.FindAsync(id);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public async Task<int> SaveChanges()
        {
            return await context.SaveChangesAsync();
        }
    }
}
