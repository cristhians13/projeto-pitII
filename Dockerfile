#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT="Development"
ENV CONECTION_STRING_CATALOGO="Server=localhost;Port=3306;Database=catalogo;User ID=root;Password=senha;Protocol=TCP;Compress=false;Pooling=true;Connection Reset=true;Min Pool Size=10;Max Pool Size=50;Allow user variables=true; sslmode=required;"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Catalogo.csproj", "."]
RUN dotnet restore "./Catalogo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Catalogo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalogo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalogo.dll"]