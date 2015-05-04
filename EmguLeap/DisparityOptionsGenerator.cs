namespace EmguLeap
{
	static class DisparityOptionsGenerator
	{
		public static DisparityOptions GetOptions()
		{
			return new DisparityOptions(64, 20, 13, 1, 5, 15, 7 * 16, 48);
		}
	}
}
