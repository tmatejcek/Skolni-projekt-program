# Návrh programu

Program je rozdělený na dvě části:

- **Interní (business) logika:** (`SaveData`, `Tournament`, `Standings`, `Storage`) – drží data a hlídá pravidla turnaje. Neobsahuje `Console.WriteLine` ani `Console.ReadLine`. Chyby se hlásí pomocí výjimek.
- **Uživatelské rozhraní:** (`Program`, `Input`) – čte vstup z klávesnice, volá interní logiku, zachytává výjimky a vypisuje výsledky.

Všechna data turnaje jsou v jednom objektu `SaveData`. `Tournament` je mění podle pravidel, `Standings` z nich počítá tabulku a `Storage` je převádí do souboru a zpět.

## Seznam tříd

Formát: třída – co drží

- **SaveData** – seznamy záznamů `PlayerRecord`, `RoundRecord`, `MatchRecord`, `ByeRecord`, seznam přezdívek přihlášených hráčů a jeden `TournamentRecord` (nebo `null`, když turnaj není založen). Každý záznam odpovídá jednomu řádku souboru.
- **Tournament** – objekt `SaveData`, nad kterým pracuje. Má metody pro hráče, turnaj, kola, zápasy, volné losy, výsledky a uzavření kola a hlídá všechna pravidla (např. hráč nejvýše jednou v kole, přihlašování jen před 1. kolem).
- **Standings** – nic (statická třída). Metoda `Calculate(SaveData)` spočítá tabulku.
- **StandingRow** – jeden řádek tabulky: pořadí, přezdívka, počet odehraných zápasů, body.
- **Storage** – cestu k souboru a případnou hlášku o poškozeném souboru (`LastRecoveryMessage`).
- **MatchResult** – výčet výsledků zápasu: `Player1Win`, `Player2Win`, `Draw`, `None`.
- **TournamentException** – zprávu o porušení pravidla turnaje.
- **StorageException** – číslo řádku a důvod chyby v souboru.
- **Input** – nic (jen metody pro bezpečné čtení textu, čísla v rozsahu, volby z menu a ano/ne).
- **Program** – objekt `Storage` se souborem `turnaj.txt`; obsahuje menu.

## Průběh jedné akce

Každá volba v menu proběhne stejně:

1. `Input` načte vstup od uživatele.
2. `Storage.Load()` načte aktuální data ze souboru.
3. `Tournament` provede změnu (nebo vyhodí `TournamentException`).
4. `Storage.Save()` data hned uloží.

Program tak jde kdykoli vypnout a po zapnutí pokračuje tam, kde skončil.

## Formát ukládání

Formát je vlastní, textový, v kódování UTF-8. Nepoužívá JSON, XML ani žádnou knihovnu.

### Princip

- Jeden řádek = jeden záznam.
- První pole řádku určuje typ záznamu, další pole jsou data.
- Pole se oddělují znakem `|`.
- Mezery a diakritika se ukládají beze změny.
- Prázdné řádky a řádky začínající `#` (komentáře) se při načítání přeskakují.

### Typy záznamů

Formát: záznam – pole – význam

- **ULOZENI-TURNAJE** – verze formátu – hlavička, vždy první řádek
- **HRAC** – přezdívka, jméno – hráč
- **TURNAJ** – název, disciplína, počet kol – otevřený turnaj (nejvýše jeden)
- **PRIHLASKA** – přezdívka – hráč přihlášený do turnaje
- **KOLO** – číslo kola, `OTEVRENE` / `UZAVRENE` – kolo
- **ZAPAS** – číslo kola, číslo zápasu, hráč 1, hráč 2, výsledek – výsledek je `VYHRA1`, `VYHRA2`, `REMIZA` nebo `-` (zatím nezadán)
- **VOLNY** – číslo kola, přezdívka – volný los v daném kole
- **KONEC** – počet záznamů – patička, vždy poslední řádek

Počet v `KONEC` je počet záznamů mezi hlavičkou a patičkou (bez komentářů a prázdných řádků).

### Zakázaný znak `|`

`|` je oddělovač polí, proto ho žádný text nesmí obsahovat. Hlídá to `Tournament` u přezdívky, jména, názvu turnaje i disciplíny, takže řádek jde při načítání rozdělit jednoduše přes `Split('|')`.

### Ukázka souboru

```
ULOZENI-TURNAJE|1
HRAC|pepa|Josef Dvořák
HRAC|eva|Eva Malá
HRAC|ondra|Ondřej Černý
TURNAJ|Podzimní šachy|šachy|3
PRIHLASKA|pepa
PRIHLASKA|eva
PRIHLASKA|ondra
KOLO|1|UZAVRENE
ZAPAS|1|1|pepa|eva|REMIZA
VOLNY|1|ondra
KONEC|10
```

### Typ záznamu, oddělování, vyhledávání, duplicity

- **Typ záznamu:** určuje ho první pole řádku. Každý řádek je samostatný, takže poškozený řádek nerozbije ostatní.
- **Oddělování:** záznamy jsou oddělené novým řádkem, pole znakem `|`.
- **Vyhledávání:** v souboru se nevyhledává. Soubor se načte celý do seznamů v paměti a hráč se hledá průchodem seznamu podle přezdívky. Pro turnaj školního klubu (desítky hráčů) je to dostatečně rychlé a kód zůstává jednoduchý.
- **Duplicity:** každý hráč je uložený jen jednou, ostatní záznamy na něj odkazují přezdívkou. Duplicitám se brání už při zadávání: `Tournament` odmítne stejnou přezdívku, dvojí přihlášení i hráče dvakrát v jednom kole. Přezdívky se porovnávají bez ohledu na velikost písmen.

### Co se neukládá

Tabulka a stav turnaje se neukládají, protože se dají vždy dopočítat (tabulku počítá `Standings`, turnaj je rozehraný, když existuje aspoň jedno kolo). Volný los se ukládá, aby staré turnaje zůstaly správně i při změně pravidel.

## Načítání

`Load` se skládá ze dvou částí: `LoadFile` soubor čte a kontroluje, `Load` kolem něj dělá ochranu proti poškozenému souboru.

- Když soubor chybí, program začne s prázdnými daty.
- Kontroluje se hlavička, `KONEC` a počet záznamů, takže se pozná cizí nebo useknutý soubor.
- U každého řádku se kontroluje počet polí, čísla (kladná), neprázdné texty a povolené hodnoty. Turnaj smí být jen jeden a hráč nesmí hrát sám proti sobě.
- Při chybě `LoadFile` vyhodí `StorageException` s číslem řádku. `Load` ji zachytí, poškozený soubor přejmenuje na `turnaj.txt.poskozeny` (případně `.poskozeny.1` atd.), aby ho další uložení nepřepsalo, a vrátí prázdná data.
- Hlášku pro uživatele `Storage` jen připraví. Vypíše ji `Program`, který si ji vezme přes `TakeRecoveryMessage()`.

## Ukládání

Data se zapíšou do `*.tmp` a ten se pak přejmenuje na cílový soubor. Když program spadne uprostřed zápisu, původní soubor zůstane celý.

## Proč tento formát

Je čitelný pro člověka, jednoduchý na parsování a odolný proti poškození. Jde rozšířit o nové typy záznamů a verze v hlavičce odliší staré soubory. Diakritika i mezery zůstávají beze změny, což ověřil test uložit → načíst → uložit.
