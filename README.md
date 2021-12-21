# TravianBotSharp (TBS)

[![TBS Discord](https://discordapp.com/api/guilds/740829801446637601/widget.png?style=banner2)](https://discord.gg/mBa4f2K)

## About

I have worked on this bot for over 2 years, ~15k LoC. It has been since completely ported to the newest version of Travian T4.5. Bot is under active development (at least until summer 2021). Instructions are on the Discord channel. If you have any question/recommendation, feel free to contact me (on Discord).

---

## Getting started

### Windows

#### Prerequisites

- Google Chrome

#### Installation

1. Download ZIP file from [here](holder)
2. Extract it to any empty folder
3. Run TbsReact file
4. Open your browser at [localhost:5001](localhost:5001)

### Linux OS

#### Prerequisites

- Docker

#### Installation

1. Download .tar.gz

    ```sh
    wget https://will.update.later/tbs-linux.tar.gz
    ```

2. Extract it

    ```sh
    hm what is command for extracting file on linux ?
    ```

3. Go to inside folder

    ``` sh
    cd tbs 
    ```

4. Create https certificate for your browser

    ``` sh
    openssl req -x509 -out localhost.crt -keyout localhost.key   -newkey rsa:2048 -nodes -sha256   -subj '/CN=localhost' -extensions EXT -config <( \
    printf "[dn]\nCN=localhost\n[req]\ndistinguished_name = dn\n[EXT]\nsubjectAltName=DNS:localhost\nkeyUsage=digitalSignature\nextendedKeyUsage=serverAuth")
    ```

    This will create 2 file in your folder - localhost.key & localhost.crt

5. Install certificate to your browser

    - For linux:

        ``` sh
            sudo mkdir /usr/local/share/ca-certificates/extra
            sudo cp localhost.crt /usr/local/share/ca-certificates/extra/localhost.crt
            sudo update-ca-certificates
        ```

        If fail, make sure the permissions are OK (755 for the folder, 644 for the file)

    - For windows:
        1. Copy localhost.crt to your windows machine
        2. Run localhost.crt file
        3. Click on "Install certificate..."
        4. Choose "Current user" and Next
        5. Choose "Automatically select the certificate store based on the type of certificate" and Next > Finish
        It should show "The import was successful."

6. Run bot

    - For normal Linux machine:

        Hm not sure :)

    - For GUI-less Linux machine:

        ``` sh
            xvfb-run TbsReact
        ```

### Mac OS

We couldn't test on this OS yet

#### Prerequisites

#### Installation

---

## Features

### - Auto-Change proxy

TBS can change proxy by itself. This means you are far less likely to be banned for using a bot. Main reason for a bot ban is high activity from same IP (12h+). TBS supports proxy authentication.

### - Auto-Settle new villages

TBS can find appropriate valleys to settle around your capital village. Or you can input coordinates in advance and it will automatically settle one valley after another.

### - Auto-Fill troops and send resources to/from capital

TBS will automatically send resources from capital village to off/deff villages, train troops there and send the remaining resources back. This is a perfect feature for any deff account.

### - Attack/Fake at specific time with multiple waves and oasis farming for T4.6

Dream-come-true for any offensive player.

### - Building list

### - Auto farming

### - Send hero to adventures

### - Multiple accounts, proxy options

### - Auto NPC when crop is above 99%

### - Send resources to new villages

### - Alert on attack

### - Auto-Claim beginner and daily quests

---

## Tech specification

~~It's a Windows Forms app written in C#. It only works on Windows. My plan is to make a web app for the frontend and I have already ported the TBS Core to the .Net Standard, so it could be used by .Net core project (cross-platform option).~~

It is now a Web app which backend written in C# and front end is made from React
