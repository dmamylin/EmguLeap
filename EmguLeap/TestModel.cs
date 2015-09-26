using System;
using System.Drawing;

namespace EmguLeap
{
    class TestModel
    {
        private readonly Action<Bitmap, Bitmap, Bitmap> OnNewDisparityImage;

        private readonly ImageForm ImageForm;

        private readonly DisparityGenerator Generator;
        private readonly CalibrationMatrixLoader MatrixLoader;
        private readonly ImageProvider Provider;

        public TestModel()
        {
            ImageForm = new ImageForm();

            MatrixLoader = new CalibrationMatrixLoader();
            Provider = new ImageProvider();
            Generator = new DisparityGenerator(MatrixLoader);

            Provider.AddNewAction(UpdateDisparity);
            OnNewDisparityImage += PresentImages;

            ImageForm.Visible = true;
        }

        private void UpdateDisparity(Bitmap[] images)
        {
            var leftIm = images[0];
            var rightIm = images[1];
            var disparityIm = Generator.CalculateDisparity(leftIm, rightIm, DisparityOptionsGenerator.GetOptions());

            OnNewDisparityImage(leftIm, rightIm, disparityIm);
        }

        private void PresentImages(Bitmap left, Bitmap right, Bitmap disp)
        {
            ImageForm.ChangeImages(new Bitmap[] { left, right, disp });
        }
    }
}
