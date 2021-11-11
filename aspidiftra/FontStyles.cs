namespace Aspidiftra
{
	/// <summary>
	///   Font styles enum.
	///   Only really exists to prevent the need for any Aspose.Pdf.*
	///   using-statements in any projects that use Aspidiftra but don't
	///   use any actual Aspose.Pdf.* functionality.
	/// </summary>
	public enum FontStyles
	{
		Regular = Aspose.Pdf.Text.FontStyles.Regular,
		Bold = Aspose.Pdf.Text.FontStyles.Bold,
		Italic = Aspose.Pdf.Text.FontStyles.Italic
	}
}