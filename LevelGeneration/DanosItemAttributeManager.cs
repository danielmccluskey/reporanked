using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.LevelGeneration
{
    public static class DanosItemAttributeManager
    {
        public static Random rng { get; private set; } = new Random();

        public static void Reset(int seed)
        {
            rng = new Random(seed);
        }

        public static float Range(float min, float max)
        {
            return (float)(min + (rng.NextDouble() * (max - min)));
        }
    }
}
