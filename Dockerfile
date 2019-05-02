FROM microsoft/dotnet:2.2-sdk

ADD LuckParser.dotnet.sln /build/LuckParser.dotnet.sln
ADD LuckParser /build/LuckParser
WORKDIR /build
RUN dotnet build LuckParser.dotnet.sln -c Release

ENTRYPOINT ["dotnet", "/build/LuckParser/bin/Release/netcoreapp2.1/LuckParser.dll"]