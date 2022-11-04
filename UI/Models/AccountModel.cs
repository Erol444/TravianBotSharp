using FluentResults;
using ReactiveUI;
using System;

namespace UI.Models
{
    public class AccountModel : ReactiveObject
    {
        public Result IsValid()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return Result.Fail("Username is empty");
            }
            if (string.IsNullOrWhiteSpace(Server))
            {
                return Result.Fail("Server is empty");
            }
            if (!Uri.TryCreate(Server, UriKind.Absolute, out _))
            {
                return Result.Fail("Server is not a valid url. It should have https:// or http:// part");
            };

            if (string.IsNullOrWhiteSpace(Password))
            {
                return Result.Fail("Password is empty");
            }

            if (!string.IsNullOrWhiteSpace(ProxyHost))
            {
                if (ProxyPort == 0)
                {
                    return Result.Fail("Proxy port is empty");
                }

                if (!string.IsNullOrWhiteSpace(ProxyUsername))
                {
                    if (string.IsNullOrWhiteSpace(ProxyPassword))
                    {
                        return Result.Fail("Proxy password is empty");
                    }
                }
            }
            return Result.Ok();
        }

        private string _server;

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        private string _username;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private string _proxyHost;

        public string ProxyHost
        {
            get => _proxyHost;
            set => this.RaiseAndSetIfChanged(ref _proxyHost, value);
        }

        private int _proxyPort;

        public int ProxyPort
        {
            get => _proxyPort;
            set => this.RaiseAndSetIfChanged(ref _proxyPort, value);
        }

        private string _proxyUsername;

        public string ProxyUsername
        {
            get => _proxyUsername;
            set => this.RaiseAndSetIfChanged(ref _proxyUsername, value);
        }

        private string _proxyPassword;

        public string ProxyPassword
        {
            get => _proxyPassword;
            set => this.RaiseAndSetIfChanged(ref _proxyPassword, value);
        }
    }
}