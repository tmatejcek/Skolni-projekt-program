# Turnaj

Konzolový program pro řízení školního turnaje: hráči, kola, výsledky, tabulka, uložení a načtení.
Viz [SPECIFIKACE.md](SPECIFIKACE.md), [NAVRH.md](NAVRH.md), [User-stories.md](User-stories.md).

Turnajová pravidla jsou oddělená od konzolového rozhraní; tabulka se počítá z uložených výsledků při každém zobrazení.

**Spuštění:** Nainstalujte .NET 10 a spusťte z kořenové složky příkaz `dotnet run --project Tournament\Tournament.csproj`.

Soubor `Skolni-projekt-program.slnx` sdružuje projekt aplikace do jednoho řešení pro Visual Studio nebo VS Code.

## Větve

- `main` – jen odevzdání (merge ze `stage`)
- `stage` – verze na kompletní testování
- `dev` – vývoj
- `feature/…` – jeden větší úkol, vytvořený z `dev`

## Workflow

1. Ticket na Trellu → *In Progress*.
2. Z `dev` vytvořit `feature/nazev`, průběžně commitovat a pushovat.
3. Pull request do `dev`.
4. PR zkontroluje a vyzkouší někdo jiný než autor, podle akceptačních kritérií.
5. Po mergi ticket → *Finished*.

Drobné opravy jdou rovnou do `dev`. Do `stage` a `main` se přímo necommituje.

## Pravidla

- Logika neobsahuje `Console.WriteLine`.
- Ukládání bez JSON/XML knihoven.
- Texty nesmí obsahovat znak `|`.
