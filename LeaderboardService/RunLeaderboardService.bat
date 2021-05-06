start "LeaderboardService" cmd /K docker-compose ^
				-f .\docker-compose.override.yml ^
				-f .\docker-compose.yml ^
				-p LeaderboardService up -d