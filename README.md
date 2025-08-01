# JSON to XML Converter – Ubiminds Challenge

This project is a .NET REST API that receives a structured JSON, validates business rules, converts it to formatted XML, and saves the result to disk.

## Requirements

- .NET 9 SDK
- Visual Studio, Rider, or VS Code
- (Optional) Docker

## Running Locally

1. Clone the repository:

```bash
git clone https://github.com/your-account/ubiminds-jsontoXml.git
cd ubiminds-jsontoXml
```

2. Run the API:

```bash
cd src/Ubiminds.Api
dotnet run
```

> The application will be available at:
- `http://localhost:5000`
- `https://localhost:5001`

## Endpoints

### 1. POST `/convert`

Receives a `DocumentInputModel` with JSON data and sends it for asynchronous processing.

Conversion rules:
- `Status == 3`
- `TestRun == true`
- `PublishDate >= 08/24/2024`

If the rules are not met, XML will not be generated.

### 2. POST `/convert/file`

Receives a `.json` file, performs the same processing, and returns a confirmation response.

Validations:
- The file must contain valid JSON
- The content must match the `DocumentInputModel` structure
- Returns 400 error with clear messages if the model is invalid

## Testing

Run all tests with:

```bash
dotnet test
```

- Unit tests validate business rules, XML conversion, and error handling
- Integration tests validate the full pipeline via WebApplicationFactory
- Tests follow the AAA pattern and are organized by folder

## Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
src/
├── Ubiminds.Api               → Controllers, Middlewares, and configuration
├── Ubiminds.Application       → Use cases, Validators, and commands
├── Ubiminds.Domain            → Pure models and contracts
├── Ubiminds.Infrastructure    → Real services, like InMemory queue and XML converter
└── Ubiminds.Tests             → Unit and integration tests
```

## Technical Highlights

- Dynamic conversion of nested JSON into structured XML with hierarchy preservation
- Robust validations using FluentValidation
- Structured logging with Serilog (console and file outputs)
- Simulated messaging with `InMemoryQueue`, easily switchable to RabbitMQ or Kafka
- `Result<T>` pattern instead of exceptions for control flow
- Clean code, single responsibility per class, and well-organized tests

## XML Output Files

Files are saved in:

```
/output/
```

With the name format:

```
ubiminds-{Title}-{Timestamp}.xml
```

They include all fields from the original JSON, respecting structure and additional fields via `AdditionalData`.

---