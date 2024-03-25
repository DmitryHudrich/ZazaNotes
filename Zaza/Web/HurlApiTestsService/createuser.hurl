POST {{HOST}}/auth/reg
Content-Type: application/json
[Options]
very-verbose: true
{
    "login": "{{MAIN_LOGIN}}",
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

