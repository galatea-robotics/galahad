using System;
using System.Drawing;
using System.Collections.Generic;

namespace Gala.Dolly.Test
{
    using Galatea.Imaging.IO;
    using Galatea.IO;

    internal class TestModule_v1347
    {
        public static void TestMethod()
        {
            TestTwoShapes();
        }


        private static void TestTwoShapes()
        {
            //Bitmap bitmap = new Bitmap(@"..\..\..\Resources\Learning\two_shapes1.png");


            Bitmap bitmap = new Bitmap(@"..\..\..\Resources\Learning\four_shapes.png");
            ImagingContextStream stream = ImagingContextStream.FromBitmap(bitmap);

            Program.TestEngine.ExecutiveFunctions.StreamContext(Program.TestEngine, Program.TestEngine.Vision.ImageAnalyzer,
                ContextType.Machine, InputType.Visual, stream, typeof(Bitmap));

            string response = TestModule.GetResponse("How many things?");



            // How many Shapes?
            
        }

        private static void TestThreeShapes()
        {
            // How many things?

            // Which shape is Green?

            // How many Triangles?
        }


        private static void TestMultipleShapes()
        {
            // How many distinct colors?

            // How many unique shapes?

        }

        private static void TestComplexShapes()
        {

        }
    }
}
