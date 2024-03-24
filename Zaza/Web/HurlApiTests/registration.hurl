POST {{HOST}}/testing/flush
HTTP 202


POST {{HOST}}/auth/reg
Content-Type: application/json
[Options]
very-verbose: true
{
    "login": "{{LOGIN}}",
    "info": {
        "firstName": "string",
        "lastName": "string",
        "description": "string",
        "country": "string",
        "photo": "string"
    },
    "password": "{{PASSWORD}}"
}
HTTP 201

POST {{HOST}}/auth/login
Content-Type: application/json
{
    "login": "{{LOGIN}}",
    "password": "{{PASSWORD}}"
}
HTTP 200
[Captures]
JWT: body replace "\"" ""


DELETE {{HOST}}/user
Authorization: Bearer {{JWT}}
HTTP 204
