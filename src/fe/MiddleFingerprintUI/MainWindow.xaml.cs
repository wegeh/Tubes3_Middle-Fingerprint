﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Windows.Input;
using System.Windows.Media;

namespace MiddleFingerprintUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isToggled = false;
        private Bitmap inputFingerprint;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None; // Remove window border and title bar
            this.ResizeMode = ResizeMode.NoResize; // Disable resizing
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Open window in the center of the screen
        }

        private void handle_upload(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image file";
            openFileDialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png"; // Filter to only show image files

            // Show open file dialog box
            bool? result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = openFileDialog.FileName;
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filename);
                    bitmap.EndInit();
                    this.inputFingerprint = new Bitmap(filename);

                    // Set the image source
                    imageUpload.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void handle_search(object sender, RoutedEventArgs e)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            FingerprintOwner result = API.SearchFingerprint(inputFingerprint, !_isToggled);
            if (result.image != null)
            {
                imageResult.Source = BitmapToImageSource(result.image);
                List<string> biodata = API.getOwnerBiodata(result.nama);
                string b = "";
                foreach (string s in biodata){
                    b += s + '\n';
                }
                MessageBox.Show(b);

            }
            else
            {
                MessageBox.Show("No match found.");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();  // Closes the MainWindow
        }

        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void AlgorithmToggle_Checked(object sender, RoutedEventArgs e)
        {
            UpdateAlgorithm();
        }

        private void AlgorithmToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateAlgorithm();
        }

        private void UpdateAlgorithm()
        {
            if (algorithmToggle.IsChecked == true)
            {
                _isToggled = true;
            }
            else
            {
                _isToggled = false;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;  // Opens the Popup
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false; // Closes the Popup
        }
    }
}