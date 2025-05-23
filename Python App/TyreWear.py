import ac
import acsys
from sim_info import info
import os
import sys
import platform
import socket
import json
import traceback
import struct
import idna.core
import platform

try:
    if platform.architecture()[0] == "64bit":
        sysdir = "stdlib64"
    else:
        sysdir = "stdlib"
    sys.path.insert(len(sys.path), "apps/python/third_party")
    os.environ["PATH"] += ";."
    sys.path.insert(len(sys.path), os.path.join("apps/python/third_party", sysdir))
    os.environ["PATH"] += ";."
except Exception as e:
    a = 1
    # ac.log("[ERROR] Error importing libraries: %s" % e)


sock = None
host = "127.0.0.1"
port = 9999


tick = 0
previous_distance = 0  # 初始化上一次的 distance
previous_lap = 0
previous_lap_distance = 0
previous_pitline = 0
previous_pitbox = 0


def acMain(ac_version):
    global sock, port
    app_window = ac.newApp("Data Sender")
    ac.setSize(app_window, 200, 100)

    # 初始化 UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.setblocking(False)  # 防止卡顿

    return "TyreWear"


def acUpdate(deltaT):
    global sock, first, port, host

    # 获取数据（举个例子：车速）

    check_pit()
    check_distance()
    check_lap()


def socketInit(sock):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.setblocking(False)  # 防止卡顿


def check_port(host, port):
    global sock
    try:
        sock.settimeout(1)
        sock.connect((host, port))
        sock.close()
        return True
    except:
        return False
    finally:
        sock.close()


def check_distance():
    global previous_distance

    distance = info.graphics.distanceTraveled

    prev_int = int(previous_distance // 10)
    curr_int = int(distance // 10)

    if curr_int > prev_int:  # 说明跨越了一个10的倍数

        speed = ac.getCarState(0, acsys.CS.SpeedKMH)
        tyreWear = info.physics.tyreWear
        distance = info.graphics.distanceTraveled
        lap = info.graphics.completedLaps
        spline_pos = info.graphics.normalizedCarPosition
        fuel = info.physics.fuel

        data = {
            "Type": "Tick",
            "Speed": speed,
            "TyreWear": list(tyreWear),
            "Distance": distance,
            "Lap": lap + spline_pos,
            "Fuel": fuel,
        }

        send_udp_data(data)

    previous_distance = distance  # 更新 previous_distance


def check_lap():
    global previous_lap, previous_lap_distance
    lap = info.graphics.completedLaps

    if lap > previous_lap:
        distance = info.graphics.distanceTraveled
        lapTime = info.graphics.lastTime

        data = {
            "Type": "Lap",
            "Lap": lap,
            "LapTime": lapTime,
            "Distance": distance,
            "LapDistance": distance - previous_lap_distance,
        }

        send_udp_data(data)
        ac.log("This Lap lasts: {}".format(data["LapDistance"]))
        previous_lap = lap
        previous_lap_distance = distance


def check_pit():
    global previous_pitbox, previous_pitline

    gear = info.physics.gear
    rpm = info.physics.rpms
    pitbox = info.graphics.isInPit
    pitline = ac.isCarInPitline(0)

    if gear == 1 and rpm <= 5:
        ac.log("Quick Pit!")

    if previous_pitline == 0 and pitline == 1:
        ac.log("In Pit!")

    if previous_pitline == 1 and pitline == 0:
        ac.log("Out Pit!")

    # ac.log(str(pit))
    if pitbox == 0 and previous_pitbox == 1:
        ac.log("Out Pit Box!")

    previous_pitbox = pitbox
    previous_pitline = pitline


def send_udp_data(data):
    json_str = json.dumps(data)

    try:
        sock.sendto(json_str.encode("utf-8"), (host, port))
    except Exception as e:
        ac.log(str(e))
