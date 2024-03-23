
# Todo: dodelat

# bebra forever.

POST {{HOST}}/auth/reg 
Content-Type: application/json
{
  "login": "string1",
  "info": {
    "firstName": "string",
    "lastName": "string",
    "description": "string",
    "country": "string",
    "photo": "string"
  },
  "password": "string"
}
HTTP 401

POST {{HOST}}/auth/login
Content-Type: application/json
{
  "login": "string1",
  "password": "string"
}
HTTP 200
[Captures]
JWT: body replace "\"" ""

GET {{HOST}}/user
Authorization: Bearer {{JWT}}
HTTP 200

