module samples.createDeleteDatabaseSample

open FSharp.CosmosDb

let connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=..."

let createDatabase () =
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database "MyDatabase"
    |> Cosmos.createDatabase
    |> Cosmos.execAsync
    |> Async.RunSynchronously

let deleteDatabase () =
    connectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database "MyDatabase"
    |> Cosmos.deleteDatabase
    |> Cosmos.execAsync
    |> Async.RunSynchronously
