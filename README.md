# Generic Data Parser API

Generyczny endpoint HTTP (ASP.NET Core Web API, .NET 9), który przyjmuje ustandaryzowany
payload JSON z danymi zakodowanymi w Base64 (`CSV` lub `INTERNAL_JSON`), parsuje je i zwraca
wynik w ujednoliconej strukturze JSON.

## Stos technologiczny

- .NET 9 / ASP.NET Core Web API (Controllers)
- System.Text.Json
- xUnit (testy)
- Docker (opcjonalne uruchomienie w kontenerze)

## Struktura repozytorium

```
GenericParserApi/
├── GenericParserApi.sln
├── Dockerfile
├── GenericParserApi/            # projekt API
│   ├── Controllers/ParserController.cs
│   ├── Models/                  # ParseRequest, ParseResponse, SupportedDataTypes
│   ├── Services/                # IParserStrategy, CSVParserStrategy, JsonParserStrategy
│   └── Program.cs
└── ParserTests/                 # testy jednostkowe (xUnit)
```

## Uruchomienie lokalnie (dotnet run)

Wymagany [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
git clone https://github.com/NazarDushka/Generic-Data-Parser-API.git
cd Generic-Data-Parser-API/GenericParserApi

dotnet run --project GenericParserApi
```

Domyślnie aplikacja wystartuje pod adresem `http://localhost:5253` (patrz
`GenericParserApi/Properties/launchSettings.json`).

### Uruchomienie testów

```bash
dotnet test
```

## Uruchomienie w Dockerze

W repozytorium znajduje się `Dockerfile`.

```bash
cd Generic-Data-Parser-API/GenericParserApi

# budowanie obrazu
docker build -t generic-parser-api .

# uruchomienie kontenera (API dostępne na http://localhost:8080)
docker run --rm -p 8080:8080 generic-parser-api
```

Po starcie kontenera endpoint jest dostępny pod `http://localhost:8080/api/v1/parse-content`.

## Kontrakt API

**POST** `/api/v1/parse-content`
Header: `Content-Type: application/json`
