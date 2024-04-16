# version 17.1.0 introduced breaking changes to RUN npm install, try change 17.0.10 tag to latest on subsequent versions
FROM trion/ng-cli-karma:17.3.3 AS ng-builder

COPY ./drr/src/UI ./drr/src/UI

WORKDIR /app/drr/src/UI

RUN npm install 

RUN npm run build --configuration=production

FROM caddy:latest as final
COPY ./drr/src/UI/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/drr/src/UI/dist /site
ENV API_URL=
EXPOSE 2015

USER 1001