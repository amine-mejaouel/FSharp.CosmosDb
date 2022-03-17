module DatabaseTests

open Expecto
open FSharp.CosmosDb

let databaseExists databaseName = 
    IntegrationTests.cosmosConnection
    |> Cosmos.database databaseName
    |> Cosmos.checkIfDatabaseExists
    |> Cosmos.execAsync
    
let createDatabaseIfNotExists databaseName =
    IntegrationTests.cosmosConnection
    |> Cosmos.database databaseName
    |> Cosmos.createDatabaseIfNotExists
    |> Cosmos.execAsync
    |> Async.Ignore
    
let deleteDatabase databaseName =
    IntegrationTests.cosmosConnection
    |> Cosmos.database databaseName
    |> Cosmos.deleteDatabase
    |> Cosmos.execAsync
    |> Async.Ignore
    
let deleteDatabaseIfExists databaseName =
    async {
        let! exists = databaseExists databaseName
        if exists then
            do! deleteDatabase databaseName
    }
    
[<Tests>]
let tests =
    testList
        "Database"
        [ testAsync "createDatabaseIfNotExists should create a database if it does not exists" {
        
            //Arrange
            let dataBaseName = "DatabaseToCreate"
            do! deleteDatabaseIfExists dataBaseName
            
            //Act
            do! createDatabaseIfNotExists dataBaseName
            
            //Assert
            let! databaseExists = databaseExists dataBaseName
            Expect.isTrue
                 databaseExists
                 "databaseCreate should create the database" }
        
          testAsync "deleteDatabase should delete the database if it exists" {
            
            //Arrange
            let databaseName = "DatabaseToDelete"
            do! createDatabaseIfNotExists databaseName
            
            //Act
            do! deleteDatabase databaseName
            
            //Assert
            let! databaseExists = databaseExists databaseName
            Expect.isFalse
                 databaseExists
                 "databaseCreate should delete the database" }
         ]

