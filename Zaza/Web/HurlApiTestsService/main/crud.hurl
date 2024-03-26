POST {{HOST}}/auth/login
[Options]
very-verbose: false
{
    "login": "{{MAIN_LOGIN}}",
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 200
[Captures]
JWT: jsonpath "$['token']"

GET {{HOST}}/user
Authorization: Bearer {{JWT}}
HTTP 200


