-> start

=== start ===
# PLAY_MUSIC_DRAMATIC

Gra dramatyczna muzyka. Armia Wielkiego Mistrza Zła nadciąga zza wzgórz — jej sztandary łopoczą na wietrze, a huk kroków jeźdźców przytłacza cały krajobraz.

W wiosce wszyscy są w gotowości. Nasz bohater, czarodziej i obrońca tego miejsca, stoi na czele z rycerzami.

Walka wybucha z całą siłą.

-> battle

=== battle ===
Walka trwa.

Niektórzy rycerze padają martwi, inni jęczą z ran. Mimo to czarodziej walczy dzielnie — jego zaklęcia powalają wielu przeciwników.

Ale nagle... zostaje otoczony przez czarnoksiężników Mistrza Zła.

-> ambush

=== ambush ===
Zaskoczony z każdej strony, nasz bohater zostaje powalony potężnym zaklęciem. Ciemność pochłania jego świadomość...

-> after_battle

=== after_battle ===
# STOP_MUSIC
# DELAY_5_SECONDS

Po dłuższym czasie... bohater odzyskuje przytomność.

Wszystko wokół jest w płomieniach. Budynki płoną, mieszkańcy krzyczą i biegają w panice.

Ale armii wroga ani śladu.

-> wake_up

=== wake_up ===
* [Wstań i rozejrzyj się.]
    Bohater powoli wstaje, czując ból w całym ciele. Ale teraz nie ma czasu na odpoczynek.
    -> talk_with_villagers
* [Leż jeszcze chwilę.]
    Czarodziej jeszcze przez moment zbiera myśli. Jednak nie może tu leżeć wiecznie.
    -> talk_with_villagers

=== talk_with_villagers ===
Postanawia porozmawiać z mieszkańcami i władcą wioski.

Czas dowiedzieć się, co dokładnie się wydarzyło... i co należy zrobić dalej.

-> next_level

=== next_level ===
# LOAD_SCENE_LEVEL_1
-> DONE