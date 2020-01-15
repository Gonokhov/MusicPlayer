using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicPlayer1
{
    public static class music
    {

        static public List<string> Name = new List<string>();

        static public List<string> patchs = new List<string>();

        static public void AddMusic(string patch)
        {
            patchs.Add(patch);
            Name.Add(Path.GetFileName(patch));
        }

    }
}