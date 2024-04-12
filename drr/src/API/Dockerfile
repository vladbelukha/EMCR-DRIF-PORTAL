FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . .
RUN dotnet restore ./drr/src/API/EMCR.DRR/EMCR.DRR.csproj

FROM build AS publish
RUN dotnet publish ./drr/src/API/EMCR.DRR/EMCR.DRR.csproj -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "EMCR.DRR.dll"]