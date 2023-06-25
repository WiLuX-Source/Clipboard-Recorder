using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Drawing;
using System.IO;

namespace ClipBR
{
    /// <summary>
    ///
    /// </summary>
    public partial class MainWindow : Window
    {
        string? prevText = null;
        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ClipboardMonitor(object sender, EventArgs e)
        {
          if (RecordBtn.IsChecked == true)
            {
                if (Clipboard.ContainsText() == true && Clipboard.GetText() != "Image")
                {
                    NotImage.Opacity = 1;
                    Image.Opacity = 0;
                    if (prevText == null && Clipboard.GetText().Contains(KeyWord.Text) || prevText != Clipboard.GetText() && Clipboard.GetText().Contains(KeyWord.Text))
                    {
                        NotImage.Text = Clipboard.GetText();
                        using (StreamWriter writer = new StreamWriter("./History.txt", true))
                        {
                            writer.WriteLine(Clipboard.GetText());
                            prevText = Clipboard.GetText();
                            writer.Close();
                        }
                    }
                }
                if (Clipboard.ContainsImage() == true)
                {
                    
                    NotImage.Opacity = 0;
                    Image.Opacity = 1;
                    Image.Source = Clipboard.GetImage();
                    string CurrentTime = DateTime.Now.ToString("dd MMMM yyyy HH-mm-ss");
                    if (Directory.Exists("./Images"))
                    {
                        System.Windows.Forms.Clipboard.GetImage().Save("./Images/" + CurrentTime + ".png");
                    } else
                    {
                        Directory.CreateDirectory("./Images");
                        System.Windows.Forms.Clipboard.GetImage().Save("./Images/" + CurrentTime + ".png");
                    }
                    Clipboard.SetText("Image");
                }
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !Topmost;
            
        }

        private void RecordBtn_Checked(object sender, RoutedEventArgs e)
        {
            timer.Tick += ClipboardMonitor;
            timer.Start();
        }

        private void RecordBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
    }   
}
