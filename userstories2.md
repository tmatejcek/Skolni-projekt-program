1	Setup: repo, větve Dev/Stage/Main, kanban, pravidla PR	Větve existují, board má sloupce, README popisuje workflow	1 h
2	Napsat User-stories.md	Stories ve tvaru „Jako pořadatel chci…, abych…“, každá pokrývá bod zadání	2 h
3	Napsat SPECIFIKACE.md (bez AI)	Slovník, bodování (výhra/remíza/volný los), seznam funkcí	3 h
4	Napsat NAVRH.md (bez AI)	Třídy, vlastní formát ukládání a jeho odůvodnění	4 h
5	Hráči: přidat, odebrat, vypsat (logika)	Jedinečná přezdívka, duplicita se odmítne, žádný Console.WriteLine	3 h
6	Turnaj: založení, přihlášení/odhlášení hráčů	Název, disciplína, počet kol; změny hráčů jen před 1. kolem; jen jeden otevřený turnaj	3 h
7	Kolo: vytvoření a ruční určení dvojic	Hráč max. 1× v kole, jen přihlášení hráči, zbylý hráč dostane volný los	4 h
8	Zápas: zadání a přepis výsledku	Výhra 1./2./remíza; přepis možný do uzavření kola; neexistující zápas se odmítne	3 h
9	Uzavření kola	Nejde uzavřít, dokud některý zápas nemá výsledek	2 h
10	Pravidlo: hráče s odehraným zápasem nejde smazat	Pokus o smazání vrátí chybu, data zůstanou	1 h
11	Výpočet tabulky	Pořadí, hráč, body, odehrané zápasy; volný los podle specifikace	4 h
12	Pomocné funkce pro vstup z klávesnice	Ošetřené text místo čísla, mimo rozsah, prázdný vstup	2 h
13	Konzolové menu: hráči	Funguje přidání, odebrání, výpis; chyby se vypíšou srozumitelně	3 h
14	Konzolové menu: turnaj a kola	Založení, přihlášení, tvorba kola, určení dvojic	4 h
15	Konzolové menu: výsledky a tabulka	Zadání výsledku, uzavření kola, výpis tabulky	3 h
16	Uložení do souboru (vlastní formát)	Ukládá hráče, turnaj, kola, zápasy, výsledky; bez JSON/XML knihoven	5 h
17	Načtení ze souboru + chybové stavy	Po načtení jde pokračovat; chybějící a poškozený soubor program nezhroutí	5 h
18	Testy business logiky	Pokryté hráči, kola, uzavření, tabulka, ukládání/načtení	4 h
19	Ověření tabulky na testovacím turnaji	4–5 hráčů, ≥3 kola, remíza a volný los; ruční výpočet shodný s programem, doloženo	3 h
20	Stage: kompletní testování, oprava chyb, merge do Main	Všechna kritéria ticketů ověřena, dokumentace odpovídá kódu	3 h
