using System;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    // Transformaciones

    public static int Clamp(int v, int min, int max)
    {
        return v < min ? min : v > max ? max : v;
    }

    public static IEnumerable<Src> Generate<Src>(Src seed, Func<Src, Src> generator) {
        while (true) {
            yield return seed;
            seed = generator(seed);
        }
    }
}
