# version 17.1.0 introduced breaking changes to RUN npm install, try change 17.0.10 tag to latest on subsequent versions
FROM trion/ng-cli-karma:17.3.3 AS node-builder
WORKDIR /src
COPY ./package*.json ./
RUN npm install --ignore-scripts
COPY . .

FROM node-builder as ng-builder
RUN npm run build -- --configuration production

FROM caddy:2-builder AS builder
RUN xcaddy build --with github.com/caddyserver/replace-response

FROM caddy:2 as final
ARG VERSION
ENV VERSION=$VERSION
ENV API_URL=
RUN chmod +rw /config/caddy
COPY --from=builder /usr/bin/caddy /usr/bin/caddy
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/embc-registrant/ /site
EXPOSE 2015

USER 1001