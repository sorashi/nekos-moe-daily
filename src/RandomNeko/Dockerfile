FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
WORKDIR /app

# Restore as distinct layers
COPY RandomNeko/RandomNeko.csproj ./RandomNeko/
RUN  dotnet restore RandomNeko/RandomNeko.csproj

# Copy everything and build
COPY . ./
RUN dotnet publish RandomNeko -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/runtime:5.0 as runner
WORKDIR /app
COPY --from=builder  /app/publish .
RUN groupadd -g 1001 app \
&& useradd -u 1001 -g app app \
&& mkdir -p /app/data \
&& chown -R app:app /app
USER app:app
WORKDIR /app/data
CMD ../RandomNeko
