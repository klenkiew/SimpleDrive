FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore
COPY ./*.csproj ./angularapp/
RUN dotnet restore ./angularapp/

# copy everything else and build the app
COPY ./*.cs ./angularapp/
COPY ./*.json ./angularapp/
WORKDIR /app/angularapp
RUN dotnet publish -c Release -o out


FROM node:10.6.0 as node

# install the Angular CLI
ENV NPM_CONFIG_PREFIX /npm-global
ENV PATH="/npm-global/bin:${PATH}"
RUN mkdir /npm-global 
RUN npm install --unsafe-perm -g @angular/cli@1.7.3

# build the angular app
WORKDIR /angular/AngularProject 
COPY ./AngularProject ./
RUN mkdir -p /angular/wwwroot
RUN ng build --output-path=/angular/wwwroot


FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=node /angular/wwwroot ./wwwroot
COPY --from=build /app/angularapp/out ./
ENTRYPOINT ["dotnet", "AngularApplication.dll"]