POST {{HOST}}/auth/reg
Content-Type: application/json
[Options]
very-verbose: false
{
    "login": "{{LOGIN}}",
    "info": {
        "firstName": "string",
        "lastName": "string",
        "description": "string",
        "country": "string",
        "photo": "string"
    },
    "password": "{{BAD_PASSWORD}}"
}
HTTP 400


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
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 201


POST {{HOST}}/auth/login
Content-Type: application/json
{
    "login": "{{LOGIN}}",
    "password": "{{BAD_PASSWORD}}"
}
HTTP 401


POST {{HOST}}/auth/login
Content-Type: application/json
[Options]
very-verbose: true
{
    "login": "{{LOGIN}}",
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 200
[Captures]
JWT: jsonpath "$['data']['token']"


DELETE {{HOST}}/user
Authorization: Bearer {{JWT}}
HTTP 204
