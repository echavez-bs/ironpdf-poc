FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# install necessary packages
RUN apt-get update \
    && apt-get install -y \
        libxkbcommon-x11-0 \
        libc6 \
        libc6-dev \
        libgtk2.0-0 \
        libnss3 \
        libatk-bridge2.0-0 \
        libx11-xcb1 \
        libxcb-dri3-0 \
        libdrm-common \
        libgbm1 \
        libasound2 \
        libxrender1 \
        libfontconfig1 \
        libxshmfence1 \
        libgdiplus \
        libva-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["PocIronPdfWord/PocIronPdfWord.csproj", "PocIronPdfWord/"]
RUN dotnet restore "PocIronPdfWord/PocIronPdfWord.csproj"
COPY . .
WORKDIR "/src/PocIronPdfWord"
RUN dotnet build "PocIronPdfWord.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PocIronPdfWord.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Verifica que los recursos existen
RUN ls -la /app/publish/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PocIronPdfWord.dll"]
