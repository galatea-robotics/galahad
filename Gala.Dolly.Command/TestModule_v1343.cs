using System.Drawing;
using System;

namespace Gala.Dolly.Test
{
    using Galatea.Imaging.IO;
    using Galatea.IO;

    internal static class TestModule_v1343
    {
        public static void TestMethod()
        {
            //TestColors();
            TestShapes();

            //TestPieShaped();
            //TestStarShape();
            //TestBlueCircle();
        }

        public static void TestColors()
        {
            bool result;

            // RED
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\STOP.png", "What COLOR?", "Red");
            System.Diagnostics.Debug.Assert(result);
            // YELLOW
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\pacman.png", "What COLOR?", "Yellow");
            System.Diagnostics.Debug.Assert(result);
            // GREEN
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\green_circle.png", "What COLOR?", "Green");
            System.Diagnostics.Debug.Assert(result);
            // BLUE
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\B.png", "What COLOR?", "Blue");
            System.Diagnostics.Debug.Assert(result);
        }

        public static void TrainColors()
        {
            bool result;

            // ORANGE
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\triangle_orange.png", "What COLOR?", "ORANGE");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("The COLOR is ORANGE!");

            // PURPLE
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\star2.png", "What COLOR?", "PURPLE");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("It's Purrrple.");
        }


        public static void TestShapes()
        {
            bool result;

            // ROUND
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\green_circle.png", "What SHAPE?", "round");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\circle_perspective.png", "What SHAPE?", "round");
            System.Diagnostics.Debug.Assert(result);

            // TRIANGULAR
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\triangle_green2.png", "What SHAPE?", "triangular");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\triangle_orange.png", "What SHAPE?", "triangular");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\triangle_yellow.png", "What SHAPE?", "triangular");
            System.Diagnostics.Debug.Assert(result);

            // QUAD
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\quad_black.png", "What SHAPE?", "FOUR CORNERS");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\quad_green.png", "What SHAPE?", "FOUR CORNERS");
            System.Diagnostics.Debug.Assert(result);

            // CHEVRON
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\chevron_purple.png", "What SHAPE?", "Chevron");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\chevron.png", "What SHAPE?", "Chevron");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\widget.png", "What SHAPE?", "Chevron");
            System.Diagnostics.Debug.Assert(result);
        }

        public static void TestPieShaped()
        {
            bool result;

            // PIE SHAPED
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\pizza.png", "What SHAPE?", "PIE SHAPED");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("The shape is PIE SHAPED!");

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\orange_pie.png", "What SHAPE?", "PIE SHAPED");
            System.Diagnostics.Debug.Assert(result);

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\pacman.png", "What SHAPE?", "PIE SHAPED");
            System.Diagnostics.Debug.Assert(result);
        }

        public static void TestStarShape()
        {
            bool result;

            // STAR
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\star2.png", "What SHAPE?", "STAR");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("The shape is a STAR!");

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\STAR_BLUE.png", "What SHAPE?", "STAR");
            System.Diagnostics.Debug.Assert(result);

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\star.png", "What SHAPE?", "STAR");
            System.Diagnostics.Debug.Assert(result);
        }

        public static void TestBlueCircle()
        {
            bool result;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\blue_circle.png", "What Color?", "Blue");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\blue_circle.png", "What Shape?", "Round");
            System.Diagnostics.Debug.Assert(result);
        }
    }
}