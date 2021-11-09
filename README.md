# aspidiftra
PDF text watermarking library, for use with Aspose PDF.

If you attempt to use this extensively without a valid Aspose license, you will probably hit errors.

# Example usage
```
var watermarkFont = new Font("Helvetica", FontStyles.Italic, new Size(.025f, Sizing.RelativeToDiagonalSize));
var watermarkAppearance = new Appearance(Color.Red, 0.6f, watermarkFont);

var pageEdgeWatermark = new PageEdgeTextWatermark(
	"This is a page edge watermark.", watermarkAppearance,
	PageEdgePosition.Bottom, Justification.Centre, Fitting.Wrap | Fitting.Shrink,
	new Size(0.03f, Sizing.RelativeToAverageSideLength), true);
  
var bannerWatermark = new BannerTextWatermark(
	"This is my banner text.", watermarkAppearance,
	Justification.Centre, Fitting.Wrap | Fitting.Shrink,
	new Size(0.08f, Sizing.RelativeToAverageSideLength),
	new CustomBannerAngle(new Angle(123.4, AngleUnits.Degrees)));

AspidiftraUtil.WatermarkPdf("C:\\source.pdf", new[] {pageEdgeWatermark, bannerWatermark}, "C:\\output.pdf");
```
# TODO
* Add a more intelligent mechanism for font size shrinking and growing.
