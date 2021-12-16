FROM ubuntu:18.04

RUN addgroup --system xusers \
  && adduser \
			--home /home/xuser \
			--disabled-password \
			--shell /bin/bash \
			--gecos "user for running X Window stuff" \
			--ingroup xusers \
			--quiet \
			xuser

RUN apt-get update && apt-get install -y --no-install-recommends \
				xvfb \
				xauth \
				x11vnc \
				x11-utils \
				x11-xserver-utils \
				wget \
		&& rm -rf /var/lib/apt/lists/*

RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN apt install ./google-chrome-stable_current_amd64.deb -y

RUN mkdir -p /Xauthority && chown -R xuser:xusers /Xauthority
VOLUME /Xauthority

ENV DISPLAY :0.0
EXPOSE 5900
COPY docker-entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
		
COPY bin/Debug/net5.0/linux-x64/publish/ App/
WORKDIR App/

# switch to user and start
USER xuser
ENTRYPOINT ["/entrypoint.sh"]