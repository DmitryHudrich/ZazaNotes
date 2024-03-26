POST {{HOST}}/auth/reg
Content-Type: application/json
[Options]
very-verbose: false
{
    "login": "BEBRUN123",
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
{
    "login": "BEBRUN123",
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 200
[Captures]
JWT: jsonpath "$['token']"


DELETE {{HOST}}/user
Authorization: Bearer {{JWT}}
HTTP 204


DELETE {{HOST}}/user
Authorization: Bearer {{JWT}}
[Options]
HTTP 400

