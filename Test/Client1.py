import socket
import json
import threading

Host_ip = "10.10.10.10"
Host_port = 5005

ServerIp = "10.10.10.10"
ServerPort = 3030

s =  socket.socket(socket.AF_INET , socket.SOCK_DGRAM)
s.bind((Host_ip , Host_port))

UserInfo = {
    "roomId" : "",
    "playerId" : ""
}

# -------------------------------The packets type---------------------------------------

def joinRequest():
    raw_packet = {
    "_type": 0,
    "roomId": "1df027b8-c7b5-4bb6-a0a4-3c486e7d7a5d",
    "payload": "joinRequest"
    }
    packet = json.dumps(raw_packet).encode()
    s.sendto(packet , (ServerIp , ServerPort))
    print("Join Request packet sent")

# ---------------------------------------------------------------------------------------

def JoinSuccess():
    raw_packet = {
        "_type": 2,
        "roomId": UserInfo["roomId"],
        "playerId" : UserInfo["playerId"],
        "payload": "joinRequest"
    }
    packet = json.dumps(raw_packet).encode()
    s.sendto(packet , (ServerIp , ServerPort))
    print("Join Success packet sent Succefully")


def listen():
    while True:
        data, conn = s.recvfrom(1024)

        print("RAW:", repr(data), "LEN:", len(data))

        if len(data) == 0:
            print("Empty UDP packet")
            continue

        try:
            string = data.decode("utf-8")
            obj = json.loads(string)
            print("Valid JSON:", obj)
            if(obj["_type"] == 1):
                UserInfo["playerId"] = obj["playerId"]
                UserInfo["roomId"] = obj["roomId"]
                JoinSuccess()

        except UnicodeDecodeError:
            print("Not UTF-8 data")

        except json.JSONDecodeError:
            print("Not JSON:", string)

listener_thread = threading.Thread(target=listen, daemon=False)
listener_thread.start()

joinRequest()
