#!/bin/bash

IP="10.10.10.10"
PORT="3030"

MESSAGE='{
  "_type": 0,
  "playerId": 1439077254,
  "roomId": "2020",
  "payload": "SEVMTE8="
}'

echo -n "$MESSAGE" | nc -u -p 5002 $IP $PORT