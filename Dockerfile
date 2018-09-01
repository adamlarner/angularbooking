FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5000

FROM node AS ngBuild
WORKDIR /ClientApp
RUN npm i -g @angular/cli
COPY AngularBooking/ClientApp/ .
RUN npm install && ng build --prod

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY AngularBooking/AngularBooking.csproj AngularBooking/
RUN dotnet restore AngularBooking/AngularBooking.csproj
COPY . .
WORKDIR /src/AngularBooking
RUN dotnet build AngularBooking.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AngularBooking.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=ngBuild /ClientApp/dist ClientApp/dist/
ENTRYPOINT ["dotnet", "AngularBooking.dll"]
