# C# SeedWork WIP
This is a small [SeedWork](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/seedwork-domain-model-base-classes-interfaces) project.

Provides easy to implement Onion Microservice Domain Driven Design Architecture.

Domain Events are dispatched in process via Mediatr Notifications.

Integration Events are dispatched via MassTransit with Outbox pattern ensuring events are persisted first to sql database before publish to Queue.

Currently there's one simple example showing monolithic app with GraphQl as Api layer.

TODO:
Add Microservice example
Add Custom OpenId Provider via OpenIdDict
Add EventSourcing project to SeedWork
