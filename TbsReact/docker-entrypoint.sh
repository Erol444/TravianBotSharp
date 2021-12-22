#!/bin/bash

if [ -z "$VNC_PASSWORD" ]; then
	echo >&2 'error: No password for VNC connection set.'
	echo >&2 '  Did you forget to add -e VNC_PASSWORD=... ?'
	exit 1
fi

if [ -z "$XFB_SCREEN" ]; then
	XFB_SCREEN=1024x768x24
fi

# now boot X-Server, tell it to our cookie and give it sometime to start up
Xvfb :0 -screen 0 $XFB_SCREEN >>~/xvfb.log 2>&1 &
sleep 10

# finally we can run the VNC-Server based on our just started X-Server
x11vnc -forever -passwd $VNC_PASSWORD -display :0 &

./TbsReact &