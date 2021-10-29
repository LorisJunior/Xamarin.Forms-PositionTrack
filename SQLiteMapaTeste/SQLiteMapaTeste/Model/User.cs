using SQLite;
using SQLiteMapaTeste.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace SQLiteMapaTeste.Model
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Sobre { get; set; }
        public byte[] Buffer { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool DisplayUserInMap { get; set; }
        public List<Item> Items{ get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Conversa> Conversas { get; set; }

        public FirebaseService fbService = new FirebaseService();

        public static User UpdatePosition(User user, Position position)
        {
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                user.Latitude = position.Latitude;
                user.Longitude = position.Longitude;
                db.CreateTable<User>();
                db.Update(App.user);
            }
            return user;
        }

        public static User GetUser(int userId)
        {
            User user;
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                db.CreateTable<User>();
                var users = db.Table<User>().ToList();
                user = users.Where(u => u.Id == userId).FirstOrDefault();
            }
            return user;
        }

        public static IEnumerable<User> GetNearUsers(double raio)
        {
            IEnumerable<User> users;
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                db.CreateTable<User>();
                users = 
                    db.Table<User>()
                    .ToList()
                    .Where(u => u.Id != App.user.Id && 
                    DistanceService
                    .CompareDistance(App.user.Latitude, App.user.Longitude, u.Latitude, u.Longitude) <= (raio / 1000));
            }
            return users;
        }

        public async Task<IEnumerable<User>> GetNearUsersFb(double raio)
        {
            IEnumerable<User> users;
            users = (await fbService.GetUsers()).Where(u => u.Id != App.user.Id &&
            (DistanceService.CompareDistance(App.user.Latitude, App.user.Longitude, u.Latitude, u.Longitude) <= (raio / 1000)));
            return users;
        }
    }
}
