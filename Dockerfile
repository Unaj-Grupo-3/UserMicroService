FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR userservice

EXPOSE 7020
EXPOSE 443
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:7020
ENV DOTNET_URLS=http://+:7020

WORKDIR /src
COPY ["Template2/Presentacion.csproj", "Template2/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]

RUN dotnet restore "./Template2/Presentacion.csproj"

COPY . .
WORKDIR "/src/Template2"
RUN dotnet build "Presentacion.csproj" -c Release -o /userservice/build

FROM build AS publish
RUN dotnet publish "Presentacion.csproj" -c Release -o /userservice/publish

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /userservice
COPY --from=publish /userservice/publish .
COPY aspnetapp.pfx /usr/local/share/ca-certificates
COPY aspnetapp.pfx /https/
RUN chmod 644 /usr/local/share/ca-certificates/aspnetapp.pfx && update-ca-certificates
ENTRYPOINT ["dotnet", "Presentacion.dll"]