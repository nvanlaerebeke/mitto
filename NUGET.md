# NuGet

## Generate Mitto NuGet Package

    cd ./src/Mitto/
    nuget pack Mitto.csproj -IncludeReferencedProjects

    nuget push Mitto.<version>.nupkg <apikey> -Source https://api.nuget.org/v3/index.json


    nuget sources add -name "github" -Source https://nuget.pkg.github.com/nvanlaerebeke/index.json -Username nvanlaerebeke -Password <GIBHUBTOKEN>

    nuget push Mitto.0.0.7.nupkg -Source github