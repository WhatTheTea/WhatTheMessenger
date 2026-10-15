# ==========================================
# Stage 1: Build Angular App
# ==========================================
FROM oven/bun:alpine AS build
WORKDIR /src

# Copy package files first for npm caching
COPY package.json bun.lock ./
RUN bun install --frozen-lockfile

COPY . .
RUN bun run build --configuration production

# ==========================================
# Stage 2: Serve with Nginx
# ==========================================
FROM nginx:alpine AS final

COPY --from=build /src/dist/client/browser /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]