set output=%~dp0dist
:Start
pushd AngularBooking\ClientApp
call npm install -g @angular/cli
call npm install
call ng build --prod --output-path %output%\ClientApp
popd
pushd AngularBooking
dotnet restore
dotnet build AngularBooking.csproj -c Release -o %output%
dotnet publish AngularBooking.csproj -c Release -o %output%
popd
