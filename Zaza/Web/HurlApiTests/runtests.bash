export HURL_HOST=http://localhost:5000

export HURL_LOGIN=bebr123123211qweqwe
export HURL_BAD_PASSWORD=bebrik
export HURL_GOOD_PASSWORD=bebr123PENIS

hurl --glob ./**.hurl --test $1 $2
