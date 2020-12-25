FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /opt/jubandistributedservicehost
COPY publish .
ENTRYPOINT ["./JubanDistributedServiceHost"]
