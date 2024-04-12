# version 17.1.0 introduced breaking changes to RUN npm install, try change 17.0.10 tag to latest on subsequent versions
FROM trion/ng-cli-karma:17.3.3 AS ng-builder

COPY ./ui ./ui

WORKDIR /app/ui

RUN npm install 

RUN npm run build 

FROM caddy:latest as final
COPY ./ui/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/ui/dist/drif /site
RUN ls /site
ENV API_URL=
EXPOSE 2015

USER 1001