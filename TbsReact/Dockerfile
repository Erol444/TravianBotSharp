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
				gnupg \
				gdebi-core \
				ca-certificates \			
		&& rm -rf /var/lib/apt/lists/*

RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub 
RUN apt-key add linux_signing_key.pub

RUN echo 'deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main' | sudo tee /etc/apt/sources.list.d/google-chrome.list
RUN apt-get install google-chrome-stable -y --no-install-recommends

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