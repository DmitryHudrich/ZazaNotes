export HURL_HOST=http://localhost:5000

export HURL_LOGIN=bebr123123211qweqwe
export HURL_BAD_PASSWORD=bebrik
export HURL_GOOD_PASSWORD=bebr123PENIS
export HURL_MAIN_LOGIN=bebraKPSS

hurl ./flushdb.hurl
hurl ./createuser.hurl
hurl --glob ./main/**.hurl --test $1 $2
