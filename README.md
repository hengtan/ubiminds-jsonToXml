# JSON to XML Converter – Ubiminds Challenge

Este projeto é uma API REST em .NET que recebe um JSON estruturado, valida regras de negócio e o converte para um XML formatado, salvando o resultado em disco.

## Requisitos

- .NET 9 SDK
- Visual Studio, Rider ou VS Code
- (Opcional) Docker

## Execução local

1. Clone o repositório:

```bash
git clone https://github.com/sua-conta/ubiminds-jsontoXml.git
cd ubiminds-jsontoXml
```

2. Rode a API:

```bash
cd src/Ubiminds.Api
dotnet run
```

> A aplicação estará disponível em:
- `http://localhost:5000`
- `https://localhost:5001`

## Endpoints

### 1. POST `/convert`

Recebe um `DocumentInputModel` com os dados em JSON e os envia para processamento assíncrono.

Regras para conversão:
- `Status == 3`
- `TestRun == true`
- `PublishDate >= 24/08/2024`

Caso as regras não sejam atendidas, o XML não é gerado.

### 2. POST `/convert/file`

Recebe um arquivo `.json`, realiza o mesmo processamento, e responde com confirmação.

Validações:
- O arquivo precisa conter um JSON válido
- O conteúdo será validado como `DocumentInputModel`
- Retorna erro 400 com mensagens claras caso o modelo seja inválido

## Testes

Execute todos os testes com:

```bash
dotnet test
```

- Testes unitários validam as regras de negócio, conversão XML e tratamento de erros
- Testes de integração validam o pipeline completo via WebApplicationFactory
- Os testes seguem o padrão AAA e estão organizados por pasta

## Arquitetura

Este projeto segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

```
src/
├── Ubiminds.Api               → Controllers, Middlewares e configuração
├── Ubiminds.Application       → Casos de uso, Validadores e comandos
├── Ubiminds.Domain            → Modelos e contratos puros
├── Ubiminds.Infrastructure    → Serviços reais, como fila InMemory e conversor XML
└── Ubiminds.Tests             → Testes unitários e de integração
```

## Destaques técnicos

- Conversão dinâmica de JSON aninhado para XML estruturado com preservação da hierarquia
- Validações robustas com FluentValidation
- Logs estruturados com Serilog para console e arquivos
- Mensageria simulada com `InMemoryQueue`, facilmente adaptável para RabbitMQ ou Kafka
- `Result<T>` pattern no lugar de exceções em fluxo de controle
- Código limpo, com responsabilidade única por classe e testes bem organizados

## Saída de arquivos XML

Os arquivos são salvos em:

```
/output/
```

Com o nome no formato:

```
ubiminds-{Title}-{Timestamp}.xml
```

Incluem todos os campos do JSON original, respeitando a estrutura e campos adicionais via `AdditionalData`.

---

Desenvolvido para o desafio final da Ubiminds com foco em Clean Code, testabilidade, extensibilidade e clareza.
