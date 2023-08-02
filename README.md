# wikipedia_metric

wikipedia source: https://dumps.wikimedia.org/enwikisource/20230701/

## Componenrts

### FileManager

Simple class for managing the file streams.

### WikimediaParser

Parses the WikiSource xml dumps from Wikimedia and extracts following pair mapping: PageTitle <-> PageLinks into a Dictionary.

### JsonManager

Saves/loads the Dictionary mapping to/from a json file. This is needed so we do not need to load the data every time from the multiple gigabytes large xml file.

### Painter

Given a result tree return coordinates and sizef for each node ready for drawing.
