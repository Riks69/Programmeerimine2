using KooliProjekt.WpfApp;
using System.Collections.ObjectModel;
using System.Windows.Input;
using KooliProjekt.WpfApp.Api;

namespace KooliProjekt.WpfApp
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public ObservableCollection<Customer> Customers { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public Predicate<Customer> ConfirmDelete { get; set; }
        public Action<string> OnError { get; set; }

        private readonly IApiClient _apiClient;

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                NotifyPropertyChanged();
            }
        }

        public object Lists { get; set; }

        public MainWindowViewModel() : this(new ApiClient())
        {
        }

        public MainWindowViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
            Customers = new ObservableCollection<Customer>();

            NewCommand = new RelayCommand<Customer>(
                customer => { SelectedCustomer = new Customer(); }
            );

            SaveCommand = new RelayCommand<Customer>(
                async customer =>
                {
                    var result = await _apiClient.Save(SelectedCustomer);
                    if (result.HasError)
                    {
                        OnError?.Invoke($"Error while saving customer: {result.Error}");
                        return;
                    }

                    await LoadCustomers();
                },
                customer => SelectedCustomer != null
            );

            DeleteCommand = new RelayCommand<Customer>(
                async customer =>
                {
                    if (ConfirmDelete?.Invoke(SelectedCustomer) ?? true)
                    {
                        var result = await _apiClient.Delete(SelectedCustomer.Id);
                        if (result.HasError)
                        {
                            OnError?.Invoke($"Error while deleting customer: {result.Error}");
                            return;
                        }

                        Customers.Remove(SelectedCustomer);
                        SelectedCustomer = null;
                    }
                },
                customer => SelectedCustomer != null
            );
        }

        public async Task LoadCustomers()
        {
            Customers.Clear();

            var result = await _apiClient.List();
            if (result.HasError)
            {
                OnError?.Invoke($"Error while loading customers: {result.Error}");
                return;
            }

            foreach (var customer in result.Value)
            {
                Customers.Add(customer);
            }
        }
    }
}