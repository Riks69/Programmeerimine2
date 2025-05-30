namespace KooliProjekt.WinFormsApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            TodoListsGrid = new DataGridView();
            IdLabel = new Label();
            IdField = new TextBox();
            TitleLabel = new Label();
            TitleField = new TextBox();
            NewButton = new Button();
            SaveButton = new Button();
            DeleteButton = new Button();
            EmailLabel = new Label();        // E-posti label
            EmailField = new TextBox();      // E-posti sisestusväli
            PasswordLabel = new Label();     // Parooli label
            PasswordField = new TextBox();   // Parooli sisestusväli

            ((System.ComponentModel.ISupportInitialize)TodoListsGrid).BeginInit();
            SuspendLayout();

            // 
            // TodoListsGrid
            // 
            TodoListsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TodoListsGrid.Location = new Point(5, 6);
            TodoListsGrid.MultiSelect = false;
            TodoListsGrid.Name = "TodoListsGrid";
            TodoListsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TodoListsGrid.Size = new Size(419, 432);
            TodoListsGrid.TabIndex = 0;

            // 
            // IdLabel
            // 
            IdLabel.AutoSize = true;
            IdLabel.Location = new Point(460, 16);
            IdLabel.Name = "IdLabel";
            IdLabel.Size = new Size(21, 15);
            IdLabel.TabIndex = 1;
            IdLabel.Text = "ID:";

            // 
            // IdField
            // 
            IdField.Location = new Point(507, 13);
            IdField.Name = "IdField";
            IdField.ReadOnly = true;
            IdField.Size = new Size(281, 23);
            IdField.TabIndex = 2;

            // 
            // TitleLabel
            // 
            TitleLabel.AutoSize = true;
            TitleLabel.Location = new Point(460, 56);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.Size = new Size(33, 15);
            TitleLabel.TabIndex = 3;
            TitleLabel.Text = "Title:";

            // 
            // TitleField
            // 
            TitleField.Location = new Point(507, 53);
            TitleField.Name = "TitleField";
            TitleField.Size = new Size(281, 23);
            TitleField.TabIndex = 4;

            // 
            // EmailLabel
            // 
            EmailLabel.AutoSize = true;
            EmailLabel.Location = new Point(460, 96);
            EmailLabel.Name = "EmailLabel";
            EmailLabel.Size = new Size(41, 15);
            EmailLabel.TabIndex = 8;
            EmailLabel.Text = "Email:";

            // 
            // EmailField
            // 
            EmailField.Location = new Point(507, 93);
            EmailField.Name = "EmailField";
            EmailField.Size = new Size(281, 23);
            EmailField.TabIndex = 9;

            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.Location = new Point(460, 136);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(58, 15);
            PasswordLabel.TabIndex = 10;
            PasswordLabel.Text = "Password:";

            // 
            // PasswordField
            // 
            PasswordField.Location = new Point(507, 133);
            PasswordField.Name = "PasswordField";
            PasswordField.Size = new Size(281, 23);
            PasswordField.TabIndex = 11;
            PasswordField.UseSystemPasswordChar = true;  // Parooli peitmine

            // 
            // NewButton
            // 
            NewButton.Location = new Point(522, 172);
            NewButton.Name = "NewButton";
            NewButton.Size = new Size(75, 23);
            NewButton.TabIndex = 5;
            NewButton.Text = "New";
            NewButton.UseVisualStyleBackColor = true;

            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(603, 172);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 6;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;

            // 
            // DeleteButton
            // 
            DeleteButton.Location = new Point(684, 172);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(75, 23);
            DeleteButton.TabIndex = 7;
            DeleteButton.Text = "Delete";
            DeleteButton.UseVisualStyleBackColor = true;

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(EmailField);
            Controls.Add(EmailLabel);
            Controls.Add(PasswordField);
            Controls.Add(PasswordLabel);
            Controls.Add(DeleteButton);
            Controls.Add(SaveButton);
            Controls.Add(NewButton);
            Controls.Add(TitleField);
            Controls.Add(TitleLabel);
            Controls.Add(IdField);
            Controls.Add(IdLabel);
            Controls.Add(TodoListsGrid);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)TodoListsGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView TodoListsGrid;
        private Label IdLabel;
        private TextBox IdField;
        private Label TitleLabel;
        private TextBox TitleField;
        private Button NewButton;
        private Button SaveButton;
        private Button DeleteButton;
        private Label EmailLabel;   // Uus label e-posti jaoks
        private TextBox EmailField; // Uus textBox e-posti jaoks
        private Label PasswordLabel;  // Uus label parooli jaoks
        private TextBox PasswordField; // Uus textBox parooli jaoks
    }
}
