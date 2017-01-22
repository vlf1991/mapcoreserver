# Mapcore Server Demo Processing
Mapcore CSGO playtesting serverside processing tool

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

### SuperSimpleHTTP
Because I was frustrated with how little HTTP and messily supported HTTP reuqests are in C#, I decided to make my own Python - requests module style library. This made it far easier when sending steam api calls.

- Make a POST request and capture output
- Make a GET request and capture output

## External Libraries used
### DDS Reader
Original Repo by [Micolous](https://github.com/micolous/igaeditor),
Refactored by [Andburn](https://github.com/andburn/dds-reader)
and Modified by Myself for a few bug-fixes
