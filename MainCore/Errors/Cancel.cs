﻿using FluentResults;

namespace MainCore.Errors
{
    public class Cancel : Error
    {
        public Cancel() : base("Stop command requested")
        {
        }
    }
}