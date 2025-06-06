# Welcome to Mousebytes! 

This platform is based on ASP.NET WEB API framework in which Angular 18.0 was used in the client side, and .NET 8.0 was used in the server side. Angular is a Typescript framework consisting of HTML templates, TypeScript components, modules, and services. The client 
communicates with the server through the Angular services to call the API endpoints in the server. In the server side, there are API controllers which are used by client services for communicating data, and they handle the business logic of the application along with the services and models. All of these elements are connected to the database (Microsof SQL Server) through the data access layer.

The database schema can be found in file <b>mb_script_schemaOnly.sql</b>.

# Prerequisite for local setup:
- VSCode (For quick launch) / Visual Studio (For additional launch settings)
- Node.js v22
- Microsoft SQL Server 2022 (or newer)
- Microsoft SQL Server Management Studio (for accessing and modifying database)
- .NET 8 SDK

# Installation:
1. Clone git repository (https://github.com/Rodent-Cognition-Core/CBAS.git) to device.
2. Using a terminal application, navigate to project folder and install frontend dependencies using command "npm install"

# Local Testing:

## VSCode:
1. Navigate to Run and Debug
2. Select .NET Core Launch (Web)
3. Application will begin launcher
4. Access the MouseBytes web page with https://staging.mousebytes.ca

## Visual Studio:
1. Select AngularSPAWebAPI_dev and launch
2. Access the MouseBytes web page with https://staging.mousebytes.ca


Please cite this article for any use of this platform: https://doi.org/10.1038/s41597-023-02106-1
