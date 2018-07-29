FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore
COPY ./AuthenticationService/*.csproj ./AuthenticationService/
COPY /Libraries ./Libraries
RUN dotnet restore ./AuthenticationService/

# copy everything else and build the app
COPY ./AuthenticationService ./AuthenticationService/
WORKDIR /app/AuthenticationService
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/AuthenticationService/out ./
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]