module DatabaseTests

open System
open Expecto
open FSharp.CosmosDb

let private connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")

let databaseExists databaseName = 
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.checkIfDatabaseExists
    |> Cosmos.execAsync
    |> Async.RunSynchronously
    
let createDatabaseIfNotExists databaseName =
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.createDatabaseIfNotExists
    |> Cosmos.execAsync
    |> Async.Ignore
    
let deleteDatabase databaseName =
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.deleteDatabase
    |> Cosmos.execAsync
    |> Async.Ignore
    
let deleteDatabaseIfExists databaseName =
    if databaseExists databaseName then
        deleteDatabase databaseName
        |> ignore
    
[<Tests>]
let tests =
    testList
        "Database"
        [ testAsync "createDatabaseIfNotExists should create a database if it does not exists" {
        
            //Arrange
            let dataBaseName = "DatabaseToCreate"
            deleteDatabaseIfExists dataBaseName
            
            //Act
            do! createDatabaseIfNotExists dataBaseName
            
            //Assert
            Expect.isTrue
                 (databaseExists dataBaseName)
                 "databaseCreate should create the database" }
        
          testAsync "deleteDatabase should delete the database if it exists" {
            
            //Arrange
            let databaseName = "DatabaseToDelete"
            do! createDatabaseIfNotExists databaseName
            
            //Act
            do! deleteDatabase databaseName
            
            //Assert
            Expect.isFalse
                 (databaseExists databaseName)
                 "databaseCreate should delete the database" }
         ]

