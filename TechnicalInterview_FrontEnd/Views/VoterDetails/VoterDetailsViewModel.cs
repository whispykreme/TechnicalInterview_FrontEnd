using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TechnicalInterview_FrontEnd.API;
using TechnicalInterview_FrontEnd.API.Models;
using TechnicalInterview_FrontEnd.Navigation;
using Unity;

namespace TechnicalInterview_FrontEnd.Views.VoterDetails
{
    public class VoterDetailsViewModel : BindableBase, INavigationAware
    {
        private readonly DataService _data;
        private readonly NavigationService _navigation;
        private readonly IUnityContainer _container;

        public DelegateCommand AcceptCommand { get; private set; }
        //
        // Clear Command to clear canvas is in the Code-Behind of the View
        //
        public DelegateCommand CancelCommand { get; private set; }

        public VoterDetailsViewModel(DataService data, NavigationService navigation, IUnityContainer container)
        {
            _data = data;
            _navigation = navigation;
            _container = container;

            AcceptCommand = new DelegateCommand(AcceptSignature);
            CancelCommand = new DelegateCommand(CancelVoting);
        }


        private Canvas _signatureCapture;
        public Canvas SignatureCapture
        {
            get 
            { 
                if (_container != null)
                {
                    UserControl uc = App.Current.Windows[0].Content as UserControl;
                    VoterDetailsView view = uc.Content as VoterDetailsView;
                    _signatureCapture = view.FindName("SignatureCapture") as Canvas;
                }

                return _signatureCapture; 
            
            }
            set { SetProperty(ref _signatureCapture, value, () => RaisePropertyChanged(nameof(SignatureCapture))); }
        }

        private VoterActivity _selectedVoter;
		public VoterActivity SelectedVoter
		{
			get { return _selectedVoter; }
			set { SetProperty(ref _selectedVoter, value, () => RaisePropertyChanged(nameof(SelectedVoter))); }
		}

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value, () => RaisePropertyChanged(nameof(Status))); }
        }

        private string _district;
        public string District
        {
            get { return _district; }
            set { SetProperty(ref _district, value, () => RaisePropertyChanged(nameof(District))); }
        }

        private void AcceptSignature()
        {
            if (SelectedVoter.StatusId != 3)
            {
                byte[] bytes = ConvertSignature();

                SaveSignatureToDatabase(bytes);
            }
            else MessageBox.Show("Voter has voted already!");
        }

        private byte[] ConvertSignature()
        {
            var rect = new Rect(SignatureCapture.RenderSize);
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(SignatureCapture), null, rect);
            }

            var rtb = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height, 96d, 96d, PixelFormats.Default);
            rtb.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        private async void SaveSignatureToDatabase(byte[] signature)
        {
            Signature saveSignature = new Signature 
            { 
                SignatureId = new Guid(),
                SignatureImage = signature,
                TimeStamp = DateTime.Now,
                VoterId = SelectedVoter.VoterId
            };

            if (await _data.SaveSignatureAsync(saveSignature) is Signature savedSignature)
            {
                // Update voter record to mark them as voted
                SelectedVoter.StatusId = 3;
                bool success = await _data.MarkVoterAsync(SelectedVoter);

                if (success)
                {
                    MessageBox.Show("Signature Saved!");
                    return;
                }
            }
                MessageBox.Show("An error occurred! Sorry for the inconvenience");
        }

        private void CancelVoting()
        {
            _navigation.NavigateToVoterSearch();
        }

        private void DrawSignatureOnCanvas(Signature signature)
        {
            if (signature.SignatureImage == null || signature.SignatureImage.Length == 0) return;
            
            var image = new BitmapImage();
            using (var mem = new MemoryStream(signature.SignatureImage))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();

            Image img = new Image();
            img.Source = image;

            SignatureCapture.Children.Add(img);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            // If we pass a parameter from the Navigation Context called "SelectedVoter", assign that value when navigating to the view.
            SelectedVoter = navigationContext.Parameters.GetValue<VoterActivity>(nameof(SelectedVoter));

            List<Status>? statuses = await _data.GetStatusesAsync();
            if (statuses != null) Status = statuses.First(x => x.StatusId == SelectedVoter.StatusId).StatusDescription;

            List<District>? districts = await _data.GetDistrictsAsync();
            if (districts != null) District = districts.First(x => x.DistrictId == SelectedVoter.DistrictId).DistrictName;

            Signature? signature = await _data.GetSignatureAsync(SelectedVoter.VoterId);
            DrawSignatureOnCanvas(signature);
        }
    }
}
