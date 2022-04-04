FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimpleBoard/SimpleBoard.csproj", "SimpleBoard/"]
RUN dotnet restore "SimpleBoard/SimpleBoard.csproj"
COPY . .
WORKDIR "/src/SimpleBoard"
RUN dotnet build "SimpleBoard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleBoard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleBoard.dll"]
