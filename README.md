# AppEncryption
R&amp;D repo for microservice encryption

run docker compose 
docker-compose -f docker-compose.dev.yml up -d

http://localhost:8200

token => my-root-token

http://localhost:8200/ui/vault/settings/mount-secret-backend

add new "transit" Secrets Engine and create new key with type "aes256-gcm96"
