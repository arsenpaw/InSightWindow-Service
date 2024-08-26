<h1>InSightWindow Service</h1>

InSightWindow is designed to solve the problem of difficult window access. Our app allows you to operate your windows remotely. You can easily monitor parameters, control the opening and closing, and ensure the security of your windows—all from your device.

This is an API part which allow you to log/sign in, grab data from window or send it , subscribe on push nitification and etc.

⬇️<strong>Installiation</strong>⬇️
1. Pull this repo
2. Install docker
3. Build docker container </br>

<strong>OR</strong>

1. Go to [docker hub][docker]
2. Pull `windowapi` and `mssql`  docker image</br>

<strong>Then:</strong>
- Create account with permisions in youre sql env.
- Change connection string in `appsettings.{ENV}.json`
- Run them using docker-compose file (locate in this repo).</br>

📌You can use another sql server but you should configure it manualy.📌</br>

📑<strong>[Here is some API docs for developers][api-docs]</strong>.📑<br>

[api-docs]: https://github.com/arsenpaw/InSightWindow-Service/blob/main/API-DOCS.md
[docker]: https://hub.docker.com/repositories/vafelka


