using Firebase.Database;
using Firebase.Database.Query;
using SQLiteMapaTeste.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteMapaTeste.Service
{
    public class FirebaseService
    {
        FirebaseClient firebase = new FirebaseClient("https://dbteste-cbb09-default-rtdb.firebaseio.com/");

        public async Task<bool> AddUser(User user)
        {
            try
            {
                user.Key = await GetKey();
                await UpdateUserAsync(user.Key, user);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        private async Task<string> GetKey()
        {
            //Esse método cria um documento vazio e retorna uma chave
            var doc = await firebase
               .Child("Users")
                  .PostAsync(new User());
            return doc.Key;
        }
        public async Task<bool> UpdateUserAsync(string key, User user)
        {
            try
            {
                await firebase
                    .Child("Users")
                    .Child(key)
                    .PutAsync(user);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> DeleteItemAsync(string key)
        {
            //TODO
            //IRÁ SERVIR PARA DELETAR ITENS
            try
            {
                await firebase
                    .Child("Itens")
                    .Child(key)
                    .DeleteAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public async Task<User> GetUserAsync(string key)
        {
            try
            {
                return await firebase
                    .Child("Users")
                    .Child(key)
                    .OnceSingleAsync<User>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<User>> GetUsers()
        {
            return (await firebase
              .Child("Users")
              .OnceAsync<User>()).Select(item => new User
              {
                  Key = item.Object.Key,
                  Id = item.Object.Id,
                  Nome = item.Object.Nome,
                  Email = item.Object.Email,
                  Sobre = item.Object.Sobre,
                  Buffer = item.Object.Buffer,
                  Latitude = item.Object.Latitude,
                  Longitude = item.Object.Longitude
              }).ToList();
        }

        
    }
}
