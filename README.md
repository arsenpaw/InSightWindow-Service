<h1>InSightWindow Service</h1>

InSightWindow is designed to solve the problem of difficult window access. Our app allows you to operate your windows remotely. You can easily monitor parameters, control the opening and closing, and ensure the security of your windows—all from your device.

This is an API part which allow you to log/sign in and grab/send data to window

⬇️<strong>Installiation</strong>⬇️
1. Pull this repo
2. Install docker
3. Build docker container 
<strong>OR</strong>
1. Go to that url [Docker hub][api-docs]. 
[api-docs]: https://hub.docker.com/repositories/vafelka
3. Pull `windowapi` and `mssql`  docker image
Then:
- Create account with permisions in youre sql env.
- Change connection string in `appsettings.{ENV}.json`
- Run them using docker-compose file (locate in this repo).</br>

📌You can use another sql server but you should configure it manualy.📌
📌The difference that when you downlod it from github you will have and avalibility to source code.📌


📑<strong>[Here is some API docs for developers][api-docs]</strong>.📑<br>

[api-docs]: https://github.com/arsenpaw/InSightWindow-Service/blob/main/API-DOCS.md



