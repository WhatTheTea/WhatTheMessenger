# ==========================================
# Stage 1: Build & Publish
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef

# Restore Dependencies 
COPY ["/WhatTheMessenger.Api/WhatTheMessenger.Api.csproj", "WhatTheMessenger.Api/"]
COPY ["/WhatTheMessenger.Core/WhatTheMessenger.Core.csproj", "WhatTheMessenger.Core/"]
COPY ["/WhatTheMessenger.Infrastructure/WhatTheMessenger.Infrastructure.csproj", "WhatTheMessenger.Infrastructure/"]
RUN dotnet restore "WhatTheMessenger.Api/WhatTheMessenger.Api.csproj"


# Generate EF bundle (Cached UNLESS files in Core or Infrastructure change!)
COPY ["/WhatTheMessenger.Core/", "WhatTheMessenger.Core/"]
COPY ["/WhatTheMessenger.Infrastructure/", "WhatTheMessenger.Infrastructure/"]
RUN dotnet ef migrations bundle \
    --project WhatTheMessenger.Infrastructure \
    --startup-project WhatTheMessenger.Infrastructure \
    -o /app/publish/efbundle

COPY ["/WhatTheMessenger.Api/", "WhatTheMessenger.Api/"]
RUN dotnet publish "WhatTheMessenger.Api/WhatTheMessenger.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

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