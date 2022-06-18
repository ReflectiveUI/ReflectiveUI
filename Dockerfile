FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
RUN apt-get update && apt-get install -y dos2unix
COPY /build/nginx.conf /build/nginx.conf
RUN dos2unix /build/nginx.conf

WORKDIR /ValuedTime
COPY /src/ .
RUN dotnet restore "ValuedTime.sln"
RUN dotnet build "ValuedTime.sln" -c Release

FROM build-env as tests
CMD ["dotnet", "test", "--logger:trx"]

FROM build-env AS publish
RUN dotnet publish "ValuedTime.WebClient/ValuedTime.WebClient.csproj" -c Release -o /publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /publish/wwwroot /usr/local/webapp/nginx/html
COPY --from=publish /build/nginx.conf /etc/nginx/nginx.conf