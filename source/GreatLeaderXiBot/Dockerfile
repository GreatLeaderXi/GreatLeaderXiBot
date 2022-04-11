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
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS base
RUN apk add libwbclient
COPY --from=ntlmssp /usr/local/lib/gssntlmssp/gssntlmssp.so /usr/local/lib/gssntlmssp/
COPY mech.ntlmssp.conf /usr/etc/gss/mech.d/

# publish
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0
WORKDIR /source
COPY . .
RUN dotnet publish "GreatLeaderXiBot/GreatLeaderXiBot.csproj" -c $BUILDCONFIG -o /app/publish --runtime alpine-x64 --self-contained true /p:Version=$VERSION /p:PublishTrimmed=true /p:PublishSingleFile=true 

# final
FROM base AS final
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./GreatLeaderXiBot"]