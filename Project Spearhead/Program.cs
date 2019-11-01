using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net.Config;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System.Xml;

namespace Project_Spearhead
{
#if WINDOWS || LINUX
    public static class Program
    {
        
        [STAThread]
        static void Main()
        {
            using(var game = new NeatMain())
                game.Run();
        }
    }
#endif
}
