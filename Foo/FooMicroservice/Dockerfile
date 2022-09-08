#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Foo/FooMicroservice/FooMicroservice.csproj", "Foo/FooMicroservice/"]
COPY ["Shared/Shared.Common/Shared.Common.csproj", "Shared/Shared.Common/"]
RUN dotnet restore "Foo/FooMicroservice/FooMicroservice.csproj"
COPY . .
WORKDIR "/src/Foo/FooMicroservice"
RUN dotnet build "FooMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FooMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FooMicroservice.dll"]