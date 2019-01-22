# Trepub
An ASP.NET Core boilerplate to start REST API develpment. 

## Using the followings:
* .NET Core 2.1
* OAuth2, ResourceOwnerPasswordFlow using IdentityServer4 
* Dapper Micro ORM
* Castle.Core aspect for transactions management
* MySQL & MySQL workbench model
* Quartz Scheduler

## Features:
* Fast & Stable
* Layered
* Independent (from web-api) transactional business layer(BSO)
* Fully applied configurable resource based authorization 
* Independent acceptance test client based on HttpClient

## Development Environment Preparation
* Database:
	* To setup database run the following scripts in order against an instance of MySql DBMS;
		* DataModel/Trepub_Schema.sql
		* DataModel/Trepub_Content.sql
	* The ConnectionString located in appsettings.json should be adjusted accordingly.

	## Test Client
* To run tests provide the test case name in the following command or omit it to run all the tests
	* dotnet bin\Trepub.Web.API.Test.dll run [testCaseName] 
## Application Client
* to run any written task against server type the following command: dotnet bin\Trepub.Web.API.Test.dll runTask [taskName] [params] [-url http://localhost:56224/]

	
## Special Commands
* to stop server from accepting any http requests or resuming it use the following command:
	* dotnet bin\Trepub.Web.API.Test.dll runTask SystemToggleProcessingTakRunner -url http://localhost:56224/
* to run the kestrel on a specific port use the following pattern
	* dotnet bin\Trepub.Web.API.dll --server.urls "http://localhost:5010"
