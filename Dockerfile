FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY ["src/Bold.Integration.Base.csproj", "/app/src/"]

COPY . /app/
RUN dotnet publish "/app/src/Bold.Integration.Base.csproj" -c Release -o /app/publish /p:UseAppHost=false 

COPY /src/appsettings.json /app/publish/
COPY /src/appsettings.Production.json /app/publish/

CMD ["dotnet", "test"]

FROM  mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update && apt-get install -y openssl
RUN echo "openssl_conf = openssl_init\n\n[openssl_init]\nssl_conf = ssl_config\n\n[ssl_config]\nsystem_default = tls_defaults\n\n[tls_defaults]\nCipherString = @SECLEVEL=2:kEECDH:kRSA:kEDH:kPSK:kDHEPSK:kECDHEPSK:-aDSS:-3DES:!DES:!RC4:!RC2:!IDEA:-SEED:!eNULL:!aNULL:!MD5:-SHA384:-CAMELLIA:-ARIA:-AESCCM8\nCiphersuites = TLS_AES_256_GCM_SHA384:TLS_CHACHA20_POLY1305_SHA256:TLS_AES_128_GCM_SHA256:TLS_AES_128_CCM_SHA256\nMinProtocol = TLSv1.2\n" >> /etc/ssl/openssl.cnf

COPY --from=build /app/publish/ /app/

WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Bold.Integration.Base.dll"]