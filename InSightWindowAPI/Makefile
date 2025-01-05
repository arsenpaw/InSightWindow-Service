dockerUp:
	DATABASE_PASSWORD=MSSQL_root docker-compose up -d --build
dockerDown:
	docker-compose down
migrationAdd:
	cd ./Infrastructure && dotnet ef migrations add --startup-project ../API $(filter-out $@,$(MAKECMDGOALS)) && cd ../
# Remove last migration
migrationRemove:
	cd ./Infrastructure && dotnet ef migrations remove --startup-project ../API
updateDatabase:
	cd ./Infrastructure && dotnet ef database update $(filter-out $@,$(MAKECMDGOALS)) --startup-project ../API
%:
	@: # ignore params after the action