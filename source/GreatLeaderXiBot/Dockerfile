# build gss-ntlmssp
FROM alpine as ntlmssp
RUN apk add --no-cache git curl
RUN apk add --no-cache make m4 autoconf automake gcc g++ krb5-dev openssl-dev gettext-dev libtool libxml2 libxslt libunistring-dev zlib-dev samba-dev
RUN git clone https://github.com/gssapi/gss-ntlmssp
WORKDIR gss-ntlmssp
RUN autoreconf -f -i
RUN ./configure --without-manpages --disable-nls
RUN make install

# base
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
COPY --from=ntlmssp /usr/local/lib/gssntlmssp/gssntlmssp.so /usr/local/lib/gssntlmssp/

# build
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /source
COPY ["GreatLeaderXiBot/GreatLeaderXiBot.csproj", "GreatLeaderXiBot/"]
COPY ["GreatLeaderXiBot.Domain/GreatLeaderXiBot.Domain.csproj", "GreatLeaderXiBot.Domain/"]
COPY ["GreatLeaderXiBot.Common/GreatLeaderXiBot.Common.csproj", "GreatLeaderXiBot.Common/"]
COPY ["GreatLeaderXiBot.Domain.Core/GreatLeaderXiBot.Domain.Core.csproj", "GreatLeaderXiBot.Domain.Core/"]
COPY ["GreatLeaderXiBot.Infrastructure/GreatLeaderXiBot.Infrastructure.csproj", "GreatLeaderXiBot.Infrastructure/"]
COPY ["GreatLeaderXiBot.Domain.Telegram/GreatLeaderXiBot.Domain.Telegram.csproj", "GreatLeaderXiBot.Domain.Telegram/"]
RUN dotnet restore "GreatLeaderXiBot/GreatLeaderXiBot.csproj"
COPY . .
WORKDIR /source/GreatLeaderXiBot
RUN dotnet build "GreatLeaderXiBot.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "GreatLeaderXiBot.csproj" -c Release -o /app/publish --runtime alpine-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true 

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GreatLeaderXiBot.dll"]