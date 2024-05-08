FROM trion/ng-cli-karma:17.3.3 AS node-builder
COPY . ./drr/src/UI
WORKDIR /app/drr/src/UI

RUN npm install --ignore-scripts

FROM node-builder as ng-builder
RUN npm run build --configuration=production

FROM caddy:2-builder AS builder
RUN xcaddy build --with github.com/caddyserver/replace-response

FROM caddy:2 as final
ARG VERSION
ENV VERSION=$VERSION
ENV API_URL=
RUN chmod +rw /config/caddy
COPY --from=builder /usr/bin/caddy /usr/bin/caddy
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/drr/src/UI/dist/drr/browser /site
EXPOSE 2015
