using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LevelEditor.Views {
    /// <summary>
    /// Interaction logic for CreateMapDialog.xaml
    /// </summary>
    public partial class CreateMapDialog : Window {

        public CreateMapDialog() {

            InitializeComponent();

            CreateButton.IsEnabled = false;
            CreateButton.Click += OnCreateClicked;
            CancelButton.Click += OnCancelClicked;
            DimensionSelectBox.SelectionChanged += OnEdit;
            NameTextBox.TextChanged += OnEdit;

            NameTextBox.Focus();

        }



        public int Dimension { get; private set; }
        public string MapName { get; private set; }



        private void OnEdit(object sender, EventArgs e)
        {

            CreateButton.IsEnabled =
                DimensionSelectBox.SelectionBoxItem != null && !string.IsNullOrWhiteSpace(NameTextBox.Text);

        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnCreateClicked(object sender, RoutedEventArgs e) {

            Finish();
        }



        private void OnKeyDownCreate(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter) {

                Finish();

            }

        }



        private void Finish() {

            if (!string.IsNullOrWhiteSpace(NameTextBox.Text) && DimensionSelectBox.SelectionBoxItem != null) {

                Dimension = Convert.ToInt32(DimensionSelectBox.SelectionBoxItem);
                MapName = NameTextBox.Text;
                DialogResult = true;
            }

        }
    }
}
