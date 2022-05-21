namespace TbsCrossPlatform.Models.Database
{
    public class Proxy
    {
        /// <summary>
        /// Access's id
        /// </summary>
        public string AccessId { get; set; }

        /// <summary>
        /// Proxy's host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Proxy's host real
        /// There is situation when proxy is on your machine
        /// connect to other vpn
        /// </summary>
        public string HostReal { get; set; }

        /// <summary>
        /// Proxy's port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Proxy's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Proxy's password
        /// </summary>
        public string Password { get; set; }
    }
}