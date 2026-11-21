# Návrh programu

Program je konzolová aplikace rozdělená podle odpovědností:

- **Uživatelské rozhraní:** (`Program`, `Input`) – zobrazuje menu a čte vstupy. `Program` předává příkazy doménové vrstvě a zobrazuje její výsledky nebo chyby.
- **Turnajová logika:** (`Tournament`, `Standings`) – kontroluje pravidla a mění stav turnaje; `Standings` počítá pořadí bez ukládání výsledné tabulky.
- **Ukládání:** (`SaveData`, `Storage`) – drží záznamy v paměti a převádí je do vlastního textového formátu a zpět.

Turnajová logika nepracuje s konzolí. Chyby pravidel předává uživatelskému rozhraní jako `TournamentException`.

## Seznam tříd

Formát: třída – co drží

- **Player** – přezdívka (`Nickname`, jedinečný identifikátor), jméno (`Name`)
- **Match** – hráč 1, hráč 2, výsledek (`MatchResult`: `Player1Win`, `Player2Win`, `Draw`, `None`)
- **Round** – číslo kola, seznam zápasů, hráč s volným losem (nebo `null`), příznak `IsClosed`
- **Tournament** – příkazy pro správu hráčů, registrace, kol, zápasů a jejich výsledků; kontroluje pravidla turnaje nad stavem `SaveData`
- **Standings** – vypočítá seřazené řádky tabulky z přihlášených hráčů, výsledků a volných losů
- **StandingRow** – pozice, přezdívka, počet odehraných zápasů a body jednoho hráče
- **SaveData** – seznamy záznamů `PlayerRecord`, `TournamentRecord`, `RoundRecord`, `MatchRecord`, `ByeRecord` (obsah souboru v paměti)
- **Storage** – cestu k souboru
- **Input** – nic (jen metody pro čtení vstupu)
- **Program** – menu, vstup a výstup konzole; předává příkazy `Tournament` a zobrazuje řádky z `Standings`

`SaveData` a jeho záznamy jsou současně reprezentací, kterou ukládá `Storage`. `Player`, `Match` a `Round` představují doménové typy; příkazy aktuálně pracují se záznamy v `SaveData`, aby zůstalo načítání a ukládání přímočaré.

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

`|` je oddělovač polí, proto ho žádný text nesmí obsahovat. Kontrolují to příkazy při zadávání textu, takže řádek jde při načítání rozdělit jednoduše přes `Split('|')`.

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

- **Typ záznamu:** určuje ho první pole řádku. Neplatný nebo nekonzistentní záznam způsobí odmítnutí celého souboru; data se nenačítají částečně.
- **Oddělování:** záznamy jsou oddělené novým řádkem, pole znakem `|`.
- **Vyhledávání:** při provedení příkazu se načte celý soubor a záznamy se prohledávají v paměti podle přezdívky.
- **Duplicity:** každý hráč je uložený jen jednou, ostatní záznamy na něj odkazují přezdívkou. Duplicitní záznam znamená poškozený soubor.

### Co se neukládá

Tabulka se neukládá, protože se při zobrazení vždy přepočítá z výsledků. Turnaj, kola, zápasy, výsledky a volné losy se ukládají.

## Načítání

- Když soubor chybí, program začne s prázdným stavem.
- Kontroluje se hlavička, `KONEC` a počet záznamů, takže se pozná cizí nebo useknutý soubor.
- U každého řádku se kontroluje počet polí, čísla a povolené hodnoty.
- Při chybě formátu nebo nekonzistentních/duplicitních záznamech se nenačte nic. Poškozený soubor se přejmenuje na `.poskozeny` (při kolizi dostane číselnou příponu), aplikace upozorní pořadatele a začne s prázdnými daty.
- Kontrolují se unikátní přezdívky hráčů, registrace, čísla kol a zápasů, odkazy na existující hráče a kola i jediné zařazení hráče v kole.

## Ukládání

Data se zapíšou do `*.tmp` a ten se pak přejmenuje na cílový soubor. Když program spadne uprostřed zápisu, původní soubor zůstane celý.

## Aktuální konzolové funkce

Menu umožňuje založit jeden turnaj, evidovat a přihlásit hráče, vytvářet kola a ručně zadávat zápasy, přidělovat volné losy, zaznamenávat výsledky, uzavírat kola a zobrazovat průběžnou tabulku. Přehled turnaje ukazuje název, disciplínu, plánovaný počet kol, přihlášené hráče a aktuální kolo. Výpis zápasů se provádí pro zvolené kolo.

Kolo lze vytvořit jen s alespoň dvěma přihlášenými hráči. Při lichém počtu hráčů musí mít právě jeden hráč volný los. Volný los se přednostně přiděluje hráči s nejmenším počtem předchozích volných losů; opakování je povoleno, pokud už žádný dostupný hráč nemá méně předchozích volných losů. Kolo lze uzavřít až po zařazení všech hráčů právě jednou a zadání všech výsledků.

Integrační testy konzolového programu se spouštějí příkazem `dotnet run --project Tournament.Tests/Tournament.Tests.csproj`.

## Proč tento formát

Je čitelný pro člověka, jednoduchý na parsování a odolný proti poškození. Jde rozšířit o nové typy záznamů a verze v hlavičce odliší staré soubory. Diakritika i mezery zůstávají beze změny, což ověřil test uložit → načíst → uložit.
