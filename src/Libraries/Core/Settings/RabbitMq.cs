﻿namespace Core.Settings
{
    public class RabbitMq
    {
        public string HostAddress { get; set; }

        public string VirtualHost { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}