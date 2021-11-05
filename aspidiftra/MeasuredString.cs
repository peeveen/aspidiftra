namespace Aspidiftra
{
	internal class MeasuredString
	{
		internal MeasuredString(string text, double length, bool isSplittable = true)
		{
			Text = text;
			Length = length;
			IsSplittable = isSplittable;
		}

		internal MeasuredString(MeasuredString measuredString, bool isSplittable)
		{
			Text = measuredString.Text;
			Length = measuredString.Length;
			IsSplittable = isSplittable;
		}

		internal string Text { get; }
		internal double Length { get; }
		internal bool IsSplittable { get; }
	}
}