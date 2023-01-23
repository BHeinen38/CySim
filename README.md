# CySim

This web app will have three different types of users: Admin, Red Team and Blue Team.

When you are using a Red Team you will be known as an attacking user. 

When you are using the Blue Team role you will be known as a defending user. 

Admin are administrators and will be presented their own respected view.


## Docker (On Linux)
### Installation

Run the following to generate a password for the SQL server:
```
echo "MSSQL_PASSWD=$(openssl rand -base64 24)" >> .env
```
*Note: Changing this value and restarting the services in docker will prevent access to the database*


To run in Development mode, append to the .env file:
```
echo "ENV=Development" >> .env
```

Next, build the CySim .NET application:
```
docker compose build
```
*Note: This is also needed to be done to test code changes using docker*


### Starting Services

To start the code in docker run this command:
```
docker compose up
```

This will start the MSSQL and .NET applications/services.
They are defined within the docker-compose.yml file.


### Initializing MSSQL

The docker-compose.yml file has an additional service that runs with `docker compose up`.
It's labeled migrate-data and all it does is set up the database to ensure it has the correct tables.


### Ending Services

To stop the docker services run:
```
docker compose down
```

As well, the MSSQL has a volume created for persistence.
This can be delete by doing the following:
```
docker compose down --volumes
```
If you lock yourself out of the database, this will reset it losing all the data in the process.

