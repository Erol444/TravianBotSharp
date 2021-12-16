#!/bin/bash

if [ -z "$VNC_PASSWORD" ]; then
	echo >&2 'error: No password for VNC connection set.'
	echo >&2 '  Did you forget to add -e VNC_PASSWORD=... ?'
	exit 1
fi

if [ -z "$XFB_SCREEN" ]; then
	XFB_SCREEN=1024x768x24
fi

if [ ! -z "$XFB_SCREEN_DPI" ]; then
	DPI_OPTIONS="-dpi $XFB_SCREEN_DPI"
fi

# cleanup previous executions
rm -fr ~/xvfb.log /tmp/.X11-unix/ /tmp/.X0-lock /.Xauthority

# first we need our security cookie and add it to user's .Xauthority
mcookie | sed -e 's/^/add :0 MIT-MAGIC-COOKIE-1 /' | xauth -q

# now place the security cookie with FamilyWild on volume so client can use it
# see http://stackoverflow.com/25280523 for details on the following command
xauth nlist :0 | sed -e 's/^..../ffff/' | xauth -f /.Xauthority/xserver.xauth nmerge -

# now boot X-Server, tell it to our cookie and give it sometime to start up
Xvfb :0 -auth ~/.Xauthority $DPI_OPTIONS -screen 0 $XFB_SCREEN >>~/xvfb.log 2>&1 &
sleep 10

google-chrome --disable-gpu --no-sandbox --disable-setuid-sandbox https://www.google.com.vn/

# finally we can run the VNC-Server based on our just started X-Server
x11vnc -forever -passwd $VNC_PASSWORD -display :0