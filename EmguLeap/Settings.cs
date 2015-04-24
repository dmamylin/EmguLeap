using System.Windows.Forms;

namespace EmguLeap
{
	public partial class Settings : Form
	{
		public Settings()
		{
			InitializeComponent();
		}

		public DisparityOptions Options
		{
			get
			{
				return  new DisparityOptions(
				 numDisp.GetData() * 16,
				 minDisp.GetData() * 5,
				 SAD.GetData() * 2 + 1,
				  disp12.GetData(),
				  preFilter.GetData(),
				  uniqueness.GetData() + 5,
				  speckle.GetData() * 16,
				  speckleRange.GetData() * 16);
			}
		}
		
			//options.numDisparities = numDisp.Value * 16;
			/////*This is maximum disparity minus minimum disparity. Always greater than 0. In the current implementation this parameter must be divisible by 16.*/
			////int numDisparities = GetSliderValue(Num_Disparities);

			//options.minDispatities = minDisp.Value * 5;
			/////*The minimum possible disparity value. Normally it is 0, but sometimes rectification algorithms can shift images, so this parameter needs to be adjusted accordingly*/
			////int minDispatities = GetSliderValue(Min_Disparities);

			//options.SAD = SAD.Value*2 + 1;
			/////*The matched block size. Must be an odd number >=1 . Normally, it should be somewhere in 3..11 range*/
			////int SAD = GetSliderValue(SAD_Window);

			//options.disp12MaxDiff = disp12.Value;
			/////* Maximum allowed difference (in integer pixel units) in the left-right disparity check. Set it to non-positive value to disable the check.*/
			////int disp12MaxDiff = GetSliderValue(Disp12MaxDiff);

			//options.PreFilterCap = preFilter.Value;
			/////*Truncation value for the prefiltered image pixels. 
			//// * The algorithm first computes x-derivative at each pixel and clips its value by [-preFilterCap, preFilterCap] interval. 
			//// * The result values are passed to the Birchfield-Tomasi pixel cost function.*/
			////int PreFilterCap = GetSliderValue(pre_filter_cap);

			//options.UniquenessRatio = uniqueness.Value + 5;
			/////*The margin in percents by which the best (minimum) computed cost function value should “win” the second best value to consider the found match correct. 
			//// * Normally, some value within 5-15 range is good enough*/
			////int UniquenessRatio = GetSliderValue(uniquenessRatio);

			//options.Speckle = speckle.Value*16;
			/////*Maximum disparity variation within each connected component. 
			//// * If you do speckle filtering, set it to some positive value, multiple of 16. 
			//// * Normally, 16 or 32 is good enough*/
			////int Speckle = GetSliderValue(Speckle_Window);

			//options.SpeckleRange = speckleRange.Value*16;
			/////*Maximum disparity variation within each connected component. If you do speckle filtering, set it to some positive value, multiple of 16. Normally, 16 or 32 is good enough.*/
			////int SpeckleRange = GetSliderValue(specklerange);

			//options.fullDP = false;
			/////*Set it to true to run full-scale 2-pass dynamic programming algorithm. It will consume O(W*H*numDisparities) bytes, 
			//// * which is large for 640x480 stereo and huge for HD-size pictures. By default this is usually false*/
			//////Set globally for ease
			//////bool fullDP = true;
	}
}
