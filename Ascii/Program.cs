using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ascii
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            ConsoleHelper.SetCurrentFont("Consolas", 5);

            var variant = "sfw";
            if (false)
            {
                variant = "nsfw";
            }

            var engine = new Engine($"games/{variant}");
            engine.Run();
        }
    }
}
