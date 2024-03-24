export HURL_HOST=http://localhost:5000

export HURL_LOGIN=bebr123123
export HURL_PASSWORD=bebrik

hurl --glob ./**.hurl --test $1 $2
