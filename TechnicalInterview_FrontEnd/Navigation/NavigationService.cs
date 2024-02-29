using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalInterview_FrontEnd.API.Models;
using TechnicalInterview_FrontEnd.Views.VoterDetails;
using TechnicalInterview_FrontEnd.Views.VoterSearch;

namespace TechnicalInterview_FrontEnd.Navigation
{
    public class NavigationService
    {
        private readonly IRegionManager _navigation;
        public NavigationService(IRegionManager navigation)
        {
            _navigation = navigation;
        }

        public void NavigateToVoterSearch()
        {
            // Clear the region if re-navigating
            ClearRegionByName("MainContent");

            _navigation.RequestNavigate("MainContent", nameof(VoterSearchView));
        }

        public void NavigateToVoterDetails(VoterActivity SelectedVoter)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(nameof(SelectedVoter), SelectedVoter);

            _navigation.RequestNavigate("MainContent", nameof(VoterDetailsView), parameters);
        }

        private void ClearRegionByName(string regionName)
        {
            foreach (var region in _navigation.Regions)
            {
                if (region.Name == regionName)
                    region.RemoveAll();
            }
        }
    }
}
