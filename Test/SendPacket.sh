#!/bin/bash

IP="10.10.10.10"
PORT="3030"

MESSAGE='{
  "_type": 0,
  "playerId": 1111,
  "roomId": "2020",
  "payload": "SEVMTE8="
}'

while [ true ]; do
    echo -n "$MESSAGE" | nc -u -w1 $IP $PORT
done