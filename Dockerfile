FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore RSql4Net.sln
RUN dotnet publish -f net5.0 -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV ASPNETCORE_URLS http://+:5001
WORKDIR /app
COPY --from=build-env /app/out .
# Create group / user used for dotnet
RUN groupadd dotnet_users && \
    useradd -ms /bin/bash dotnet_user && \
    chmod 770 /app && \
    chown -R dotnet_user:dotnet_users /app
# force docker to use dotnet user

USER dotnet_user

# export asp.net core api port
EXPOSE 5001
ENTRYPOINT ["dotnet", "RSql4Net.Samples.dll"]