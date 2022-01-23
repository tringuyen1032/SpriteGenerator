﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace SpriteGenerator
{
    public partial class SpritesForm : Form
    {
        private bool[] buttonGenerateEnabled = new bool[3];
        private LayoutProperties layoutProp = new LayoutProperties();
        public bool done = false;

        public SpritesForm()
        {
            InitializeComponent();
            layoutProp.layout = radioButtonAutomaticLayout.Text;
        }

        //Generate button click event. Start generating output image and CSS file.
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            layoutProp.outputSpriteFilePath = textBoxOutputImageFilePath.Text;
            layoutProp.outputCssFilePath = textBoxOutputCSSFilePath.Text;
            layoutProp.distanceBetweenImages = (int)numericUpDownDistanceBetweenImages.Value;
            layoutProp.marginWidth = (int)numericUpDownMarginWidth.Value;
            Sprite sprite = new Sprite(layoutProp);
            sprite.Create();
            //Sprite sprite = new Sprite(inputFilePaths, textBoxOutputImageFilePath.Text, textBoxOutputCSSFilePath.Text, layout,
            //    (int)numericUpDownDistanceBetweenImages.Value, (int)numericUpDownMarginWidth.Value, imagesInRow, imagesInColumn);
            string message = "Create image sprite success!";
            string title = "Success";
            MessageBox.Show(message, title);
            this.textBoxInputDirectoryPath.Text = "";
            this.textBoxOutputCSSFilePath.Text = "";
            this.textBoxOutputImageFilePath.Text = "";
            buttonGenerateEnabled = new bool[3];
            this.labelNumber.Text = "Number of selected files: 0";
        }

        //Browse input images folder.
        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string[] filters = { ".png", ".jpg", ".jpeg", ".gif", ".JPG" };
                layoutProp.inputFilePaths = (from filter in filters
                                             from file in Directory.GetFiles(folderBrowserDialog.SelectedPath)
                                             where file.EndsWith(filter)
                                             select file).ToArray();
                //If there is no file with the enabled formats in the choosen directory.
                if (layoutProp.inputFilePaths.Length == 0)
                    MessageBox.Show("This directory does not contain image files.");

                //If there are files with the enabled formats in the choosen directory.
                else
                {
                    this.labelNumber.Text = "Number of selected files: " + layoutProp.inputFilePaths.Length;
                    this.textBoxInputDirectoryPath.Text = folderBrowserDialog.SelectedPath;

                    buttonGenerateEnabled[0] = true;
                    buttonGenerate.Enabled = buttonGenerateEnabled.All(element => element == true);

                    radioButtonAutomaticLayout.Checked = true;
                    int width = Image.FromFile(layoutProp.inputFilePaths[0]).Width;
                    int height = Image.FromFile(layoutProp.inputFilePaths[0]).Height;

                    //Horizontal layout radiobutton is enabled only when all image heights are the same.
                    radioButtonHorizontalLayout.Enabled = layoutProp.inputFilePaths.All(file => OpenImage(file).Height == height);

                    //Vertical layout radiobutton is enabled only when all image widths are the same.
                    radioButtonVerticalLayout.Enabled = layoutProp.inputFilePaths.All(file => OpenImage(file).Width == width);

                    //Rectangular layout radiobutton is enabled only when all image heights and all image widths are the same.
                    radioButtonRectangularLayout.Enabled = radioButtonHorizontalLayout.Enabled && radioButtonVerticalLayout.Enabled;

                    //Setting rectangular layout dimensions.
                    if (radioButtonRectangularLayout.Enabled)
                    {
                        numericUpDownImagesInRow.Minimum = 1;
                        numericUpDownImagesInRow.Maximum = layoutProp.inputFilePaths.Length;
                        layoutProp.imagesInRow = (int)numericUpDownImagesInRow.Value;
                        layoutProp.imagesInColumn = (int)numericUpDownImagesInColumn.Value;
                    }
                    else
                    {
                        numericUpDownImagesInRow.Minimum = 0;
                        numericUpDownImagesInColumn.Minimum = 0;
                        numericUpDownImagesInRow.Value = 0;
                        numericUpDownImagesInColumn.Value = 0;
                    }
                }
            }
        }

        private Image OpenImage(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return ScaleByPercent(Image.FromStream(fs));
        }

        static Image ScaleByPercent(Image imgPhoto)
        {
            imgPhoto = (Image)new Bitmap(imgPhoto, new Size((int)(imgPhoto.Width * 0.1), (int)(imgPhoto.Height * 0.1)));

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * 0.1);
            int destHeight = (int)(sourceHeight * 0.1);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                    imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            grPhoto.Dispose();
            return bmPhoto;
        }

        //Select output image file path.
        private void buttonSelectOutputImageFilePath_Click(object sender, EventArgs e)
        {
            saveFileDialogOutputImage.ShowDialog();
            if (saveFileDialogOutputImage.FileName != "")
            {
                if (buttonGenerateEnabled[2] && textBoxOutputCSSFilePath.Text[0] != saveFileDialogOutputImage.FileName[0])
                    MessageBox.Show("Output image and CSS file must be on the same drive.");
                else
                {
                    this.textBoxOutputImageFilePath.Text = saveFileDialogOutputImage.FileName;
                    buttonGenerateEnabled[1] = true;
                    buttonGenerate.Enabled = buttonGenerateEnabled.All(element => element == true);
                    this.textBoxOutputCSSFilePath.Text = saveFileDialogOutputImage.FileName.Substring(0, saveFileDialogOutputImage.FileName.Length - 4) + ".css";
                    buttonGenerateEnabled[2] = true;
                    buttonGenerate.Enabled = buttonGenerateEnabled.All(element => element == true);
                }
            }
        }

        //Select output CSS file path.
        private void buttonSelectOutputCssFilePath_Click(object sender, EventArgs e)
        {
            saveFileDialogOutputCss.ShowDialog();
            if (saveFileDialogOutputCss.FileName != "")
            {
                if (buttonGenerateEnabled[1] &&
                    textBoxOutputImageFilePath.Text[0] != saveFileDialogOutputCss.FileName[0])
                    MessageBox.Show("Output image and CSS file must be on the same drive.");
                else
                {
                    this.textBoxOutputCSSFilePath.Text = saveFileDialogOutputCss.FileName;
                    buttonGenerateEnabled[2] = true;
                    buttonGenerate.Enabled = buttonGenerateEnabled.All(element => element == true);
                }
            }
        }

        //Rectangular layout radiobutton checked change.
        private void radioButtonRectangularLayout_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonLayout_CheckedChanged(sender, e);
            //Enabling numericupdowns to select layout dimension.
            if (radioButtonRectangularLayout.Checked)
            {
                numericUpDownImagesInRow.Enabled = true;
                numericUpDownImagesInColumn.Enabled = true;
                labelX.Enabled = true;
                labelSprites.Enabled = true;
                numericUpDownImagesInRow.Maximum = layoutProp.inputFilePaths.Length;
            }

            //Disabling numericupdowns
            else
            {
                numericUpDownImagesInRow.Enabled = false;
                numericUpDownImagesInColumn.Enabled = false;
                labelX.Enabled = false;
                labelSprites.Enabled = false;
            }
        }

        //Checked change event for all layout radiobutton.
        private void radioButtonLayout_CheckedChanged(object sender, EventArgs e)
        {
            //Setting layout field value.
            if (((RadioButton)sender).Checked)
                layoutProp.layout = ((RadioButton)sender).Text;
        }

        //Sprites in row numericupdown value changed event
        private void numericUpDownImagesInRow_ValueChanged(object sender, EventArgs e)
        {
            int numberOfFiles = layoutProp.inputFilePaths.Length;
            //Setting sprites in column numericupdown value
            numericUpDownImagesInColumn.Minimum = numberOfFiles / (int)numericUpDownImagesInRow.Value;
            numericUpDownImagesInColumn.Minimum += (numberOfFiles % (int)numericUpDownImagesInRow.Value) > 0 ? 1 : 0;
            numericUpDownImagesInColumn.Maximum = numericUpDownImagesInColumn.Minimum;

            layoutProp.imagesInRow = (int)numericUpDownImagesInRow.Value;
            layoutProp.imagesInColumn = (int)numericUpDownImagesInColumn.Value;
        }

        private void SpritesForm_Load(object sender, EventArgs e)
        {

        }
    }
}
