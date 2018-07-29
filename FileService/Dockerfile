FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore
COPY ./FileService/*.csproj ./FileService/
COPY /Libraries ./Libraries
RUN dotnet restore ./FileService/

# copy everything else and build the app
COPY ./FileService ./FileService/
WORKDIR /app/FileService
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/FileService/out .
ENTRYPOINT ["dotnet", "FileService.dll"]