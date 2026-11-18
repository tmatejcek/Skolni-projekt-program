# Návrh programu

Program je rozdělený na dvě části:

- **Interní (business) logika:** (`Player`, `Match`, `Round`, `Tournament`, `Standings`, `Storage`) – drží data a hlídá pravidla turnaje. Neobsahuje `Console.WriteLine` ani `Console.ReadLine`. Chyby se hlásí pomocí výjimek.
- **Uživatelské rozhraní:** (`Program`, `Input`) – čte vstup z klávesnice, volá interní logiku, zachytává výjimky a vypisuje výsledky.

## Seznam tříd

Formát: třída – co drží

- **Player** – přezdívka (`Nickname`, jedinečný identifikátor), jméno (`Name`)
- **Match** – hráč 1, hráč 2, výsledek (`MatchResult`: `Win1`, `Win2`, `Draw`, `None`)
- **Round** – číslo kola, seznam zápasů, hráč s volným losem (nebo `null`), příznak `IsClosed`
- **Tournament** – název, disciplína, počet kol, seznam všech hráčů, seznam přihlášených hráčů, seznam kol
- **Standings** – řádky tabulky (pořadí, hráč, body, počet odehraných zápasů)
- **SaveData** – seznamy záznamů `PlayerRecord`, `TournamentRecord`, `RoundRecord`, `MatchRecord`, `ByeRecord` (obsah souboru v paměti)
- **Storage** – cestu k souboru
- **Input** – nic (jen metody pro čtení vstupu)
- **Program** – aktuální turnaj

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

`|` je oddělovač polí, proto ho žádný text nesmí obsahovat. Hlídá to `Input`, takže řádek jde při načítání rozdělit jednoduše přes `Split('|')`.

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
- **Vyhledávání:** v souboru se nevyhledává. Načte se jednou při startu a hráči se drží v `Dictionary` podle přezdívky.
- **Duplicity:** každý hráč je uložený jen jednou, ostatní záznamy na něj odkazují přezdívkou. Duplicitní záznam znamená poškozený soubor.

### Co se neukládá

Tabulka a stav turnaje se neukládají, protože se dají vždy dopočítat. Volný los se ukládá, aby staré turnaje zůstaly správně i při změně pravidel.

## Načítání

- Když soubor chybí, program začne s prázdným stavem.
- Kontroluje se hlavička, `KONEC` a počet záznamů, takže se pozná cizí nebo useknutý soubor.
- U každého řádku se kontroluje počet polí, čísla a povolené hodnoty.
- Při chybě se nenačte nic a `Storage` vyhodí `StorageException` s číslem řádku.
- Z `SaveData` se turnaj sestaví přes metody `Tournament`, takže pravidla se kontrolují na jednom místě.

## Ukládání

Data se zapíšou do `*.tmp` a ten se pak přejmenuje na cílový soubor. Když program spadne uprostřed zápisu, původní soubor zůstane celý.

## Proč tento formát

Je čitelný pro člověka, jednoduchý na parsování a odolný proti poškození. Jde rozšířit o nové typy záznamů a verze v hlavičce odliší staré soubory. Diakritika i mezery zůstávají beze změny, což ověřil test uložit → načíst → uložit.
