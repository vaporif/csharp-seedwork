FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app

COPY . .

RUN dotnet publish -c Release examples/graphql-ddd-monolith/ConferencePlanner.Api --output build -r linux-x64 --no-self-contained -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:PublishReadyToRun=true

FROM public.ecr.aws/lambda/dotnet:7
WORKDIR /app

COPY --from=build /app/build /app

ENTRYPOINT ["./ConferencePlanner.Api"]
