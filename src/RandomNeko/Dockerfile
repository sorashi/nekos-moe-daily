FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
WORKDIR /app
COPY . ./
RUN dotnet publish RandomNeko -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/runtime:5.0 as runner
WORKDIR /app
COPY --from=builder  /app/publish .
RUN groupadd -g 1000 app \
&& useradd -u 1000 -g app app \
&& mkdir -p /app/data \
&& chown -R app:app /app/data
USER app:app
VOLUME ['/app/data']
WORKDIR /app/data
CMD ../RandomNeko
