module ContainerTests

open System
open Expecto
open FSharp.CosmosDb

let private connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")
let private databaseName = "DatabaseForContainerTests"

let containerExists containerName = 
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.checkIfContainerExists
    |> Cosmos.execAsync
    |> Async.RunSynchronously
    
let createContainerIfNotExists containerName =
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.createContainerIfNotExists
    |> Cosmos.execAsync
    |> Async.RunSynchronously
    |> ignore
    
[<Tests>]
let tests =
    testList
        "Container"
        [ test "createContainer should create a container if it does not exists" {
            let containerName = "MyContainer"
            
            DatabaseTests.createDatabaseIfNotExists databaseName
            |> ignore
            
            connectionString
            |> Cosmos.fromConnectionString
            |> Cosmos.database databaseName
            |> Cosmos.container containerName
            |> Cosmos.createContainerIfNotExists
            |> Cosmos.execAsync
            |> Async.RunSynchronously
            |> ignore
            
            Expect.isTrue
                 (containerExists containerName)
                 "createContainer should create the container" }
        
          test "deleteContainer should delete the container" {
            let containerName = "ContainerToDelete"
            
            DatabaseTests.createDatabaseIfNotExists databaseName
            |> ignore
            
            createContainerIfNotExists containerName
            |> ignore
            
            connectionString
            |> Cosmos.fromConnectionString
            |> Cosmos.database databaseName
            |> Cosmos.container containerName
            |> Cosmos.deleteContainer
            |> Cosmos.execAsync
            |> Async.RunSynchronously
            |> ignore
            
            Expect.isFalse
                 (containerExists containerName)
                 "deleteContainer should delete the container" }
         ]

//TODO: refactor
//TODO: deal with /id
