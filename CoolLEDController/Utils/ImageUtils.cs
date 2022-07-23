using CoolLEDProtocols;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;

namespace CoolLEDController.Utils
{
    public static class ImageUtils
    {
        private static float THREASHHOLD = 0.5f;

        public static Frames ConvertGifToFrames(Image image, int speed, bool invert = false)
        {
            List<Frame> frames = new List<Frame>();

            List<Bitmap> bitmaps = ConvertGif(image);
            foreach (Bitmap bitmap in bitmaps)
            {
                Frame frame = ConvertBitmapToFrame(bitmap, invert);
                frames.Add(frame);
            }

            return new Frames(frames, speed);
        }

        public static Frame ConvertImageToFrame(Image image, bool invert = false)
        {
            return ConvertBitmapToFrame(new Bitmap(image), invert);
        }

        public static Frame ConvertBitmapToFrame(Bitmap input, bool invert = false)
        {
            List<LEDState> states = new List<LEDState>();

            input = Resize(input);
            input = MakeGrayscale3(input);

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Color pixel = input.GetPixel(x, y);
                    float brightness = pixel.GetBrightness();
                    bool isOn = brightness > THREASHHOLD;
                    if (invert) isOn = !isOn;

                    states.Add(new LEDState(isOn));
                }
            }

            return new Frame(states);
        }

        public static List<Bitmap> ConvertGif(Image gif)
        {
            int numberOfFrames = gif.GetFrameCount(FrameDimension.Time);
            List<Bitmap> frames = new List<Bitmap>();

            for (int i = 0; i < numberOfFrames; i++)
            {
                gif.SelectActiveFrame(FrameDimension.Time, i);
                frames.Add(new Bitmap((Image)gif.Clone()));
            }

            return frames;
        }


        // Author switchonthecode
        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
                   });
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    attributes.SetColorMatrix(colorMatrix);

                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        public static Bitmap Resize(Bitmap original)
        {
            Rectangle outputRect = new Rectangle(0, 0, 48, 12);
            Bitmap outputBitmap = new Bitmap(48, 12);

            outputBitmap.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(outputBitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(original, outputRect, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return outputBitmap;
        }

        public static Image DownloadImage(string url)
        {
            var wc = new WebClient();
            return Image.FromStream(wc.OpenRead(url));
        }
    }
}