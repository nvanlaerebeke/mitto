# NuGet

## Generate Mitto NuGet Package

    cd ./src/Mitto/
    nuget pack Mitto.csproj -IncludeReferencedProjects

    nuget push Mitto.<version>.nupkg <apikey> -Source https://api.nuget.org/v3/index.json