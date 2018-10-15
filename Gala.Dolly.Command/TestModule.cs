using System.Drawing;
using System;

namespace Gala.Dolly.Test
{
    using Galatea.Imaging.IO;
    using Galatea.IO;

    internal static class TestModule
    {
        public static void TestMethod()
        {
            //TestModule_v1343.TestMethod();
            //TestModule_v1344.TestMethod();
            //TestModule_v1345.TestMethod();
            //TestModule_v1346.TestMethod();

            TestModule_v1347.TestMethod();
        }

        public static bool Evaluate(string path, string input, string expectedResult)
        {
            string response = GetResponse(path, input);
            return response.ToUpper().Contains(expectedResult.ToUpper());
        }
        public static string GetResponse(string path, string input)
        {
            Bitmap bitmap = new Bitmap(path);
            return GetResponse(bitmap, input);
        }
        public static string GetResponse(Bitmap bitmap, string input)
        {
            ImagingContextStream stream = ImagingContextStream.FromBitmap(bitmap);

            Program.TestEngine.ExecutiveFunctions.StreamContext(Program.TestEngine, Program.TestEngine.Vision.ImageAnalyzer,
                ContextType.Machine, InputType.Visual, stream, typeof(Bitmap));

            return GetResponse(input);
        }
        public static string GetResponse(string input)
        {
            return Program.TestEngine.ExecutiveFunctions.GetResponse(Program.TestEngine.AI.LanguageModel, Program.TestEngine.User, input);
        }
    }
}