using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using ImageCircle.Forms.Plugin.Abstractions;
using Plugin.Geolocator;
using Plugin.ImageEdit;
using SQLiteMapaTeste.Model;
using SQLiteMapaTeste.Service;
using SQLiteMapaTeste.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Circle circle = new Circle();

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
                    circle.Radius = Distance.FromMeters(raio);
                }
            }
        }

        public MapPage()
        {
            InitializeComponent();
            BindingContext = this;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.PinClicked += OnPinClicked;
            CreatePin(App.user, true);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(App.user.Latitude, App.user.Longitude), Distance.FromMeters(5000)), true);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Pega a posição do usuário e começa a receber as atualizações de posição
            locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(new TimeSpan(0, 0, 0), 100);
            var position = await locator.GetPositionAsync();


            var center = new Position(position.Latitude, position.Longitude);
            App.user = User.UpdatePosition(App.user, center);
            CreatePin(App.user, true);

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

            try
            {
                App.user = User.UpdatePosition(App.user, center);
                userPin.Position = center;

            }
            catch (Exception)
            {
            }
            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);
        }
        public void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            Pin pin = e.Pin;
            int userId = (int)pin.Tag;
            var user = User.GetUser(userId);

            //Envio uma mensagem para a TabPage ir para a ChatPage
            MessagingCenter.Send<object, int>(user, "click", 2);
        }
        public void CreatePin(User user, bool isMyPin)
        {
            



            Stream stream = null;
            try
            {
                stream = new MemoryStream(user.Buffer);
            }
            catch (Exception)
            {
            }
            Pin pin = new Pin()
            {
                Icon = BitmapDescriptorFactory.FromView(new BindingPinView(stream)),
                //Icon = BitmapDescriptorFactory.FromStream(new MemoryStream(croped)),
                Type = PinType.Place,
                Label = "Olá, vms comprar juntos!",
                ZIndex = 5,
                Tag = user.Id
            };
            
            if (user.Latitude != 0 || user.Longitude != 0)
            {
                pin.Position = new Position(user.Latitude, user.Longitude);
            }

            if (isMyPin == true)
            {
                userPin = pin;
                map.Pins.Add(userPin);
            }
            else
            {
                map.Pins.Add(pin);
            }
        }
        private void NearUsersClicked(object sender, EventArgs e)
        {
            CleanMap(map.Pins);
            CreatePin(App.user, true);
            AddNearUsers();
        }
        public void AddNearUsers()
        {
            var users = User.GetNearUsers(Raio);
            foreach (var user in users)
            {
                CreatePin(user, false);
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
        public void CleanMap(IEnumerable<object> o)
        {
            if (o is IList<Circle> && o != null)
            {
                try
                {
                    map.Circles.Clear();
                }
                catch (Exception)
                {
                }
            }
            else if (o is IList<Pin> && o != null)
            {
                try
                {
                    map.Pins.Clear();
                }
                catch (Exception)
                {
                }
            }
            else
            {
                return;
            }
        }
        
    }
}