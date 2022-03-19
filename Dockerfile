#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DocumentSaver/DocumentSaver.csproj", "DocumentSaver/"]
RUN dotnet restore "DocumentSaver/DocumentSaver.csproj"
COPY . .
WORKDIR "/src/DocumentSaver"
RUN dotnet build "DocumentSaver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentSaver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "DocumentSaver.dll"]