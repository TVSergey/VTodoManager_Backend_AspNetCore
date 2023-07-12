FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vtodo.Web/Vtodo.Web.csproj", "Vtodo.Web/"]
RUN dotnet restore "Vtodo.Web/Vtodo.Web.csproj"
COPY . .
WORKDIR "/src/Vtodo.Web"
RUN dotnet build "Vtodo.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vtodo.Web.csproj" -c Release -o /app/publish

RUN dotnet tool install --global dotnet-ef --version 7.0.8 
ENV PATH="$PATH:/root/.dotnet/tools"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vtodo.Web.dll"]
