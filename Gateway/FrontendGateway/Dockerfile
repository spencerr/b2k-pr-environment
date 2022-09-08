#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gateway/FrontendGateway/FrontendGateway.csproj", "Gateway/FrontendGateway/"]
COPY ["Bar/BarMicroservice.Client/BarMicroservice.Client.csproj", "Bar/BarMicroservice.Client/"]
COPY ["Shared/Shared.Common/Shared.Common.csproj", "Shared/Shared.Common/"]
COPY ["Shared/Shared.SourceGenerators/Shared.SourceGenerators.csproj", "Shared/Shared.SourceGenerators/"]
COPY ["Foo/FooMicroservice.Client/FooMicroservice.Client.csproj", "Foo/FooMicroservice.Client/"]
RUN dotnet restore "Gateway/FrontendGateway/FrontendGateway.csproj"
COPY . .
WORKDIR "/src/Gateway/FrontendGateway"
RUN dotnet build "FrontendGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FrontendGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontendGateway.dll"]