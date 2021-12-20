FROM ubuntu:18.04

RUN apt-get update --fix-missing && apt-get install -y --no-install-recommends \
				xvfb \
				xauth \
				x11vnc \
				x11-utils \
				x11-xserver-utils \
				wget \
				gnupg \
				gdebi-core \
				ca-certificates \	
				coreutils \
		&& rm -rf /var/lib/apt/lists/*

ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE 1
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add

RUN echo 'deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main' | tee /etc/apt/sources.list.d/google-chrome.list
RUN apt-get update && apt-get install google-chrome-stable -y --no-install-recommends

ENV DISPLAY :0.0
EXPOSE 5900
COPY docker-entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

#COPY bin/Debug/net5.0/linux-x64/publish/ App/
COPY bin/Publish/Linux App
WORKDIR /App

EXPOSE 5000
EXPOSE 5001

# switch to user and start
ENTRYPOINT ["/entrypoint.sh"]