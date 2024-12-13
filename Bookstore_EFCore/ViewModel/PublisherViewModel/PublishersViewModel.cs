using BookstoreAdmin.Dialog.Publisher;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.PublisherViewModel
{
    internal class PublishersViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Publisher _selectedPublisher;
        private int? _newPublisherFoundedYear;
        public int? NewPublisherFoundedYear
        {
            get => _newPublisherFoundedYear;
            set
            {
                _newPublisherFoundedYear = value;
                OnPropertyChanged(nameof(NewPublisherFoundedYear));

                if (SelectedPublisher != null)
                {
                    SelectedPublisher.PublisherFoundedYear = value;
                    OnPropertyChanged(nameof(PublisherFoundedYearDisplay));
                }
            }
        }

        public ObservableCollection<Publisher> Publishers { get; set; }

        public string PublisherFoundedYearDisplay =>
       SelectedPublisher?.PublisherFoundedYear?.ToString() ?? "Unknown";

        public Publisher SelectedPublisher
        {
            get => _selectedPublisher;
            set
            {
                _selectedPublisher = value;
                OnPropertyChanged(nameof(SelectedPublisher));
                OnPropertyChanged(PublisherFoundedYearDisplay);
                (DeletePublisherCommand as RelayCommand)?.NotifyCanExecuteChanged();
                UpdateSelectedPublisherBooks();
            }
        }

        public ICommand DeletePublisherCommand { get; }
        public ICommand OpenAddPublisherDialogCommand { get; }
        public ICommand OpenUpdatePublisherDialogCommand { get; }


        public PublishersViewModel()
        {
            _dbContext = new BookstoreDbContext();

            LoadPublishers();

            DeletePublisherCommand = new RelayCommand(DeletePublisher, CanDeletePublisher);
            OpenAddPublisherDialogCommand = new AsyncRelayCommand(OpenAddPublisherDialogAsync);
            OpenUpdatePublisherDialogCommand = new AsyncRelayCommand(OpenUpdatePublisherDialogAsync);
        }

        private void UpdateSelectedPublisherBooks()
        {
            if (SelectedPublisher == null)
                return;

            var publisherId = SelectedPublisher.PublisherId;

            SelectedPublisher.Books = new ObservableCollection<Book>(
                _dbContext.Books
                    .AsNoTracking()
                    .Where(b => b.PublisherId == publisherId)
                    .Include(b => b.Author)
                    .ToList());
        }

        public void LoadPublishers()
        {
            Publishers = new ObservableCollection<Publisher>(
                _dbContext.Publishers
                .Include(p => p.Books)
                .ThenInclude(b => b.Author)
                .OrderBy(p => p.PublisherName)
                .ToList());
        }

        private async Task OpenAddPublisherDialogAsync()
        {
            var tcs = new TaskCompletionSource<Publisher>();
            var dialogViewModel = new AddNewPublisherDialogViewModel(tcs);

            dialogViewModel.PublisherCreated += publisher =>
            {
                Publishers.Add(publisher);
                SelectedPublisher = publisher;
            };

            var dialog = new AddNewPublisherDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                Debug.WriteLine("Publisher dialog returned true.");
            }
            else
            {
                Debug.WriteLine("Publisher dialog was canceled.");
            }
        }
        public async Task OpenUpdatePublisherDialogAsync()
        {
            if (SelectedPublisher == null)
            {
                MessageBox.Show("Please select a publisher to update.", "No publisher selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tcs = new TaskCompletionSource<Publisher>();
            var dialogViewModel = new UpdatePublisherDialogViewModel(SelectedPublisher, tcs);

            var dialog = new UpdatePublisherDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var updatedPublisher = await tcs.Task;
                if (updatedPublisher != null)
                {
                    try
                    {
                        _dbContext.Entry(SelectedPublisher).State = EntityState.Detached;

                        var publisherToUpdate = _dbContext.Publishers
                            .FirstOrDefault(p => p.PublisherId == updatedPublisher.PublisherId);

                        if (publisherToUpdate != null)
                        {
                            publisherToUpdate.PublisherName = updatedPublisher.PublisherName;
                            publisherToUpdate.PublisherCountry = updatedPublisher.PublisherCountry;
                            publisherToUpdate.PublisherFoundedYear = updatedPublisher.PublisherFoundedYear;

                            await _dbContext.SaveChangesAsync();

                            MessageBox.Show($"The publisher \"{publisherToUpdate.PublisherName}\" has been updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            OnPropertyChanged(nameof(PublisherFoundedYearDisplay));
                            var index = Publishers.IndexOf(SelectedPublisher);
                            if (index >= 0)
                            {
                                Publishers[index] = null;
                                Publishers[index] = publisherToUpdate;
                                SelectedPublisher = publisherToUpdate;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while updating the publisher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Update dialog was canceled.");
            }
        }

        private void DeletePublisher()
        {
            if (SelectedPublisher == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the publisher '{SelectedPublisher.PublisherName}'?",
                "Delete Publisher",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbContext.Entry(SelectedPublisher).State = EntityState.Detached;

                    var publisherToDelete = _dbContext.Publishers.Include(p => p.Books).FirstOrDefault(p => p.PublisherId == SelectedPublisher.PublisherId);

                    if (publisherToDelete != null)
                    {
                        _dbContext.Publishers.Remove(publisherToDelete);
                        _dbContext.SaveChanges();

                        Publishers.Remove(SelectedPublisher);
                        SelectedPublisher = null;

                        MessageBox.Show("Author deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the publisher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool CanDeletePublisher()
        {
            return SelectedPublisher != null;
        }

    }
}
