using SQLite;
using People.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace People
{
    public class PersonRepository
    {
        string _dbPath;

        public string StatusMessage { get; set; }

        // Variable para la conexión SQLite asíncrona
        private SQLiteAsyncConnection conn;

        private async Task Init()
        {
            if (conn != null)
                return;

            conn = new SQLiteAsyncConnection(_dbPath);
            await conn.CreateTableAsync<SL_Person>();
        }

        public PersonRepository(string dbPath)
        {
            _dbPath = dbPath;                        
        }

        public async Task AddNewPersonAsync(string name)
        {            
            int result = 0;
            try
            {
                await Init();

                // validación básica para asegurar que se ingresó un nombre
                if (string.IsNullOrEmpty(name))
                    throw new Exception("Valid name required");

                result = await conn.InsertAsync(new SL_Person { Name = name });
                
                StatusMessage = string.Format("{0} record(s) added (Name: {1})", result, name);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
            }

        }

        public async Task<List<SL_Person>> GetAllPeopleAsync()
        {
            // Inicializar y luego recuperar una lista de objetos Person de la base de datos en una lista
            try
            {
                await Init();
                return await conn.Table<SL_Person>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<SL_Person>();
        }

        public async Task UpdatePersonAsync(SL_Person person)
        {
            try
            {
                await Init();
                int result = await conn.UpdateAsync(person);
                StatusMessage = string.Format("{0} record(s) updated (Name: {1})", result, person.Name);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to update {0}. Error: {1}", person.Name, ex.Message);
            }
        }

        public async Task DeletePersonAsync(int id)
        {
            try
            {
                await Init();
                var person = await conn.GetAsync<SL_Person>(id);
                int result = await conn.DeleteAsync(person);
                StatusMessage = string.Format("{0} record(s) deleted (ID: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete person with ID {0}. Error: {1}", id, ex.Message);
            }
        }
    }
}
