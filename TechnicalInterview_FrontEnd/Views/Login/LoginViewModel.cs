using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnicalInterview_FrontEnd.API;
using TechnicalInterview_FrontEnd.Navigation;

namespace TechnicalInterview_FrontEnd.Views.Login
{
    public class LoginViewModel : BindableBase
    {
        private readonly DataService _data;
        private readonly NavigationService _navigation;

        private string _username = "username";
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value, () => RaisePropertyChanged(nameof(Username))); }
        }

        private string _password = "password";
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value, () => RaisePropertyChanged(nameof(Password))); }
        }

        public DelegateCommand LoginCommand { get; private set; }

        public LoginViewModel(DataService data, NavigationService navigation)
        {
            _data = data;
            _navigation = navigation;

            LoginCommand = new DelegateCommand(Login);
        }

        private async void Login()
        {
            if (await _data.ValidateUserAsync(Username, Password))
            {
                _navigation.NavigateToVoterSearch();
            }
            else
            {
                MessageBox.Show("Incorrect Username or Password");
            }
        }
    }
}
