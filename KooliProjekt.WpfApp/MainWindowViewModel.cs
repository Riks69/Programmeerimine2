﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using KooliProjekt.WpfApp.Api;

namespace KooliProjekt.WpfApp
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public ObservableCollection<Customer> Lists { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public Predicate<Customer> ConfirmDelete { get; set; }

        private readonly IApiClient _apiClient;

        public MainWindowViewModel() : this(new ApiClient())
        {
        }

        public MainWindowViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;

            Lists = new ObservableCollection<Customer>();

            NewCommand = new RelayCommand<Customer>(
                // Execute
                list =>
                {
                    SelectedItem = new Customer();
                }
            );

            SaveCommand = new RelayCommand<Customer>(
                // Execute
                async list =>
                {
                    await _apiClient.Save(SelectedItem);
                    await Load();
                },
                // CanExecute
                list =>
                {
                    return SelectedItem != null;
                }
            );

            DeleteCommand = new RelayCommand<Customer>(
                // Execute
                async list =>
                {
                    if(ConfirmDelete != null)
                    {
                        var result = ConfirmDelete(SelectedItem);
                        if(!result)
                        {
                            return;
                        }
                    }

                    await _apiClient.Delete(SelectedItem.Id);
                    Lists.Remove(SelectedItem);
                    SelectedItem = null;
                },
                // CanExecute
                list =>
                {
                    return SelectedItem != null;
                }
            );
        }

        public async Task Load()
        {
            Lists.Clear();

            var result = await _apiClient.List();

            if (result.Value != null)
            {
                foreach (var customer in result.Value)
                {
                    Lists.Add(customer);
                }
            }
            else
            {
                // Võib-olla logi vea info või näita kasutajale teadet
                // MessageBox.Show(result.Error); // kui UI kontekstis sobib
            }
        }

        private Customer _selectedItem;
        public Customer SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }
    }
}