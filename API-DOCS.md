# 📄 This is an API docs 📄 
## 📌 All endpoinst have that type of routing  `{URL}/api/{CONTROLLER_NAME}/{NANE}` 📌
So there will be written only `{CONTROLLER_NAME}/{ENDPOINT_NAME}` part
### 1. Authorization:
 1. POST `Auth/create` </br>
    Accept JSON in body</br>
```json
  {
    "email": "string@gmail.com",
    "password": "12345678",
    "firstName": "String",
    "lastName": "Intovich"
  }
```
 2. POST `Auth/login`</br>
    Accept JSON in body</br>
```json
   {
    "email": "string",
    "password": "string"
   }
```
   Set tokens in coockies and return token in headers
```json
  "refresh-token": "rS6Ke7WRbC/n7AlvYgFRagsbva1gYIdVzNy3x2K62/rcX6oMpEZj5z6sfxYo/xcbfcptyJV1gg=="
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJlM2U4ZDFlNS1lMzFlLTQ4YWEtMTQzYS0wOGRjOTYyZjJkMjIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3N"
```

 3. POST `Auth/refresh-tokens`</br>
   Accept refresh and JWT tokens in header</br>
   Set new tokens in coockies and return token in headers
```json
  "refresh-token": "rS6Ke7WRbC/n7AlvYgFRagsbva1gYIdVzNy3x2K62/rcX6oMpEZj5z6sfxYo/xcbfcptyJV1gg=="
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJlM2U4ZDFlNS1lMzFlLTQ4YWEtMTQzYS0wOGRjOTYyZjJkMjIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3N"refresh-token":
```

### ❗❗❗ ALL NEXT ENDPOINT NEED JWT TOKEN IN HEADERS TO WORK ❗❗❗
```json
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJlM2U4ZDFlNS1lMzFlLTQ4YWEtMTQzYS0wOGRjOTYyZjJkMjIi"
```
### 2. UserDb
  1. GET/DELETE `UsersDb/concreteUser` - delete or get user.
     
  2. PUT  `UsersDb/concreteUser
    Accept JSON in body</br>
```json
   {
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string"
}
```
  3.POST `UserDb/BindTo` or `UserDb/UnbindFrom`</br>
    Accept target device id in query params
```json
  "deviceId": "guid",
```
### 3. DeviceDb
  1. GET/DELETE `DeviceDb` - delete or get user.
     
  2. POST  `DeviceDb/{id}`</br>
  Accept ID in URL and body json </br>
 ❗`"deviceType"`must be supported by an api because this type represent an object in TPH sql db ❗
```json
{
  "deviceType": "Window"
}
```
  3. POST `DeviceDb/DeviceOfUser` </br>
   Return an list of  user`s devices
```json
  [{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "deviceType": "Window"
  }]
```

### 3. FireBaseTokens  
  1. POST  `FireBaseTokens/{token}`</br>
  Accept TOKEN in URL </br>
 ❗`"{token}"`must be represent and firebase client device id, so it make an subscription to accept and push notofocation❗
