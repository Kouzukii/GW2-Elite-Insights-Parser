FROM microsoft/dotnet:2.1-sdk

ADD LuckParser.sln /build/LuckParser.sln
ADD LuckParser /build/LuckParser
WORKDIR /build
RUN dotnet build -c Release

ENTRYPOINT ["dotnet", "/build/LuckParser/bin/Release/netcoreapp2.1/LuckParser.dll"]