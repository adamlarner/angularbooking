output=`dirname "$0"`/dist
mkdir "$output"
pushd "$output"
output="$PWD"
popd
pushd AngularBooking/ClientApp
npm install -g @angular/cli
npm install
ng build --prod --output-path "$output"/ClientApp
popd
pushd AngularBooking
dotnet restore
dotnet build AngularBooking.csproj -c Release -o "$output"
dotnet publish AngularBooking.csproj -c Release -o "$output"
popd
