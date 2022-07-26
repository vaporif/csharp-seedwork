# C# SeedWork WIP
This is a Collection [SeedWork](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/seedwork-domain-model-base-classes-interfaces) projects.

Provides easy to implement Onion Microservice Domain Driven Design Architecture.

Domain Events are dispatched in process via Mediatr Notifications.

Integration Events are dispatched via MassTransit with Outbox pattern ensuring events are persisted first to sql database before publish to Queue.

Currently there's one simple example showing monolithic app with GraphQl HotChocolate as Api layer.

As this is a cutout refactoring of existing project in production it's currently under construction.

TODO:
Add Microservice example
Add Custom OpenId Provider via OpenIdDict
Add EventSourcing project to SeedWork
