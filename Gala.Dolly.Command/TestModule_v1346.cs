using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Gala.Dolly.Test
{
    using Galatea.AI.Abstract;
    using Galatea.AI.Imaging;
    using Galatea.Imaging.IO;
    using Galatea.IO;

    internal static class TestModule_v1346
    {
        static bool _result;

        public static void TestMethod()
        {
            //TestGreenBlue();
            //TestOrangeYellow();
            //// Regression Tests
            //OneColorTests();
            // Training
            //TrainPolygons();

            //// Other
            //TestBackgrounds();
            //TestOvals();
            //TestSerializedHybridColor();

            TestLightDark();
        }

        private static void TestGreenBlue()
        {
            TestHybridColorResult(@"..\..\..\Resources\Learning\green_blue_star.png", new[] { "Green", "Blue" });
        }
        private static void TestOrangeYellow()
        {
            TestHybridColorResult(@"..\..\..\Resources\Learning\orange_yellow_crescent.png", new[] { "ORANGE", "Yellow" });
        }
        private static void TestHybridColorResult(string path, string[] templateTokens)
        {
            ColorTemplate namedTemplate = GetColorTemplate(path);

            // Validate Test Results
            foreach (string token in templateTokens)
            {
                _result = namedTemplate.FriendlyName.Contains(token);
                Debug.Assert(_result, string.Format("FriendlyName does not contain the label '{0}'.", token));

                _result = namedTemplate.TemplateRelationships.Contains(token);
                Debug.Assert(_result, string.Format("TemplateRelationships does not contain the ColorTemplate '{0}'.", token));

                _result = namedTemplate.TemplateRelationships[token].RelationshipType == TemplateRelationshipType.Contains;
                Debug.Assert(_result, string.Format("TemplateRelationships['{0}'].RelationshipType must be 'TemplateRelationshipType.Contains'.", token));
            }
        }

        private static void TestSerializedHybridColor()
        {
            ColorTemplate namedTemplate = GetColorTemplate(@"..\..\..\Resources\Learning\green_blue_star.png");

            _result = CheckTemplateRelationship(namedTemplate.TemplateRelationships[0], "Blue", TemplateRelationshipType.Contains);
            Debug.Assert(_result);
            _result = CheckTemplateRelationship(namedTemplate.TemplateRelationships[1], "Green", TemplateRelationshipType.Contains);
            Debug.Assert(_result);
        }

        private static bool CheckTemplateRelationship(TemplateRelationship relationship, string expectedName, TemplateRelationshipType expectedType)
        {
            bool result;

            result = relationship.RelatedItem.Name == expectedName;
            result = relationship.RelationshipType == expectedType && result;

            return result;
        }

        private static void OneColorTests()
        {
            // RED
            TestOneColorTemplate(@"..\..\..\Resources\Learning\STOP.png", "Red");
            // YELLOW
            TestOneColorTemplate(@"..\..\..\Resources\Learning\pacman.png", "Yellow");
            // GREEN
            TestOneColorTemplate(@"..\..\..\Resources\Learning\green_circle.png", "Green");
            // BLUE
            TestOneColorTemplate(@"..\..\..\Resources\Learning\Symbols\B.png", "Blue");
        }
        private static ColorTemplate GetColorTemplate(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            ImagingContextStream stream = ImagingContextStream.FromBitmap(bitmap);

            Program.TestEngine.ExecutiveFunctions.StreamContext(Program.TestEngine, Program.TestEngine.Vision.ImageAnalyzer,
                ContextType.Machine, InputType.Visual, stream, typeof(Bitmap));

            return (ColorTemplate)Program.TestEngine.AI.RecognitionModel.IdentifyTemplate(TemplateType.Color);
        }
        private static void TestOneColorTemplate(string path, string expectedColorName)
        {
            ColorTemplate namedTemplate = GetColorTemplate(path);

            // Check Name
            _result = namedTemplate.FriendlyName.Equals(expectedColorName);
            Debug.Assert(_result);

            // Check Relationships
            _result = namedTemplate.TemplateRelationships.Count == 0;
            Debug.Assert(_result);
        }

        private static void TrainPolygons()
        {
            string response;

            response = TestModule.GetResponse(@"..\..\..\Resources\Learning\pentagon.png", "What Shape?");
            response = TestModule.GetResponse("It's a Pentagon.");
            response = TestModule.GetResponse(@"..\..\..\Resources\Learning\hexagon.png", "What Shape?");
            response = TestModule.GetResponse("It's a Hexagon.");
            response = TestModule.GetResponse(@"..\..\..\Resources\Learning\STOP.png", "What Shape?");
            response = TestModule.GetResponse("It's an Octagon.");
        }

        private static void TestBackgrounds()
        {
            //TestBackground(@"..\..\..\Resources\Learning\oval_grey.png");           // BackgroundIsBlack: false
            //TestBackground(@"..\..\..\Resources\Learning\circle_perspective.png");  // BackgroundIsBlack: false
            //TestBackground(@"..\..\..\Resources\Learning\quad_red.png");            // BackgroundIsBlack: false

            //TestBackground(@"..\..\..\Resources\Learning\oval.png");                // BackgroundIsBlack: true
            //TestBackground(@"..\..\..\Resources\Learning\oval1.png");               // BackgroundIsBlack: true
            TestBackground(@"..\..\..\Resources\Learning\crescent3.png");           // BackgroundIsBlack: true
        }

        private static void TestBackground(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            ImagingContextStream stream = ImagingContextStream.FromBitmap(bitmap);

            Program.TestEngine.ExecutiveFunctions.StreamContext(Program.TestEngine, Program.TestEngine.Vision.ImageAnalyzer,
                ContextType.Machine, InputType.Visual, stream, typeof(Bitmap));

            // Save Bitmaps from Initialization
            FileInfo fi = new FileInfo(path);
            string fn = fi.Name.Replace(fi.Extension, "");

            File.Copy("bitmap_source.png", $"{fn}_bitmap.png", true);
            //File.Copy("bitmap_blobified.png", $"{fn}_bitmap_blobified.png", true);
            File.Copy("bitmap_blobified1.png", $"{fn}_bitmap_blobified1.png", true);

            // Object Recognition
            var result = Program.TestEngine.AI.RecognitionModel.NameEntity();
            //if (File.Exists("bitmap1.png")) File.Copy("bitmap1.png", $"{fn}_bitmap1.png", true);
            //if (File.Exists("bitmap2.png")) File.Copy("bitmap2.png", $"{fn}_bitmap2.png", true);

            // Get Gui Bitmap Results
            ColorTemplate ct = (ColorTemplate)result.TemplatePattern[TemplateType.Color];
            ShapeTemplate st = (ShapeTemplate)result.TemplatePattern[TemplateType.Shape];

            if (st.BitmapBlob != null)
            {
                Bitmap bmpResult = GuiImaging.GetBitmapBlobImage(st.BitmapBlob, ct.GetColor(), Color.FromArgb(232, 232, 232));
                bmpResult.Save($"{fn}_bitmap_result.png", System.Drawing.Imaging.ImageFormat.Png);

                //File.Copy("bitmap1.png", $"{fn}_bitmap1.png", true);

                Color pointColor = Color.FromArgb(64, Color.Cyan);
                Color firstPointColor = Color.FromArgb(64, Color.Red);
                Color lastPointColor = Color.FromArgb(64, Color.Blue);

                if (st.BlobPoints != null)
                {
                    // Get Initial Points
                    GuiPointsGraphics.DrawInitialPoints(pointColor, firstPointColor, lastPointColor, GuiPointShape.Cross, st.BlobPoints.InitialPoints, bmpResult);
                    //bmpResult.Save($"{fn}_bitmap2.png", System.Drawing.Imaging.ImageFormat.Png);

                    // Get final Blob Points
                    GuiPointsGraphics.DrawBlobPoints(bmpResult, st.BlobPoints, Color.Magenta, Color.Red, Color.Blue, Color.Green, Color.Yellow);
                    bmpResult.Save($"{fn}_bitmap_points.png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
 
            // Delete Exists
            File.Delete("bitmap_source.png");
            //if (File.Exists("bitmap1.png")) File.Delete("bitmap1.png");
            //if (File.Exists("bitmap2.png")) File.Delete("bitmap2.png");
            File.Delete("bitmap_blobified.png");
            File.Delete("bitmap_blobified1.png");
            File.Delete("bitmap_fromBits.png");
        }

        private static void TestOvals()
        {
            string response = TestModule.GetResponse(@"..\..\..\Resources\Learning\oval.png", "What Shape?");
        }


        private static void TestLightDark()
        {
            ColorTemplate namedTemplate;

            // Pink
            namedTemplate = GetColorTemplate(@"..\..\..\Resources\Learning\pink_star.png");
            Debug.Assert(namedTemplate.FriendlyName.ToUpper() == "LIGHT RED");
            Debug.Assert(namedTemplate.TemplateRelationships.Count == 1);
            Debug.Assert(namedTemplate.TemplateRelationships[0].RelatedItem.FriendlyName.ToUpper() == "RED");
            Debug.Assert(namedTemplate.TemplateRelationships[0].RelationshipType == TemplateRelationshipType.Comparison);

            // Navy
            namedTemplate = GetColorTemplate(@"..\..\..\Resources\Learning\triangle_navy.png");
            Debug.Assert(namedTemplate.FriendlyName.ToUpper() == "DARK BLUE");
            Debug.Assert(namedTemplate.TemplateRelationships.Count == 1);
            Debug.Assert(namedTemplate.TemplateRelationships[0].RelatedItem.FriendlyName.ToUpper() == "BLUE");
            Debug.Assert(namedTemplate.TemplateRelationships[0].RelationshipType == TemplateRelationshipType.Comparison);

            // Maroon
            namedTemplate = GetColorTemplate(@"..\..\..\Resources\Learning\pentagon_maroon.png");
            Debug.Assert(namedTemplate.FriendlyName.ToUpper() == "DARK RED");
            Debug.Assert(namedTemplate.TemplateRelationships.Count == 1);
            Debug.Assert(namedTemplate.TemplateRelationships[0].RelatedItem.FriendlyName.ToUpper() == "RED");
            Debug.Assert(namedTemplate.TemplateRelationships[0].RelationshipType == TemplateRelationshipType.Comparison);
        }
    }
}
