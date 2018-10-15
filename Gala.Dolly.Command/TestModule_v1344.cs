using System.Drawing;
using System;

namespace Gala.Dolly.Test
{
    using Galatea.AI.Abstract;
    using Galatea.Imaging.IO;
    using Galatea.IO;

    internal static class TestModule_v1344
    {
        private static BaseTemplate _namedTemplate;

        public static void TestMethod()
        {
            Program.TestEngine.ExecutiveFunctions.ContextRecognition += ExecutiveFunctions_ContextRecognition;

            //DebugSymbols();

            //TestSymbolLearning();

            //DoLetters();
            DoNumbers();
            //DoLowercaseLetters();

            OutputSymbolBitmaps();
        }

        private static void ExecutiveFunctions_ContextRecognition(object sender, ContextRecognitionEventArgs e)
        {
            _namedTemplate = e.NamedTemplate;
        }

        private static void DebugSymbols()
        {
            bool result;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\D.png", "What SYMBOL?", "Letter D");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\1.png", "What SYMBOL?", "Number 1");
            System.Diagnostics.Debug.Assert(result);
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\2.png", "What SYMBOL?", "Number 2");
            System.Diagnostics.Debug.Assert(result);
        }


        private static void OutputSymbolBitmaps()
        {
            foreach(SymbolTemplate sT in Program.TestEngine.DataAccessManager[TemplateType.Symbol])
            {
                Bitmap bmp = Galatea.AI.Imaging.GuiImaging.GetAsciiBitmap(sT.AsciiBitmap, sT.AsciiBitmapSize);

                string fileName = System.IO.Path.Combine("Symbols\\", sT.Name + ".png");
                bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void DoNumbers()
        {
            bool result;
            string response;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\1.png", "What SYMBOL?", "Number 1"); response = TestModule.GetResponse("The SYMBOL is Number 1!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\2.png", "What SYMBOL?", "Number 2"); response = TestModule.GetResponse("The SYMBOL is Number 2!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\3.png", "What SYMBOL?", "Number 3"); response = TestModule.GetResponse("The SYMBOL is Number 3!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\4.png", "What SYMBOL?", "Number 4"); response = TestModule.GetResponse("The SYMBOL is Number 4!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\5.png", "What SYMBOL?", "Number 5"); response = TestModule.GetResponse("The SYMBOL is Number 5!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\6.png", "What SYMBOL?", "Number 6"); response = TestModule.GetResponse("The SYMBOL is Number 6!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\7.png", "What SYMBOL?", "Number 7"); response = TestModule.GetResponse("The SYMBOL is Number 7!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\8.png", "What SYMBOL?", "Number 8"); response = TestModule.GetResponse("The SYMBOL is Number 8!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\9.png", "What SYMBOL?", "Number 9"); response = TestModule.GetResponse("The SYMBOL is Number 9!");
        }
        private static void DoLetters()
        {
            bool result;
            string response;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\E.png", "What SYMBOL?", "Letter E"); response = TestModule.GetResponse("The SYMBOL is Letter E!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\F.png", "What SYMBOL?", "Letter F"); response = TestModule.GetResponse("The SYMBOL is Letter F!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\H.png", "What SYMBOL?", "Letter H"); response = TestModule.GetResponse("The SYMBOL is Letter H!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\I.png", "What SYMBOL?", "Letter I"); response = TestModule.GetResponse("The SYMBOL is Letter I!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\J.png", "What SYMBOL?", "Letter J"); response = TestModule.GetResponse("The SYMBOL is Letter J!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\K.png", "What SYMBOL?", "Letter K"); response = TestModule.GetResponse("The SYMBOL is Letter K!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\L.png", "What SYMBOL?", "Letter L"); response = TestModule.GetResponse("The SYMBOL is Letter L!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\M.png", "What SYMBOL?", "Letter M"); response = TestModule.GetResponse("The SYMBOL is Letter M!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\N.png", "What SYMBOL?", "Letter N"); response = TestModule.GetResponse("The SYMBOL is Letter N!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\O.png", "What SYMBOL?", "Letter O"); response = TestModule.GetResponse("The SYMBOL is Letter O!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\P.png", "What SYMBOL?", "Letter P"); response = TestModule.GetResponse("The SYMBOL is Letter P!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\Q.png", "What SYMBOL?", "Letter Q"); response = TestModule.GetResponse("The SYMBOL is Letter Q!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\R.png", "What SYMBOL?", "Letter R"); response = TestModule.GetResponse("The SYMBOL is Letter R!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\S.png", "What SYMBOL?", "Letter S"); response = TestModule.GetResponse("The SYMBOL is Letter S!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\T.png", "What SYMBOL?", "Letter T"); response = TestModule.GetResponse("The SYMBOL is Letter T!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\U.png", "What SYMBOL?", "Letter U"); response = TestModule.GetResponse("The SYMBOL is Letter U!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\V.png", "What SYMBOL?", "Letter V"); response = TestModule.GetResponse("The SYMBOL is Letter V!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\W.png", "What SYMBOL?", "Letter W"); response = TestModule.GetResponse("The SYMBOL is Letter W!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\X.png", "What SYMBOL?", "Letter X"); response = TestModule.GetResponse("The SYMBOL is Letter X!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\Y.png", "What SYMBOL?", "Letter Y"); response = TestModule.GetResponse("The SYMBOL is Letter Y!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\Z.png", "What SYMBOL?", "Letter Z"); response = TestModule.GetResponse("The SYMBOL is Letter Z!");
        }
        private static void DoLowercaseLetters()
        {
            bool result;
            string response;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\a_lc.png", "What SYMBOL?", "lowercase a"); response = TestModule.GetResponse("The SYMBOL is lowercase a!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\b_lc.png", "What SYMBOL?", "lowercase b"); response = TestModule.GetResponse("The SYMBOL is lowercase b!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\c_lc.png", "What SYMBOL?", "lowercase c"); response = TestModule.GetResponse("The SYMBOL is lowercase c!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\d_lc.png", "What SYMBOL?", "lowercase d"); response = TestModule.GetResponse("The SYMBOL is lowercase d!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\e_lc.png", "What SYMBOL?", "lowercase e"); response = TestModule.GetResponse("The SYMBOL is lowercase e!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\f_lc.png", "What SYMBOL?", "lowercase f"); response = TestModule.GetResponse("The SYMBOL is lowercase f!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\g_lc.png", "What SYMBOL?", "lowercase g"); response = TestModule.GetResponse("The SYMBOL is lowercase g!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\h_lc.png", "What SYMBOL?", "lowercase h"); response = TestModule.GetResponse("The SYMBOL is lowercase h!");
            //result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\i_lc.png", "What SYMBOL?", "lowercase i"); response = TestModule.GetResponse("The SYMBOL is lowercase i!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\j_lc.png", "What SYMBOL?", "lowercase j"); response = TestModule.GetResponse("The SYMBOL is lowercase j!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\k_lc.png", "What SYMBOL?", "lowercase k"); response = TestModule.GetResponse("The SYMBOL is lowercase k!");
            //result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\l_lc.png", "What SYMBOL?", "lowercase l"); response = TestModule.GetResponse("The SYMBOL is lowercase l!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\m_lc.png", "What SYMBOL?", "lowercase m"); response = TestModule.GetResponse("The SYMBOL is lowercase m!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\n_lc.png", "What SYMBOL?", "lowercase n"); response = TestModule.GetResponse("The SYMBOL is lowercase n!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\o_lc.png", "What SYMBOL?", "lowercase o"); response = TestModule.GetResponse("The SYMBOL is lowercase o!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\p_lc.png", "What SYMBOL?", "lowercase p"); response = TestModule.GetResponse("The SYMBOL is lowercase p!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\q_lc.png", "What SYMBOL?", "lowercase q"); response = TestModule.GetResponse("The SYMBOL is lowercase q!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\r_lc.png", "What SYMBOL?", "lowercase r"); response = TestModule.GetResponse("The SYMBOL is lowercase r!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\s_lc.png", "What SYMBOL?", "lowercase s"); response = TestModule.GetResponse("The SYMBOL is lowercase s!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\t_lc.png", "What SYMBOL?", "lowercase t"); response = TestModule.GetResponse("The SYMBOL is lowercase t!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\u_lc.png", "What SYMBOL?", "lowercase u"); response = TestModule.GetResponse("The SYMBOL is lowercase u!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\v_lc.png", "What SYMBOL?", "lowercase v"); response = TestModule.GetResponse("The SYMBOL is lowercase v!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\w_lc.png", "What SYMBOL?", "lowercase w"); response = TestModule.GetResponse("The SYMBOL is lowercase w!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\x_lc.png", "What SYMBOL?", "lowercase x"); response = TestModule.GetResponse("The SYMBOL is lowercase x!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\y_lc.png", "What SYMBOL?", "lowercase y"); response = TestModule.GetResponse("The SYMBOL is lowercase y!");
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\symbols\z_lc.png", "What SYMBOL?", "lowercase z"); response = TestModule.GetResponse("The SYMBOL is lowercase z!");
        }

        private static void TestSymbolLearning()
        {
            bool result;

            // Dollar Sign
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\$.png", "What SYMBOL?", "DOLLAR SIGN");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("The SYMBOL is DOLLAR SIGN!");

            // Pound Sign
            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\#.png", "What SYMBOL?", "POUND SIGN");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("It's POUND SIGN");

            // NOW CHECK IF SYMBOLS WERE LEARNED
            ICreator creator = Program.TestEngine.User;

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\$.png", "What SYMBOL?", "DOLLAR SIGN");
            System.Diagnostics.Debug.Assert(result);
            System.Diagnostics.Debug.Assert(creator == _namedTemplate.Creator);

            result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\#.png", "What SYMBOL?", "POUND SIGN");
            System.Diagnostics.Debug.Assert(result);
            System.Diagnostics.Debug.Assert(creator == _namedTemplate.Creator);
        }

        private static void StarSymbolRelation()
        {
            // ASTERISK
            bool result = TestModule.Evaluate(@"..\..\..\Resources\Learning\Symbols\asterisk.png", "What SYMBOL?", "STAR");
            System.Diagnostics.Debug.Assert(!result);

            TestModule.GetResponse("It's a STAR");

            // TODO:  Relate Asterisk Symbol with Star Shape
        }
    }
}                                                                                                          
                                                                                                           
                                                                                                           
                                                                                                                