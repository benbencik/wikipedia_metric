# Wikipedia metric

The idea of the project is to somehow search the wikipedia and compare distance of two arbitrary articles. The distance is defined as the number of links between the articles. I was interested to see what I could learn about the data and see which approaches work best.

What I want to do:

- program search of wikipedia
- try if clusteriung works and could help with the search
- analyze and identify the most commonly traversed paths between articles
- identify potential knowledge gaps or areas where information is lacking. Look for isolated clusters or articles with few connections

Započtový program budem robiť spoločne s Markom Čechovičom 
Program bude hľadať najkratšiu vzdialenosť medzi wikipedia článkami, podľa hyperlinkov v článkoch. 

Započták chceme rozdeliť na časti:
    Parsovanie stiahnutej Wikipedie
    Vyhľadávanie článkov
    Hľadanie najkratšej vzdialenosti
    Vizualizácia
Z toho ja budem pracovať na:
    Hľadanie najkratšej vzdialenosti
        naivné prechádzanie všetkých linkov bude pravdepodobne pomalé
        v tejto časti by som chcel nájsť vhodný algoritmus na vyhľadávanie vymyslieť heuristiky, ktorými orežem počet článkov cez ktoré budem prechádzať
        súčasťou toho bude aj rozbor dát a nejaká štatistika (napríklad by som sa chcel pozrieť na témy ktoré zgrupujú veľa článkov napr.: veda, história, filozofia...)
    Vizualizácia
        cestu medzi článkami chcem interaktívne vizualizovať v grafickom rozhraní pomocou grafu
    Dokumentácia & Testy: tie si budeme obaja písať ku svojej časti


3925454
544246
347645