POST {{HOST}}/auth/reg
Content-Type: application/json
[Options]
very-verbose: true
{
    "login": "CHANGEPASSWORD",
    "info": {
        "firstName": "string",
        "lastName": "string",
        "description": "string",
        "country": "string",
        "photo": "string"
    },
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 201


POST {{HOST}}/auth/login
Content-Type: application/json
[Options]
very-verbose: true
{
    "login": "CHANGEPASSWORD",
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 200
[Captures]
JWT: jsonpath "$['token']"


POST {{HOST}}/auth/password
Authorization: Bearer {{JWT}}
Content-Type: application/json
[Options]
very-verbose: true
{
    "oldPassword": "{{GOOD_PASSWORD}}",
    "newPassword": "beb"
}
HTTP 400


POST {{HOST}}/auth/password
Authorization: Bearer {{JWT}}
Content-Type: application/json
[Options]
very-verbose: true
{
    "oldPassword": "{{GOOD_PASSWORD}}",
    "newPassword": "bebra123ASD"
}
HTTP 200


POST {{HOST}}/auth/login
Content-Type: application/json
[Options]
very-verbose: true
{
    "login": "CHANGEPASSWORD",
    "password": "bebra123ASD"
}
HTTP 200
[Captures]
JWT: jsonpath "$['token']"


