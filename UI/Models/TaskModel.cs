﻿using MainCore.Enums;
using System;

namespace UI.Models
{
    public class TaskModel
    {
        public string Task { get; set; }
        public DateTime ExecuteAt { get; set; }
        public TaskStage Stage { get; set; }
    }
}