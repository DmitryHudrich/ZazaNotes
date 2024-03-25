POST {{HOST}}/auth/login
{
    "login": "{{MAIN_LOGIN}}",
    "password": "{{GOOD_PASSWORD}}"
}
HTTP 200
[Captures]
JWT: jsonpath "$['data']['token']"

GET {{HOST}}/user
Authorization: Bearer {{JWT}}
HTTP 200


