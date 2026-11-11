# User stories – Turnajový program

## Kdo program používá

| Role | Popis | Co potřebuje |
|------|-------|--------------|
| **Pořadatel** | Člen školního klubu, který turnaj organizuje. Jediný, kdo s programem přímo pracuje. | Evidovat hráče, založit turnaj, tvořit kola a dvojice, zadávat výsledky, sledovat tabulku, turnaj uložit a později v něm pokračovat. |
| **Hráč** | Účastník turnaje. S programem nepracuje, ale zajímá ho jeho pořadí a body (pořadatel mu je ukáže nebo vytiskne). | Aby byly jeho výsledky a body správně. |
| **Vývojář** | Člen týmu (a zadavatel technických požadavků), který program dál rozšiřuje. | Přehledný kód, oddělená logika a výpis, snadné změny zadání. |

> Poznámka: Hra (šachy, piškvorky…) v programu není. Program eviduje pouze výsledky.

---

## 1. Hráči

**US-01 – Přidání hráče**
Jako pořadatel chci přidat hráče se jménem a přezdívkou, abych je mohl přihlásit do turnaje.
- Přezdívka je jedinečná a slouží jako identifikátor.
- Prázdné jméno nebo přezdívka se odmítne.

**US-02 – Výpis hráčů**
Jako pořadatel chci vypsat všechny hráče, abych měl přehled, koho mám v evidenci.

**US-03 – Odebrání hráče**
Jako pořadatel chci odebrat hráče, abych mohl opravit chybně zadaného nebo nepotřebného hráče.
- Hráče s odehraným zápasem nejde smazat.
- Pokus o smazání neexistujícího hráče skončí srozumitelnou chybou.

**US-04 – Hlídání duplicitní přezdívky**
Jako pořadatel chci, aby program nedovolil dvě stejné přezdívky, abych si hráče nikdy nespletl.

---

## 2. Turnaj

**US-05 – Založení turnaje**
Jako pořadatel chci založit turnaj s názvem, disciplínou a počtem kol, abych mohl začít organizovat soutěž.
- Počet kol musí být kladné celé číslo.
- Otevřený je vždy jen jeden turnaj.

**US-06 – Přihlášení hráče do turnaje**
Jako pořadatel chci přihlásit hráče do turnaje, abych určil, kdo se zúčastní.
- Jde to jen do doby, než začne první kolo.
- Přihlásit jde jen hráče, kteří existují v evidenci.

**US-07 – Odhlášení hráče z turnaje**
Jako pořadatel chci odhlásit hráče z turnaje, abych mohl řešit omluvenky nebo omyly.
- Jde to jen do doby, než začne první kolo.

**US-08 – Přehled turnaje**
Jako pořadatel chci vidět základní informace o turnaji (název, disciplína, počet kol, přihlášení hráči, aktuální kolo), abych věděl, v jakém stavu turnaj je.

---

## 3. Kola

**US-09 – Vytvoření nového kola**
Jako pořadatel chci vytvořit nové kolo, abych mohl turnaj posunout dál.
- Nejde vytvořit další kolo, pokud předchozí není uzavřené.
- Nejde vytvořit více kol, než je počet kol turnaje.

**US-10 – Ruční určení dvojic**
Jako pořadatel chci ručně určit dvojice z přihlášených hráčů, abych mohl losování udělat po svém.
- Hráč smí být v kole nejvýše jednou.
- Do kola jdou jen hráči přihlášení do turnaje.

**US-11 – Volný los**
Jako pořadatel chci, aby hráč, který zbyl bez soupeře, dostal volný los, abych nemusel řešit lichý počet hráčů.
- Hráč s volným losem je v kole veden a dostane body podle pravidel ve specifikaci.

**US-12 – Uzavření kola**
Jako pořadatel chci kolo uzavřít, abych zafixoval jeho výsledky a mohl začít další kolo.
- Kolo nejde uzavřít, dokud nemají výsledek všechny zápasy.

**US-13 – Výpis kola**
Jako pořadatel chci vypsat dvojice a výsledky vybraného kola, abych je mohl ukázat hráčům.

---

## 4. Zápasy a výsledky

**US-14 – Zadání výsledku zápasu**
Jako pořadatel chci zadat výsledek zápasu (výhra prvního hráče, výhra druhého hráče, remíza), abych zaznamenal průběh turnaje.
- Nejde zadat výsledek neexistujícího zápasu.

**US-15 – Oprava výsledku**
Jako pořadatel chci přepsat již zadaný výsledek, abych mohl opravit překlep.
- Jde to jen dokud kolo není uzavřené.

---

## 5. Tabulka

**US-16 – Zobrazení tabulky**
Jako pořadatel chci vidět tabulku s pořadím, hráčem, body a počtem odehraných zápasů, abych věděl, jak turnaj stojí.
- Tabulka se po každé změně výsledku přepočítá.
- Řazení je podle bodů sestupně.

**US-17 – Správné bodování**
Jako pořadatel chci, aby se body počítaly podle jasných pravidel (výhra, remíza, volný los), aby byla tabulka spravedlivá a dala se ověřit.
- Pravidla jsou popsána v SPECIFIKACE.md.

**US-18 – Shoda bodů**
Jako pořadatel chci, aby program při shodě bodů neselhal a hráče rozumně seřadil, abych měl tabulku vždy ucelenou.
- Plnohodnotný rozstřel se zatím neřeší.

**US-19 – Ověření tabulky**
Jako vývojář chci ověřit na připraveném turnaji (4–5 hráčů, min. 3 kola, remíza, volný los), že tabulka odpovídá ručnímu výpočtu, abych měl důkaz správnosti.

---

## 6. Ukládání a načítání

**US-20 – Uložení turnaje**
Jako pořadatel chci rozdělaný turnaj uložit do souboru, abych mohl program vypnout a nepřišel o data.
- Ukládají se hráči, turnaj, všechna kola, zápasy i výsledky.

**US-21 – Načtení turnaje**
Jako pořadatel chci při příštím spuštění turnaj načíst, abych mohl plynule pokračovat tam, kde jsem skončil.

**US-22 – Chybějící soubor**
Jako pořadatel chci, aby program při chybějícím souboru nespadl, abych mohl začít s čistým stavem.

**US-23 – Poškozený soubor**
Jako pořadatel chci, aby program poškozený soubor rozpoznal a srozumitelně mě upozornil, abych nezačal pracovat s nesmyslnými daty.
- Poškozený soubor nesmí přepsat použitelná data bez upozornění.

**US-24 – Vlastní formát ukládání**
Jako vývojář chci vlastní textový formát (bez JSON/XML knihoven), abych se naučil práci se soubory a řetězci.
- Formát umožňuje rozpoznat typ záznamu, oddělit záznamy, efektivně vyhledávat a ošetřit duplicity.
- Formát je popsán a odůvodněn v NAVRH.md.

---

## 7. Odolnost vstupů

**US-25 – Ošetření vstupů z klávesnice**
Jako pořadatel chci, aby program nespadl při špatném vstupu (text místo čísla, číslo mimo rozsah, prázdný vstup), abych se mohl překlepnout bez následků.
- Program vypíše srozumitelnou chybu a zeptá se znovu.

**US-26 – Ochrana před nepovoleným stavem**
Jako pořadatel chci, aby program nedovolil nesmyslné operace (hráč v kole dvakrát, přihlášení do rozehraného turnaje, uzavření kola bez výsledků), aby data zůstala vždy správná.

**US-27 – Přehledné menu**
Jako pořadatel chci jednoduché textové menu, abych program ovládal bez učení se příkazů.

---

## 8. Technické a procesní požadavky

**US-28 – Oddělení logiky od výpisu**
Jako vývojář chci mít turnajovou logiku oddělenou od vypisování (žádné `Console.WriteLine` v logice), abych mohl logiku testovat a později změnit rozhraní.

**US-29 – Připravenost na změny zadání**
Jako vývojář chci mít kód navržený tak, aby šel snadno rozšířit (např. jiné bodování, losování programem, rozstřel), protože zadání se během pololetí bude měnit.

**US-30 – Dokumentace**
Jako člen týmu chci mít aktuální SPECIFIKACI, NAVRH a User-stories, abychom se v projektu orientovali a mohli ho odevzdat.

**US-31 – Kanban a Git workflow**
Jako člen týmu chci vést kanban board a pracovat přes větve Dev / Stage / Main a pull requesty, aby byla práce týmu přehledná, zkontrolovaná a hodnotitelná.
- Ticket je v Done až po merge přes pull request a otestování někým jiným než autorem.

---

## Otázky pro zákazníka (k upřesnění)

1. Kolik bodů za výhru, remízu a volný los? (navrhujeme 3 / 1 / 3, nebo 1 / 0,5 / 1 – nutno potvrdit)
2. Smí se hráč s volným losem dostat k volnému losu vícekrát za turnaj?
3. Co když je v turnaji méně než 2 hráči – lze vůbec začít první kolo?
4. Má se tabulka při shodě bodů řadit jen podle abecedy, nebo podle pořadí přihlášení?
5. Smí pořadatel odebrat hráče z již uzavřeného turnaje?

---

## Budoucí rozšíření (zatím mimo rozsah)

- Automatické losování dvojic
- Rozstřel při shodě bodů (např. Buchholz, vzájemný zápas)
- Více souběžných turnajů
