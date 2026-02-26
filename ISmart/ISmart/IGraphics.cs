using System.Collections.Generic;
using System.Drawing;

namespace ISmart;

public interface IGraphics
{
	Image ScreenShot();

	Image Barcode(string serialNumber);

	Image QrCode(string text);

	Image JoinImages(List<Image> images);
}
