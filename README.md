# AngularBooking

AngularBooking is a booking/reservation application comprising of Angular 6 and ASP.NET Core 2.1, that includes basic functionality for viewing, creating, and managing bookings. The system is intended to be either extended or used in part for more bespoke projects, and so only contains light implementations for services such as user management and authentication.

<a href="https://adamlarner.co.uk/images/angularbooking/1.jpg"><img src="https://adamlarner.co.uk/images/angularbooking/1.jpg" width=400 alt="image 1" /></a>&nbsp;&nbsp;&nbsp;
<a href="https://adamlarner.co.uk/images/angularbooking/2.jpg"><img src="https://adamlarner.co.uk/images/angularbooking/2.jpg" width=400 alt="image 2" /></a>&nbsp;&nbsp;&nbsp;
<a href="https://adamlarner.co.uk/images/angularbooking/3.jpg"><img src="https://adamlarner.co.uk/images/angularbooking/3.jpg" width=400 alt="image 3" /></a>&nbsp;&nbsp;&nbsp;
<a href="https://adamlarner.co.uk/images/angularbooking/4.jpg"><img src="https://adamlarner.co.uk/images/angularbooking/4.jpg" width=400 alt="image 4" /></a>&nbsp;&nbsp;&nbsp;

This application references the following 3rd party Angular libraries:
- [ng-bootstrap](https://ng-bootstrap.github.io)
- [primeng](https://www.primefaces.org/primeng)
- [ng4-loading-spinner](https://www.npmjs.com/package/ng4-loading-spinner)

## Features!

  - Live listings of available showings, filtered by date and event/venue. 
  - Venues consist of rooms, where floor layouts can be configured. This includes defining the number of columns/rows of available seating (locations), as well as the addition of isles to provide visual separation of locations.
  - Admin module for managing all aspects of the booking system. This includes the addition of events and venues, the creation of new showings, and also the viewing/editing of existing bookings.
  - Pricing strategies, which provide a simple way of configuring admission pricing for different groups of people.
  - Booking module, which includes a simple seat allocation component for selecting available seating before completing the booking. Selection relies soley on clicking/tapping available seats to toggle the type of admission that you desire.
  - Simple JWT authentication system, which includes a memory-based token revocation service for invalidating tokens on logout. This service provides simple user login/logout, and leverages the use of cookies for token storage. XRSF protection is also included, and automatically handled within Angular using an HttpInterceptor.
  - Simple email service, which provides email verification and booking confirmation services. SMTP server information can be pre-configured prior to starting the service through the use of environment variables.


## Installation

AngularBooking requires the [ASP.NET Core SDK](https://www.microsoft.com/net/download) (minimum 2.1) and [Node.js](https://nodejs.org/) to build, and requires the [ASP.NET Core Runtime](https://www.microsoft.com/net/download) (minimum v2.1) to run.

Prior to installation the Angular CLI needs to be installed, the packages for the Angular client application need to be restored, and a production build of the application created (replace \*output\* with desired output directory path for application):


```sh
$ cd angularbooking/AngularBooking/ClientApp
$ npm install -g @angular/cli
$ npm install && ng build --prod --output-path *output*/ClientApp
```

The ASP.NET Core project then needs to be built and published:

```sh
$ cd ..
$ dotnet restore 
$ dotnet build AngularBooking.csproj -c Release -o *output*
$ dotnet publish AngularBooking.csproj -c Release -o *output*
```

You can also use the provided sh/bat files, which place the published application within a folder named "dist".

Then simply run the application:

```sh
$ dotnet AngularBooking.dll
```

## Docker

Launching the application in Docker is quick and easy, since the multi-stage dockerfile does most of the heavy lifting.

You can build the image from the project directory:

```sh
$ cd angularbooking
$ docker build -t angularbooking .
```

You can also build directly from the git repository:

```sh
docker build -t angularbooking https://github.com/adamlarner/angularbooking.git#master
```

To run the image (mapping container port 5000 to host port 80):

```sh
docker run -p 80:5000 angularbooking:latest
```

You should now have access to the application at http://docker_ip_here:port.

There are a number of environment variables that can be modified, including one which enables the demo mode. This will clear the existing database, and populate it with seed data for demonstration purposes. To modify these you can directly edit the dockerfile, or set the environment variables within the docker run command. For example, to enable demo mode:

```sh
docker run -p 80:5000 -e BOOKING_DEMO="true" angularbooking:latest
```

## Environment Variables

#### Security

|Name|Action|Default|
|----|------|-------|
|JWT_ISSUER|JWT authentication issuer claim|localhost|
|JWT_AUDIENCE|JWT authentication audience claim|localhost|
|JWT_KEY|JWT key secret, used in the process of generating secure JWT tokens|\$\$Secret_Test_Key_Here$$|
#### Demo
|Name|Action|Default|
|----|------|-------|
|BOOKING_DEMO|Enables demo mode|false|
|BOOKING_DEMO_USER|Email/username for admin user|admin@angularbookingdemo.com|
|BOOKING_DEMO_PASSWORD|Password for admin user (note: needs to comply with default password complexity)|Password.01|
|BOOKING_DEMO_SEED|Parent for seed settings, and can be ignored|N/A|
|BOOKING_DEMO_SEED__DAYS|Generates data for this number of days|7|
|BOOKING_DEMO_SEED__ROOMS|Generates data for this number of rooms, per venue|5|
|BOOKING_DEMO_SEED__MIN_ROOM_COLUMNS|Minimum number of seating columns possible when generating room layout|6|
|BOOKING_DEMO_SEED__MAX_ROOM_COLUMNS|Maximum number of seating columns possible when generating room layout|12|
|BOOKING_DEMO_SEED__MIN_ROOM_ROWS|Minimum number of seating rows possible when generating room layout|6|
|BOOKING_DEMO_SEED__MAX_ROOM_ROWS|Maximum number of seating rows possible when generating room layout|12|

#### Email
|Name|Action|Default|
|----|------|-------|
|BOOKING_SMTP|Semi-colon delimited string containing connection details for an SMTP server. If not configured, email registration confirmation is disabled. The string format is as follows: host;port;username;password;fromAddress(optional).|Undefined|

## Additional Information

A socket exception is thrown from within the ASP.NET Core SPA development server whenever the service is stopped from the console window (ctrl+c). It appears to be a proxy issue, and I suspect will be/is resolved in a later version of ASP NET Core.

There are a ton of optimizations that can be made (specifically around client-side caching/invalidation), but these are generally per-application implementations based upon how often the client needs updated information. The client-side services can easily be modified to include caching if so desired.

HTTPS support is not enabled within this project by default. If you need it, you can either enable it in code and rebuild the source, or host the application behind a reverse proxy.

There is a test project included which I used for the TDD aspect of the ASP.NET Core service, as well as Jasmine/Karma spec files that I used for the development of the Angular client application. These aren't needed for the running of the application, however I've left them in the event that anyone finds them useful/insightful. There are examples of mock testing for both the client and server, as well as plenty of examples utilising the ASP.NET in-memory test server.

And finally, I started the server implementation by using the "Unit Of Work" pattern, mainly because I had originally planned on abstracting the data access layer, but also since it meant that I could write the test cases without having to mock large parts of DbContext. Of course Entity Framework already utilises this pattern, so if anyone wants to rip it out and rewrite the tests then you're welcome to do so.

Hopefully you find some part of this project useful! ^_^

## License
MIT
