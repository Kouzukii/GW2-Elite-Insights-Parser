FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS builder

ADD LuckParser.dotnet.sln /build/LuckParser.dotnet.sln
ADD LuckParser /build/LuckParser
WORKDIR /build
RUN dotnet build LuckParser.dotnet.sln -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2

COPY --from=builder /build/LuckParser/bin/Release/netcoreapp2.1/publish /LuckParser

ENTRYPOINT ["dotnet", "/LuckParser/LuckParser.dll"]
