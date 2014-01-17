Some experiments with generating terrain and putting unity projects on git.


Link to webplayer: 

https://dl.dropboxusercontent.com/u/36520367/meshy/Build.html


Instructions:

Fly with mouse and WASD. Click the screen to lock the mouse, press escape to unlock. Pressing space at any point will regenerate the terrain. It's pretty slow and might take a few seconds.




It uses midpoint displacement(diamond-square algorithm) and has also an implementation of a sorta fractal algorithm.

The smoothing algorithm only attempts to smooth points within a certain height to try and make a "playable" surface, which has few ambiguous slopes and most are either quite flat or quite not.
