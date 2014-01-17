Link to webplayer: https://dl.dropboxusercontent.com/u/36520367/meshy/Build.html


Some experiments with generating terrain and putting unity projects on git.


You can fly around and regenerate the terrain with space. It uses midpoint displacement(diamond-square algorithm) and has also an implementation of a sorta fractal algorithm.

The smoothing algorithm only attempts to smooth points within a certain height to try and make a "playable" surface, which has few ambiguous slopes and most are either quite flat or quite not.
