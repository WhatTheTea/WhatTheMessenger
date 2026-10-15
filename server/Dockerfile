# ==========================================
# Stage 1: Build & Publish
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first to leverage Docker layer caching for NuGet restore
COPY ["/WhatTheMessenger.Api/WhatTheMessenger.Api.csproj", "WhatTheMessenger.Api/"]
COPY ["/WhatTheMessenger.Core/WhatTheMessenger.Core.csproj", "WhatTheMessenger.Core/"]
COPY ["/WhatTheMessenger.Infrastructure/WhatTheMessenger.Infrastructure.csproj", "WhatTheMessenger.Infrastructure/"]

RUN dotnet restore "WhatTheMessenger.Api/WhatTheMessenger.Api.csproj"

COPY / /src/

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Stage 2: Runtime Image
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

USER app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WhatTheMessenger.Api.dll"]