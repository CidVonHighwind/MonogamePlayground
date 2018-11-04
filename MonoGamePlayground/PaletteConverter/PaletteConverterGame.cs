using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.PaletteConverter
{
    class ColorData
    {
        public Texture2D SprTexture;
        public Color[] colors = new Color[4];
    }

    class PaletteConverterGame : IGame
    {
        private string StrColorData =
            "{{0x03, 0x96, 0x87, 0xFF},{0x03, 0x6B, 0x4D, 0xFF},{0x03, 0x55, 0x2B, 0xFF},{0x03, 0x44, 0x14, 0xFF}},{{0xEF, 0xFA, 0xF5, 0xFF},{0x70, 0xC2, 0x86, 0xFF},{0x57, 0x69, 0x2F, 0xFF},{0x20, 0x19, 0x0B, 0xFF}},{{0xFF, 0xFF, 0xFF, 0xFF},{0xAA, 0xAA, 0xAA, 0xFF},{0x55, 0x55, 0x55, 0xFF},{0x00, 0x00, 0x00, 0xFF}},{{0xC8, 0xE8, 0xF8, 0xFF},{0x48, 0x90, 0xD8, 0xFF},{0x20, 0x34, 0xA8, 0xFF},{0x50, 0x18, 0x30, 0xFF}},{{0xAA, 0xE0, 0xE0, 0xFF},{0x7C, 0xB8, 0xB0, 0xFF},{0x5B, 0x82, 0x72, 0xFF},{0x17, 0x34, 0x39, 0xFF}},{{0xA5, 0xEB, 0xD4, 0xFF},{0x7C, 0xB8, 0x62, 0xFF},{0x5D, 0x76, 0x27, 0xFF},{0x39, 0x39, 0x1D, 0xFF}}";
        
        List<Color[]> ColorList = new List<Color[]>();
        List<ColorData> colorDataList = new List<ColorData>();

        private Texture2D sprWhite;

        private string colorDirPath = "D:\\Desktop\\GearboyColor";

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            LoadImages();
            //RenameStates();

            ColorList = ExtractData(StrColorData);

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (var i = 0; i < colorDataList.Count; i++)
                spriteBatch.Draw(colorDataList[i].SprTexture, new Vector2(0, i * colorDataList[i].SprTexture.Height), Color.White);

            //for (int i = 0; i < ColorList.Count; i++)
            //{
            //    for (int j = 0; j < ColorList[i].Length; j++)
            //    {
            //        spriteBatch.Draw(sprWhite, new Rectangle(j * 32, i * 32, 32, 32), ColorList[i][3 - j]);
            //    }
            //}

            spriteBatch.End();
        }

        public void RenameStates()
        {
            var files = Directory.GetFiles("D:\\Desktop\\state");

            for (var i = 0; i < files.Length; i++)
                File.Move(files[i], files[i].Replace("slot", "state"));
        }

        public void LoadImages()
        {
            DirectoryInfo di = new DirectoryInfo(colorDirPath);

            if (!di.Exists)
                return;

            FileSystemInfo[] files = di.GetFileSystemInfos();
            var orderedFiles = files.OrderBy(f => f.Name);

            foreach (var fileSystemInfo in orderedFiles)
            {
                var newColorData = new ColorData();

                using (var file = File.Open(fileSystemInfo.FullName, FileMode.Open))
                    newColorData.SprTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, file);

                for (var j = 0; j < 4; j++)
                {
                    var colorData = new Color[newColorData.SprTexture.Width * newColorData.SprTexture.Height];
                    newColorData.SprTexture.GetData(colorData);
                    var width = newColorData.SprTexture.Width;
                    var colorWidth = width / 4;
                    var halfWidth = colorWidth / 2;
                    newColorData.colors[3 - j] = colorData[j * colorWidth + halfWidth];
                }

                colorDataList.Add(newColorData);
            }

            //colorDataList.Sort(CompareColorData);
            GetData();
        }

        private int CompareColorData(ColorData dataOne, ColorData dataTwo)
        {
            var colorOneMixR = 0;
            var colorOneMixG = 0;
            var colorOneMixB = 0;
            var colorTwoMixR = 0;
            var colorTwoMixG = 0;
            var colorTwoMixB = 0;

            for (var i = 0; i < 4; i++)
            {
                colorOneMixR += dataOne.colors[i].R;
                colorOneMixG += dataOne.colors[i].G;
                colorOneMixB += dataOne.colors[i].B;

                colorTwoMixR += dataTwo.colors[i].R;
                colorTwoMixG += dataTwo.colors[i].G;
                colorTwoMixB += dataTwo.colors[i].B;
            }

            var colorOneD = colorOneMixR * 0.2126 + colorOneMixG * 7152 + colorOneMixB * 0722;
            var colorTwoD = colorTwoMixR * 0.2126 + colorTwoMixG * 7152 + colorTwoMixB * 0722;
            return (int)(colorOneD - colorTwoD);

            var diff = 0;
            for (var i = 0; i < 4; i++)
            {
                var colorOne = dataOne.colors[i].R * 0.2126 + dataOne.colors[i].G * 7152 + dataOne.colors[i].B * 0722;
                var colorTwo = dataTwo.colors[i].R * 0.2126 + dataTwo.colors[i].G * 7152 + dataTwo.colors[i].B * 0722;
                diff += (int)(colorOne - colorTwo);
            }

            return diff;
        }

        public void GetData()
        {
            string outputString = "";

            for (var i = 0; i < colorDataList.Count; i++)
            {
                outputString += "{";
                for (var j = 0; j < 4; j++)
                {
                    outputString += "{";
                    outputString += string.Format("0x{0:X2}, 0x{1:X2}, 0x{2:X2}, 0x{3:X2}",
                        (int)colorDataList[i].colors[j].R, (int)colorDataList[i].colors[j].G,
                        (int)colorDataList[i].colors[j].B, (int)colorDataList[i].colors[j].A);
                    outputString += "}";
                    if (j < 3)
                        outputString += ", ";
                }
                outputString += "},\n";
            }

            Clipboard.SetText(outputString);
        }

        public List<Color[]> ExtractData(string strInput)
        {
            strInput = strInput.Replace("}", "").Replace("{", "").Replace(" ", "").Replace("0x", "");
            var split = strInput.Split(',');

            var state = 0;
            var colorIndex = 0;
            var clData = new List<Color[]>();
            var newColor = new Color[4];

            for (var i = 0; i < split.Length; i++)
            {
                if (colorIndex == 0)
                    newColor[state].R = (byte)Convert.ToInt32(split[i], 16);
                else if (colorIndex == 1)
                    newColor[state].G = (byte)Convert.ToInt32(split[i], 16);
                else if (colorIndex == 2)
                    newColor[state].B = (byte)Convert.ToInt32(split[i], 16);
                else if (colorIndex == 3)
                    newColor[state].A = (byte)Convert.ToInt32(split[i], 16);

                colorIndex++;
                if (colorIndex >= 4)
                {
                    state++;
                    colorIndex = 0;
                    if (state >= 4)
                    {
                        clData.Add(newColor);
                        newColor = new Color[4];
                        state = 0;
                    }
                }
            }

            return clData;
        }
    }
}
