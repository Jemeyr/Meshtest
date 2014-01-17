Some experiments with generating terrain and putting unity projects on git.


You can fly around and regenerate the terrain with spac.e It uses midpoint displacement(diamond-square algorithm) and has also an implementation of a sorta fractal algorithm.

The smoothing algorithm only attempts to smooth points within a certain height to try and make a "playable" surface, which has few ambiguous slopes and most are either quite flat or quite not.
