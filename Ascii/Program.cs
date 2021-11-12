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
            var engine = new Engine(await File.ReadAllTextAsync("map.txt"),1);
            await engine.Run();
        }



    }
}
