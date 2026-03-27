using CreditSimulator.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CreditSimulator.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        public async Task Init()
        {
            if (_db != null)
                return;

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CreditSim.db");
            _db = new SQLiteAsyncConnection(dbPath);
            await _db.CreateTableAsync<SimulationRecord>();
        }

        public async Task<int> SaveSimulationAsync(SimulationRecord record)
        {
            await Init();
            return await _db.InsertAsync(record);
        }

        public async Task<List<SimulationRecord>> GetSimulationsAsync()
        {
            await Init();
            return await _db.Table<SimulationRecord>().OrderByDescending(s => s.CreatedAt).ToListAsync();
        }
    }
}
