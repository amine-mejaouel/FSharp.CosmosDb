module IntegrationTests

open System
open FSharp.CosmosDb

let private connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")

let cosmosConnection =
    connectionString
    |> Cosmos.fromConnectionString
