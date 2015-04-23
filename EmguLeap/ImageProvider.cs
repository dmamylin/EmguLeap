using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Leap;

namespace EmguLeap
{
	class ImageProvider
	{
		private Controller controller;
		private ImageListener listener;

		public ImageProvider()
		{
			Console.WriteLine("Image provider created.");
			controller = new Controller();
			controller.SetPolicy(Controller.PolicyFlag.POLICY_IMAGES);

			listener = new ImageListener();

			controller.AddListener(listener);
		}

		public void AddNewAction(Action<Bitmap[]> newAction)
		{
			listener.OnNewImages += newAction;
		}
	}

	class ImageListener : Listener
	{
		private Stopwatch sw;

		public ImageListener()
		{
			Console.WriteLine("Listener listens.");
			sw = new Stopwatch();
			sw.Start();
		}

		public override void OnImages(Controller controller)
		{
			try
			{
				if (sw.ElapsedMilliseconds < 50)
					return;

				sw.Reset();
				Console.WriteLine("Got new images.");
				var images = controller.Images.Select(GrayscaleBitmapConverter.Convert).ToArray();
				if (OnNewImages != null)
					OnNewImages(images);
				sw.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public Action<Bitmap[]> OnNewImages;
	}

	static class GrayscaleBitmapConverter
	{
		public static Bitmap Convert(Leap.Image image)
		{
			var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);
			ColorPalette grayscale = bitmap.Palette;
			for (int i = 0; i < 256; i++)
			{
				grayscale.Entries[i] = Color.FromArgb((int)255, i, i, i);
			}
			bitmap.Palette = grayscale;
			Rectangle lockArea = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			BitmapData bitmapData = bitmap.LockBits(lockArea, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			byte[] rawImageData = image.Data;
			System.Runtime.InteropServices.Marshal.Copy(rawImageData, 0, bitmapData.Scan0, image.Width * image.Height);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
