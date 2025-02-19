 FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
 WORKDIR /app
 EXPOSE 44311
 FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
 WORKDIR /src
 COPY ["AGROCHEM.csproj", ""]
 RUN dotnet restore "./AGROCHEM.csproj"
 COPY . .
 WORKDIR "/src/."
 RUN dotnet build "AGROCHEM.csproj" -c Release -o /app/build
 FROM build AS publish
 RUN dotnet publish "AGROCHEM.csproj" -c Release -o /app/publish
 FROM base AS final
 WORKDIR /app
 COPY --from=publish /app/publish .
 ENTRYPOINT ["dotnet", "AGROCHEM.dll"]