using ImageResizerApp;
using Microsoft.Win32;
using System.Windows;

namespace ImageResizerAApp
{
    public partial class AddEditImageWindow : Window
    {
        public ImageDetail ImageDetail { get; private set; }

        public AddEditImageWindow(ImageDetail imageDetail = null)
        {
            InitializeComponent();

            if (imageDetail != null)
            {
                // Editing an image
                ImageDetail = imageDetail;
                txtImageName.Text = imageDetail.Name;
                txtWidth.Text = imageDetail.Width.ToString();
                txtHeight.Text = imageDetail.Height.ToString();
                txtRotation.Text = imageDetail.Rotation.ToString();
            }
            else
            {
                // Adding a new image
                ImageDetail = new ImageDetail();
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                ImageDetail.ImagePath = openFileDialog.FileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate input, assign values to ImageDetail, and close the window
            ImageDetail.Name = txtImageName.Text;
            ImageDetail.Width = int.Parse(txtWidth.Text);
            ImageDetail.Height = int.Parse(txtHeight.Text);
            ImageDetail.Rotation = int.Parse(txtRotation.Text);
            DialogResult = true; // Close window with success
        }
    }
}
