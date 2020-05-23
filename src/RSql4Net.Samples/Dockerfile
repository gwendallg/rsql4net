FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Autumn.Mvc.Samples/Autumn.Mvc.Samples.csproj Autumn.Mvc.Samples/
COPY Autumn.Mvc/RSql4Net.csproj Autumn.Mvc/
RUN dotnet restore Autumn.Mvc.Samples/Autumn.Mvc.Samples.csproj
COPY . .
WORKDIR /src/Autumn.Mvc.Samples
RUN dotnet build Autumn.Mvc.Samples.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Autumn.Mvc.Samples.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Autumn.Mvc.Samples.dll"]
