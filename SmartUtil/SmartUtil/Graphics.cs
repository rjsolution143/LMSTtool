using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using BarcodeLib;
using ISmart;
using QRCoder;

namespace SmartUtil;

public class Graphics : IGraphics
{
	private string TAG => GetType().FullName;

	public Image ScreenShot()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		List<Image> list = new List<Image>();
		Screen[] allScreens = Screen.AllScreens;
		foreach (Screen val in allScreens)
		{
			Image val2 = (Image)new Bitmap(val.Bounds.Width, val.Bounds.Height, (PixelFormat)2498570);
			Graphics.FromImage(val2).CopyFromScreen(val.Bounds.X, val.Bounds.Y, 0, 0, val.Bounds.Size, (CopyPixelOperation)13369376);
			list.Add(val2);
		}
		if (list.Count < 1)
		{
			throw new NotSupportedException("No screens available for screen shot");
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		int num = 0;
		int num2 = 0;
		foreach (Image item in list)
		{
			num = Math.Max(num, item.Height);
			num2 += item.Width;
		}
		Image val3 = (Image)new Bitmap(num2, num, (PixelFormat)2498570);
		Graphics val4 = Graphics.FromImage(val3);
		int num3 = 0;
		foreach (Image item2 in list)
		{
			val4.DrawImage(item2, new Point(num3, 0));
			num3 += item2.Width;
		}
		return val3;
	}

	public Image Barcode(string serialNumber)
	{
		return Barcode.DoEncode((TYPE)28, serialNumber, true, Color.Black, Color.White, 300, 100);
	}

	public Image QrCode(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		QRCodeGenerator val = new QRCodeGenerator();
		try
		{
			QRCodeData val2 = val.CreateQrCode(text, (ECCLevel)0, false, false, (EciMode)0, -1);
			try
			{
				PngByteQRCode val3 = new PngByteQRCode(val2);
				try
				{
					Image val4 = Image.FromStream((Stream)new MemoryStream(val3.GetGraphic(6, true)));
					try
					{
						return (Image)new Bitmap(val4, new Size(300, 300));
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public Image JoinImages(List<Image> images)
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		if (images.Count < 1)
		{
			throw new NotSupportedException("No images found");
		}
		if (images.Count < 2)
		{
			return images[0];
		}
		bool flag = true;
		foreach (Image image in images)
		{
			if (image.Width != images[0].Width)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			foreach (Image image2 in images)
			{
				if (image2.Height != images[0].Height)
				{
					throw new NotSupportedException("Images sizes do not match");
				}
			}
		}
		int num = images[0].Height;
		int num2 = images[0].Width;
		if (flag)
		{
			num = 0;
			foreach (Image image3 in images)
			{
				num += image3.Height;
			}
		}
		else
		{
			num2 = 0;
			foreach (Image image4 in images)
			{
				num2 += image4.Width;
			}
		}
		Bitmap val = new Bitmap(num2, num);
		Graphics val2 = Graphics.FromImage((Image)(object)val);
		try
		{
			int num3 = 0;
			foreach (Image image5 in images)
			{
				if (flag)
				{
					val2.DrawImage(image5, 0, num3);
					num3 += image5.Height;
				}
				else
				{
					val2.DrawImage(image5, num3, 0);
					num3 += image5.Width;
				}
			}
			return (Image)(object)val;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}
}
