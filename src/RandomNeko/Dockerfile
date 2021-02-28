FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
COPY . .
CMD dotnet publish RandomNeko -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/runtime:5.0 as runner
COPY --from=builder  /publish/ ./
WORKDIR data
CMD ["dotnet", "../RandomNeko.dll"]