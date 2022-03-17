module ContainerTests

open Expecto
open FSharp.CosmosDb

let private databaseName = "DatabaseForContainerTests"

let containerExists containerName = 
    IntegrationTests.cosmosConnection
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.checkIfContainerExists
    |> Cosmos.execAsync
    
let createContainerIfNotExists containerName partitionKey =
    IntegrationTests.cosmosConnection
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.createContainerIfNotExists partitionKey
    |> Cosmos.execAsync
    |> Async.Ignore
    
let deleteContainer containerName =
    IntegrationTests.cosmosConnection
    |> Cosmos.database databaseName
    |> Cosmos.container containerName
    |> Cosmos.deleteContainer
    |> Cosmos.execAsync
    |> Async.Ignore
    
let deleteContainerIfExists containerName =
    async {
        let! exists = containerExists containerName
        if exists then
            do! deleteContainer containerName
    }
    
[<Tests>]
let tests =
    testList
        "Container"
        [ testAsync "createContainerIfNotExists should create a container if it does not exists" {
            
            //Arrange
            let containerName = "MyContainer"
            
            do! DatabaseTests.createDatabaseIfNotExists databaseName
            do! deleteContainer containerName
            
            //Act
            do! createContainerIfNotExists containerName "/id"
            
            //Assert
            let! exists = containerExists containerName
            Expect.isTrue
                 exists
                 "createContainer should create the container" }
        
          testAsync "deleteContainer should delete the container" {
            
            //Arrange
            let containerName = "ContainerToDelete"
            
            do! DatabaseTests.createDatabaseIfNotExists databaseName
            
            do! createContainerIfNotExists containerName "/id"
            
            //Act
            do! deleteContainer containerName
            
            //Assert
            let! exists = containerExists containerName
            Expect.isFalse
                 exists
                 "deleteContainer should delete the container" }
          
          testAsync "createContainerIfNotExists should create the container with correct Partition Key" {
            
            //Arrange
            let containerName = "ContainerForPartitionKeyTest"
            let partitionKey = "/myId"
            
            do! DatabaseTests.createDatabaseIfNotExists databaseName
            do! deleteContainerIfExists containerName
            
            //Act
            do! createContainerIfNotExists containerName partitionKey
            
            //Assert
            let! actualPartitionKey =
                async {
                    let! properties =
                        IntegrationTests.cosmosConnection
                        |> Cosmos.database databaseName
                        |> Cosmos.container containerName
                        |> Cosmos.getContainerProperties
                        |> Cosmos.execAsync
                        
                    return properties
                           |> Option.map (function p -> p.PartitionKeyPath)
                }
                
            Expect.equal
                 (actualPartitionKey |> Option.get)
                 partitionKey
                 "createContainer should create the container with correct Partition Key" }
         ]

//TODO: refactor setup method for DatabaseTests and ContainerTests
//TODO: move created helper method used in test into the library code
//TODO: For next step simplify the API, you know each operation is going through many Cosmos.... lines