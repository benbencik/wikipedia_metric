# wikipedia_metric

~~The regexes adds about ~2x time complexity. On one 970MB wikisource dump it takes to just go through the file about 2,5s. With the regex matching it takes about 5.5s on my pc.~~

After some regex optimizations performance of the parser from reading the file to creating a map of title-links takes ~4s.
