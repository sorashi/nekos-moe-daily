FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
COPY . .
CMD ["dotnet", "publish", "-c Release"]

FROM mcr.microsoft.com/dotnet/runtime:5.0 as runner
COPY --from=builder  /RandomNeko/bin/Release/net5.0/publish/ ./
WORKDIR data
CMD ["dotnet", "../RandomNeko.dll"]