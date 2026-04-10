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

- ***/api/poem/search?title=<tytuł>&author=[autor]&page=\<number strony\>*** — zwraca stronę zawierającą listę metadanych wyszukanych poematów. Do metadanych należą idetnyfikator (ID) poematu, tytuł oraz autor.

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

- ***/api/poem/\<identyfikator poematu\>/<identyfikator języka>*** — zwraca poemat przetłumaczony na odpowiedni język.

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

