start "AuthenticationService" cmd /K docker-compose ^
				-f .\docker-compose.override.yml ^
				-f .\docker-compose.yml ^
				-p AuthenticationService up -d