# Mapcore Server Demo Processing
##Examples of the software output:
![A flow map](http://i.imgur.com/TkLPT4e.png)
![A heatmap](http://i.imgur.com/gXhkgYw.png)

## Parts of the software
### DemoHeatmap
The bulk of the software, which handles all sequencing of downloading and extracting files. Still has a lot of junk in it as well as obsolete code

- Read the demo file
- Download and parse workshop maps
- Extract and save bsp files

### SharpHeatMaps
Needed to plot data onto image files

- Apply and load gradient maps
- Plot splodges onto a density map
- Bake density map into Heatmap then back to bitmap

![A gradient map applied to a kitty](http://i.imgur.com/rWE8OsW.png)

### SuperSimpleHTTP
Because I was frustrated with how little HTTP and messily supported HTTP reuqests are in C#, I decided to make my own Python - requests module style library. This made it far easier when sending steam api calls.

- Make a POST request and capture output
- Make a GET request and capture output

### DemoHeatmapGUI
The UI portion of the code! At the moment very minimal
![The UI currently](http://i.imgur.com/NYF0gVe.png)

## External Libraries used
### Demoinfogo
- This library was incredibly useful in parsing demo files and getting neccessary information for the fundemental parts of the program to run.

Repo [Here](https://github.com/StatsHelix/demoinfo)

### DDS Reader
Original Repo by [Micolous](https://github.com/micolous/igaeditor),
Refactored by [Andburn](https://github.com/andburn/dds-reader)
and Modified by Myself for a few bug-fixes
