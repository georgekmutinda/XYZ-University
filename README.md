# XYZ University API

## Overview
A scalable student validation API built with ASP.NET Core.
The system uses Redis for caching and RabbitMQ for asynchronous
event-driven processing.

## Architecture
- ASP.NET Core Web API
- Entity Framework Core
- Redis (caching)
- RabbitMQ (message broker)
- JWT Authentication

## Validation Flow
1. Client sends student validation request
2. API checks Redis cache
3. API queries database if cache misses
4. API publishes validation event to RabbitMQ
5. Consumers process events asynchronously
6. API returns response immediately

## Why RabbitMQ?
RabbitMQ is used to decouple side-effects such as auditing,
analytics, and notifications from the API request lifecycle.

## Why Redis?
Redis is used to cache validation results and reduce database load
under high traffic.

## Getting Started
1. Clone the repository
2. Configure environment variables
3. Start Redis
4. Start RabbitMQ
5. Run the API

## Technologies
- .NET
- Entity Framework Core
- Redis
- RabbitMQ
