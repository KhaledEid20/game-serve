FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

WORKDIR /src

COPY *.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet build -c release -o /app/build

RUN dotnet ef database update 

FROM builder AS publish
RUN dotnet publish -c release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app 

COPY --from=publish /app/publish .
EXPOSE 5051

ENTRYPOINT [ "dotnet" , "game-server.dll" ]