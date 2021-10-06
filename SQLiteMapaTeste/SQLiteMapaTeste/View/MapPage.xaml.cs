using Plugin.Geolocator;
using SQLiteMapaTeste.Model;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Circle circle;
        Plugin.Geolocator.Abstractions.IGeolocator locator = null;
        Pin userPin;
        private double raio = 3000;

        public double Raio
        {
            get => raio;
            set
            {
                if (raio != value)
                {
                    raio = value;
                    OnPropertyChanged();
                    UpdateCircleRadius();
                }
            }
        }
        public MapPage()
        {
            circle = new Circle();
            InitializeComponent();
            BindingContext = this;

            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.PinClicked += OnPinClicked;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Pega a posição do usuário e começa a receber as atualizações de posição
            CreatePin(App.user);

            locator = CrossGeolocator.Current;

            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(new TimeSpan(0, 0, 0), 100);
            var position = await locator.GetPositionAsync();

            App.user = User.UpdatePosition(App.user, position);
           
            var center = new Position(position.Latitude, position.Longitude);

            userPin.Position = center;
            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);
        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            CleanMap(map.Pins);
            //Para de receber as posições
            await locator.StopListeningAsync();
        }
        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var center = new Position(e.Position.Latitude, e.Position.Longitude);
            userPin.Position = center;
            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);
        }
        private void NearUsersClicked(object sender, EventArgs e)
        {
            CleanMap(map.Pins);
            CreatePin(App.user);
            AddNearUsers();
        }
        public void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            Pin pin = e.Pin;
            int userId = (int)pin.Tag;
            var user = User.GetById(userId);

            //Envio uma mensagem para a TabPage ir para a ChatPage
            MessagingCenter.Send<object, int>(user, "click", 2);
        }
        public void CreatePin(User user)
        {
            userPin = new Pin()
            {
                Icon = BitmapDescriptorFactory.FromStream(new MemoryStream(user.Buffer)),
                Type = PinType.Place,
                Label = "Olá, vms comprar juntos!",
                ZIndex = 5,
                Tag = user.Id
            };
            if(user.Latitude != 0 || user.Longitude != 0)
            {
                userPin.Position = new Position(user.Latitude, user.Longitude);
            }
            map.Pins.Add(userPin);
        }
        public void AddNearUsers()
        {
            var users = User.GetNearUsers(Raio);
            foreach (var user in users)
            {
                CreatePin(user);
            }
        }
        public void CreateCircleShapeAt(Position position)
        {
            CleanMap(map.Circles);

            circle.Center = position;
            circle.Radius = Distance.FromMeters(Raio);
            circle.StrokeColor = Color.DodgerBlue;
            circle.StrokeWidth = 6f;
            circle.FillColor = Color.FromRgba(0, 0, 255, 32);
            circle.Tag = "CIRCLE";

            map.Circles.Add(circle);
        }
        public void UpdateCircleRadius()
        {
            circle.Radius = Distance.FromMeters(Raio);
        }
        public void CleanMap(IEnumerable<object> o)
        {
            if (o is IList<Circle> && o != null)
            {
                map.Circles.Clear();
            }
            else if (o is IList<Pin> && o != null)
            {
                map.Pins.Clear();
            }
            else
            {
                return;
            }
        }

    }
}