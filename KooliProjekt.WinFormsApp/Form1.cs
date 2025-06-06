using KooliProjekt.PublicAPI.Api;

namespace KooliProjekt.WinFormsApp
{
    public partial class Form1 : Form
    {
        private IApiClient _apiClient;
        private Customer _selectedCustomer;

        public Form1()
        {
            InitializeComponent();

            _apiClient = new ApiClient();  // Initsialiseerime API klienti

            // Ühendame nupud nende sündmustega
            NewButton.Click += NewButton_Click;
            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            TodoListsGrid.SelectionChanged += TodoListsGrid_SelectionChanged;
        }

        // NewButton sündmuse käitleja
        private void NewButton_Click(object? sender, EventArgs e)
        {
            // Tühjenda kõik väljundväljad
            IdField.Clear();
            TitleField.Clear();
            _selectedCustomer = null;  // Ei ole valitud klienti
        }

        // SaveButton sündmuse käitleja
        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            if (_selectedCustomer == null || string.IsNullOrEmpty(IdField.Text)) // Kui klient ei ole valitud või ID puudub
            {
                // Loome uue kliendi
                _selectedCustomer = new Customer
                {
                    Name = TitleField.Text,
                    IsRegistered = true,
                    Email = EmailField.Text,
                    Password = PasswordField.Text
                };

                // Salvestame uue kliendi API-sse
                await _apiClient.Save(_selectedCustomer);
            }
            else
            {
                // Kui klient on juba olemas, uuendame andmed
                _selectedCustomer.Name = TitleField.Text;
                _selectedCustomer.Email = EmailField.Text;
                _selectedCustomer.Password = PasswordField.Text;

                // Salvestame andmed API kaudu
                await _apiClient.Save(_selectedCustomer);
            }

            // Laadime andmed uuesti
            await LoadData();
        }

        // DeleteButton sündmuse käitleja
        private async void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_selectedCustomer == null) return;

            var confirmResult = MessageBox.Show("Kas olete kindel, et soovite kustutada?", "Kustutamine", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                await _apiClient.Delete(_selectedCustomer.Id);
                _selectedCustomer = null; // Tühjendame valitud kliendi

                // Laadime andmed uuesti
                await LoadData();
            }
        }

        // TodoListsGrid valiku muutumise käitleja
        private void TodoListsGrid_SelectionChanged(object? sender, EventArgs e)
        {
            if (TodoListsGrid.SelectedRows.Count == 0) return;

            var customer = (Customer)TodoListsGrid.SelectedRows[0].DataBoundItem;

            if (customer == null)
            {
                IdField.Clear();
                TitleField.Clear();
            }
            else
            {
                _selectedCustomer = customer;
                IdField.Text = customer.Id.ToString();
                TitleField.Text = customer.Name;
                EmailField.Text = customer.Email;
                PasswordField.Text = customer.Password;
            }
        }

        // Andmete laadimine
        private async Task LoadData()
        {
            var response = await _apiClient.List();

            if (response.HasError)
            {
                MessageBox.Show($"Viga andmete laadimisel: {response.Error}");
                return;
            }

            TodoListsGrid.AutoGenerateColumns = true;
            TodoListsGrid.DataSource = response.Value;
        }

        // Form laadimise ajal, laadime andmed API-st
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadData();  // Laadige andmed vormi laadimisel
        }
    }
}
