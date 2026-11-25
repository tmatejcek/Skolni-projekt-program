# User stories – Turnajový program

## Kdo program používá

| Role | Popis | Co potřebuje |
|------|-------|--------------|
| **Pořadatel** | Člen školního klubu, který turnaj organizuje. Jediný, kdo s programem přímo pracuje. | Evidovat hráče, založit turnaj, tvořit kola a dvojice, zadávat výsledky, hlídat tabulku, turnaj uložit a příště v něm pokračovat. |
| **Hráč** | Účastník turnaje. Program sám nepoužívá, ale chce vědět, jak je na tom s pořadím a body (pořadatel mu je ukáže nebo vytiskne). | Aby měl výsledky a body správně. |
| **Vývojář** | Člen týmu (a zároveň zadavatel technických požadavků), který program dál vylepšuje. | Přehledný kód, logika oddělená od výpisu a zadání, které jde snadno měnit. |

> Poznámka: Samotná hra (šachy, piškvorky…) v programu není. Program eviduje jen výsledky.

---

## 1. Hráči

**US-01 – Přidání hráče**
Jako pořadatel chci přidat hráče se jménem a přezdívkou, abych ho mohl přihlásit do turnaje.
- Přezdívka je jedinečná a slouží jako identifikátor.
- Prázdné jméno nebo přezdívka se nepřijme.

**US-02 – Výpis hráčů**
Jako pořadatel chci vypsat všechny hráče, abych měl přehled, koho vlastně evidujeme.

**US-03 – Odebrání hráče**
Jako pořadatel chci hráče odebrat, abych mohl opravit omyl nebo se zbavit někoho, kdo už není potřeba.
- Hráče, který už odehrál zápas, smazat nejde.
- Když zkusím smazat hráče, který neexistuje, program mi srozumitelně řekne, co je špatně.

**US-04 – Hlídání duplicitní přezdívky**
Jako pořadatel chci, aby program nepustil dvě stejné přezdívky, abych si hráče nikdy nespletl.

---

## 2. Turnaj

**US-05 – Založení turnaje**
Jako pořadatel chci založit turnaj s názvem, disciplínou a počtem kol, abych mohl začít soutěž organizovat.
- Počet kol musí být kladné celé číslo.
- Otevřený je vždy jen jeden turnaj.

**US-06 – Přihlášení hráče do turnaje**
Jako pořadatel chci přihlásit hráče do turnaje, abych určil, kdo se zúčastní.
- Jde to jen do chvíle, než začne první kolo.
- Přihlásit jde jen hráče, kteří jsou v evidenci.

**US-07 – Odhlášení hráče z turnaje**
Jako pořadatel chci hráče z turnaje odhlásit, abych mohl vyřešit omluvenky nebo překlepy.
- Jde to jen do chvíle, než začne první kolo.

**US-08 – Přehled turnaje**
Jako pořadatel chci vidět základní info o turnaji (název, disciplína, počet kol, přihlášení hráči, aktuální kolo), abych věděl, jak na tom turnaj zrovna je.

---

## 3. Kola

**US-09 – Vytvoření nového kola**
Jako pořadatel chci vytvořit nové kolo, abych mohl turnaj posunout dál.
- Další kolo nejde vytvořit, dokud není to předchozí uzavřené.
- Nejde vytvořit víc kol, než kolik má turnaj.

**US-10 – Ruční určení dvojic**
Jako pořadatel chci dvojice z přihlášených hráčů poskládat ručně, abych mohl losování udělat po svém.
- Hráč smí být v kole maximálně jednou.
- Do kola se dostanou jen hráči přihlášení do turnaje.

**US-11 – Volný los**
Jako pořadatel chci, aby hráč, kterému nevyšel soupeř, dostal volný los, abych se nemusel trápit s lichým počtem hráčů.
- Hráč s volným losem je v kole veden a body dostane podle pravidel ve specifikaci.

**US-12 – Uzavření kola**
Jako pořadatel chci kolo uzavřít, abych zafixoval jeho výsledky a mohl pustit další kolo.
- Kolo nejde uzavřít, dokud nemají výsledek všechny zápasy.

**US-13 – Výpis kola**
Jako pořadatel chci vypsat dvojice a výsledky vybraného kola, abych je mohl ukázat hráčům.

---

## 4. Zápasy a výsledky

**US-14 – Zadání výsledku zápasu**
Jako pořadatel chci zadat výsledek zápasu (výhra prvního hráče, výhra druhého hráče, remíza), abych zaznamenal, jak turnaj probíhá.
- Výsledek neexistujícího zápasu zadat nejde.

**US-15 – Oprava výsledku**
Jako pořadatel chci přepsat už zadaný výsledek, abych mohl opravit překlep.
- Jde to jen do doby, než se kolo uzavře.

---

## 5. Tabulka

**US-16 – Zobrazení tabulky**
Jako pořadatel chci vidět tabulku s pořadím, hráčem, body a počtem odehraných zápasů, abych věděl, jak turnaj stojí.
- Tabulka se po každé změně výsledku sama přepočítá.
- Řadí se podle bodů sestupně.

**US-17 – Správné bodování**
Jako pořadatel chci, aby se body počítaly podle jasných pravidel (výhra, remíza, volný los), aby byla tabulka férová a dala se zkontrolovat.
- Pravidla jsou popsaná v SPECIFIKACE.md.

**US-18 – Shoda bodů**
Jako pořadatel chci, aby program při shodě bodů nespadl a hráče rozumně seřadil, abych měl tabulku pořád pohromadě.
- Plnohodnotný rozstřel zatím neřešíme.

**US-19 – Ověření tabulky**
Jako vývojář chci na připraveném turnaji (4–5 hráčů, aspoň 3 kola, remíza, volný los) ověřit, že tabulka sedí s ručním výpočtem, abych měl důkaz, že to funguje.

---

## 6. Ukládání a načítání

**US-20 – Uložení turnaje**
Jako pořadatel chci rozehraný turnaj uložit do souboru, abych mohl program vypnout a o nic nepřišel.
- Ukládají se hráči, turnaj, všechna kola, zápasy i výsledky.

**US-21 – Načtení turnaje**
Jako pořadatel chci při dalším spuštění turnaj načíst, abych mohl pokračovat přesně tam, kde jsem skončil.

**US-22 – Chybějící soubor**
Jako pořadatel chci, aby program nespadl, když soubor chybí, a mohl jsem začít s čistým stavem.

**US-23 – Poškozený soubor**
Jako pořadatel chci, aby program poznal poškozený soubor a srozumitelně mě upozornil, abych nezačal pracovat s nesmyslnými daty.
- Poškozený soubor nesmí bez varování přepsat data, která se dají použít.

**US-24 – Vlastní formát ukládání**
Jako vývojář chci vlastní textový formát (bez JSON/XML knihoven), abych si osahal práci se soubory a řetězci.
- Formát umožní poznat typ záznamu, oddělit záznamy, rychle vyhledávat a ošetřit duplicity.
- Formát je popsaný a zdůvodněný v NAVRH.md.

---

## 7. Odolnost vstupů

**US-25 – Ošetření vstupů z klávesnice**
Jako pořadatel chci, aby program nespadl při špatném vstupu (text místo čísla, číslo mimo rozsah, prázdný vstup), abych se mohl klidně překlepnout.
- Program vypíše srozumitelnou chybu a zeptá se znovu.

**US-26 – Ochrana před nepovoleným stavem**
Jako pořadatel chci, aby mi program nedovolil nesmysly (hráč v kole dvakrát, přihlášení do už rozehraného turnaje, uzavření kola bez výsledků), aby data zůstala pořád správně.

**US-27 – Přehledné menu**
Jako pořadatel chci jednoduché textové menu, abych program ovládal, aniž bych se musel učit nějaké příkazy.

---

## 8. Technické a procesní požadavky

**US-28 – Oddělení logiky od výpisu**
Jako vývojář chci mít turnajovou logiku oddělenou od vypisování (žádné `Console.WriteLine` v logice), abych ji mohl testovat a případně pozdějc změnit rozhraní.

**US-29 – Připravenost na změny zadání**
Jako vývojář chci kód navrhnout tak, aby šel snadno rozšířit (třeba o jiné bodování, losování programem nebo rozstřel), protože zadání se během pololetí bude určitě měnit.

**US-30 – Dokumentace**
Jako člen týmu chci mít aktuální SPECIFIKACI, NAVRH a User-stories, abychom se v projektu vyznali a mohli ho odevzdat.

**US-31 – Kanban a Git workflow**
Jako člen týmu chci vést kanban board a pracovat přes větve Dev / Stage / Main a pull requesty, aby byla práce týmu přehledná, zkontrolovaná a dala se ohodnotit.
- Ticket je v Done až po merge přes pull request a po otestování někým jiným než autorem.

---

## Otázky pro zákazníka (k upřesnění)

1. Kolik bodů za výhru, remízu a volný los? (navrhujeme 3 / 1 / 3, nebo 1 / 0,5 / 1 – chce to potvrdit)
2. Může hráč dostat volný los za turnaj víckrát?
3. Co když jsou v turnaji míň než 2 hráči – dá se první kolo vůbec začít?
4. Má se tabulka při shodě bodů řadit jen podle abecedy, nebo podle pořadí přihlášení?
5. Smí pořadatel odebrat hráče z už uzavřeného turnaje?

---

## Budoucí rozšíření (zatím mimo rozsah)

- Automatické losování dvojic
- Rozstřel při shodě bodů (např. Buchholz, vzájemný zápas)
- Více souběžných turnajů
