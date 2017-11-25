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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
/*
 * Ok so what is this program for? 
 * -> use your writting characters to create a 'font'
 *    for generating a fake handmade document
 *    that looks like a handmade document
 * -> add an option to highlight important text (put color then text)
 * -> add an option to use lines on the paper:
 *    generate a sheet of paper and put your text into it (have to remove white back of all .png's files)
 * -> generate a picture
 */
namespace NotesCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool TextSelected = false;
        private bool FontLoaded = false;
        private bool Converted = false;
        private String dir = "font/";
        public MainWindow()
        {
            InitializeComponent();
            UpdateStatus();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void buttonConvert_Click(object sender, RoutedEventArgs e)
        {
            string[] pathsToPrint = new string[textBoxToConvert.Text.Length];
            // for now getting text from textbox
            // TODO: read text from file
            pathsToPrint = convertEntered(textBoxToConvert.Text);
            CombineBitmap(pathsToPrint, textBoxSaveAs.Text);
            UpdateStatus();
        }

        private void buttonLoadFont_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            dir = fbd.SelectedPath.ToString();
            dir.Replace("\\",  "/");
            if(dir != null)FontLoaded = true;
            UpdateStatus();
        }

        private void buttonSelectText_Click(object sender, RoutedEventArgs e)
        {

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            labelStatus.Foreground = System.Windows.Media.Brushes.Black;
            if (TextSelected)
            {
                labelTextSelected.Foreground = System.Windows.Media.Brushes.Green;
                labelTextSelected.Content = "Text is selected!";
            }
            else
            {
                labelTextSelected.Foreground = System.Windows.Media.Brushes.Red;
                labelTextSelected.Content = "Text is not selected!";
            }
            if (FontLoaded)
            {
                labelFontLoaded.Foreground = System.Windows.Media.Brushes.Green;
                labelFontLoaded.Content = "Font is selected!";
            }
            else
            {
                labelFontLoaded.Foreground = System.Windows.Media.Brushes.Red;
                labelFontLoaded.Content = "Font is not selected!";
            }
            if (Converted) {
                labelConverted.Foreground = System.Windows.Media.Brushes.Green;
                labelConverted.Content = "Text converted successfully!\nYour notes are ready!";
            }
            else
            {
                labelConverted.Content = "";
            }
        }

        public string[] convertEntered(string Entered)
        {
            string[] alphabet = new string[29];
            int[] convertedText = new int[Entered.Length];
            int index = 0;
            // convert entered text (char after char) to int
            foreach(char oneEntered in Entered)
            {
                //this if changes all characters to be small
                if (oneEntered > 64 && oneEntered < 91) convertedText[index] = (int)oneEntered - 97 + 32;
                //small characters
                else if (oneEntered > 96 && oneEntered < 123) convertedText[index] = (int)oneEntered - 97;
                else if (oneEntered == '.') convertedText[index] = 26;
                else if (oneEntered == ',') convertedText[index] = 27;
                else convertedText[index] = 28;
                    index++;
            }
            // set font locations (a = a.png, b = b.png...) for now 2 chars 'a' and 'b' only
            for (int i = 0; i < 26; i++)
            {
                char oneLetter = (char)(97 + i);
                alphabet[i] += dir + "/" + oneLetter + ".png";
            }
            alphabet[26] += dir + "/" + "dot.png";
            alphabet[27] += dir + "/" + "prz.png";
            alphabet[28] += dir + "/" + "spa.png";
            string[] pathsToPrint = new string[convertedText.Length];
            // pathsToPrint - table of paths to all characters (.png)
            // => TODO: this tab can be big, think about reducing it's size (easy but for later)
            for (int i = 0; i < convertedText.Length; i++)
            {
                pathsToPrint[i] = alphabet[convertedText[i]];
            }
            // return paths of .png-s to print
            return pathsToPrint;
        }
        public static void CombineBitmap(string[] files, string saveAsName)
        { // Function copied from the internet (stackoverflow)
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

           try
           {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.White);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(offset, height - image.Height, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
                
                finalImage.Save(saveAsName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //finalImage.Dispose();
                //return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

               throw ex;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }
        
    }
}
