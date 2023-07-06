FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vtodo.Web/Vtodo.Web.csproj", "Vtodo.Web/"]
RUN dotnet restore "Vtodo.Web/Vtodo.Web.csproj"
COPY . .
WORKDIR "/src/Vtodo.Web"
RUN dotnet build "Vtodo.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vtodo.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vtodo.Web.dll"]
