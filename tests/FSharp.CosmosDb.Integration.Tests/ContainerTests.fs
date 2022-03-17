module ContainerTests

open Expecto
open FSharp.CosmosDb

let private databaseName = "DatabaseForContainerTests"

let containerExists containerName = 
    Common.connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.checkIfContainerExists
    |> Cosmos.execAsync
    |> Async.RunSynchronously
    
let createContainerIfNotExists containerName =
    Common.connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.createContainerIfNotExists
    |> Cosmos.execAsync
    |> Async.Ignore
    
[<Tests>]
let tests =
    testList
        "Container"
        [ testAsync "createContainer should create a container if it does not exists" {
            let containerName = "MyContainer"
            
            do! DatabaseTests.createDatabaseIfNotExists databaseName
            
            do! Common.connectionString
                |> Cosmos.fromConnectionString
                |> Cosmos.database databaseName
                |> Cosmos.container containerName
                |> Cosmos.createContainerIfNotExists
                |> Cosmos.execAsync
                |> Async.Ignore
            
            Expect.isTrue
                 (containerExists containerName)
                 "createContainer should create the container" }
        
          testAsync "deleteContainer should delete the container" {
            let containerName = "ContainerToDelete"
            
            DatabaseTests.createDatabaseIfNotExists databaseName
            |> ignore
            
            do! createContainerIfNotExists containerName
            
            do! Common.connectionString
                |> Cosmos.fromConnectionString
                |> Cosmos.database databaseName
                |> Cosmos.container containerName
                |> Cosmos.deleteContainer
                |> Cosmos.execAsync
                |> Async.Ignore
            
            Expect.isFalse
                 (containerExists containerName)
                 "deleteContainer should delete the container" }
         ]

//TODO: refactor
//TODO: deal with /id
