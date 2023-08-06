# wikipedia_metric

wikipedia source: https://dumps.wikimedia.org/enwikisource/20230701/

## Componenrts

### FileManager

Simple class for managing the file streams. Provides methods for getting StreamReader, StreamWriter and line enumerator. Handles errors that might occur during file reading/writing.

### WikimediaParser

Parses the WikiSource xml dumps from Wikimedia and extracts following pair mapping: PageTitle <-> PageLinks into a Dictionary. It is a simple automaton that have just 2 states that uses regex matches on whole lines of the xml to parse out what is needed.

### JsonManager

Stores/loads the Dictionary mapping to/from a JSON file. This is needed so we do not need to load the data every time from the multiple gigabytes large xml file. JSON was chosen for its user-readability and ease of user-editting although its compression ratio is not the best and we are aware that there are better formats for this job. The parser itself is a little more complex automaton than the WikimediaParser's one with several states and does not leverage regexes. It should be noted that it is a format specific algorithm that can only load file in the same format as it stores it in, but the format is valid JSON format and the values can be edited by user if the formating is preserved.

### Painter

Given a result tree return coordinates and sizef for each node ready for drawing. It is done by a recursive algorithm with backtracking that chooses random coordinates for a given node and check if there are any conflicts, if not it continues to the neighboring node(s) (the search is depth first). The algorithm has several parameters that can be tuned to get different looking results and each run produces a random unique solution. Since there is involved randomness, depending on the parameters the algorithm can seem to be stuck because the search space could be enormous and it might by chance choose the wrong paths multiple times in the row and in such case it is better to kill it and run again (the run time should be well under 1 second). This is one area that the algorithm could be improved in but the final program is dealing with this behavior so it is not a problem for us. 
