using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnicalInterview_FrontEnd.API;
using TechnicalInterview_FrontEnd.API.Models;
using TechnicalInterview_FrontEnd.Navigation;

namespace TechnicalInterview_FrontEnd.Views.VoterSearch
{
    public class VoterSearchViewModel : BindableBase
    {
        private readonly DataService _data;
        private readonly NavigationService _navigation;

        public DelegateCommand SearchCommand { get; private set; }

        public VoterSearchViewModel(DataService data, NavigationService navigation)
        {
            _data = data;
            _navigation = navigation;

            SearchCommand = new DelegateCommand(SearchForVoters);
        }

        private List<VoterActivity> _voterSearchList = new List<VoterActivity>();
        public List<VoterActivity> VoterSearchList
        {
            get { return _voterSearchList; }
            set { SetProperty(ref _voterSearchList, value, () => RaisePropertyChanged(nameof(VoterSearchList))); }
        }

        private VoterActivity _selectedVoter;
        public VoterActivity SelectedVoter
        {
            get { return _selectedVoter; }
            set 
            { 
                SetProperty(ref _selectedVoter, value, () => RaisePropertyChanged(nameof(SelectedVoter))); 

                if (_selectedVoter != null)
                {
                    _navigation.NavigateToVoterDetails(_selectedVoter);
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value, () => RaisePropertyChanged(nameof(LastName))); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value, () => RaisePropertyChanged(nameof(FirstName))); }
        }

        private string _voterId;
        public string VoterId
        {
            get { return _voterId; }
            set { SetProperty(ref _voterId, value, () => RaisePropertyChanged(nameof(VoterId))); }
        }

        private async void SearchForVoters()
        {
            // Make sure user has entered something into search fields before trying to search.
            if (TestForValidSearchCriteria())
            {
                // Search By Name
                if (!string.IsNullOrEmpty(LastName) || !string.IsNullOrEmpty(FirstName))
                {
                    List<VoterActivity>? voterList = await _data.SearchVotersByNameAsync(LastName, FirstName);
                    if (voterList != null) VoterSearchList = voterList;
                    return;
                }
                // Search By ID
                if (!string.IsNullOrEmpty(VoterId))
                {
                    // This only returns one voter
                    VoterActivity? voter = await _data.SearchVotersByIdAsync(VoterId); 
                    if (voter is not null) VoterSearchList = new List<VoterActivity>() { voter };
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please make sure you have entered a VoterID, Last Name, or First Name to search a Voter");
            }
        }

        /// <summary>
        /// Tests VoterId, LastName, and FirstName for null/empty
        /// </summary>
        /// <returns>true if there is some value in one of the search fields</returns>
        private bool TestForValidSearchCriteria()
        {
            if (string.IsNullOrEmpty(VoterId) && string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName))
            {
                return false;
            }

            return true;
        }
    }
}
