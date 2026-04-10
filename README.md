
<span style="font-size: 1.1rem; font-weight: bold;opacity:0.8"> Uwaga</span>

<span style="opacity:0.8">Dodałem drobną zmianę w pliku [TranslationService](./backend/PoetryGlobal/Application/Features/Poems/Services/TranslationService/TranslationService.cs).</span>
```cs
foreach (var line in sourceLines) // Przed
foreach (var line in sourceLines.ToList().GetRange(0, 1)) // Po
```
<span style="opacity:0.8">Chodzi o to, by serwis tłumaczył tylko jedną linijkę wiersza dla celów demonstracyjnych. Tłumaczenie całego wiersza łatwo prowadzi do przekroczenia limitów ze strony API wykorzystywanego do tłumaczeń.</span>


# Poetry Global

Aplikacja udostępniająca poezję angielską w szerokiej gamie języków.

## Szybki dev setup
Należy sklonować repozytorium oraz wejść do katalogu głównego.

Następnie, należy upewnić się, że **daemon Dockera** jest uruchomiony oraz komenda `docker` działa. Jeśli tak, do uruchomienia aplikacji wystarczy wkleić następującą linię do terminalu:

```bash
docker compose build; docker compose up
```

Wówczas API zostanie udostępnione poprzez http://localhost:5000.


## Funkcjonalność 
Poetry Global ma formę REST API, które na żądanie daje użytkownikowi wybrany poemat w wybranym języku.

## API
- ***/api/language*** — zwraca wszystkie dostępne języki. Każdy język ma swój numeryczny identyfikator (ID) oraz 
kod *ISO 3166-1 alpha-2*.

Przykład odpowiedzi:
```json
{
    "languages": [
        {"id": 1, "code": en},
        ...
    ]
}
```

- ***/api/poem/search?title=<tytuł>&author=[autor]&page=\<number strony\>*** — zwraca stronę zawierającą listę metadanych wyszukanych poematów. Do metadanych należą identyfikator (ID) poematu, tytuł oraz autor.

Przykład odpowiedzi:
```json
{
    "poemMetadata": [
        {
            "id": 1,
            "title": "The Raven",
            "Author": "Edgar Allan Poe",
        },
        ...
    ]
}
```

- ***/api/poem/\<identyfikator poematu\>/<identyfikator języka>*** — zwraca poemat przetłumaczony na odpowiedni język. Działa wyłącznie wtedy, gdy poemat o odpowiednim ID istnieje już w bazie danych. Aby do tego doszło, musi on zostać wyszukany poprzez ***/api/poem/search***.

Przykład odpowiedzi:
```json
{
    "poem":{
        "id":22,
        "title":"To Frances S. Osgood",
        "author":"Edgar Allan Poe",
        "languageId":1,
        "isOriginal":true,
        "versionText": "Przetłumaczenie poematu\nDaje pomoc światu"
    }
}
```

# Integracja z API
Aplikacja zbudowana jest w sposób modularny, dzięki czemu można ją połączyć z dowolnym API pozwalającym na wyszukiwanie wierszy według tytułu oraz autora oraz dostęp do ich tekstów.

## Wykorzystywane API
W aktualnej formie, aplikacja pozyskuje poematy z serwisu [PoetryDb](https://poetrydb.org). 

W razie potrzeby, poematy tłumaczone są za pomocą serwisu [MyMemory](https://https://mymemory.translated.net/). Jest to rozwiązenie prowizoryczne — zależało mi na tym, aby do działania aplikacji nie były potrzebne klucze API. Docelowo, powinien tu zostać wykorzystany serwis taki jak Google Translate lub model LLM.

## Inteligentne zapisywanie danych
Serwer zapisuje pobrane wiersze w bazie danych oraz zapisuje wyniki wyszukiwań w pamięci, dzięki czemu z czasem maleje potrzeba wysyłania zapytań do zewnętrznego API. Podobnie jest z tłumaczeniami. 

Ponadto, baza danych została zaprojektowana by umożliwić wyszukiwanie oparte na *trigramach*. Umożliwia to skuteczniejsze wyszukiwanie wierszy, które istnieją już w bazie danych.

## Wybrane foldery oraz pliki
**[compose.yml](./compose.yml)** — służy do koordynacji budowania oraz uruchomienia kontenerów zawierających bazę danych oraz serwer.

**[backend/PoetryGlobal](./backend/PoetryGlobal/)** — tutaj znajduje się większość implementacji serwera. Zawiera on następujące foldery:
- **Features/Poems** — tutaj umieszczony jest kod ściśle związany z udostępnianiem poematów. Istnieje to kilka sekcji, według których podzieliłem implementację:
    - **Repositories** — obiekty umożliwiające łatwy dostęp do bazy danych. Każde *repository* reprezentuje jakąś tabelę w bazie.
    - **Cache** — cache przechowywane w pamięci aplikacji, wspierające dostęp z wielu wątków.
    - **Services** — abstrakcje (interfejsy oraz klasy) reprezentujące serwisy, w szczególności umożliwiają one dostęp do zewnętrznych API
    - **Orchestration** — zawiera abstrakcje koordynujące działania wyżej wymienionych elementów w celu realizowania właściwej funkcjonalności aplikacji.
    - **Controllers** — warstwa kontrolerów zgodnie z konwencjami ASP.NET.

    Dodatkowo, istnieją dwie sekcje związane z obiektami danych: 
    - **Models** — obiekty odpowiadające rekordom w bazie danych. Dzielą się one na zwykłe modele (`IModel`), które mogą być niekompletne i możliwie nie zostały jeszcze utrwalone w bazie danych, oraz modele utrwalone (`IPersistedModel`), które reprezentują rekordy pobrane z bazy danch.
    - **DTOs** — obiekty danych służące do usprawnienie przepływu danych między serwisami/warstwami aplikacji oraz klientami.

- **SharedLibraries**. Zawiera on wszelkie biblioteki używane przez pozostałe elementy aplikacji.

- **SharedDIDependencies** zawiera dodatkowe elementy "wstrzykiwane" do aplikacji (*dependency injection*), które nie są unikalne dla konkretnej funkcjonalności.

**[Program.cs](./backend/PoetryGlobal/Program.cs)** — program konfigurujący oraz uruchamiający serwer.

**[init.sql](./db/initdb/init.sql)** — plik służący do inicjalizacji bazy danych. Definiuje on tabele, relacje między nimi oraz indeksy.

<br/>
Zacząłem też implementować prowizoryczny frontend, jednakże nie zdążyłem przygotować poprawnie działającego deploymentu (problem z dostępem poza kontenerem).




