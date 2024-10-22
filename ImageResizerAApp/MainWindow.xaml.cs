using ImageResizerAApp;
using ImageResizerApp;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ImageResizerAApp
{
    public partial class MainWindow : Window
    {
        private List<ImageDetail> imageList = new List<ImageDetail>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Image Selection Logic
        private void ImageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageList.SelectedItem != null)
            {
                var selectedImage = (ImageDetail)ImageList.SelectedItem;
                txtImageName.Text = selectedImage.Name;
                txtWidth.Text = selectedImage.Width.ToString();
                txtHeight.Text = selectedImage.Height.ToString();
                txtRotation.Text = selectedImage.Rotation.ToString();
                imgDisplay.Source = selectedImage.ImageSource;
            }
        }

        // Exit button logic
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Export to Excel
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = ".xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Image Details");
                    worksheet.Cells[1, 1].Value = "Image Name";
                    worksheet.Cells[1, 2].Value = "Rotation";
                    worksheet.Cells[1, 3].Value = "Width";
                    worksheet.Cells[1, 4].Value = "Height";
                    worksheet.Cells[1, 5].Value = "Image Path";

                    for (int i = 0; i < imageList.Count; i++)
                    {
                        var imageDetail = imageList[i];
                        worksheet.Cells[i + 2, 1].Value = imageDetail.Name;
                        worksheet.Cells[i + 2, 2].Value = imageDetail.Rotation;
                        worksheet.Cells[i + 2, 3].Value = imageDetail.Width;
                        worksheet.Cells[i + 2, 4].Value = imageDetail.Height;
                        worksheet.Cells[i + 2, 5].Value = imageDetail.ImageSource.ToString();
                    }

                    // Save the Excel file
                    var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    package.SaveAs(fileStream);
                    fileStream.Close();
                }

                MessageBox.Show("Exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Add Image logic
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            AddEditImageWindow addWindow = new AddEditImageWindow();
            if (addWindow.ShowDialog() == true)
            {
                var newImage = addWindow.ImageDetail;
                imageList.Add(newImage);
                ImageList.Items.Add(newImage);
                ImageList.SelectedItem = newImage; // Select newly added image
            }
        }

        // Edit Image logic
        private void EditImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageList.SelectedItem != null)
            {
                var selectedImage = (ImageDetail)ImageList.SelectedItem;
                AddEditImageWindow editWindow = new AddEditImageWindow(selectedImage);
                if (editWindow.ShowDialog() == true)
                {
                    ImageList.Items.Refresh();
                }
            }
        }

        // Delete Image logic
        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageList.SelectedItem != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete the selected image?",
                    "Delete Image", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    imageList.Remove((ImageDetail)ImageList.SelectedItem);
                    ImageList.Items.Remove(ImageList.SelectedItem);
                }
            }
        }

        // Save Image Logic
        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageList.SelectedItem != null)
            {
                var selectedImage = (ImageDetail)ImageList.SelectedItem;

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg",
                    DefaultExt = ".png"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // Load the original image
                    BitmapImage bitmapImage = new BitmapImage(new Uri(selectedImage.ImageSource.ToString()));
                    var writeableBitmap = new WriteableBitmap(bitmapImage);

                    // Resize
                    int width = int.Parse(txtWidth.Text);
                    int height = int.Parse(txtHeight.Text);
                    var resizedBitmap = new TransformedBitmap(writeableBitmap, new ScaleTransform((double)width / writeableBitmap.PixelWidth, (double)height / writeableBitmap.PixelHeight));

                    // Rotate if necessary
                    int rotation = int.Parse(txtRotation.Text);
                    if (rotation != 0)
                    {
                        var rotateTransform = new RotateTransform(rotation);
                        resizedBitmap = new TransformedBitmap(resizedBitmap, rotateTransform);
                    }

                    // Save the image to file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        BitmapEncoder encoder;
                        if (filePath.EndsWith(".png"))
                        {
                            encoder = new PngBitmapEncoder();
                        }
                        else // default to JPEG
                        {
                            encoder = new JpegBitmapEncoder();
                        }
                        encoder.Frames.Add(BitmapFrame.Create(resizedBitmap));
                        encoder.Save(fileStream);
                    }

                    MessageBox.Show("Image saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select an image to save.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
