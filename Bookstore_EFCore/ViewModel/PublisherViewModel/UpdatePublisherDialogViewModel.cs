using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.PublisherViewModel
{
    internal class UpdatePublisherDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Publisher> _tcs;
        public int PublisherId { get; }
        public string NewPublisherName { get; set; }
        public string NewPublisherCountry { get; set; }
        public int? NewPublisherFoundedYear { get; set; }

        public ICommand UpdatePublisherCommand { get; }

        public UpdatePublisherDialogViewModel(Publisher selectedPublisher,
           TaskCompletionSource<Publisher> tcs)
        {
            _tcs = tcs;

            PublisherId = selectedPublisher.PublisherId;
            NewPublisherName = selectedPublisher.PublisherName;
            NewPublisherCountry = selectedPublisher.PublisherCountry;
            NewPublisherFoundedYear = selectedPublisher.PublisherFoundedYear;


            UpdatePublisherCommand = new RelayCommand<Window>(UpdatePublisher);
        }

        private void UpdatePublisher(Window window)
        {
            if (string.IsNullOrWhiteSpace(NewPublisherName) || NewPublisherCountry == null)
            {
                MessageBox.Show("All required fields must be filled to update the author.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var updatedPublisher = new Publisher
            {
                PublisherId = PublisherId,
                PublisherName = NewPublisherName,
                PublisherCountry = NewPublisherCountry,
                PublisherFoundedYear = NewPublisherFoundedYear,
            };

            if (window != null)
            {
                _tcs.SetResult(updatedPublisher);
                window.DialogResult = true;
            }
            else
            {
                MessageBox.Show("An unexpected error occurred: Unable to close the dialog.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
