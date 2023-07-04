# Closest distance of two wikipedia pages by hyperlinks

## Parts

### Parsing downloaded wikipedia

- extract the usefull information
- store in some reasonable format

### Page search

- suggest actual articles when searching (eg. misspel)

### Traversing pages

- use statistical approach with heuristics to "traverse" through links connecting two or more topics

### Visualisation

- visualize the traversing as a 2D(3D?) graph

## Implementation details

Wikipedia can be dumped in multiple ~800MB files or one large file. It contains only the text an some metadata about each article. If there is a link in the article it is in following format: `[[link1 link2 linkN]]`. We can parse it and find corresponding linked articles using article metadata.
