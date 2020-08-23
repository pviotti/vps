
up:
	docker-compose -d up

log:
	docker-compose logs

down:
	docker-compose down

caddy-reload:
	docker exec caddy caddy reload --config /etc/caddy/Caddyfile --adapter caddyfile

caddy-restart:
	docker caddy restart

rm:
	docker-compose rm 
	#sudo rm -rf data/* logs/* config/*

reload-caddy:
	docker exec -w /etc/caddy caddy caddy reload