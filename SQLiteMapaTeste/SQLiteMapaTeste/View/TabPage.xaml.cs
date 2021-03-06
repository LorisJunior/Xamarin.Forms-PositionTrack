using SQLiteMapaTeste.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabPage : TabbedPage
    {
        public TabPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, int>(this, "click", (arg, idx) =>
            {
                var chat = this.Children[idx] as ChatPage;
                var localUser = arg as User;
                chat.LocalUser = localUser;
                CurrentPage = this.Children[idx];
            });
        }
    }
}