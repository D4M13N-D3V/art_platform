# Comissions.app

## Description

This is a application that provides a platform to allow creatives to sell their services without intense moderation.

## Table of Contents

- [Project Name](#project-name)
  - [Description](#description)
  - [Table of Contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
  - [Getting Started](#getting-started)
    - [Clone the Repository](#clone-the-repository)
    - [Build and Run with Docker](#build-and-run-with-docker)
  - [Usage](#usage)
  - [Swagger UI](#swagger-ui)
  - [Contributing](#contributing)
  - [License](#license)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/D4M13N-D3V/art_platform
cd art_platform
```

### Build and Run with Docker

Build the Docker image:

```bash
docker build -t art-platform -f ./src/ArtPlatform.API/Dockerfile --force-rm .
```

Run the Docker container:

```bash
docker run -p 8080:80 art-platform
```

The API should be accessible at `http://localhost:8080`.

## Usage

Describe how to use your API and any specific details or considerations that users need to be aware of.

## Swagger UI

The API is documented using Swagger UI. Once the application is running, you can access the Swagger UI at:

```plaintext
http://localhost:8080/swagger
```

This provides an interactive interface for testing and exploring your API endpoints.

## Contributing

If you would like to contribute to the project, please follow the guidelines in [CONTRIBUTING.md](CONTRIBUTING.md).

## License

This project is licensed under the [License Name] - see the [LICENSE.md](LICENSE.md) file for details.