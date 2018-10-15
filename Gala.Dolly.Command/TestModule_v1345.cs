using System.Drawing;
using System;

namespace Gala.Dolly.Test
{
    using Galatea.AI.Abstract;

    internal static class TestModule_v1345
    {
        public static void TestMethod()
        {
            //TestEntityTemplateLabel();
            //TestUnknownEntity();
            //TestSymbolEntity();

            //TrainPacMan();
            //TrainStopSign();

            //TestPacMan();
            TestStopSign();
            //TestEntityLearning();
        }

        private static void TestEntityTemplateLabel()
        {
            bool result;

            // GREEN CIRCLE
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\green_circle.png", "What IS IT?", "Green Round Shape");
            System.Diagnostics.Debug.Assert(result);

            // ORANGE TRIANGLE
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\triangle_orange.png", "What IS IT?", "Orange Triangular Shape");
            System.Diagnostics.Debug.Assert(result);

            // BLUE STAR
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\STAR_BLUE.png", "What IS IT?", "Blue Star");
            System.Diagnostics.Debug.Assert(result);
        }
        private static void TestUnknownEntity()
        {
            bool result;

            // BOWTIE
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\bowtie.png", "What IS IT?", "Bowtie");
            System.Diagnostics.Debug.Assert(result);
            // STOP SIGN
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\STOP.png", "What IS IT?", "Stop Sign");
            System.Diagnostics.Debug.Assert(result);
            // EDWIN
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\edwin.png", "What IS IT?", "Black Shape");
            System.Diagnostics.Debug.Assert(result);
        }
        private static void TestSymbolEntity()
        {
            bool result;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\A.png", "What IS IT?", "Letter A");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\K.png", "What IS IT?", "Letter K");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\S.png", "What IS IT?", "Letter S");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\3.png", "What IS IT?", "Number 3");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\7.png", "What IS IT?", "Number 7");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\$.png", "What IS IT?", "DOLLAR SIGN");
            System.Diagnostics.Debug.Assert(result);
        }

        private static void TrainStopSign()
        {
            bool result;
            
            // Teach the Entity is called "Stop Sign"
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\STOP.png", "What IS IT?", "RED OCTAGON");
            System.Diagnostics.Debug.Assert(result);

            TestModule.GetResponse("It's a STOP SIGN.");

            // Only Red Octagon
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\green_octagon.png", "What IS IT?", "GREEN OCTAGON");
            System.Diagnostics.Debug.Assert(result);

            TestStopSign();
        }
        private static void TestStopSign()
        {
            // Check another Octagon
            bool result = TestModule.Evaluate(@"..\..\..\Resources\Learning\green_octagon.png", "What IS IT?", "Green Octagon");
            System.Diagnostics.Debug.Assert(result);
        }
        private static void TrainPacMan()
        {
            bool result;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\pacman.png", "What IS IT?", "YELLOW PIE");
            System.Diagnostics.Debug.Assert(result);

            TestModule.GetResponse("It's PAC-MAN.");

            // Only yellow Pie Shape is Pac-Man
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\pizza.png", "What IS IT?", "RED PIE");
            System.Diagnostics.Debug.Assert(result);

            TestPacMan();
        }
        private static void TestPacMan()
        {
            bool result = TestModule.Evaluate(@"..\..\..\Resources\Learning\pacman.png", "What IS IT?", "PAC-MAN");
            System.Diagnostics.Debug.Assert(result);

            // TODO:  **WHO** is it?
        }

        private static void TestEntityLearning()
        {
            bool result;

            // ADD A BUNCH OF ROUND SHAPES SO IT LEARNS WHAT A CIRCLE IS
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\orange_circle.png", "What IS IT?", "Orange Round Shape");
            System.Diagnostics.Debug.Assert(result);
            TestModule.GetResponse("It's an Orange Circle!");

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\green_circle.png", "What IS IT?", "Green Round Shape");
            System.Diagnostics.Debug.Assert(result);
            TestModule.GetResponse("It's a Green Circle!");

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\red_circle.png", "What IS IT?", "Red Round Shape");
            System.Diagnostics.Debug.Assert(result);
            TestModule.GetResponse("It's a Red Circle!");

            // Test Existing Entity Labels
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\circle.png", "What IS IT?", "Green Circle");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\orange_circle.png", "What IS IT?", "Orange Circle");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\red_circle.png", "What IS IT?", "Red Circle");
            System.Diagnostics.Debug.Assert(result);

            // TODO:  Relate the three Circle entities and define a new "Circle" Entity
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\blue_circle.png", "What IS IT?", "Blue Circle");
            System.Diagnostics.Debug.Assert(result);
        }
    }
}                                                                                                          
                                                                                                           
                                                                                                           
                                                                                                                