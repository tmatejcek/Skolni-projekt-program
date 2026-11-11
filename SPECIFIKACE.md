# Specifikace – Turnajový systém

## 1. Slovník pojmů
* **Hráč:** (player) Účastník turnajů. Má celé jméno a unikátní přezdívku.
* **Přezdívka:** (nickname) Hlavní identifikátor hráče v programu (nerozlišují se velká a malá písmena).
* **Turnaj:** (tournament) Konkrétní soutěž (název, hra, počet kol). Otevřený je vždy jen jeden.
* **Kolo:** (round) Část turnaje se zápasy. Dokud je otevřené, dají se měnit výsledky. Uzavřít jde až po zadání všech zápasů.
* **Zápas:** (match) Dvojice hráčů v rámci kola a jejich výsledek.
* **Volný los:** (single) Situace při lichém počtu hráčů, kdy jeden v kole nehraje. Dostane body, ale nezapočte se mu odehraný zápas.
* **Tabulka:** (leaderboard) Aktuální pořadí podle bodů. Nikam se neukládá, pokaždé se přepočítává z výsledků.

## 2. Bodování
* **Výhra:** 3 body (+1 zápas)
* **Remíza:** 1 bod (+1 zápas)
* **Prohra:** 0 bodů (+1 zápas)
* **Volný los:** 3 body (+0 zápasů)

Při rovnosti bodů rozhoduje abeceda podle přezdívky. Rozstřely ani vzájemné zápasy zatím neřešíme.

## 3. Seznam funkcí

### Správa hráčů
* Přidání hráče (jméno a unikátní přezdívka).
* Výpis všech registrovaných hráčů.
* Smazání hráče (jde jen v případě, že ještě neodehrál žádný zápas).

### Správa turnaje
* Založení turnaje (název, disciplína, celkový počet kol).
* Přihlašování a odhlašování hráčů (lze jen do začátku prvního kola).

### Kola a zápasy
* Vytvoření nového kola a ruční zadání dvojic (hráč může být v kole jen jednou).
* Přiřazení volného losu při lichém počtu hráčů.
* Zadání a přepsání výsledku zápasu (výhra 1 / výhra 2 / remíza).
* Uzavření kola (lze provést až po zadání všech výsledků v kole).

### Tabulka
* Výpis průběžného pořadí (pořadí, přezdívka, odehrané zápasy, body).
* Přepočet přímo z dat bez ukládání tabulky na disk.

### Ukládání a načítání
* Uložení stavu do vlastního textového souboru přes oddělovač `|` (žádný JSON ani XML).
* Bezpečný zápis přes dočasný soubor `.tmp` proti poškození při pádu.
* Kontrola integrity při načítání (hlavička, počet řádků v patičce `KONEC`).
* Při poškození soubor přejmenovat na `.poskozeny` a pokračovat s prázdným stavem.

### Vstupy a konzole
* Kontrola všech vstupů z klávesnice (neplatná čísla, texty mimo rozsah, prázdné vstupy).
* Oddělení logiky od UI – turnajová logika nesmí obsahovat `Console.WriteLine`.